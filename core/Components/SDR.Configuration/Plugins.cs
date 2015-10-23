using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace SDR.Configuration
{
    public static class Plugins
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginName">DotSpatial.SDR.Plugins.ALI</param>
        /// <param name="configSection">DotSpatial.SDR.Plugins.ALI.Properties.GlobalCadConfig</param>
        /// <param name="lookupKey"></param>
        /// <returns></returns>
        public static string GetPluginApplicationConfigValue(string plugin, string section, string lookupKey)
        {
            var config = GetFullConfigName(plugin, section);
            var node = GetSectionNode(plugin, config);
            if (node != null)
            {
                var attrColl = node.ChildNodes[0].Attributes;
                if (attrColl == null) return null;
                // find the key we are looking for
                for (var i = 0; i <= attrColl.Count - 1; i++)
                {
                    var lookup = attrColl[i].Value;
                    if (lookup == lookupKey)
                    {


                        return node.InnerText;
                    }
                }

            }



            return null;
        }

        private static XmlNode GetSectionNode(string plugin, string config)
        {
            var asm = Assembly.Load(plugin);
            var cf = GetDllConfiguration(asm);  // load the assembly configuration
            var sg = cf.SectionGroups["applicationSettings"]; // application level settings
            if (sg == null) return null;
            var s = sg.Sections[config];  // grab the requested section from the group
            if (s == null) return null;
            // load the raw xml into a document for parsing
            var xml = s.SectionInformation.GetRawXml();
            var xdoc = new XmlDocument();
            xdoc.LoadXml(xml);
            return xdoc.HasChildNodes ? xdoc.ChildNodes[0] : null;
        }

        private static string GetFullConfigName(string pluginName, string configSection)
        {
            // create the full configuration section name
            return pluginName + ".Properties." + configSection;
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
