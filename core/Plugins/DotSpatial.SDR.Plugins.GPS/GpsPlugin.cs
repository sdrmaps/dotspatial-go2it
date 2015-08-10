using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;

namespace DotSpatial.SDR.Plugins.GPS
{
    public class GpsPlugin : Extension
    {
        #region Constants and Fields

        private const string HomeMenuKey = HeaderControl.HomeRootItemKey;

        private const string PluginKey = "kPanel_Gps";
        private const string PluginCaption = "Global Positioning System Module";

        private MapFunctionGps _mapFunction;
        private GpsPanel _gpsPanel;
        private DockablePanel _dockPanel;

        private bool _isFunctionActive;  // eliminate redundant calls when active or not
        #endregion
    }
}
