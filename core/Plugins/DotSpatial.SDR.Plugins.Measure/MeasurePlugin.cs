using System;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using DotSpatial.SDR.Controls;
using DotSpatial.SDR.Plugins.Measure.Properties;

namespace DotSpatial.SDR.Plugins.Measure
{
    public class MeasurePlugin : Extension
    {
        #region Constants and Fields

        private Map _map ;
        private const string HomeMenuKey = HeaderControl.HomeRootItemKey;

        private const string PluginKey = "kPanel_Measure";
        private MapFunctionMeasure _painter;
        private MeasurePanel _measurePanel;
        private DockablePanel _dockPanel;

        #endregion

        public override void Activate()
        {
            // add the buttons for measurement tool
            App.HeaderControl.Add(new SimpleActionItem(HomeMenuKey, "Measure", MeasureTool_Click)
            {
                GroupCaption = "Measure_Area_Distance",
                ToolTipText = "Measure Map Area and Distance",
                SmallImage = Resources.measure_16,
                LargeImage = Resources.measure_32
            });
            // generate the measurement tool display panel
            _measurePanel = new MeasurePanel();
            _dockPanel = new DockablePanel(PluginKey, "Measure_Area_Distance", _measurePanel, DockStyle.Fill);
            App.DockManager.Add(_dockPanel);

            App.DockManager.ActivePanelChanged += DockManager_ActivePanelChanged;
            base.Activate();
        }

        void DockManager_ActivePanelChanged(object sender, DockablePanelEventArgs e)
        {
            DockPanelInfo dockInfo;
            if (e.ActivePanelKey.StartsWith("kMap_"))
            {
                // update the measurement toool to ref correct map
            }
            if (e.ActivePanelKey == PluginKey)
            {
                // activate the panel now
                // App.DockManager.Remove(PluginKey);
            }
            if (e.ActivePanelKey != PluginKey)
            {
                // deactivate the panel
            }
            //DockPanelInfo dockInfo;
            //var dockControl = (DockingControl)sender;
            //var key = dockablePanelEventArgs.ActivePanelKey;
            //if (!key.StartsWith("kMap_")) return;
            //if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;
            //if (_userLegend != null)
            //{
            //    _userLegend.RootNodes.Clear();
            //}
            //// update the active _map for the legend now
            //_map = (Map)dockInfo.DotSpatialDockPanel.InnerControl;
            //_map.Legend = _userLegend;
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            App.DockManager.Remove(PluginKey);
            base.Deactivate();
        }

        private void MeasureTool_Click(object sender, EventArgs e)
        {
            App.DockManager.SelectPanel(PluginKey);
            
            if (_painter == null)
                _painter = new MapFunctionMeasure(App.Map, _measurePanel);

            if (!App.Map.MapFunctions.Contains(_painter))
                App.Map.MapFunctions.Add(_painter);

            App.Map.FunctionMode = FunctionMode.None;
            App.Map.Cursor = Cursors.Cross;
            _painter.Activate();
        }
    }
}