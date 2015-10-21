using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml;
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

        public bool AdminModeActive
        {
            get { return UserSettings.Default.AdminModeActive; }
            set { UserSettings.Default.AdminModeActive = value; }
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
            var newDict = settings;
            var xmlDict = new XmlSerializableDictionary<string, string>();
            foreach (KeyValuePair<string, string> kvPair in newDict)
            {
                xmlDict.Add(kvPair.Key, kvPair.Value);
            }
            using (TextWriter writer = new Utf8StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(writer))
                {
                    xmlDict.WriteXml(xmlWriter);
                    xmlWriter.Close();
                }
                var str = writer.ToString();
                writer.Close();
                return str;
            }
        }

        private static Dictionary<string, string>FetchDictionarySetting(string setting)
        {
            if (setting.Length <= 0) return null; // no setting currently exists
            var xmlDict = new XmlSerializableDictionary<string, string>();
            using (TextReader reader = new StringReader(setting))
            {
                using (XmlReader xmlReader = XmlReader.Create(reader))
                {
                    xmlDict.ReadXml(xmlReader);
                    xmlReader.Close();
                }
                reader.Close();
            }
            return xmlDict.ToDictionary(kvPair => kvPair.Key, kvPair => kvPair.Value);
        }
    }
}
