using System;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using DotSpatial.SDR.Plugins.Measure.Properties;

namespace DotSpatial.SDR.Plugins.Measure
{
    public class MeasurePlugin : Extension
    {
        #region Constants and Fields

        private const string HomeMenuKey = HeaderControl.HomeRootItemKey;

        private MapFunctionMeasure _painter;
        private MeasurePanel _measurePanel;
        private DockablePanel _dockPanel;

        #endregion

        private const string PluginKey = "kPanel_Measure";


        public override void Activate()
        {
            App.HeaderControl.Add(new SimpleActionItem(HomeMenuKey, "Measure", MeasureTool_Click)
            {
                GroupCaption = "Measure_Area_Distance",
                ToolTipText = "Measure Map Area and Distance",
                SmallImage = Resources.measure_16,
                LargeImage = Resources.measure_32
            });
            _measurePanel = new MeasurePanel();
            _dockPanel = new DockablePanel(PluginKey, "Measure_Area_Distance", _measurePanel, DockStyle.Fill);
            App.DockManager.ActivePanelChanged += DockManager_ActivePanelChanged;
            base.Activate();
        }

        void DockManager_ActivePanelChanged(object sender, DockablePanelEventArgs e)
        {
            if (e.ActivePanelKey != PluginKey)
            {
                App.DockManager.Remove(PluginKey);
            }
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            App.DockManager.Remove(PluginKey);
            base.Deactivate();
        }

        private void MeasureTool_Click(object sender, EventArgs e)
        {
            App.DockManager.Add(_dockPanel);
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