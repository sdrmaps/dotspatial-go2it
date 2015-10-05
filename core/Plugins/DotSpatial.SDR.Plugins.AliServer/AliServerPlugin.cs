using System;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using SdrConfig = SDR.Configuration;

namespace DotSpatial.SDR.Plugins.AliServer
{
    public class AliServerPlugin : Extension
    {
        #region Constants and Fields
        
        private const string HomeMenuKey = HeaderControl.HomeRootItemKey;

        private const string PluginKey = "kPanel_AliServer";
        private const string PluginCaption = "Connect with SDR AliServer";

        private DockablePanel _dockPanel;

        private bool _isFunctionActive;  // eliminate redundant calls when active or not
        #endregion

        public override void Activate()
        {
            // add in the button controls for this plugin to the header
            //App.HeaderControl.Add(new SimpleActionItem(HomeMenuKey, "Measure", MeasureTool_Click)
            //{
            //    GroupCaption = "Measure_Area_Distance",
            //    ToolTipText = "Click to Measure Area and Distance",
            //    SmallImage = Resources
            //    LargeImage = Resources
            //});
            // generate the measurement tool display panel and add it to the tool panel
            // _aliServerPanel = new AliServerPanel();
            // _dockPanel = new DockablePanel(PluginKey, PluginCaption, _aliServerPanel, DockStyle.Fill);
            // App.DockManager.Add(_dockPanel);
            // App.DockManager.ActivePanelChanged += DockManagerOnActivePanelChanged;
            // App.DockManager.PanelHidden += DockManagerOnPanelHidden;

            base.Activate();
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            App.DockManager.Remove(PluginKey);

            App.DockManager.ActivePanelChanged -= DockManagerOnActivePanelChanged;
            App.DockManager.PanelHidden -= DockManagerOnPanelHidden;

            base.Deactivate();
        }

        private void AddAliServerMapFunction()
        {
            //if (_mapFunction == null)
            //{
            //    _mapFunction = new MapFunctionMeasure(_aliServerPanel);
            //    // handle the functionactivated event, and fire button toggle for visual display
            //    _mapFunction.FunctionActivated += OnMapFunctionOnFunctionActivated;
            //}
            //if (!App.Map.MapFunctions.Contains(_mapFunction))
            //{
            //    App.Map.MapFunctions.Add(_mapFunction);
            //}
            //_mapFunction.Map = App.Map;
        }

        private void DockManagerOnActivePanelChanged(object sender, DockablePanelEventArgs e)
        {
            //if (_isFunctionActive) return;  // no need to do anything if this already the active tool

            //var key = e.ActivePanelKey;
            //var map = App.Map as Map;
            //if (map == null) return;

            //if (SdrConfig.User.Go2ItUserSettings.Instance.AdminModeActive) return;

            //if (key.StartsWith("kMap_"))
            //{
            //    // check if the measurement function exists for this map
            //    AddAliServerMapFunction();
            //    // event binding to watch for function mode changes (to deactivate the tool)
            //    map.FunctionModeChanged += MapOnFunctionModeChanged;
            //    // check that this tool is the active tool of the map now
            //    if (App.Map.FunctionMode == FunctionMode.None &&
            //        SdrConfig.User.Go2ItUserSettings.Instance.ActiveFunctionPanel == PluginKey)
            //    {
            //        map.MouseLeave += MapOnMouseLeave;  // add local map event binding
            //        if (_mapFunction != null)
            //        {
            //            // set the cursor and activate the measure mode
            //            App.Map.Cursor = Cursors.Cross;
            //            _isFunctionActive = true;
            //            _mapFunction.Activate();
            //        }
            //    }
            //}
            //else if (key == PluginKey)
            //{
            //    // check if this tool function panel is the active function or not
            //    if (App.Map.FunctionMode == FunctionMode.None &&
            //        SdrConfig.User.Go2ItUserSettings.Instance.ActiveFunctionPanel == PluginKey)
            //    {
            //        if (_mapFunction != null)  // validate we have a map function
            //        {
            //            // set the cursor and activate the measure mode
            //            App.Map.Cursor = Cursors.Cross;
            //            _isFunctionActive = true;
            //            _mapFunction.Activate();
            //        }
            //    }
            //}
        }

        private void DockManagerOnPanelHidden(object sender, DockablePanelEventArgs e)
        {
            //if (!_isFunctionActive) return;  // dont waste time if this is not the active tool

            //// clear any measurements regardless of panel type
            //_measurePanel.ClearMeasurements();

            //var key = e.ActivePanelKey;
            //var map = App.Map as Map;
            //if (map == null) return;

            //if (key.StartsWith("kMap_"))
            //{
            //    // remove the event binding on this map (since its being hidden) on function mode changes
            //    map.FunctionModeChanged -= MapOnFunctionModeChanged;

            //    // lets look and see if this tool is currently the active tool and deactivate it if so
            //    if (map.FunctionMode == FunctionMode.None &&
            //        SdrConfig.User.Go2ItUserSettings.Instance.ActiveFunctionPanel == PluginKey)
            //    {
            //        // remove local map event binding
            //        map.MouseLeave -= MapOnMouseLeave;
            //        if (_mapFunction != null)
            //        {
            //            _isFunctionActive = false;
            //            _mapFunction.Deactivate();
            //        }
            //    }
            //}
            //else if (key == PluginKey)
            //{
            //    // check if this tool function panel is the active function or not
            //    if (App.Map.FunctionMode == FunctionMode.None &&
            //        SdrConfig.User.Go2ItUserSettings.Instance.ActiveFunctionPanel == PluginKey)
            //    {
            //        // remove local map event binding
            //        map.MouseLeave -= MapOnMouseLeave;
            //        if (_mapFunction != null)
            //        {
            //            _isFunctionActive = false;
            //            _mapFunction.Deactivate();
            //        }
            //    }
            //}
        }
    }
}