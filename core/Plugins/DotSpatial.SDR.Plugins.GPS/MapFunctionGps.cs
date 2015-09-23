using System;
using System.Collections.Generic;
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
            ConfigureSettings();
        }

        private void ConfigureSettings()
        {
            Devices.AllowBluetoothConnections = UserSettings.Default.AllowBluetooth;
            Devices.AllowSerialConnections = UserSettings.Default.AllowSerial;
            // TODO: in here we check the state and run a detect and start if we can


        }

        private void HandleDetectionEvents()
        {
            Devices.DeviceDetectionCanceled += DevicesOnDeviceDetectionCanceled;
            Devices.DeviceDetectionCompleted += DevicesOnDeviceDetectionCompleted;
            Devices.DeviceDetected += DevicesOnDeviceDetected;
        }

        /// <summary>
        /// Handle all NMEA events and pass them along to the Panel for display
        /// </summary>
        private void HandleNmeaEvents()
        {
            // TODO: break these out into localized methods for adding map function calls for display
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
            _gpsPanel.BeginDetection += delegate(object sender, EventArgs args)
            {
                Devices.BeginDetection();
            };
            _gpsPanel.CancelDetection += delegate(object sender, EventArgs args)
            {
                // TODO: investigate the pause here
                Devices.CancelDetection();
                Devices.Undetect();
            };
            _gpsPanel.DeviceStart += delegate(object sender, EventArgs args)
            {
                try
                {
                    foreach (Device gpsDevice in _gpsDevices)
                    {
                        if (gpsDevice.Name != _gpsPanel.DeviceName) continue;
                        _nmeaInterpreter.Start(gpsDevice);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Failed to connect with GPS");
                }
            };
            _gpsPanel.DeviceStop += delegate(object sender, EventArgs args)
            {
                _nmeaInterpreter.Stop();
            };
            _gpsPanel.DevicePause += delegate(object sender, EventArgs args)
            {
                _nmeaInterpreter.Pause();
            };
            _gpsPanel.DeviceResume += delegate(object sender, EventArgs args)
            {
                _nmeaInterpreter.Resume();
            };
        }

        private void DevicesOnDeviceDetectionCanceled(object sender, EventArgs eventArgs)
        {
            _gpsDevices.Clear();
        }

        private void DevicesOnDeviceDetected(object sender, DeviceEventArgs deviceEventArgs)
        {
            var device = (Device) sender;
            var btDevices = Devices.BluetoothDevices;
            var sDevices = Devices.SerialDevices;


            // _gpsDevices.Add((Device)sender);
        }

        private void DevicesOnDeviceDetectionCompleted(object sender, EventArgs eventArgs)
        {
            _gpsPanel.DetectionCompleted(_gpsDevices);
        }
        #endregion
    }
}
