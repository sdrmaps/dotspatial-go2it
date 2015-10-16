using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Positioning;
using DotSpatial.Projections;
using DotSpatial.SDR.Plugins.GPS.Properties;
using DotSpatial.Symbology;
using DotSpatial.Topology;
using Point = DotSpatial.Topology.Point;
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
            Devices.AllowBluetoothConnections = UserSettings.Default.AllowBluetooth;
            Devices.AllowSerialConnections = UserSettings.Default.AllowSerial;
            // check the status of the connection last run
            var devStatus = UserSettings.Default.DeviceStatus;
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

        /// <summary>
        /// Handle all NMEA events and pass them along to the Panel for display
        /// </summary>
        private void HandleNmeaEvents()
        {
            _nmeaInterpreter.BearingChanged += delegate(object sender, AzimuthEventArgs args) { _gpsPanel.NmeaBearingChanged(sender, args); };
            _nmeaInterpreter.AltitudeChanged += delegate(object sender, DistanceEventArgs args) { _gpsPanel.NmeaAltitudeChanged(sender, args); };
            _nmeaInterpreter.SpeedChanged += delegate(object sender, SpeedEventArgs args) { _gpsPanel.NmeaSpeedChanged(sender, args); };
            _nmeaInterpreter.DateTimeChanged += delegate(object sender, DateTimeEventArgs args) { _gpsPanel.NmeaDateTimeChanged(sender, args); };
            _nmeaInterpreter.PositionChanged += delegate(object sender, PositionEventArgs args)
            {
                _gpsPanel.NmeaPositionChanged(sender, args);
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
            _nmeaInterpreter.ExceptionOccurred += delegate(object sender, ExceptionEventArgs args)
            {
                _gpsPanel.NmeaExceptionOccured(sender, args);
            };
            _nmeaInterpreter.FixLost += delegate(object sender, EventArgs args) { _gpsPanel.NmeaFixLost(sender, args); };
            _nmeaInterpreter.FixAcquired += delegate(object sender, EventArgs args) { _gpsPanel.NmeaFixAcquired(sender, args); };
            _nmeaInterpreter.ConnectionLost += delegate(object sender, ExceptionEventArgs args) { _gpsPanel.NmeaConnectionLost(sender, args); };
            _nmeaInterpreter.Paused += delegate(object sender, EventArgs args) { _gpsPanel.NmeaPaused(sender, args); };
            _nmeaInterpreter.Resumed += delegate(object sender, EventArgs args) { _gpsPanel.NmeaResumed(sender, args); };
        }

        private Coordinate ConvertLatLonToMap(double lon, double lat)
        {
            if (!Map.Projection.Equals(KnownCoordinateSystems.Geographic.World.WGS1984))
            {
                ProjectionInfo mapProjInfo = ProjectionInfo.FromEsriString(Map.Projection.ToEsriString());
                double[] xy = new double[2];
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
            if (Map != null)
            {
                if (!double.IsNaN(args.Position.Latitude.DecimalDegrees))
                {
                    if (_displayQueue == null)
                    {
                        PointShape pointShape;
                        Enum.TryParse(SdrConfig.Project.Go2ItProjectSettings.Instance.GpsPointStyle, true, out pointShape);
                        _displayQueue = new GpsDisplayQueue(
                            pointShape,
                            SdrConfig.Project.Go2ItProjectSettings.Instance.GpsPointSize,
                            SdrConfig.Project.Go2ItProjectSettings.Instance.GpsPointColor,
                            SdrConfig.Project.Go2ItProjectSettings.Instance.GpsDisplayCount);
                        // refresh the map to display our updated gps trail
                        _displayQueue.ListUpdated += (o, eventArgs) => Map.Refresh();

                        SdrConfig.Project.Go2ItProjectSettings.Instance.GpsDisplayCountChanged +=
                            delegate
                            {
                                _displayQueue.GpsPointDisplayCount = SdrConfig.Project.Go2ItProjectSettings.Instance.GpsDisplayCount;
                            };
                        SdrConfig.Project.Go2ItProjectSettings.Instance.GpsPointColorChanged +=
                            delegate
                            {
                                _displayQueue.GpsNewPointColor = SdrConfig.Project.Go2ItProjectSettings.Instance.GpsPointColor;
                            };
                        SdrConfig.Project.Go2ItProjectSettings.Instance.GpsPointStyleChanged +=
                            delegate
                            {
                                PointShape ps;
                                Enum.TryParse(SdrConfig.Project.Go2ItProjectSettings.Instance.GpsPointStyle, true, out ps);
                                _displayQueue.GpsPointShape = ps;
                            };
                        SdrConfig.Project.Go2ItProjectSettings.Instance.GpsPointSizeChanged +=
                            delegate
                            {
                                _displayQueue.GpsPointSize = SdrConfig.Project.Go2ItProjectSettings.Instance.GpsPointSize;
                            };
                    }
                    AddGpsTrail();

                    Coordinate c = ConvertLatLonToMap(args.Position.Longitude.DecimalDegrees, args.Position.Latitude.DecimalDegrees);
                    _latestGpsPoint = new Point(c);

                    HandleGpsInterval();
                }
            }
        }

        private void HandleGpsInterval()
        {
            SdrConfig.Project.Go2ItProjectSettings.Instance.GpsIntervalTypeChanged +=
                (sender, args) => HandleGpsInterval();

            if (SdrConfig.Project.Go2ItProjectSettings.Instance.GpsIntervalType == "Time")
            {
                _intervalList = null;
                if (_intervalTimer == null)
                {
                    var interval = SdrConfig.Project.Go2ItProjectSettings.Instance.GpsIntervalValue * 1000;
                    SdrConfig.Project.Go2ItProjectSettings.Instance.GpsIntervalValueChanged +=
                        delegate
                        {
                            _intervalTimer.Interval = SdrConfig.Project.Go2ItProjectSettings.Instance.GpsIntervalValue * 1000;
                        };
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
                _intervalTimer = null;
                var interval = SdrConfig.Project.Go2ItProjectSettings.Instance.GpsIntervalValue - 1;
                if (_intervalList == null)
                {
                    _intervalList = new ArrayList(interval);
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
                Devices.AllowBluetoothConnections = UserSettings.Default.AllowBluetooth;
                Devices.AllowSerialConnections = UserSettings.Default.AllowSerial;
                Devices.BeginDetection();
            };
            _gpsPanel.CancelDetection += delegate
            {
                _gpsDevices.Clear();
                Devices.CancelDetection(true);
            };
            _gpsPanel.DeviceStart += delegate
            {
                TryStart(UserSettings.Default.DeviceName);
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
                TryStart(UserSettings.Default.DeviceName);
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
