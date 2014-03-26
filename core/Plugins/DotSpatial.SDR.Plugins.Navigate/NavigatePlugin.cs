using System;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using DotSpatial.Data;
using DotSpatial.SDR.Controls;
using DotSpatial.SDR.Plugins.Navigate.Properties;
using DotSpatial.Symbology;
using DotSpatial.Topology;

namespace DotSpatial.SDR.Plugins.Navigate
{
    public class NavigatePlugin : Extension
    {
        #region Constants and Fields

        private Map _map;
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
                GroupCaption = "Navigate_Zoom_previous",
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

            App.DockManager.ActivePanelChanged += DockManagerOnActivePanelChanged;
            App.DockManager.PanelHidden += DockManagerOnPanelHidden;

            base.Activate();
        }

        private void DockManagerOnPanelHidden(object sender, DockablePanelEventArgs e)
        {
            DockPanelInfo dockInfo;
            var dockControl = (TabDockingControl)sender;
            var key = e.ActivePanelKey;
            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;

            if (!key.StartsWith("kMap_")) return;
            // grab the active map from the dockinginfo object
            _map = (Map)dockInfo.DotSpatialDockPanel.InnerControl;

            // setup all the events for navigation controls on the map
            _map.Layers.LayerSelected -= LayersOnLayerSelected;
            _map.MapFrame.ViewExtentsChanged -= MapFrameOnViewExtentsChanged;
        }

        private void DockManagerOnActivePanelChanged(object sender, DockablePanelEventArgs e)
        {
            DockPanelInfo dockInfo;
            var dockControl = (TabDockingControl)sender;
            var key = e.ActivePanelKey;
            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;

            if (!key.StartsWith("kMap_")) return;
            // grab the active map from the dockinginfo object
            _map = (Map)dockInfo.DotSpatialDockPanel.InnerControl;
            var mapFrame = _map.MapFrame as MapFrame;
            // posible set the mapframe here?

            // setup all the events for navigation controls on the map
            _map.Layers.LayerSelected += LayersOnLayerSelected;
            _map.MapFrame.ViewExtentsChanged += MapFrameOnViewExtentsChanged;
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            _map.Layers.LayerSelected -= LayersOnLayerSelected;
            _map.MapFrame.ViewExtentsChanged -= MapFrameOnViewExtentsChanged;
            base.Deactivate();
        }

        #endregion

        #region Methods

        private void LayersOnLayerSelected(object sender, LayerSelectedEventArgs layerSelectedEventArgs)
        {
            _zoomToLayer.Enabled = _map.Layers.SelectedLayer != null;
        }

        private void MapFrameOnViewExtentsChanged(object sender, ExtentArgs extentArgs)
        {
            var mapFrame = sender as MapFrame;
            if (mapFrame == null) return;
            _zoomNext.Enabled = mapFrame.CanZoomToNext();
            _zoomPrevious.Enabled = mapFrame.CanZoomToPrevious();
        }

        /// <summary>
        /// Move (Pan) the map
        /// </summary>
        private void PanTool_Click(object sender, EventArgs e)
        {
            _map.FunctionMode = FunctionMode.Pan;
        }

        /// <summary>
        /// Zoom In
        /// </summary>
        private void ZoomIn_Click(object sender, EventArgs e)
        {
            _map.FunctionMode = FunctionMode.ZoomIn;
        }

        /// <summary>
        /// Zoom to previous extent
        /// </summary>
        private void ZoomNext_Click(object sender, EventArgs e)
        {
            _map.MapFrame.ZoomToNext();
        }

        /// <summary>
        /// Zoom Out
        /// </summary>
        private void ZoomOut_Click(object sender, EventArgs e)
        {
            _map.FunctionMode = FunctionMode.ZoomOut;
        }

        /// <summary>
        /// Zoom to previous extent
        /// </summary>
        private void ZoomPrevious_Click(object sender, EventArgs e)
        {
            _map.MapFrame.ZoomToPrevious();
        }

        /// <summary>
        /// Zoom to the currently selected layer
        /// </summary>
        private void ZoomToLayer_Click(object sender, EventArgs e)
        {
            var layer = _map.Layers.SelectedLayer;
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
            _map.ZoomToMaxExtent();
        }

        private void ZoomToLayer(IMapLayer layerToZoom)
        {
            const double eps = 1e-7;
            IEnvelope layerEnvelope = layerToZoom.Extent.ToEnvelope();
            if (layerEnvelope.Width > eps && layerEnvelope.Height > eps)
            {
                layerEnvelope.ExpandBy(layerEnvelope.Width / 10, layerEnvelope.Height / 10); // work item #84
            }
            else
            {
                const double zoomInFactor = 0.05; // fixed zoom-in by 10% - 5% on each side
                var newExtentWidth = _map.ViewExtents.Width * zoomInFactor;
                var newExtentHeight = _map.ViewExtents.Height * zoomInFactor;
                layerEnvelope.ExpandBy(newExtentWidth, newExtentHeight);
            }
            _map.ViewExtents = layerEnvelope.ToExtent();
        }

        #endregion
    }
}