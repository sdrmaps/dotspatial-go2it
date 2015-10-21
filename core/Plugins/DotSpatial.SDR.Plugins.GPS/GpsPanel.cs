using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using DotSpatial.Positioning;
using DotSpatial.SDR.Controls;

namespace DotSpatial.SDR.Plugins.GPS
{
    public sealed partial class GpsPanel : UserControl
    {
        private ProgressPanel _progressPanel;  // progress panel displayed on device detection
        private readonly Dictionary<string, string> _detectedDevices;  // lookup for deviceName/connectionType

        #region Constructors
        public GpsPanel()
        {
            InitializeComponent();
            _detectedDevices = new Dictionary<string, string>();
            chkAllowSerial.Checked = PluginSettings.Instance.AllowSerial;
            chkAllowBluetooth.Checked = PluginSettings.Instance.AllowBluetooth;
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when device detection should start
        /// </summary>
        public event EventHandler BeginDetection;
        /// <summary>
        /// Occurs when device detection needs canceled
        ///  </summary>
        public event EventHandler CancelDetection;
        /// <summary>
        /// Occurs when NMEA device should start
        /// </summary>
        public event EventHandler DeviceStart;
        /// <summary>
        /// Occurs when NMEA device is to stop
        /// </summary>
        public event EventHandler DeviceStop;
        /// <summary>
        /// Occurs when NMEA device is to pause
        /// </summary>
        public event EventHandler DevicePause;
        /// <summary>
        /// Occurs when NMEA device is to resume
        /// </summary>
        public event EventHandler DeviceResume;
        #endregion

        #region Form Events
        private void gpsDetectCancel_Click(object sender, EventArgs e)
        {
            _detectedDevices.Clear();
            cmbName.Items.Clear();
            cmbName.Text = string.Empty;
            txtConnType.Text = string.Empty;
            gpsStartStop.Enabled = false;
            gpsPauseResume.Enabled = false;
            if (gpsDetectCancel.Text == @"Detect")
            {
                gpsDetectCancel.Text = @"Cancel";
                PluginSettings.Instance.AllowBluetooth = chkAllowBluetooth.Checked; // save user setting
                PluginSettings.Instance.AllowSerial = chkAllowSerial.Checked; // save user setting
                chkAllowSerial.Enabled = false;
                chkAllowBluetooth.Enabled = false;
                if (_progressPanel == null)
                {
                    _progressPanel = new ProgressPanel();
                    _progressPanel.StartProgress("Detecting Devices...");
                }
                PluginSettings.Instance.DeviceStatus = DeviceStatus.Detecting;  // save user setting
                BeginDetection(sender, e);
            }
            else
            {
                gpsDetectCancel.Text = @"Detect";
                chkAllowSerial.Enabled = true;
                chkAllowBluetooth.Enabled = true;
                if (_progressPanel != null)
                {
                    _progressPanel.StopProgress();
                    _progressPanel = null;
                }
                PluginSettings.Instance.DeviceStatus = DeviceStatus.Unavailable;  // save user setting
                CancelDetection(sender, e);
            }
            txtStatus.Text = PluginSettings.Instance.DeviceStatus.ToString();
        }

        private void gpsStartStop_Click(object sender, EventArgs e)
        {
            if (gpsStartStop.Text == @"Start")
            {
                DeviceStart(sender, e);
            }
            else
            {
                DeviceStop(sender, e);
            }
        }

        private void gpsPauseResume_Click(object sender, EventArgs e)
        {
            if (gpsPauseResume.Text == @"Pause")
            {
                DevicePause(sender, e);
            }
            else
            {
                DeviceResume(sender, e);
            }
        }

        private void cmbName_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtConnType.Text = String.Empty;
            txtStatus.Text = DeviceStatus.Unavailable.ToString();
            if (cmbName.Text.Length > 0)
            {
                string outVal;
                bool success = _detectedDevices.TryGetValue(cmbName.Text, out outVal);
                if (success)
                {
                    txtStatus.Text = DeviceStatus.Detected.ToString();
                    txtConnType.Text = outVal;
                }
            }
        }
        #endregion

        #region Event Handlers
        public void DetectionCompleted(Dictionary<Device, string> devices)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                if (_progressPanel != null)
                {
                    _progressPanel.StopProgress();
                    _progressPanel = null;
                }
                gpsDetectCancel.Text = @"Detect";
                chkAllowSerial.Enabled = true;
                chkAllowBluetooth.Enabled = true;

                if (devices.Count > 0)
                {
                    gpsStartStop.Enabled = true;
                    PluginSettings.Instance.DeviceStatus = DeviceStatus.Detected;  // save user setting
                    foreach (KeyValuePair<Device, string> kvPair in devices)
                    {
                        _detectedDevices.Add(kvPair.Key.Name, kvPair.Value);
                        cmbName.Items.Add(kvPair.Key.Name);
                    }
                    if (devices.Count > 1)
                    {
                        if (PluginSettings.Instance.DeviceName != string.Empty)
                        {
                            cmbName.SelectedIndex = cmbName.Items.IndexOf(PluginSettings.Instance.DeviceName) != -1 ? cmbName.Items.IndexOf(PluginSettings.Instance.DeviceName) : 0;
                        }
                        else
                        {
                            cmbName.SelectedIndex = 0;  // the saved setting is blank
                        }
                    }
                    else
                    {
                        cmbName.SelectedIndex = 0;  // there is only one option to work with
                    }
                }
                else
                {
                    gpsStartStop.Enabled = false;
                    PluginSettings.Instance.DeviceStatus = DeviceStatus.Unavailable;  // save user setting
                }
                txtStatus.Text = PluginSettings.Instance.DeviceStatus.ToString();
                PluginSettings.Instance.DeviceName = cmbName.Text;
            }));
        }

        public void NmeaExceptionOccured(object sender, ExceptionEventArgs args)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                PluginSettings.Instance.DeviceStatus = DeviceStatus.Exception;  // save user setting
                txtStatus.Text = PluginSettings.Instance.DeviceStatus.ToString();
            }));
        }

        public void NmeaPaused(object sender, EventArgs args)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                gpsPauseResume.Text = @"Resume";
                PluginSettings.Instance.DeviceStatus = DeviceStatus.Paused;  // save user setting
                txtStatus.Text = PluginSettings.Instance.DeviceStatus.ToString();
            }));
        }

        public void NmeaResumed(object sender, EventArgs args)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                gpsPauseResume.Text = @"Pause";
                PluginSettings.Instance.DeviceStatus = DeviceStatus.Connected;  // save user setting
                txtStatus.Text = PluginSettings.Instance.DeviceStatus.ToString();
            }));
        }

        public void NmeaFixLost(object sender, EventArgs args)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                PluginSettings.Instance.DeviceStatus = DeviceStatus.FixLost;  // save user setting
                txtStatus.Text = PluginSettings.Instance.DeviceStatus.ToString();
            }));
        }

        public void NmeaFixAcquired(object sender, EventArgs args)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                PluginSettings.Instance.DeviceStatus = DeviceStatus.Connected;  // save user setting
                txtStatus.Text = PluginSettings.Instance.DeviceStatus.ToString();
            }));
        }

        public void NmeaConnectionLost(object sender, ExceptionEventArgs args)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                PluginSettings.Instance.DeviceStatus = DeviceStatus.ConnectionLost;  // save user setting
                txtStatus.Text = PluginSettings.Instance.DeviceStatus.ToString();
            }));
        }

        public void NmeaBearingChanged(object sender, AzimuthEventArgs args)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                txtBearing.Text = args.Azimuth.ToString();
            }));
        }

        public void NmeaAltitudeChanged(object sender, DistanceEventArgs args)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                txtAltitude.Text = args.Distance.ToString();
            }));
        }

        public void NmeaSpeedChanged(object sender, SpeedEventArgs args)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                txtSpeed.Text = args.Speed.ToString();
            }));
        }

        public void NmeaDateTimeChanged(object sender, DateTimeEventArgs args)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                txtDate.Text = args.DateTime.ToString(CultureInfo.InvariantCulture);
            }));
        }

        public void NmeaStarted(object sender, EventArgs args)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                gpsStartStop.Text = @"Stop";
                gpsDetectCancel.Enabled = false;
                gpsPauseResume.Enabled = true;
                cmbName.Enabled = false;
                PluginSettings.Instance.DeviceStatus = DeviceStatus.Connected;  // save user setting
                txtStatus.Text = DeviceStatus.Connected.ToString();
            }));
        }

        public void NmeaStopped(object sender, EventArgs args)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                gpsStartStop.Text = @"Start";
                gpsPauseResume.Text = @"Pause";
                gpsDetectCancel.Enabled = true;
                gpsPauseResume.Enabled = false;
                cmbName.Enabled = true;
                PluginSettings.Instance.DeviceStatus = DeviceStatus.Disconnected;  // save user setting
                txtStatus.Text = DeviceStatus.Disconnected.ToString();
                txtDate.Text = string.Empty;
                txtPosition.Text = string.Empty;
                txtAltitude.Text = string.Empty;
                txtSpeed.Text = string.Empty;
                txtBearing.Text = string.Empty;
            }));
        }

        public void NmeaPositionChanged(object sender, PositionEventArgs args)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                txtPosition.Text = args.Position.ToString();
            }));
        }
        #endregion

        private void chkAllowSerial_CheckedChanged(object sender, EventArgs e)
        {
            PluginSettings.Instance.AllowSerial = chkAllowSerial.Checked;
        }

        private void chkAllowBluetooth_CheckedChanged(object sender, EventArgs e)
        {
            PluginSettings.Instance.AllowBluetooth = chkAllowBluetooth.Checked;
        }

        private void gpsSettings_Click(object sender, EventArgs e)
        {
            var settingsForm = new GpsDisplaySettings();
            settingsForm.ShowDialog();
        }
    }
}
