using System;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using DotSpatial.SDR.Plugins.ALI.Properties;
using SdrConfig = SDR.Configuration;

namespace DotSpatial.SDR.Plugins.ALI
{
    public class AliPlugin : Extension
    {
        #region Constants and Fields
        
        private const string HomeMenuKey = HeaderControl.HomeRootItemKey;

        private const string PluginKey = "kPanel_Ali";
        private const string PluginCaption = "Automatic Location Information (ALI) Interface";

        private MapFunctionAli _mapFunction;
        private AliPanel _aliPanel;
        private DockablePanel _dockPanel;

        private bool _isFunctionActive;  // eliminate redundant calls on hide/show/mode change
        #endregion

        public override void Activate()
        {
            // add in the button controls for this plugin to the header
            App.HeaderControl.Add(new SimpleActionItem(HomeMenuKey, "ALI", AliTool_Click)
            {
                GroupCaption = "Ali_Interface",
                ToolTipText = "Click to Connect with ALI",
                SmallImage = Resources.info_16,
                LargeImage = Resources.info_32
            });
            // generate the ali interface display panel and add it to the tool panel
            _aliPanel = new AliPanel();
            _dockPanel = new DockablePanel(PluginKey, PluginCaption, _aliPanel, DockStyle.Fill);
            App.DockManager.Add(_dockPanel);
            App.DockManager.ActivePanelChanged += DockManagerOnActivePanelChanged;
            App.DockManager.PanelHidden += DockManagerOnPanelHidden;

            // initialize the ali function and check if it should/can run
            _mapFunction = new MapFunctionAli(_aliPanel);

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

        private void AddAliMapFunction()
        {
            if (_mapFunction == null)
            {
                _mapFunction = new MapFunctionAli(_aliPanel);
                // TODO: not sure we need this on this function yet
                // handle the functionactivated event, and fire button toggle for visual display
                // _mapFunction.FunctionActivated += OnMapFunctionOnFunctionActivated;
            }
            if (!App.Map.MapFunctions.Contains(_mapFunction))
            {
                App.Map.MapFunctions.Add(_mapFunction);
            }
            _mapFunction.Map = App.Map;
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
            }
        }

        private void DockManagerOnActivePanelChanged(object sender, DockablePanelEventArgs e)
        {
            if (_isFunctionActive) return;  // no need to do anything if this already the active tool

            var key = e.ActivePanelKey;
            var map = App.Map as Map;
            if (map == null) return;

            if (SdrConfig.User.Go2ItUserSettings.Instance.AdminModeActive) return;

            if (key.StartsWith("kMap_"))
            {
                // check if the ALI function exists for this map
                AddAliMapFunction();
                // event binding to watch for function mode changes (to deactivate the tool)
                map.FunctionModeChanged += MapOnFunctionModeChanged;
                // check that this tool is the active tool of the map now
                if (App.Map.FunctionMode == FunctionMode.None &&
                    SdrConfig.User.Go2ItUserSettings.Instance.ActiveFunctionPanel == PluginKey)
                {
                    if (_mapFunction != null)
                    {
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
                        _isFunctionActive = true;
                        _mapFunction.Activate();
                    }
                }
            }
        }

        private void DockManagerOnPanelHidden(object sender, DockablePanelEventArgs e)
        {
            if (!_isFunctionActive) return;  // dont waste time if this is not the active tool

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
                    if (_mapFunction != null)
                    {
                        _isFunctionActive = false;
                        _mapFunction.Deactivate();
                    }
                }
            }
        }

        /// <summary>
        /// ALI Interface Display Tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AliTool_Click(object sender, EventArgs e)
        {
            if (App.Map == null) return;
            // update the prefs for tracking the active tools modes and panels
            App.Map.FunctionMode = FunctionMode.None;
            App.DockManager.SelectPanel(PluginKey);
        }
    }
}