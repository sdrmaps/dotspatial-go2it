using System;
using SDR.Configuration.Properties;

namespace SDR.Configuration.User
{
    /// <summary>
    /// User - level settings including interface persistence, symbology, colors, etc
    /// </summary>
    public class Go2ItUserSettings
    {
        /// <summary>
        /// Access SiteStructure.Instance to get the singleton object then call methods on that instance.
        /// </summary>
        public static Go2ItUserSettings Instance { get; private set; }
        /// <summary>
        /// Creates a new settings object with default values.
        /// This is a private constructor, meaning no outsiders have access.
        /// </summary>
        private Go2ItUserSettings() {}

        static Go2ItUserSettings()
        {
            Instance = new Go2ItUserSettings();
        }

        public string CoordinateDisplayMode
        {
            get { return UserSettings.Default.CoordinateDisplayMode; }
            set
            {
                UserSettings.Default.CoordinateDisplayMode = value;
                UserSettings.Default.Save();
            }
        }

        public int GridHeaderColumnCount
        {
            get { return UserSettings.Default.GridHeaderColumnCount;  }
            set { 
                UserSettings.Default.GridHeaderColumnCount = value;
                UserSettings.Default.Save();
            }
        }

        public string ActiveFunctionPanel
        {
            get { return UserSettings.Default.ActiveFunctionPanel; }
            set
            {
                UserSettings.Default.ActiveFunctionPanel = value;
                UserSettings.Default.Save();
            }
        }

        public event EventHandler AdminModeChanged;
        public bool AdminModeActive
        {
            get { return UserSettings.Default.AdminModeActive; }
            set
            {
                UserSettings.Default.AdminModeActive = value;
                OnAdminModeChanged(EventArgs.Empty);
            }
        }
        protected virtual void OnAdminModeChanged(EventArgs e)
        {
            if (AdminModeChanged != null)
                AdminModeChanged(this, e);
        }

        public string ActiveFunctionMode
        {
            get { return UserSettings.Default.ActiveFunctionMode; }
            set
            {
                UserSettings.Default.ActiveFunctionMode = value;
                UserSettings.Default.Save();
            }
        }

        public XmlSerializableDictionary<string, string> ResponderUnitLocation
        {
            get
            {
                var xmlString = UserSettings.Default.ResponderUnitLocation;
                var xmlDict = new XmlSerializableDictionary<string, string>();
                xmlDict.FromXmlString(xmlString);
                return xmlDict;
            }
            set
            {
                UserSettings.Default.ResponderUnitLocation = value.ToXmlString();
                UserSettings.Default.Save();
            }
        }
    }
}
