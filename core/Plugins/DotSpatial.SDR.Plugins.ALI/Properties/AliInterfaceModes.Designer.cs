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
    internal sealed partial class AliInterfaceModes : global::System.Configuration.ApplicationSettingsBase {
        
        private static AliInterfaceModes defaultInstance = ((AliInterfaceModes)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new AliInterfaceModes())));
        
        public static AliInterfaceModes Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Disabled")]
        public string Disabled {
            get {
                return ((string)(this["Disabled"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("SDR AliServer")]
        public string Sdraliserver {
            get {
                return ((string)(this["Sdraliserver"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("GlobalCAD Log")]
        public string Globalcad {
            get {
                return ((string)(this["Globalcad"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Enterpol Database")]
        public string Enterpol {
            get {
                return ((string)(this["Enterpol"]));
            }
        }
    }
}
