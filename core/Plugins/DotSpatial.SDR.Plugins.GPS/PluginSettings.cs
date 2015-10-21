using System;
using System.Drawing;
using DotSpatial.SDR.Plugins.GPS.Properties;

namespace DotSpatial.SDR.Plugins.GPS
{
    public class PluginSettings
    {
        /// <summary>
        /// Access SiteStructure.Instance to get the singleton object then call methods on that instance.
        /// </summary>
        public static PluginSettings Instance { get; private set; }
        /// <summary>
        /// Creates a new settings object with default values.
        /// This is a private constructor, meaning no outsiders have access.
        /// </summary>
        private PluginSettings() { }

        static PluginSettings()
        {
            Instance = new PluginSettings();
        }

        public string DeviceName
        {
            get { return UserSettings.Default.DeviceName; }
            set
            {
                UserSettings.Default.DeviceName = value;
                UserSettings.Default.Save();
            }
        }

        public bool AllowSerial
        {
            get
            {
                return UserSettings.Default.AllowSerial;
            }
            set
            {
                UserSettings.Default.AllowSerial = value;
                UserSettings.Default.Save();
            }
        }

        public bool AllowBluetooth
        {
            get
            {
                return UserSettings.Default.AllowBluetooth;
            }
            set
            {
                UserSettings.Default.AllowBluetooth = value;
                UserSettings.Default.Save();
            }
        }

        public DeviceStatus DeviceStatus
        {
            get
            {
                var funcMode = UserSettings.Default.DeviceStatus;
                if (funcMode.Length <= 0) return DeviceStatus.Unavailable;
                DeviceStatus ds;
                Enum.TryParse(funcMode, true, out ds);
                return ds;
            }
            set
            {
                UserSettings.Default.DeviceStatus = value.ToString();
                UserSettings.Default.Save();
            }
        }

        public event EventHandler GpsIntervalTypeChanged;
        public string GpsIntervalType
        {
            get { return UserSettings.Default.IntervalType; }
            set
            {
                if (UserSettings.Default.IntervalType != value)
                {
                    UserSettings.Default.IntervalType = value;
                    UserSettings.Default.Save();
                    OnGpsIntervalTypeChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnGpsIntervalTypeChanged(EventArgs e)
        {
            if (GpsIntervalTypeChanged != null)
                GpsIntervalTypeChanged(this, e);
        }

        public event EventHandler GpsIntervalValueChanged;
        public int GpsIntervalValue
        {
            get { return UserSettings.Default.IntervalValue; }
            set
            {
                if (UserSettings.Default.IntervalValue != value)
                {
                    UserSettings.Default.IntervalValue = value;
                    UserSettings.Default.Save();
                    OnGpsIntervalValueChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnGpsIntervalValueChanged(EventArgs e)
        {
            if (GpsIntervalValueChanged != null)
                GpsIntervalValueChanged(this, e);
        }

        public event EventHandler GpsDisplayCountChanged;
        public int GpsDisplayCount
        {
            get { return UserSettings.Default.DisplayCount; }
            set
            {
                if (UserSettings.Default.DisplayCount != value)
                {
                    UserSettings.Default.DisplayCount = value;
                    UserSettings.Default.Save();
                    OnGpsDisplayCountChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnGpsDisplayCountChanged(EventArgs e)
        {
            if (GpsDisplayCountChanged != null)
                GpsDisplayCountChanged(this, e);
        }

        public event EventHandler GpsPointColorChanged;
        public Color GpsPointColor
        {
            get { return UserSettings.Default.PointColor; }
            set
            {
                if (UserSettings.Default.PointColor != value)
                {
                    UserSettings.Default.PointColor = value;
                    UserSettings.Default.Save();
                    OnGpsPointColorChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnGpsPointColorChanged(EventArgs e)
        {
            if (GpsPointColorChanged != null)
                GpsPointColorChanged(this, e);
        }

        public event EventHandler GpsPointStyleChanged;
        public string GpsPointStyle
        {
            get { return UserSettings.Default.PointStyle; }
            set
            {
                if (UserSettings.Default.PointStyle != value)
                {
                    UserSettings.Default.PointStyle = value;
                    UserSettings.Default.Save();
                    OnGpsPointStyleChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnGpsPointStyleChanged(EventArgs e)
        {
            if (GpsPointStyleChanged != null)
                GpsPointStyleChanged(this, e);
        }

        public event EventHandler GpsPointSizeChanged;
        public int GpsPointSize
        {
            get { return UserSettings.Default.PointSize; }
            set
            {
                if (UserSettings.Default.PointSize != value)
                {
                    UserSettings.Default.PointSize = value;
                    UserSettings.Default.Save();
                    OnGpsPointSizeChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnGpsPointSizeChanged(EventArgs e)
        {
            if (GpsPointSizeChanged != null)
                GpsPointSizeChanged(this, e);
        }
    }
}

