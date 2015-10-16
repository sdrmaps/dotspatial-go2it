using System;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using DotSpatial.SDR.Plugins.GPS.Properties;
using SdrConfig = SDR.Configuration;

namespace DotSpatial.SDR.Plugins.GPS
{
    public class GpsPlugin : Extension
    {
        #region Constants and Fields

        private const string HomeMenuKey = HeaderControl.HomeRootItemKey;

        private const string PluginKey = "kPanel_Gps";
        private const string PluginCaption = "Global Positioning System (GPS) Interface";

        private MapFunctionGps _mapFunction;
        private GpsPanel _gpsPanel;
        private DockablePanel _dockPanel;

        private bool _isFunctionActive;  // flag to eliminate redundant calls on hide/show,functionmode change
        #endregion

        public override void Activate()
        {
            // add in the button controls for this plugin to the header
            App.HeaderControl.Add(new SimpleActionItem(HomeMenuKey, "GPS", GpsTool_Click)
            {
                GroupCaption = "Gps_Setup",
                ToolTipText = "Click to Configure GPS",
                SmallImage = Resources.info_16,
                LargeImage = Resources.info_32,
                Key = PluginKey,
            });
            // generate the gps interface display panel and add it to the tool panel
            _gpsPanel = new GpsPanel();
            _dockPanel = new DockablePanel(PluginKey, PluginCaption, _gpsPanel, DockStyle.Fill);
            App.DockManager.Add(_dockPanel);
            App.DockManager.ActivePanelChanged += DockManagerOnActivePanelChanged;
            App.DockManager.PanelHidden += DockManagerOnPanelHidden;
            // initialize the gps function and check if it should/can start 
            _mapFunction = new MapFunctionGps(_gpsPanel);

            base.Activate();
        }

        public override void Deactivate()
        {
            App.HeaderControl.Remove(PluginKey);
            App.DockManager.Remove(PluginKey);

            App.DockManager.ActivePanelChanged -= DockManagerOnActivePanelChanged;
            App.DockManager.PanelHidden -= DockManagerOnPanelHidden;

            base.Deactivate();
        }

        private void AddGpsMapFunction()
        {
            if (_mapFunction == null)
            {
                _mapFunction = new MapFunctionGps(_gpsPanel);
            }
            if (!App.Map.MapFunctions.Contains(_mapFunction))
            {
                App.Map.MapFunctions.Add(_mapFunction);
            }
            _mapFunction.Map = App.Map;
        }

        private void MapOnFunctionModeChanged(object sender, EventArgs eventArgs)
        {
            if (!_isFunctionActive) return;  // dont waste time if this is not the active tool

            var map = sender as Map;
            if (map == null) return;
            if (_mapFunction == null) return;

            if (map.FunctionMode != FunctionMode.None)
            {
                _isFunctionActive = false;
                _mapFunction.Deactivate();
            }
        }

        private void DockManagerOnActivePanelChanged(object sender, DockablePanelEventArgs e)
        {
            if (_isFunctionActive) return;  // no need to do anything if this already the active tool

            var key = e.ActivePanelKey;
            var map = App.Map as Map;
            if (map == null) return;

            if (SdrConfig.User.Go2ItUserSettings.Instance.AdminModeActive) return;

            if (key.StartsWith("kMap_"))
            {
                // check that the GPS function exists for this map
                AddGpsMapFunction();
                // event binding to watch for function mode changes (to deactivate the tool)
                map.FunctionModeChanged += MapOnFunctionModeChanged;
                // check that this tool is the active tool of the map now
                if (App.Map.FunctionMode == FunctionMode.None &&
                    SdrConfig.User.Go2ItUserSettings.Instance.ActiveFunctionPanel == PluginKey)
                {
                    if (_mapFunction != null)
                    {
                        App.Map.Cursor = Cursors.Arrow;
                        _isFunctionActive = true;
                        _mapFunction.Activate();
                    }
                }
            }
            else if (key == PluginKey)
            {
                // check if this tool function panel is the active function or not
                if (App.Map.FunctionMode == FunctionMode.None &&
                    SdrConfig.User.Go2ItUserSettings.Instance.ActiveFunctionPanel == PluginKey)
                {
                    if (_mapFunction != null)  // validate we have a map function
                    {
                        App.Map.Cursor = Cursors.Arrow;
                        _isFunctionActive = true;
                        _mapFunction.Activate();
                    }
                }
            }
        }

        private void DockManagerOnPanelHidden(object sender, DockablePanelEventArgs e)
        {
            if (!_isFunctionActive) return;  // dont waste time if this is not the active tool

            var key = e.ActivePanelKey;
            var map = App.Map as Map;
            if (map == null) return;

            if (key.StartsWith("kMap_"))
            {
                // remove the event binding on this map (since its being hidden) on function mode changes
                map.FunctionModeChanged -= MapOnFunctionModeChanged;

                // lets look and see if this tool is currently the active tool and deactivate it if so
                if (map.FunctionMode == FunctionMode.None &&
                    SdrConfig.User.Go2ItUserSettings.Instance.ActiveFunctionPanel == PluginKey)
                {
                    if (_mapFunction != null)
                    {
                        _isFunctionActive = false;
                        _mapFunction.Deactivate();
                    }
                }
            }
            else if (key == PluginKey)
            {
                // check if this tool function panel is the active function or not
                if (App.Map.FunctionMode == FunctionMode.None &&
                    SdrConfig.User.Go2ItUserSettings.Instance.ActiveFunctionPanel == PluginKey)
                {
                    if (_mapFunction != null)
                    {
                        _isFunctionActive = false;
                        _mapFunction.Deactivate();
                    }
                }
            }
        }

        /// <summary>
        /// GPS Setup Tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GpsTool_Click(object sender, EventArgs e)
        {
            if (App.Map == null) return;
            // update the prefs for tracking the active tools modes and panels
            App.Map.FunctionMode = FunctionMode.None;
            App.DockManager.SelectPanel(PluginKey);
        }
    }
}
