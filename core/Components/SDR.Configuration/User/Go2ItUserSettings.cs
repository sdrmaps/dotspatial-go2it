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
        private Go2ItUserSettings()
        {
        }

        static Go2ItUserSettings()
        {
            Instance = new Go2ItUserSettings();
        }

        public int GridHeaderColumnCount
        {
            get { return Properties.UserSettings.Default.GridHeaderColumnCount;  }
            set { 
                Properties.UserSettings.Default.GridHeaderColumnCount = value;
                Properties.UserSettings.Default.Save();
            }
        }

    }
}
