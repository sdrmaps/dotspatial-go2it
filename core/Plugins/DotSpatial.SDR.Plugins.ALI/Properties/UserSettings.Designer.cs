﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DotSpatial.SDR.Plugins.ALI.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed partial class UserSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static UserSettings defaultInstance = ((UserSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new UserSettings())));
        
        public static UserSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ActiveGlobalCadCommLog {
            get {
                return ((string)(this["ActiveGlobalCadCommLog"]));
            }
            set {
                this["ActiveGlobalCadCommLog"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>CallTime</string>
  <string>Phone</string>
  <string>Address</string>
  <string>Street</string>
  <string>ServiceClass</string>
  <string>Customer</string>
  <string>CallDate</string>
  <string>City</string>
  <string>State</string>
  <string>Sector</string>
  <string>X</string>
  <string>Y</string>
  <string>Uncertainty</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection DgvSortOrder {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["DgvSortOrder"]));
            }
            set {
                this["DgvSortOrder"] = value;
            }
        }
    }
}
