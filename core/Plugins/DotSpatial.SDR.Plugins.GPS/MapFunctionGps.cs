using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Positioning;
using DotSpatial.Projections;
using DotSpatial.Symbology;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using SdrConfig = SDR.Configuration;

namespace DotSpatial.SDR.Plugins.GPS
{
    public class MapFunctionGps : MapFunction
    {
        private GpsPanel _gpsPanel;
        private readonly NmeaInterpreter _nmeaInterpreter;
        private readonly Dictionary<Device, string> _gpsDevices;
        private bool _isAutoStart;
        private Point _latestGpsPoint;
        private System.Timers.Timer _intervalTimer;
        private ArrayList _intervalList;
        private GpsDisplayQueue _displayQueue;
        private readonly ProjectionInfo _wgs84Projection = KnownCoordinateSystems.Geographic.World.WGS1984;

        #region Constructors

        public MapFunctionGps(GpsPanel gp)
        {
            Name = "MapFunctionGps";
            YieldStyle = YieldStyles.AlwaysOn;

            _gpsPanel = gp;
            _gpsDevices = new Dictionary<Device, string>();
            _nmeaInterpreter = new NmeaInterpreter { AllowAutomaticReconnection = true };

            HandleDetectionEvents();
            HandleNmeaEvents();
            HandleGpsPanelEvents();
            SetupActivateGps();
        }

        public bool IsGpsActive()
        {
            return _nmeaInterpreter.IsRunning;
        }

        private void SetupActivateGps()
        {
            Devices.AllowBluetoothConnections = PluginSettings.Instance.AllowBluetooth;
            Devices.AllowSerialConnections = PluginSettings.Instance.AllowSerial;
            // check the status of the connection last run
            var devStatus = PluginSettings.Instance.DeviceStatus.ToString();
            if (devStatus == DeviceStatus.Connected.ToString() ||
                devStatus == DeviceStatus.Paused.ToString() ||
                devStatus == DeviceStatus.Detected.ToString())
                {
                    if (devStatus != DeviceStatus.Detected.ToString())
                    {
                        _isAutoStart = true;
                    }
                    Devices.BeginDetection();
                }
        }

        private void HandleDetectionEvents()
        {
            Devices.DeviceDetectionCompleted += DevicesOnDeviceDetectionCompleted;
            Devices.DeviceDetected += DevicesOnDeviceDetected;
        }

        private void UpdateSettings_ResponderPosition(NmeaInterpreter nmea)
        {
            if (nmea.Position.IsInvalid || nmea.Position.IsEmpty) return;
            if (double.IsNaN(nmea.Position.Latitude.DecimalDegrees) ||
                double.IsNaN(nmea.Position.Longitude.DecimalDegrees)) return;

            var xmlDict = new SdrConfig.XmlSerializableDictionary<string, string>
            {
                {"latitude", nmea.Position.Latitude.DecimalDegrees.ToString(CultureInfo.InvariantCulture)},
                {"longitude", nmea.Position.Longitude.DecimalDegrees.ToString(CultureInfo.InvariantCulture)},
                {"timestamp", nmea.DateTime.ToString(CultureInfo.InvariantCulture)}
            };
            // update application level user config setting to store responder unit location with timestamp
            SdrConfig.User.Go2ItUserSettings.Instance.ResponderUnitLocation = xmlDict;
        }

        /// <summary>
        /// Handle all NMEA events and pass them along to the Panel for display
        /// </summary>
        private void HandleNmeaEvents()
        {
            _nmeaInterpreter.BearingChanged += (sender, args) => _gpsPanel.NmeaBearingChanged(sender, args);
            _nmeaInterpreter.AltitudeChanged += (sender, args) => _gpsPanel.NmeaAltitudeChanged(sender, args);
            _nmeaInterpreter.SpeedChanged += (sender, args) => _gpsPanel.NmeaSpeedChanged(sender, args);
            _nmeaInterpreter.DateTimeChanged += (sender, args) => _gpsPanel.NmeaDateTimeChanged(sender, args);
            _nmeaInterpreter.PositionChanged += delegate(object sender, PositionEventArgs args)
            {
                _gpsPanel.NmeaPositionChanged(sender, args);
                var nmea = sender as NmeaInterpreter;
                UpdateSettings_ResponderPosition(nmea);
                PlotGpsPointToMap(args);
            };
            _nmeaInterpreter.Started += delegate(object sender, EventArgs args)
            {
                _gpsPanel.NmeaStarted(sender, args);
                AddGpsTrail();
            };
            _nmeaInterpreter.Stopped += delegate(object sender, EventArgs args)
            {
                _gpsPanel.NmeaStopped(sender, args);
                ClearGpsTrail();
            };
            _nmeaInterpreter.ExceptionOccurred += (sender, args) => _gpsPanel.NmeaExceptionOccured(sender, args);
            _nmeaInterpreter.FixLost += (sender, args) => _gpsPanel.NmeaFixLost(sender, args);
            _nmeaInterpreter.FixAcquired += (sender, args) => _gpsPanel.NmeaFixAcquired(sender, args);
            _nmeaInterpreter.ConnectionLost += (sender, args) => _gpsPanel.NmeaConnectionLost(sender, args);
            _nmeaInterpreter.Paused += (sender, args) => _gpsPanel.NmeaPaused(sender, args);
            _nmeaInterpreter.Resumed += (sender, args) => _gpsPanel.NmeaResumed(sender, args);
        }

        private Coordinate ConvertLatLonToMap(double lon, double lat)
        {
            if (!Map.Projection.Equals(KnownCoordinateSystems.Geographic.World.WGS1984))
            {
                ProjectionInfo mapProjInfo = ProjectionInfo.FromEsriString(Map.Projection.ToEsriString());
                var xy = new double[2];
                xy[0] = lon;
                xy[1] = lat;
                var z = new double[1];
                Reproject.ReprojectPoints(xy, z, _wgs84Projection, mapProjInfo, 0, 1);
                return new Coordinate(xy[0], xy[1]);
            }
            return new Coordinate(lon, lat);
        }

        private void PlotGpsPointToMap(PositionEventArgs args)
        {
            if (Map == null) return;

            if (double.IsNaN(args.Position.Latitude.DecimalDegrees) ||
                double.IsNaN(args.Position.Longitude.DecimalDegrees)) return;

            if (_displayQueue == null)
            {
                PointShape pointShape;
                Enum.TryParse(PluginSettings.Instance.GpsPointStyle, true, out pointShape);
                _displayQueue = new GpsDisplayQueue(
                    pointShape,
                    PluginSettings.Instance.GpsPointSize,
                    PluginSettings.Instance.GpsPointColor,
                    PluginSettings.Instance.GpsDisplayCount);

                // refresh the map to display our updated gps trail
                _displayQueue.ListUpdated += (o, eventArgs) => Map.Refresh();

                PluginSettings.Instance.GpsDisplayCountChanged +=
                    delegate
                    {
                        _displayQueue.GpsPointDisplayCount = PluginSettings.Instance.GpsDisplayCount;
                    };
                PluginSettings.Instance.GpsPointColorChanged +=
                    delegate
                    {
                        _displayQueue.GpsNewPointColor = PluginSettings.Instance.GpsPointColor;
                    };
                PluginSettings.Instance.GpsPointStyleChanged +=
                    delegate
                    {
                        PointShape ps;
                        Enum.TryParse(PluginSettings.Instance.GpsPointStyle, true, out ps);
                        _displayQueue.GpsPointShape = ps;
                    };
                PluginSettings.Instance.GpsPointSizeChanged +=
                    delegate
                    {
                        _displayQueue.GpsPointSize = PluginSettings.Instance.GpsPointSize;
                    };
            }
            AddGpsTrail();

            Coordinate c = ConvertLatLonToMap(args.Position.Longitude.DecimalDegrees, args.Position.Latitude.DecimalDegrees);
            _latestGpsPoint = new Point(c);

            HandleGpsInterval();
        }

        private void HandleGpsInterval()
        {
            PluginSettings.Instance.GpsIntervalTypeChanged += (sender, args) => HandleGpsInterval();

            if (PluginSettings.Instance.GpsIntervalType == "Time")
            {
                if (_intervalList != null)
                {
                    _intervalList = null;
                    PluginSettings.Instance.GpsIntervalValueChanged -= InstanceOnGpsIntervalValueChanged;
                }
                if (_intervalTimer == null)
                {
                    var interval = PluginSettings.Instance.GpsIntervalValue * 1000;
                    PluginSettings.Instance.GpsIntervalValueChanged += InstanceOnGpsIntervalValueChanged;
                    _intervalTimer = new System.Timers.Timer(interval) {AutoReset = true};
                    _intervalTimer.Elapsed += delegate
                    {
                        _displayQueue.Enqueue(_latestGpsPoint);
                    };
                    _intervalTimer.Enabled = true;
                }
            }
            else
            {
                if (_intervalTimer != null)
                {
                    _intervalTimer.Enabled = false;
                    _intervalTimer = null;
                    PluginSettings.Instance.GpsIntervalValueChanged -= InstanceOnGpsIntervalValueChanged;
                }
                var interval = PluginSettings.Instance.GpsIntervalValue - 1;
                if (_intervalList == null)
                {
                    _intervalList = new ArrayList(interval);
                    PluginSettings.Instance.GpsIntervalValueChanged += InstanceOnGpsIntervalValueChanged;
                }
                if (_intervalList.Count == interval)
                {
                    _intervalList.Clear();
                    _displayQueue.Enqueue(_latestGpsPoint);
                }
                else
                {
                    _intervalList.Add(_latestGpsPoint);
                }
            }
        }

        private void InstanceOnGpsIntervalValueChanged(object sender, EventArgs eventArgs)
        {
            if (PluginSettings.Instance.GpsIntervalType == "Time")
            {
                if (_intervalTimer != null)
                {
                    _intervalTimer.Interval = PluginSettings.Instance.GpsIntervalValue * 1000;
                }
            }
            else
            {
                if (_intervalList != null)
                {
                    _intervalList.Capacity = PluginSettings.Instance.GpsIntervalValue - 1;
                }
            }
        }

        /// <summary>
        /// Handle all the button events that can occur on the GPS Panel
        /// </summary>
        private void HandleGpsPanelEvents()
        {
            _gpsPanel.DevicePause += delegate { _nmeaInterpreter.Pause(); };
            _gpsPanel.DeviceResume += delegate { _nmeaInterpreter.Resume(); };
            _gpsPanel.BeginDetection += delegate
            {
                _gpsDevices.Clear();
                Devices.AllowBluetoothConnections = PluginSettings.Instance.AllowBluetooth;
                Devices.AllowSerialConnections = PluginSettings.Instance.AllowSerial;
                Devices.BeginDetection();
            };
            _gpsPanel.CancelDetection += delegate
            {
                _gpsDevices.Clear();
                Devices.CancelDetection(true);
            };
            _gpsPanel.DeviceStart += delegate
            {
                TryStart(PluginSettings.Instance.DeviceName);
            };
            _gpsPanel.DeviceStop += delegate
            {
                _nmeaInterpreter.Stop();
                if (_intervalTimer != null)
                {
                    _intervalTimer.Stop();
                    _intervalTimer = null;
                }
            };
        }

        private void DevicesOnDeviceDetected(object sender, DeviceEventArgs deviceEventArgs)
        {
            var flag = false;
            var device = (Device) sender;
            var btDevices = Devices.BluetoothDevices;
            if (btDevices.Any(btDevice => btDevice.Equals(device)))
            {
                _gpsDevices.Add(device, "Bluetooth");
                flag = true;
            }
            if (flag) return;
            var sDevices = Devices.SerialDevices;
            if (sDevices.Any(sDevice => sDevice.Equals(device)))
            {
                _gpsDevices.Add(device, "Serial");
            }
        }

        private void TryStart(string deviceName)
        {
            try
            {
                foreach (KeyValuePair<Device, string> kvPair in _gpsDevices)
                {
                    if (kvPair.Key.Name != deviceName) continue;
                    _nmeaInterpreter.Start(kvPair.Key);
                    break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Connection to GPS failed");
            }
        }

        private void DevicesOnDeviceDetectionCompleted(object sender, EventArgs eventArgs)
        {
            _gpsPanel.DetectionCompleted(_gpsDevices);
            if (_isAutoStart)  // only fired on startup if the previous run had an active GPS session
            {
                _isAutoStart = false;
                TryStart(PluginSettings.Instance.DeviceName);
            }
        }

        private void AddGpsTrail()
        {
            if (Map == null || _displayQueue == null) return;
            if (!Map.MapFrame.DrawingLayers.Contains(_displayQueue.GpsGraphicsLayer))
            {
                Map.MapFrame.DrawingLayers.Add(_displayQueue.GpsGraphicsLayer);
            }
        }

        private void ClearGpsTrail()
        {
            if (Map == null || _displayQueue == null) return;
            if (Map.MapFrame.DrawingLayers.Contains(_displayQueue.GpsGraphicsLayer))
            {
                Map.MapFrame.DrawingLayers.Remove(_displayQueue.GpsGraphicsLayer);
            }
        }

        protected override void OnActivate()
        {
            if (_gpsPanel == null || _gpsPanel.IsDisposed)
            {
                _gpsPanel = new GpsPanel();
                HandleDetectionEvents();
                HandleNmeaEvents();
                HandleGpsPanelEvents();
            }
            _gpsPanel.Show();
            base.OnActivate();
        }

        /// <summary>
        /// Allows for new behavior during deactivation.
        /// </summary>
        protected override void OnDeactivate()
        {
            ClearGpsTrail();
            base.OnDeactivate();
        }
        #endregion
    }
}
