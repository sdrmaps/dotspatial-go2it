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

        private bool _isFunctionActive;  // flag to eliminate redundant calls on hide/show,functionmode change
        #endregion

        public override void Activate()
        {
            App.HeaderControl.Add(new SimpleActionItem(HomeMenuKey, "Search", SearchTool_Click)
            {
                GroupCaption = "Search",
                ToolTipText = "Search for Attributes and Features",
                SmallImage = Resources.search_16,
                LargeImage = Resources.search_32,
                Key = PluginKey
            });
            // generate the search tool display panel and add to tool panel
            _searchPanel = new SearchPanel();
            _dockPanel = new DockablePanel(PluginKey, PluginCaption, _searchPanel, DockStyle.Fill);
            App.DockManager.Add(_dockPanel);

            // hotkeys for this plugin
            HotKeyManager.AddHotKey(new HotKey(Keys.F4, "Road Search"), "Activate_Road_Search");

            App.DockManager.ActivePanelChanged += DockManagerOnActivePanelChanged;
            App.DockManager.PanelHidden += DockManagerOnPanelHidden;

            // watch for hotkeys activated via the mainform plugin
            HotKeyManager.HotKeyEvent += HotKeyManagerOnHotKeyEvent;
            base.Activate();
        }

        private void HotKeyManagerOnHotKeyEvent(string action)
        {
            switch (action)
            {
                case "Activate_Road_Search":
                    if (_isFunctionActive)
                    {
                        _searchPanel.SearchRoad_Click(null, null);
                    }
                    else
                    {
                        SearchTool_Click(null, null);
                    }
                    break;
            }
        }

        public override void Deactivate()
        {
            App.HeaderControl.Remove(PluginKey);
            App.DockManager.Remove(PluginKey);

            App.DockManager.ActivePanelChanged -= DockManagerOnActivePanelChanged;
            App.DockManager.PanelHidden -= DockManagerOnPanelHidden;

            base.Deactivate();
        }

        private void AddSearchMapFunction()
        {
            if (_mapFunction == null)
            {
                _mapFunction = new MapFunctionSearch(_searchPanel, App.SerializationManager.CurrentProjectFile)
                {
                    // set this to allow us to perform map panel selections from inside MapFunctionSearch.cs
                    TabDockingControl = App.DockManager as TabDockingControl,
                };
                _mapFunction.FunctionActivated += OnMapFunctionOnFunctionActivated;
            }
            if (!App.Map.MapFunctions.Contains(_mapFunction))
            {
                App.Map.MapFunctions.Add(_mapFunction);
            }
            _mapFunction.Map = App.Map;
        }

        private void OnMapFunctionOnFunctionActivated(object sender, EventArgs eventArgs)
        {
            _searchPanel.ActivateSearchModeButton();
        }

        private void DockManagerOnActivePanelChanged(object sender, DockablePanelEventArgs dockablePanelEventArgs)
        {
            if (_isFunctionActive) return;  // no need to do anything if this already the active tool

            var key = dockablePanelEventArgs.ActivePanelKey;
            var map = App.Map as Map;
            if (map == null) return;

            if (SdrConfig.User.Go2ItUserSettings.Instance.AdminModeActive) return;

            if (key.StartsWith("kMap_"))
            {
                // check if the search function exists for this map
                AddSearchMapFunction();
                // event binding to watch for function mode changes (to deactivate the tool)
                map.FunctionModeChanged += MapOnFunctionModeChanged;
                // validate this is an active toolpanel
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
                _searchPanel.DeactivateSearchModeButtons();
            }
        }

        private void DockManagerOnPanelHidden(object sender, DockablePanelEventArgs e)
        {
            if (!_isFunctionActive) return;  // dont waste time if this is not the active tool

            _searchPanel.ClearSearches();

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
                    // remove local map event binding if it exists here
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
                    // remove any local map binding if it exists
                    if (_mapFunction != null)
                    {
                        _isFunctionActive = false;
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
            if (App.Map == null) return;
            // update the prefs for tracking the active tools modes and panels
            App.Map.FunctionMode = FunctionMode.None;
            App.DockManager.SelectPanel(PluginKey);
        }
    }
}