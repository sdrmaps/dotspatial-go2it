using System;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using DotSpatial.Data;
using DotSpatial.SDR.Controls;
using DotSpatial.SDR.Plugins.Navigate.Properties;
using DotSpatial.Symbology;
using GeoAPI.Geometries;

namespace DotSpatial.SDR.Plugins.Navigate
{
    public class NavigatePlugin : Extension
    {
        #region Constants and Fields

        private const string HomeMenuKey = HeaderControl.HomeRootItemKey;

        private SimpleActionItem _zoomNext;
        private SimpleActionItem _zoomPrevious;
        private SimpleActionItem _zoomToLayer;

        #endregion

        #region Public Methods

        public override void Activate()
        {
            App.HeaderControl.Add(new SimpleActionItem(HomeMenuKey, "Pan", PanTool_Click)
            {
                GroupCaption = "Navigate_Pan",
                ToolTipText = "Pan",
                SmallImage = Resources.pan_16,
                LargeImage = Resources.pan_32
            });
            App.HeaderControl.Add(new SimpleActionItem(HomeMenuKey, "In", ZoomIn_Click)
            {
                GroupCaption = "Navigate_Zoom_In",
                ToolTipText = "Zoom In",
                SmallImage = Resources.zoom_in_16,
                LargeImage = Resources.zoom_in_32
            });
            App.HeaderControl.Add(new SimpleActionItem(HomeMenuKey, "Out", ZoomOut_Click)
            {
                GroupCaption = "Navigate_Zoom_Out",
                ToolTipText = "Zoom Out",
                SmallImage = Resources.zoom_out_16,
                LargeImage = Resources.zoom_out_32
            });
            App.HeaderControl.Add(new SimpleActionItem(HomeMenuKey, "Extent", ZoomToMaxExtents_Click)
            {
                GroupCaption = "Navigate_Zoom_Max_Extent",
                ToolTipText = "Zoom to Max Extent",
                SmallImage = Resources.zoom_extent_16,
                LargeImage = Resources.zoom_extent_32
            });
            _zoomPrevious = new SimpleActionItem(HomeMenuKey, "Previous", ZoomPrevious_Click)
            {
                GroupCaption = "Navigate_Zoom_Previous",
                ToolTipText = "Previous Zoom View",
                SmallImage = Resources.zoom_prev_16,
                LargeImage = Resources.zoom_prev_32,
                Enabled = false
            };
            App.HeaderControl.Add(_zoomPrevious);
            _zoomNext = new SimpleActionItem(HomeMenuKey, "Next", ZoomNext_Click)
            {
                GroupCaption = "Navigate_Zoom_Next",
                ToolTipText = "Next Zoom View",
                SmallImage = Resources.zoom_next_16,
                LargeImage = Resources.zoom_next_32,
                Enabled = false
            };
            App.HeaderControl.Add(_zoomNext);
            _zoomToLayer = new SimpleActionItem(HomeMenuKey, "To Layer", ZoomToLayer_Click)
            {
                GroupCaption = "Navigate_Zoom_To_Layer",
                ToolTipText = "Zoom To Selected Layer",
                SmallImage = Resources.zoom_to_layer_16,
                LargeImage = Resources.zoom_to_layer_32,
                Enabled = false
            };
            App.HeaderControl.Add(_zoomToLayer);

            // hotkeys for this plugin
            HotKeyManager.AddHotKey(new HotKey(Keys.F1, "Zoom to Max Extent"), "Navigate_Zoom_Max_Extent");
            HotKeyManager.AddHotKey(new HotKey(Keys.F10, "Zoom Out"), "Navigate_Set_Zoom_Out");
            HotKeyManager.AddHotKey(new HotKey(Keys.F11, "Zoom In"), "Navigate_Set_Zoom_In");

            // dockmanager events for display of various maps on tab panels
            App.DockManager.ActivePanelChanged += DockManagerOnActivePanelChanged;
            App.DockManager.PanelHidden += DockManagerOnPanelHidden;

            // watch for hotkeys activated via the mainform plugin
            HotKeyManager.HotKeyEvent += HotKeyManagerOnHotKeyEvent;
            base.Activate();
        }

        private void MapOnFunctionModeChanged(object sender, EventArgs eventArgs)
        {

        }

        private void HotKeyManagerOnHotKeyEvent(string action)
        {
            switch (action)
            {
                case "Navigate_Zoom_Max_Extent":
                    ZoomToMaxExtents_Click(null, null);
                    break;
                case "Navigate_Set_Zoom_In":
                    App.Map.MapFrame.ZoomIn();
                    break;
                case "Navigate_Set_Zoom_Out":
                    App.Map.MapFrame.ZoomOut();
                    break;
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

            map.Layers.LayerSelected -= LayersOnLayerSelected;

            var mapFrame = map.MapFrame as EventMapFrame;
            if (mapFrame == null) return;

            mapFrame.ViewExtentsChanged -= MapFrameOnViewExtentsChanged;
            map.FunctionModeChanged -= MapOnFunctionModeChanged;
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

            map.Layers.LayerSelected += LayersOnLayerSelected;

            var mapFrame = map.MapFrame as EventMapFrame;
            if (mapFrame == null) return;

            mapFrame.ViewExtentsChanged += MapFrameOnViewExtentsChanged;
            map.FunctionModeChanged += MapOnFunctionModeChanged;

            _zoomNext.Enabled = mapFrame.CanZoomToNext();
            _zoomPrevious.Enabled = mapFrame.CanZoomToPrevious();
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            App.DockManager.ActivePanelChanged -= DockManagerOnActivePanelChanged;
            App.DockManager.PanelHidden -= DockManagerOnPanelHidden;

            HotKeyManager.HotKeyEvent -= HotKeyManagerOnHotKeyEvent;

            var map = App.Map as Map;
            if (map == null) return;

            map.Layers.LayerSelected -= LayersOnLayerSelected;
            map.FunctionModeChanged -= MapOnFunctionModeChanged;

            var mapFrame = App.Map.MapFrame as EventMapFrame;
            if (mapFrame == null) return;

            mapFrame.ViewExtentsChanged -= MapFrameOnViewExtentsChanged;
            base.Deactivate();
        }

        #endregion

        #region Methods

        private void LayersOnLayerSelected(object sender, LayerSelectedEventArgs layerSelectedEventArgs)
        {
            if (App.Map == null) return;
            _zoomToLayer.Enabled = App.Map.Layers.SelectedLayer != null;
        }

        private void MapFrameOnViewExtentsChanged(object sender, ExtentArgs extentArgs)
        {
            var mapFrame = sender as EventMapFrame;
            if (mapFrame == null) return;
            _zoomNext.Enabled = mapFrame.CanZoomToNext();
            _zoomPrevious.Enabled = mapFrame.CanZoomToPrevious();
        }

        /// <summary>
        /// Move (Pan) the map
        /// </summary>
        private void PanTool_Click(object sender, EventArgs e)
        {
            if (App.Map == null) return;
            App.Map.FunctionMode = FunctionMode.Pan;
        }

        /// <summary>
        /// Zoom In
        /// </summary>
        private void ZoomIn_Click(object sender, EventArgs e)
        {
            if (App.Map == null) return;
            App.Map.FunctionMode = FunctionMode.ZoomIn;
        }

        /// <summary>
        /// Zoom to previous extent
        /// </summary>
        private void ZoomNext_Click(object sender, EventArgs e)
        {
            if (App.Map == null) return;
            App.Map.MapFrame.ZoomToNext();
        }

        /// <summary>
        /// Zoom Out
        /// </summary>
        private void ZoomOut_Click(object sender, EventArgs e)
        {
            if (App.Map == null) return;
            App.Map.FunctionMode = FunctionMode.ZoomOut;
        }

        /// <summary>
        /// Zoom to previous extent
        /// </summary>
        private void ZoomPrevious_Click(object sender, EventArgs e)
        {
            if (App.Map == null) return;
            App.Map.MapFrame.ZoomToPrevious();
        }

        /// <summary>
        /// Zoom to the currently selected layer
        /// </summary>
        private void ZoomToLayer_Click(object sender, EventArgs e)
        {
            if (App.Map == null) return;
            var layer = App.Map.Layers.SelectedLayer;
            if (layer != null)
            {
                ZoomToLayer(layer);
            }
        }

        /// <summary>
        /// Zoom to maximum extents
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ZoomToMaxExtents_Click(object sender, EventArgs e)
        {
            if (App.Map == null) return;
            App.Map.ZoomToMaxExtent();
        }

        private void ZoomToLayer(IMapLayer layerToZoom)
        {
            const double eps = 1e-7;
            Envelope layerEnvelope = layerToZoom.Extent.ToEnvelope();
            if (layerEnvelope.Width > eps && layerEnvelope.Height > eps)
            {
                layerEnvelope.ExpandBy(layerEnvelope.Width / 10, layerEnvelope.Height / 10); // work item #84
            }
            else
            {
                const double zoomInFactor = 0.05; // fixed zoom-in by 10% - 5% on each side
                var newExtentWidth = App.Map.ViewExtents.Width * zoomInFactor;
                var newExtentHeight = App.Map.ViewExtents.Height * zoomInFactor;
                layerEnvelope.ExpandBy(newExtentWidth, newExtentHeight);
            }
            App.Map.ViewExtents = layerEnvelope.ToExtent();
        }

        #endregion
    }
}