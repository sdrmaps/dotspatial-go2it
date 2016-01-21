using System;
using System.Collections.Generic;
using DotSpatial.SDR.Plugins.Search.Properties;
using SDR.Configuration;

namespace DotSpatial.SDR.Plugins.Search
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
        private PluginSettings()
        {
        }

        static PluginSettings()
        {
            Instance = new PluginSettings();
        }

        public SearchMode SearchMode
        {
            get
            {
                var funcMode = UserSettings.Default.SearchMode;
                if (funcMode.Length <= 0) return SearchMode.Address;
                SearchMode sm;
                Enum.TryParse(funcMode, true, out sm);
                return sm;
            }
            set
            {
                UserSettings.Default.SearchMode = value.ToString();
                UserSettings.Default.Save();
            }
        }

        public Dictionary<string, string> CellSectorIndexColumnOrder
        {
            get { return FetchDictionarySetting(UserSettings.Default.CellSectorIndexColumnOrder); }
            set
            {
                UserSettings.Default.CellSectorIndexColumnOrder = DictionarySettingsString(value);
                UserSettings.Default.Save();
            }
        }

        public Dictionary<string, string> ParcelIndexColumnOrder
        {
            get { return FetchDictionarySetting(UserSettings.Default.ParcelIndexColumnOrder); }
            set
            {
                UserSettings.Default.ParcelIndexColumnOrder = DictionarySettingsString(value);
                UserSettings.Default.Save();
            }
        }

        public Dictionary<string, string> RoadIndexColumnOrder
        {
            get { return FetchDictionarySetting(UserSettings.Default.RoadIndexColumnOrder); }
            set
            {
                UserSettings.Default.RoadIndexColumnOrder = DictionarySettingsString(value);
                UserSettings.Default.Save();
            }
        }

        public Dictionary<string, string> AddressIndexColumnOrder
        {
            get { return FetchDictionarySetting(UserSettings.Default.AddressIndexColumnOrder); }
            set
            {
                UserSettings.Default.AddressIndexColumnOrder = DictionarySettingsString(value);
                UserSettings.Default.Save();
            }
        }

        public Dictionary<string, string> KeyLocationIndexColumnOrder
        {
            get { return FetchDictionarySetting(UserSettings.Default.KeyLocationIndexColumnOrder); }
            set
            {
                UserSettings.Default.KeyLocationIndexColumnOrder = DictionarySettingsString(value);
                UserSettings.Default.Save();
            }
        }

        private static string DictionarySettingsString(Dictionary<string, string> settings)
        {
            var xmlDict = new XmlSerializableDictionary<string, string>();
            foreach (KeyValuePair<string, string> kvPair in settings)
            {
                xmlDict.Add(kvPair.Key, kvPair.Value);
            }
            return xmlDict.ToXmlString();
        }

        private static Dictionary<string, string> FetchDictionarySetting(string setting)
        {
            if (setting.Length <= 0) return null; // no setting currently exists
            var xmlDict = new XmlSerializableDictionary<string, string>();
            xmlDict.FromXmlString(setting);
            return xmlDict;
        }
    }
}
