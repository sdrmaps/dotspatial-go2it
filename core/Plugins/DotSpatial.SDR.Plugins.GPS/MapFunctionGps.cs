using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Positioning;
using DotSpatial.SDR.Plugins.GPS.Properties;

namespace DotSpatial.SDR.Plugins.GPS
{
    public class MapFunctionGps : MapFunction
    {
        private readonly GpsPanel _gpsPanel;
        private readonly NmeaInterpreter _nmeaInterpreter;
        private readonly Dictionary<Device, string> _gpsDevices;
        private bool _isAutoStart;

        #region Constructors

        public MapFunctionGps(GpsPanel gp)
        {
            _gpsPanel = gp;
            _gpsDevices = new Dictionary<Device, string>();
            _nmeaInterpreter = new NmeaInterpreter { AllowAutomaticReconnection = true };
            Configure();
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
                    _isAutoStart = true;
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
            // TODO: break these out into localized methods for adding map function calls for display as needed
            _nmeaInterpreter.BearingChanged += delegate(object sender, AzimuthEventArgs args) { _gpsPanel.NmeaBearingChanged(sender, args); };
            _nmeaInterpreter.AltitudeChanged += delegate(object sender, DistanceEventArgs args) { _gpsPanel.NmeaAltitudeChanged(sender, args); };
            _nmeaInterpreter.SpeedChanged += delegate(object sender, SpeedEventArgs args) { _gpsPanel.NmeaSpeedChanged(sender, args); };
            _nmeaInterpreter.DateTimeChanged += delegate(object sender, DateTimeEventArgs args) { _gpsPanel.NmeaDateTimeChanged(sender, args); };
            _nmeaInterpreter.PositionChanged += delegate(object sender, PositionEventArgs args) { _gpsPanel.NmeaPositionChanged(sender, args); };
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
                TryStart(_gpsPanel.DeviceName);
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
            if (_isAutoStart)
            {
                _isAutoStart = false;
                TryStart(UserSettings.Default.DeviceName);
            }
        }
        #endregion
    }
}
