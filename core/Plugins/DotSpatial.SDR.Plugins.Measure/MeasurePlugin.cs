﻿using System;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using DotSpatial.SDR.Controls;
using DotSpatial.SDR.Plugins.Measure.Properties;
using SdrConfig = SDR.Configuration;

namespace DotSpatial.SDR.Plugins.Measure
{
    public class MeasurePlugin : Extension
    {
        #region Constants and Fields

        private const string HomeMenuKey = HeaderControl.HomeRootItemKey;

        private const string PluginKey = "kPanel_Measure";
        private const string PluginCaption = "Measure Area and Distance";

        private MapFunctionMeasure _mapFunction;
        private MeasurePanel _measurePanel;
        private DockablePanel _dockPanel;

        private bool _isFunctionActive;  // flag to reduce redundant calls

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
            if (_mapFunction == null)
            {
                _mapFunction = new MapFunctionMeasure(_measurePanel);
                // handle the button checking to enable and disable them visually
                _mapFunction.FunctionActivated += OnMapFunctionOnFunctionActivated;
                _mapFunction.FunctionDeactivated += OnMapFunctionOnFunctionDeactivated;
            }
            if (!App.Map.MapFunctions.Contains(_mapFunction))
            {
                App.Map.MapFunctions.Add(_mapFunction);
            }
            _mapFunction.Map = App.Map;
        }

        private void OnMapFunctionOnFunctionDeactivated(object sender, EventArgs args)
        {
            // on activation of the function check the measurement mode button in use
            _measurePanel.ToggleMeasurementModeButton(false);
        }

        private void OnMapFunctionOnFunctionActivated(object sender, EventArgs args)
        {
            // on activation of the function check the measurement mode button in use
            _measurePanel.ToggleMeasurementModeButton(true);
        }

        private void DockManagerOnActivePanelChanged(object sender, DockablePanelEventArgs e)
        {
            var dockControl = (TabDockingControl)sender;
            var key = e.ActivePanelKey;

            DockPanelInfo dockInfo;
            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;

            var map = App.Map as Map;
            if (map == null) return;

            // if the function is already active, then stop wasting time here
            if (_isFunctionActive) return;

            if (key.StartsWith("kMap_"))
            {
                // check if the measurement function exists for this map
                AddMeasureMapFunction();
                // event binding to watch for function mode changes
                map.FunctionModeChanged += MapOnFunctionModeChanged;
                // check that this tool is the active tool of the map now
                if (App.Map.FunctionMode == FunctionMode.None &&
                    SdrConfig.User.Go2ItUserSettings.Instance.ActiveFunctionPanel == PluginKey)
                {
                    // add local map event binding
                    map.MouseLeave += MapOnMouseLeave;
                    // set the cursor and activate the measure mode
                    App.Map.Cursor = Cursors.Cross;
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
                        // set the cursor and activate the measure mode
                        App.Map.Cursor = Cursors.Cross;
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
                _mapFunction.Deactivate();
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
                    // remove local map event binding
                    map.MouseLeave -= MapOnMouseLeave;
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
                    // remove local map event binding
                    map.MouseLeave -= MapOnMouseLeave;
                    if (_mapFunction != null)
                    {
                        _isFunctionActive = false;
                        _mapFunction.Deactivate();
                    }
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
            // update the prefs for tracking the active tools modes and panels
            App.Map.FunctionMode = FunctionMode.None;
            App.DockManager.SelectPanel(PluginKey);
        }
    }
}