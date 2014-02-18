using System;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using DotSpatial.SDR.Plugins.Search.Properties;

namespace DotSpatial.SDR.Plugins.Search
{
    public class SearchPlugin : Extension
    {
        #region Constants and Fields

        private SearchPanel _searchPanel;
        private DockablePanel _dockPanel;

        #endregion

        private const string PluginKey = "kPanel_Search";

        public override void Activate()
        {
            App.HeaderControl.Add(new SimpleActionItem(HeaderControl.HomeRootItemKey, "Search", SearchTool_Click)
            {
                GroupCaption = "Search",
                ToolTipText = "Search for Attributes and Features",
                SmallImage = Resources.info_rhombus_16x16,
                LargeImage = Resources.info_rhombus_32x32
            });
            _searchPanel = new SearchPanel();
            _dockPanel = new DockablePanel(PluginKey, "Search", _searchPanel, DockStyle.Fill);     
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

        /// <summary>
        /// Identifier Tool
        /// </summary>
        private void SearchTool_Click(object sender, EventArgs e)
        {
            App.DockManager.Add(_dockPanel);    
            App.DockManager.SelectPanel(PluginKey);
            // App.DockManager.SelectPanel("kPanel_Measure");
            // _MeasurePanel.Show();
            //if (_Painter == null)
            //    _Painter = new MapFunctionMeasure(App.Map, _MeasurePanel);

            //if (!App.Map.MapFunctions.Contains(_Painter))
            //    App.Map.MapFunctions.Add(_Painter);

            //App.Map.FunctionMode = FunctionMode.None;
            //App.Map.Cursor = Cursors.Cross;
            //_Painter.Activate();
        }
    }
}