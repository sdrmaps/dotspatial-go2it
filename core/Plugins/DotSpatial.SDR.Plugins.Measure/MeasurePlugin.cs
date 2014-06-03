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

        private const string HomeMenuKey = HeaderControl.HomeRootItemKey;

        private const string PluginKey = "kPanel_Measure";
        private const string PluginCaption = "Measure Area and Distance";

        private MapFunctionMeasure _painter;
        private MeasurePanel _measurePanel;
        private DockablePanel _dockPanel;
        private bool _activeFunction;

        #endregion

        public override void Activate()
        {
            // add in the button controls for this plugin to the header
            App.HeaderControl.Add(new SimpleActionItem(HomeMenuKey, "Measure", MeasureTool_Click)
            {
                GroupCaption = "Measure_Area_Distance",
                ToolTipText = "Measure Map Area and Distance",
                SmallImage = Resources.measure_16,
                LargeImage = Resources.measure_32
            });
            // generate the measurement tool display panel and add it to the tool panel
            _measurePanel = new MeasurePanel();
            _dockPanel = new DockablePanel(PluginKey, PluginCaption, _measurePanel, DockStyle.Fill);
            App.DockManager.Add(_dockPanel);

            App.DockManager.ActivePanelChanged += DockManagerOnActivePanelChanged;
            App.DockManager.PanelHidden += DockManagerOnPanelHidden;

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

        private void AddMeasureMapFunction()
        {
            if (_painter == null)
            {
                _painter = new MapFunctionMeasure(_measurePanel);
            }
            if (!App.Map.MapFunctions.Contains(_painter))
            {
                App.Map.MapFunctions.Add(_painter);
            }
            _painter.Map = App.Map;
        }

        private void DockManagerOnActivePanelChanged(object sender, DockablePanelEventArgs e)
        {
            var dockControl = (TabDockingControl)sender;
            var key = e.ActivePanelKey;

            DockPanelInfo dockInfo;
            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;

            if (key.StartsWith("kMap_"))
            {
                if (App.Map == null) return;
                AddMeasureMapFunction();
                if (_activeFunction)
                {
                    // add local map event binding
                    // var map = App.Map as Control;
                    // map.MouseLeave += MapOnMouseLeave;
                    // activate the new local map function
                    _painter.Activate();
                    App.Map.FunctionMode = FunctionMode.None;
                    App.Map.Cursor = Cursors.Cross;
                }
            }
            else if (key == PluginKey)
            {
                if (!_activeFunction)
                {
                    // _painter = App.Map.GetMapFunction("MapFunctionMeasure") as MapFunctionMeasure;
                    // var mFunc = App.Map.GetMapFunction("MapFunctionMeasure") as MapFunctionMeasure;
                    // mFunc.Activate();
                    _painter.Activate();
                    App.Map.FunctionMode = FunctionMode.None;
                    App.Map.Cursor = Cursors.Cross;
                    //_painter.Activate();
                }
            }
        }

        private void DockManagerOnPanelHidden(object sender, DockablePanelEventArgs e)
        {
            // clear any measurements regardless of panel type
            _measurePanel.ClearMeasurements();

            var dockControl = (TabDockingControl)sender;
            var key = e.ActivePanelKey;

            DockPanelInfo dockInfo;
            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;

            if (key.StartsWith("kMap_"))
            {
                if (_activeFunction)
                {
                    // remove local map event binding
                    // var map = App.Map as Control;
                    // map.MouseLeave -= MapOnMouseLeave;
                    // deactivate the local map function
                    // var mapFunc = App.Map.GetMapFunction("MapFunctionMeasure") as MapFunctionMeasure;
                    // mapFunc.Deactivate();
                    // set the defaults back for this map
                    _painter.Deactivate();
                    App.Map.FunctionMode = FunctionMode.None;
                    App.Map.Cursor = Cursors.Default;
                }
            }
            else if (key == PluginKey)
            {
                if (_activeFunction)
                {
                    _activeFunction = false;
                    _painter.Deactivate();
                    App.Map.FunctionMode = FunctionMode.None;
                    App.Map.Cursor = Cursors.Default;
                    // var map = App.Map as Control;
                    // map.MouseLeave -= MapOnMouseLeave;
                    // _painter.Deactivate();
                }
            }
        }

        private void MapOnMouseLeave(object sender, EventArgs eventArgs)
        {
            App.Map.Invalidate();
        }

        private void MeasureTool_Click(object sender, EventArgs e)
        {
            App.DockManager.SelectPanel(PluginKey);  // select the display panel
            _activeFunction = true;
        }
    }
}