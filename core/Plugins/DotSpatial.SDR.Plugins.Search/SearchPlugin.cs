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
               _mapFunction = new MapFunctionSearch(_searchPanel)
               {
                   // set the tab docking control so we can do panel selections from MapFunctionSearch.cs
                   TabDockingControl = App.DockManager as TabDockingControl
               };
            }
            if (!App.Map.MapFunctions.Contains(_mapFunction))
            {
                App.Map.MapFunctions.Add(_mapFunction);
            }
            _mapFunction.Map = App.Map;
        }

        private void DockManagerOnActivePanelChanged(object sender, DockablePanelEventArgs dockablePanelEventArgs)
        {
            var dockControl = (TabDockingControl)sender;
            var key = dockablePanelEventArgs.ActivePanelKey;

            DockPanelInfo dockInfo;
            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;

            if (key.StartsWith("kMap_"))
            {
                if (App.Map == null) return;
                AddSearchMapFunction();
                // setup an event binding to watch for any time the functionmode changes
                App.Map.FunctionModeChanged += MapOnFunctionModeChanged;
                // verify that a functionmode is not currently active
                // and that the active tool panel is in fact this panel (check that this tool is active)
                if (App.Map.FunctionMode == FunctionMode.None &&
                    SdrConfig.User.Go2ItUserSettings.Instance.ActiveFunctionPanel == PluginKey)
                {
                    App.Map.Cursor = Cursors.Default;
                    _mapFunction.Activate();
                    // add local map event binding below if needed
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
                        App.Map.Cursor = Cursors.Default;
                        _mapFunction.Activate();
                    }
                }
            }
        }

        private void MapOnFunctionModeChanged(object sender, EventArgs eventArgs)
        {
            // update the user settings to reflect active functionmode
            var map = sender as Map;
            if (map != null && map.FunctionMode != FunctionMode.None)
            {
                _mapFunction.Deactivate();
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

                // TODO: catch all for map == null shit



                // remove the event binding on this map (since its being hidden) on functionmodechanges
                App.Map.FunctionModeChanged += MapOnFunctionModeChanged;

                if (App.Map.FunctionMode == FunctionMode.None &&
                    SdrConfig.User.Go2ItUserSettings.Instance.ActiveFunctionPanel == PluginKey)
                {
                    // deactivate the local map function mode
                    if (_mapFunction != null)
                    {
                        _mapFunction.Deactivate();
                    }
                }
            }
            else if (key == PluginKey)
            {
                if (App.Map.FunctionMode == FunctionMode.None &&
                    SdrConfig.User.Go2ItUserSettings.Instance.ActiveFunctionPanel == PluginKey)
                {
                    // deactivate the local map function mode and remove any map binding if it exists
                    App.Map.Cursor = Cursors.Default;
                    if (_mapFunction != null)
                    {
                        _mapFunction.Deactivate();
                    }
                }
            }
        }

        /// <summary>
        /// Search Features and Attributes Tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchTool_Click(object sender, EventArgs e)
        {
            // update the prefs for tracking the active tools modes and panels
            App.Map.FunctionMode = FunctionMode.None;
            App.DockManager.SelectPanel(PluginKey);
        }
    }
}