using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Positioning;
using DotSpatial.SDR.Plugins.GPS.Properties;

namespace DotSpatial.SDR.Plugins.GPS
{
    public class MapFunctionGps : MapFunction
    {
        private GpsPanel _gpsPanel;
        private readonly NmeaInterpreter _nmeaInterpreter;
        private readonly Dictionary<Device, string> _gpsDevices;
        private bool _isAutoStart;

        // drawing layers used by this tool
        // private FeatureSet _pointGraphics;
        // private MapPointLayer _pointGraphicsLayer;

        #region Constructors

        public MapFunctionGps(GpsPanel gp)
        {
            _gpsPanel = gp;
            _gpsDevices = new Dictionary<Device, string>();
            _nmeaInterpreter = new NmeaInterpreter { AllowAutomaticReconnection = true };
            Configure();
        }

        public bool IsGpsActive()
        {
            return _nmeaInterpreter.IsRunning;
        }

        private void Configure()
        {
            Name = "MapFunctionGps";
            YieldStyle = YieldStyles.AlwaysOn;
            HandleDetectionEvents();
            HandleNmeaEvents();
            HandleGpsPanelEvents();
            ConfigureActivateGps();
        }

        private void ConfigureActivateGps()
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
                // TODO: right here is where we staret with drawing the points to map
                _gpsPanel.NmeaPositionChanged(sender, args);
            };
            _nmeaInterpreter.Started += delegate(object sender, EventArgs args) { _gpsPanel.NmeaStarted(sender, args); };
            _nmeaInterpreter.Stopped += delegate(object sender, EventArgs args) { _gpsPanel.NmeaStopped(sender, args); };
            _nmeaInterpreter.ExceptionOccurred += delegate(object sender, ExceptionEventArgs args) { _gpsPanel.NmeaExceptionOccured(sender, args); };
            _nmeaInterpreter.FixLost += delegate(object sender, EventArgs args) { _gpsPanel.NmeaFixLost(sender, args); };
            _nmeaInterpreter.FixAcquired += delegate(object sender, EventArgs args) { _gpsPanel.NmeaFixAcquired(sender, args); };
            _nmeaInterpreter.ConnectionLost += delegate(object sender, ExceptionEventArgs args) { _gpsPanel.NmeaConnectionLost(sender, args); };
            _nmeaInterpreter.Paused += delegate(object sender, EventArgs args) { _gpsPanel.NmeaPaused(sender, args); };
            _nmeaInterpreter.Resumed += delegate(object sender, EventArgs args) { _gpsPanel.NmeaResumed(sender, args); };
        }

        /// <summary>
        /// Handle all the button events that can occur on the GPS Panel
        /// </summary>
        private void HandleGpsPanelEvents()
        {
            _gpsPanel.DeviceStop += delegate { _nmeaInterpreter.Stop(); };
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
            base.OnDeactivate();
        }
        #endregion
    }
}
