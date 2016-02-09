using System;
using System.Diagnostics;
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

        private bool _isFunctionActive;  // eliminate redundant calls on hide/show/functionmode changes
        private bool _isPluginActive;  // flag to determine if the plugin has already been activated on ali mode change

        #endregion

        #region Properties
        public static AliMode CurrentAliMode
        {
            get
            {
                var aliMode = SdrConfig.Project.Go2ItProjectSettings.Instance.AliMode;
                if (aliMode.Length <= 0) return AliMode.Disabled;
                AliMode am;
                Enum.TryParse(aliMode, true, out am);
                return am;
            }
        }

        public static AliAvl CurrentAliAvl
        {
            get
            {
                if (SdrConfig.Project.Go2ItProjectSettings.Instance.AliUseNetworkfleet)
                {
                    return AliAvl.Networkfleet;
                }
                if (SdrConfig.Project.Go2ItProjectSettings.Instance.AliUseEnterpolAvl)
                {
                    return AliAvl.Enterpolavl;
                }
                return AliAvl.Disabled;
            }
        }
        #endregion

        public override void Activate()
        {
            // watch for the change of alimode/aliavl to activate/deactivate this plugin as needed
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliModeChanged += OnAliModeChanged;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliUseNetworkfleetChanged += OnAliModeChanged;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliUseEnterpolAvlChanged += OnAliModeChanged;

            // determine if the plugin is currently activated or not
            if (CurrentAliMode != AliMode.Disabled || CurrentAliAvl != AliAvl.Disabled)
            {
                ActivateAliPlugin();  // go ahead and activate the plugin now
            }
            if (_mapFunction != null)
            {
                _mapFunction.ConfigureAliClient(CurrentAliMode, CurrentAliAvl);  // configure the ali client for mode type
            }
            base.Activate();
        }

        private void ActivateAliPlugin()
        {
            // add in the button controls for this plugin to the header
            App.HeaderControl.Add(new SimpleActionItem(HomeMenuKey, "ALI", AliTool_Click)
            {
                GroupCaption = "Ali_Interface",
                ToolTipText = "Click to Connect with ALI",
                SmallImage = Resources.info_16,
                LargeImage = Resources.info_32,
                Key = PluginKey,
            });
            // generate the ali interface display panel and add it to the tool panel
            _aliPanel = new AliPanel();
            _dockPanel = new DockablePanel(PluginKey, PluginCaption, _aliPanel, DockStyle.Fill);
            App.DockManager.Add(_dockPanel);
            // plugin wide events
            App.DockManager.ActivePanelChanged += DockManagerOnActivePanelChanged;
            App.DockManager.PanelHidden += DockManagerOnPanelHidden;
            // initialize the full map ali function now
            _mapFunction = new MapFunctionAli(_aliPanel);
            _isPluginActive = true;  // set the status of the plugin to active
        }

        public override void Deactivate()
        {
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliModeChanged -= OnAliModeChanged;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliUseNetworkfleetChanged -= OnAliModeChanged;
            // TODO:
            // SdrConfig.Project.Go2ItProjectSettings.Instance.AliUseEnterpolAvlChanged -= OnAliModeChanged;
            
            if (_isPluginActive)
            {
                DeactivateAliPlugin();
            }
            base.Deactivate();
        }

        private void DeactivateAliPlugin()
        {
            App.HeaderControl.Remove(PluginKey);
            App.DockManager.Remove(PluginKey);

            App.DockManager.ActivePanelChanged -= DockManagerOnActivePanelChanged;
            App.DockManager.PanelHidden -= DockManagerOnPanelHidden;

            _isPluginActive = false;  // set our status flag back to false
        }

        private void OnAliModeChanged(object sender, EventArgs eventArgs)
        {
            if (CurrentAliMode != AliMode.Disabled || CurrentAliAvl != AliAvl.Disabled)
            {
                if (!_isPluginActive)  // check if it has already been activated
                {
                    ActivateAliPlugin();  // activate the ali plugin now
                }
                if (_mapFunction != null)
                {
                    _mapFunction.ConfigureAliClient(CurrentAliMode, CurrentAliAvl);
                }
            }
            else  // ali interface mode has been set to disabled
            {
                if (_isPluginActive)  // check if it has ever been activated previous to this mode change
                {
                    DeactivateAliPlugin();  // go ahead and deactivate the plugin now
                }
            }
        }

        private void AddAliMapFunction()
        {
            if (_mapFunction == null)
            {
                _mapFunction = new MapFunctionAli(_aliPanel);
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
            if (_isFunctionActive) return; // no need to do anything if this already the active tool

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
                // if avl is activated then assign the paint event
                if (SdrConfig.Project.Go2ItProjectSettings.Instance.AliUseEnterpolAvl) // ||
                    // SdrConfig.Project.Go2ItProjectSettings.Instance.AliUseNetworkfleet)
                {
                    _mapFunction.AddAvlMapPaintEvent();
                }
                // check that this tool is the active tool of the map now
                if (App.Map.FunctionMode == FunctionMode.None &&
                    SdrConfig.User.Go2ItUserSettings.Instance.ActiveFunctionPanel == PluginKey)
                {
                    if (_mapFunction != null)
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
                    if (_mapFunction != null) // validate we have a map function
                    {
                        App.Map.Cursor = Cursors.Arrow;
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
                // if avl is activated remove the paint event from this map
                if (SdrConfig.Project.Go2ItProjectSettings.Instance.AliUseEnterpolAvl) // ||
                    // SdrConfig.Project.Go2ItProjectSettings.Instance.AliUseNetworkfleet)
                {
                    _mapFunction.RemoveAvlMapPaintEvent();
                }
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