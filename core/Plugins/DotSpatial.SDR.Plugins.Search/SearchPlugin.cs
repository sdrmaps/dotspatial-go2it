using System;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using DotSpatial.SDR.Controls;
using DotSpatial.SDR.Plugins.Search.Properties;
using SdrConfig = SDR.Configuration;

namespace DotSpatial.SDR.Plugins.Search
{
    public class SearchPlugin : Extension
    {
        #region Constants and Fields

        private const string HomeMenuKey = HeaderControl.HomeRootItemKey;

        private const string PluginKey = "kPanel_Search";
        private const string PluginCaption = "Search for Attributes and Features";

        private MapFunctionSearch _mapFunction;
        private SearchPanel _searchPanel;
        private DockablePanel _dockPanel;
        private bool _activeFunction;

        #endregion

        public override void Activate()
        {
            App.HeaderControl.Add(new SimpleActionItem(HomeMenuKey, "Search", SearchTool_Click)
            {
                GroupCaption = "Search",
                ToolTipText = "Search for Attributes and Features",
                SmallImage = Resources.info_16,
                LargeImage = Resources.info_32
            });
            // generate the search tool display panel and add to tool panel
            _searchPanel = new SearchPanel();
            _dockPanel = new DockablePanel(PluginKey, PluginCaption, _searchPanel, DockStyle.Fill);
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

            base.Activate();
        }

        private void AddSearchMapFunction()
        {
            if (_mapFunction == null)
            {
               _mapFunction = new MapFunctionSearch(_searchPanel);
            }
        }

        private void DockManagerOnActivePanelChanged(object sender, DockablePanelEventArgs dockablePanelEventArgs)
        {
            var dockControl = (TabDockingControl)sender;
            var key = dockablePanelEventArgs.ActivePanelKey;

            DockPanelInfo dockInfo;
            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;

            if (key.StartsWith("kMap_"))
            {
                AddSearchMapFunction();
                if (_activeFunction)
                {
                    App.Map.FunctionMode = FunctionMode.None;
                    App.Map.Cursor = Cursors.Default;
                    _mapFunction.Activate();
                }
            }
            else if (key == PluginKey)
            {
                if (!_activeFunction)
                {
                    App.Map.FunctionMode = FunctionMode.None;
                    App.Map.Cursor = Cursors.Default;
                    _mapFunction.Activate();
                }
            }
        }

        private void DockManagerOnPanelHidden(object sender, DockablePanelEventArgs dockablePanelEventArgs)
        {
            var dockControl = (TabDockingControl)sender;
            var key = dockablePanelEventArgs.ActivePanelKey;

            DockPanelInfo dockInfo;
            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;

            if (key.StartsWith("kMap_"))
            {
                if (_activeFunction)
                {
                    // deactivate the local map function mode
                    App.Map.FunctionMode = FunctionMode.None;
                    App.Map.Cursor = Cursors.Default;
                }
            }
            else if (key == PluginKey)
            {
                if (_activeFunction)
                {
                    // deactivate the local map function mode
                    App.Map.FunctionMode = FunctionMode.None;
                    App.Map.Cursor = Cursors.Default;
                    _mapFunction.Deactivate();
                    _activeFunction = false; // no longer an active function
                }
            }
        }

        /// <summary>
        /// Search Tool
        /// </summary>
        private void SearchTool_Click(object sender, EventArgs e)
        {
            App.DockManager.SelectPanel(PluginKey); // select the display panel
            _activeFunction = true;
        }
    }
}