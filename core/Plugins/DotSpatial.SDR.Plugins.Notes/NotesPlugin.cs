using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using DotSpatial.SDR.Controls;
using DotSpatial.SDR.Plugins.Notes.Properties;
using SdrConfig = SDR.Configuration;

namespace DotSpatial.SDR.Plugins.Notes
{
    public class NotesPlugin : Extension
    {
        #region Constants and Fields

        private const string HomeMenuKey = HeaderControl.HomeRootItemKey;
        private const string PluginCaption = "Click to Create, Edit and Delete Notes";

        private SimpleActionItem _actionBtn;  // enable and disable button if notes layer is present
        private MapFunctionNotes _mapFunction;

        private bool _isFunctionActive;  // is the current function active
        #endregion

        #region Public Methods

        public override void Activate()
        {
            _actionBtn = new SimpleActionItem(HomeMenuKey, "Notes", NotesTool_Click)
            {
                GroupCaption = "Create_Edit_Delete_Note",
                ToolTipText = PluginCaption,
                SmallImage = Resources.notes_16,
                LargeImage = Resources.notes_32,
                Enabled = false
            };
            App.HeaderControl.Add(_actionBtn);  // add control button to application header
  
            // hotkeys for this plugin
            HotKeyManager.AddHotKey(new HotKey(Keys.F8, "Delete Note"), "Delete_Note");
            HotKeyManager.AddHotKey(new HotKey(Keys.F9, "Add/Edit Note"), "Add_Edit_Note");

            App.DockManager.ActivePanelChanged += DockManagerOnActivePanelChanged;
            App.DockManager.PanelHidden += DockManagerOnPanelHidden;

            SdrConfig.Project.Go2ItProjectSettings.Instance.NotesLayerChanged += InstanceOnNotesLayerChanged;

            // watch for hotkeys activated via the mainform plugin
            HotKeyManager.HotKeyEvent += HotKeyManagerOnHotKeyEvent;

            base.Activate();
        }

        private void HotKeyManagerOnHotKeyEvent(string action)
        {
            switch (action)
            {
                case "Delete_Note":
                    if (_isFunctionActive && _actionBtn.Enabled)
                    {
                        _mapFunction.HotKeyDeleteNote();
                    }
                    break;
                case "Add_Edit_Note":
                    if (_actionBtn.Enabled)
                    {
                        NotesTool_Click(null, null);
                    }
                    break;
            }
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();

            App.DockManager.ActivePanelChanged -= DockManagerOnActivePanelChanged;
            App.DockManager.PanelHidden -= DockManagerOnPanelHidden;

            SdrConfig.Project.Go2ItProjectSettings.Instance.NotesLayerChanged -= InstanceOnNotesLayerChanged;

            base.Deactivate();
        }

        private void InstanceOnNotesLayerChanged(object sender, EventArgs eventArgs)
        {
            // determine if a new notes layer has been set and reflect enabled state of button
            _actionBtn.Enabled = SdrConfig.Project.Go2ItProjectSettings.Instance.NotesLayer.Length > 0;
        }

        private void MapFunctionOnFunctionDeactivated(object sender, EventArgs eventArgs)
        {
            _isFunctionActive = false;
        }

        private void MapFunctionOnFunctionActivated(object sender, EventArgs eventArgs)
        {
            // no tool use allowed during admin mode
            if (SdrConfig.User.Go2ItUserSettings.Instance.AdminModeActive) return;
            // set the function to active and update the cursor
            _isFunctionActive = true;
            App.Map.Cursor = Cursors.Cross;
        }

        private void DockManagerOnActivePanelChanged(object sender, DockablePanelEventArgs e)
        {
            var key = e.ActivePanelKey;
            var map = App.Map as Map;
            if (map == null) return;

            if (key.StartsWith("kMap_"))
            {
                // check if the notes function exists for this map
                AddNotesMapFunctionToMap();
                // event binding to watch for function mode changes (to deactivate the tool)
                map.FunctionModeChanged += MapOnFunctionModeChanged;
                // if this is the currently active tool then activate the tool for this map
                if (App.Map.FunctionMode == FunctionMode.None && _isFunctionActive)
                {
                    if (_mapFunction != null)
                    {
                        _mapFunction.Activate();
                    }
                }
            }
            else if (key.StartsWith("kPanel_"))
            {
                // a kPanel tool has been selected check if this tool is active and deactivate if so
                if (App.Map.FunctionMode == FunctionMode.None && _isFunctionActive)
                {
                    if (_mapFunction != null)  // validate we have a map function
                    {
                        _mapFunction.Deactivate();
                    }
                }
            }
        }

        private void DockManagerOnPanelHidden(object sender, DockablePanelEventArgs e)
        {
            var key = e.ActivePanelKey;
            var map = App.Map as Map;
            if (map == null) return;

            if (key.StartsWith("kMap_"))
            {
                // remove the event binding on this map (since its being hidden) on function mode changes
                map.FunctionModeChanged -= MapOnFunctionModeChanged;
                // lets look and see if this tool is currently the active tool and deactivate it if so
                if (map.FunctionMode == FunctionMode.None && _isFunctionActive)
                {
                    if (_mapFunction != null)
                    {
                        _mapFunction.Deactivate();
                    }
                }
            }
        }

        private void MapOnFunctionModeChanged(object sender, EventArgs eventArgs)
        {
            // this is fired if any map function change occurs (ie the built in tools are used)
            if (!_isFunctionActive) return;  // dont waste time if this is not the active tool

            var map = sender as Map;
            if (map == null) return;
            if (_mapFunction == null) return;

            if (map.FunctionMode != FunctionMode.None)
            {
                _mapFunction.Deactivate();
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Create, Edit and Delete features on the "notes" layer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotesTool_Click    (object sender, EventArgs e)
        {
            if (App.Map == null) return;
            // update the prefs for tracking the active tool modes and panels
            App.Map.FunctionMode = FunctionMode.None;
            _mapFunction.Activate();
        }

        private void AddNotesMapFunctionToMap()
        {
            if (_mapFunction == null)
            {
                _mapFunction = new MapFunctionNotes();
                _mapFunction.FunctionActivated += MapFunctionOnFunctionActivated;
                _mapFunction.FunctionDeactivated += MapFunctionOnFunctionDeactivated;
            }
            if (!App.Map.MapFunctions.Contains(_mapFunction))
            {
                App.Map.MapFunctions.Add(_mapFunction);
            }
            _mapFunction.Map = App.Map;

            // lets see if this map has the notes layer available
            var layers = App.Map.GetFeatureLayers();
            foreach (IMapFeatureLayer mapLayer in layers)
            {
                if (mapLayer != null &&
                    String.IsNullOrEmpty(Path.GetFileNameWithoutExtension((mapLayer.DataSet.Filename)))) return;
                if (mapLayer == null) continue;
                var lyrName = Path.GetFileNameWithoutExtension((mapLayer.DataSet.Filename));
                if (lyrName == SdrConfig.Project.Go2ItProjectSettings.Instance.NotesLayer)
                {
                    _actionBtn.Enabled = true;
                    _mapFunction.NotesLayer = mapLayer;
                    return;
                }
                _actionBtn.Enabled = false;
            }
        }
        #endregion
    }
}