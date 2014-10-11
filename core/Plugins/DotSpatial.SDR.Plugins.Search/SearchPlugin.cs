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

        private bool _isFunctionActive;  // flag to eliminate redundant MapFunctionSearch.OnActivate() calls
        #endregion

        public override void Activate()
        {
            App.HeaderControl.Add(new SimpleActionItem(HomeMenuKey, "Search", SearchTool_Click)
            {
                GroupCaption = "Search",
                ToolTipText = "Search for Attributes and Features",
                SmallImage = Resources.search_16,
                LargeImage = Resources.search_32
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
                    // set this to allow us to perform map panel selections from inside MapFunctionSearch.cs
                    TabDockingControl = App.DockManager as TabDockingControl
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
            var dockControl = (TabDockingControl)sender;
            var key = dockablePanelEventArgs.ActivePanelKey;

            DockPanelInfo dockInfo;
            
            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;

            var map = App.Map as Map;
            if (map == null) return;

            // if the function is already active, then stop wasting time here
            if (_isFunctionActive) return;  // not even sure if this is doing anythi9ng anymore investigate

            if (key.StartsWith("kMap_"))
            {
                // check if the search function exists for this map
                AddSearchMapFunction();
                // setup an event binding to watch for any time the functionmode changes
                App.Map.FunctionModeChanged += MapOnFunctionModeChanged;
                // check that this tool is the active tool of the map now
                if (App.Map.FunctionMode == FunctionMode.None &&
                    SdrConfig.User.Go2ItUserSettings.Instance.ActiveFunctionPanel == PluginKey)
                {
                    // add local map event binding here if it exists
                    App.Map.Cursor = Cursors.Default;
                    _isFunctionActive = true;
                    _mapFunction.Activate();
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
                        App.Map.Cursor = Cursors.Default;
                        _isFunctionActive = true;
                        _mapFunction.Activate();
                    }
                }
            }
        }

        private void MapOnFunctionModeChanged(object sender, EventArgs eventArgs)
        {
            var map = sender as Map;
            if (map == null) return;
            if (_mapFunction == null) return;

            if (map.FunctionMode != FunctionMode.None)
            {
                _isFunctionActive = false;
                // TODO: not sure if we need to fully deactivate yet...
                _mapFunction.Deactivate();
                _searchPanel.DeactivateSearchModeButtons();
            }
            else
            {
                if (SdrConfig.User.Go2ItUserSettings.Instance.ActiveFunctionPanel == PluginKey)
                {
                    App.Map.Cursor = Cursors.Default;
                    _isFunctionActive = true;
                    _mapFunction.Activate();
                }
            }
        }

        private void DockManagerOnPanelHidden(object sender, DockablePanelEventArgs dockablePanelEventArgs)
        {
            _searchPanel.ClearSearches();

            var dockControl = (TabDockingControl)sender;
            var key = dockablePanelEventArgs.ActivePanelKey;

            DockPanelInfo dockInfo;
            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;

            var map = App.Map as Map;
            if (map == null) return;

            if (key.StartsWith("kMap_"))
            {
                // remove the event binding on this map (since its being hidden) on function mode changes
                map.FunctionModeChanged -= MapOnFunctionModeChanged;

                // lets look and see if this tool is currently the active tool and deactivate it if so
                if (App.Map.FunctionMode == FunctionMode.None &&
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
            // update the prefs for tracking the active tools modes and panels
            App.Map.FunctionMode = FunctionMode.None;
            App.DockManager.SelectPanel(PluginKey);
        }
    }
}