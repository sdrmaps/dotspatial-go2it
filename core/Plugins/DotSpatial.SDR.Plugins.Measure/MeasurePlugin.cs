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
                ToolTipText = "Click to Measure Area and Distance",
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
                    App.Map.FunctionMode = FunctionMode.None;
                    App.Map.Cursor = Cursors.Cross;
                    _painter.Activate();
                    // add local map event binding
                    var map = App.Map as Control;
                    if (map != null) map.MouseLeave += MapOnMouseLeave;
                }
            }
            else if (key == PluginKey)
            {
                if (!_activeFunction)
                {
                    App.Map.FunctionMode = FunctionMode.None;
                    App.Map.Cursor = Cursors.Cross;
                    _painter.Activate();
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
                    var map = App.Map as Control;
                    if (map != null) map.MouseLeave -= MapOnMouseLeave;
                    // deactivate the local map function mode
                    App.Map.FunctionMode = FunctionMode.None;
                    App.Map.Cursor = Cursors.Default;
                    _painter.Deactivate();
                }
            }
            else if (key == PluginKey)
            {
                if (_activeFunction)
                {
                    // remove local map event binding
                    var map = App.Map as Control;
                    if (map != null) map.MouseLeave -= MapOnMouseLeave;
                    // deactivate the local map function mode
                    App.Map.FunctionMode = FunctionMode.None;
                    App.Map.Cursor = Cursors.Default;
                    _painter.Deactivate();
                    _activeFunction = false; // no longer an active function
                }
            }
        }

        private void MapOnMouseLeave(object sender, EventArgs eventArgs)
        {
            App.Map.Invalidate();
        }

        /// <summary>
        /// Measure Area and Distance Tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeasureTool_Click(object sender, EventArgs e)
        {
            App.DockManager.SelectPanel(PluginKey);  // select the display panel
            _activeFunction = true;
        }
    }
}