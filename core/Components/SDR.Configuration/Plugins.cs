using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Text;
using System.Xml;

namespace SDR.Configuration
{
    public static class Plugins
    {
        public static string GetPluginApplicationConfigValue(string plugin, string section, string key)
        {
            var config = GetFullConfigName(plugin, section);
            var node = GetSectionNode(plugin, config);

            for (var i = 0; i <= node.ChildNodes.Count - 1; i++)
            {
                var val = node.ChildNodes[i].InnerText;  // value of this child node
                var attrColl = node.ChildNodes[i].Attributes;
                if (attrColl != null)
                {
                    var lookup = attrColl[0].Value;  // lookup key
                    if (lookup == key)
                    {
                        return val;
                    }
                }
            }
            return null;
        }

        public static Dictionary<string, string> GetPluginApplicationConfigSectionAsDict(string plugin, string section)
        {
            var config = GetFullConfigName(plugin, section);
            var node = GetSectionNode(plugin, config);

            var retDict = new Dictionary<string, string>();

            for (var i = 0; i <= node.ChildNodes.Count - 1; i++)
            {
                var val = node.ChildNodes[i].InnerText;  // value of this child node
                var attrColl = node.ChildNodes[i].Attributes;
                if (attrColl != null)
                {
                    var lookup = attrColl[0].Value;  // lookup key
                    retDict.Add(lookup, val);
                }
            }
            return retDict;
        }

        private static XmlNode GetSectionNode(string plugin, string config)
        {
            var asm = Assembly.Load(plugin);
            var cf = GetDllConfiguration(asm);  // load the assembly configuration
            var sg = cf.SectionGroups["applicationSettings"]; // application level settings
            if (sg == null) return null;

            var s = sg.Sections[config];  // snatch the requested config section from the group
            if (s == null) return null;

            // load raw xml into document for parsing
            var xml = s.SectionInformation.GetRawXml();
            var xdoc = new XmlDocument();
            xdoc.LoadXml(xml);

            return xdoc.ChildNodes[0];  // return the section node
        }

        private static string GetFullConfigName(string plugin, string section)
        {
            return plugin + ".Properties." + section;  // create the full configuration section name
        }

        private static System.Configuration.Configuration GetDllConfiguration(Assembly targetAsm)
        {
            var configFile = targetAsm.Location + ".config";
            var map = new ExeConfigurationFileMap
            {
                ExeConfigFilename = configFile
            };
            return ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
        }
    }
}
