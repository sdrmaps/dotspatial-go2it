using System;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using DotSpatial.Data;
using DotSpatial.SDR.Controls;
using DotSpatial.SDR.Plugins.Notes.Properties;
using DotSpatial.Symbology;
using DotSpatial.Topology;

namespace DotSpatial.SDR.Plugins.Notes
{
    public class NotesPlugin : Extension
    {
        #region Constants and Fields
        private const string HomeMenuKey = HeaderControl.HomeRootItemKey;

        private bool _isFunctionActive;  // eliminate redundant calls when active or not
        #endregion

        #region Public Methods

        public override void Activate()
        {
            // add control button to application header
            App.HeaderControl.Add(new SimpleActionItem(HomeMenuKey, "Notes", NotesTool_Click)
            {
                GroupCaption = "Create_Edit_Delete_Note",
                ToolTipText = "Click to Create/Edit/Delete Note",
                SmallImage = Resources.notes_16,
                LargeImage = Resources.notes_32
            });
            base.Activate();
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            App.DockManager.ActivePanelChanged -= DockManagerOnActivePanelChanged;
            App.DockManager.PanelHidden -= DockManagerOnPanelHidden;

            //var map = App.Map as Map;
            //if (map == null) return;

            //var mapFrame = App.Map.MapFrame as EventMapFrame;
            //if (mapFrame == null) return;

            base.Deactivate();
        }

        private void MapOnFunctionModeChanged(object sender, EventArgs eventArgs)
        {
            var map = (Map)sender;
            if (map != null)
            {
                var mapFrame = (EventMapFrame)map.MapFrame;
                if (mapFrame != null)
                {

                }
            }
        }

        private void DockManagerOnPanelHidden(object sender, DockablePanelEventArgs e)
        {
            var dockControl = (TabDockingControl)sender;
            var key = e.ActivePanelKey;

            DockPanelInfo dockInfo;
            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;

            if (!key.StartsWith("kMap_")) return;

            var map = (Map)dockInfo.DotSpatialDockPanel.InnerControl;
            if (map == null) return;

            var mapFrame = map.MapFrame as EventMapFrame;
            if (mapFrame == null) return;
        }

        private void DockManagerOnActivePanelChanged(object sender, DockablePanelEventArgs e)
        {
            var dockControl = (TabDockingControl)sender;
            var key = e.ActivePanelKey;

            DockPanelInfo dockInfo;
            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;

            if (!key.StartsWith("kMap_")) return;

            var map = (Map)dockInfo.DotSpatialDockPanel.InnerControl;
            if (map == null) return;

            var mapFrame = map.MapFrame as EventMapFrame;
            if (mapFrame == null) return;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create, Edit and Delete features on the "notes" layer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotesTool_Click(object sender, EventArgs e)
        {
            if (App.Map == null) return;
            // update the prefs for tracking the active tools modes and panels
            App.Map.FunctionMode = FunctionMode.None;
        }

        #endregion
    }
}