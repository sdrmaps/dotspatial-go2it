using System.Collections.Specialized;
using DotSpatial.SDR.Plugins.ALI.Properties;

namespace DotSpatial.SDR.Plugins.ALI
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

        public string ActiveGlobalCadCommLog
        {
            get { return UserSettings.Default.ActiveGlobalCadCommLog; }
            set
            {
                UserSettings.Default.ActiveGlobalCadCommLog = value;
                UserSettings.Default.Save();
            }
        }

        public StringCollection SdrAliServerDgvSortOrder
        {
            get { return UserSettings.Default.SdrAliServerDgvSortOrder; }
            set
            {
                var dif = false;
                for (var i = 0; i <= UserSettings.Default.SdrAliServerDgvSortOrder.Count - 1; i++)
                {
                    if (value[i] == UserSettings.Default.SdrAliServerDgvSortOrder[i]) continue;
                    dif = true;
                    break;
                }
                if (!dif) return;
                UserSettings.Default.SdrAliServerDgvSortOrder = value;
                UserSettings.Default.Save();
            }
        }

        public StringCollection EnterpolDgvSortOrder
        {
            get { return UserSettings.Default.EnterpolDgvSortOrder; }
            set
            {
                var dif = false;
                for (var i = 0; i <= UserSettings.Default.EnterpolDgvSortOrder.Count - 1; i++)
                {
                    if (value[i] == UserSettings.Default.EnterpolDgvSortOrder[i]) continue;
                    dif = true;
                    break;
                }
                if (!dif) return;
                UserSettings.Default.EnterpolDgvSortOrder = value;
                UserSettings.Default.Save();
            }
        }

    }
}

