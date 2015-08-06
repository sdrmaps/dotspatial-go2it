using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using DotSpatial.Data;
using DotSpatial.SDR.Controls;
using DotSpatial.Symbology;
using DotSpatial.Topology;
using Go2It.Properties;

namespace Go2It
{
    public class OverviewMap : Extension
    {
        [Import("Shell")]
        internal ContainerControl Shell { get; set; }

        private ProjectManager ProjectManager { get; set; }
        private OverviewMapForm _overviewMapForm;
        private readonly Dictionary<string, Map> _thumbMaps = new Dictionary<string, Map>();

        private Map _overviewMap;
        private FeatureSet _marker;
        private MapLineLayer _markerLayer;

        private const string HomeMenuKey = DotSpatial.Controls.Header.HeaderControl.HomeRootItemKey;

        public override void Activate()
        {
            App.HeaderControl.Add(new SimpleActionItem(HomeMenuKey, "OverviewMap", ToggleOverviewTool_Click)
            {
                GroupCaption = "Overview_Toggle",
                ToolTipText = "Toggle Overview Map Visibility",
                SmallImage = Resources.legend_16,
                LargeImage = Resources.legend_32
            });

            ProjectManager = (ProjectManager)App.SerializationManager;
            App.DockManager.ActivePanelChanged += DockManagerOnActivePanelChanged;
            App.DockManager.PanelHidden += DockManagerOnPanelHidden;
            base.Activate();
        }

        private void DockManagerOnPanelHidden(object sender, DockablePanelEventArgs e)
        {
            var dockControl = (TabDockingControl)sender;
            var key = e.ActivePanelKey;

            DockPanelInfo dockInfo;
            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;

            if (!key.StartsWith("kMap_")) return;

            // var map = (Map)dockInfo.DotSpatialDockPanel.InnerControl;
            var map = App.Map;
            if (map == null) return;

            var mapFrame = map.MapFrame as EventMapFrame;
            if (mapFrame == null) return;

            mapFrame.ViewExtentsChanged -= MapFrameOnViewExtentsChanged;
            _overviewMap.MapFrame.DrawingLayers.Remove(_markerLayer);
        }

        private void DockManagerOnActivePanelChanged(object sender, DockablePanelEventArgs e)
        {
            DockPanelInfo dockInfo;
            var dockControl = (DockingControl)sender;
            var key = e.ActivePanelKey;

            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;
            if (!key.StartsWith("kMap_")) return;

            var map = (Map)dockInfo.DotSpatialDockPanel.InnerControl;
            if (map == null) return;

            var mapFrame = map.MapFrame as EventMapFrame;
            if (mapFrame == null) return;

            mapFrame.ViewExtentsChanged += MapFrameOnViewExtentsChanged;

            if (_overviewMapForm == null)
            {
                _overviewMapForm = new OverviewMapForm();
                _overviewMapForm.ResizeEnd += OverviewMapFormOnResize;
                // create the drawing layer for our overviewmap
                _marker = new FeatureSet(FeatureType.Line);
                _markerLayer = new MapLineLayer(_marker);
                _markerLayer.Symbolizer = new LineSymbolizer(Color.Yellow, 2);
            }

            Map overViewMap;
            if (!_thumbMaps.TryGetValue(key, out overViewMap))
            {
                overViewMap = ProjectManager.CreateNewMap(key + "_thumb");
                overViewMap.Layers.AddRange(map.Layers);
                _thumbMaps.Add(key, overViewMap);
            }

            overViewMap.Dock = DockStyle.Fill;
            overViewMap.Height = _overviewMapForm.Height;
            overViewMap.Width = _overviewMapForm.Width;

            _overviewMapForm.Controls.Clear();
            _overviewMap = overViewMap;
            _overviewMap.MapFrame.DrawingLayers.Add(_markerLayer);
            _overviewMapForm.Controls.Add(_overviewMap);
        }

        private void MapFrameOnViewExtentsChanged(object sender, ExtentArgs extentArgs)
        {
            if (_overviewMapForm.Visible)  // make sure the overview form is visible
            {
                DrawViewExtentPolygon();
                _overviewMap.Invalidate();
            }
        }

        private void DrawViewExtentPolygon()
        {
            var mapFrame = App.Map.MapFrame as EventMapFrame;
            if (mapFrame == null) return;

            double maxX = mapFrame.ViewExtents.MaxX;
            double maxY = mapFrame.ViewExtents.MaxY;
            double minX = mapFrame.ViewExtents.MinX;
            double minY = mapFrame.ViewExtents.MinY;

            var coords = new List<Coordinate>
            {
                new Coordinate(minX, maxY),
                new Coordinate(maxX, maxY),
                new Coordinate(maxX, minY),
                new Coordinate(minX, minY),
                new Coordinate(minX, maxY)
            };

            var ls = new LineString(coords);
            _marker.Features.Clear();
            _marker.AddFeature(ls);
            _overviewMap.MapFrame.Invalidate();
        }

        private void OverviewMapFormOnResize(object sender, EventArgs eventArgs)
        {
            // zoom to max extent
            var map = (Map)_overviewMapForm.Controls[0];
            map.ZoomToMaxExtent();
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            App.DockManager.ActivePanelChanged -= DockManagerOnActivePanelChanged;
            _overviewMapForm.ResizeEnd -= OverviewMapFormOnResize;

            var map = App.Map as Map;
            if (map == null) return;

            var mapFrame = App.Map.MapFrame as EventMapFrame;
            if (mapFrame == null) return;

            mapFrame.ViewExtentsChanged -= MapFrameOnViewExtentsChanged;
            
            base.Deactivate();
        }

        private void ToggleOverviewTool_Click(object sender, EventArgs e)
        {
            if (_overviewMapForm.Visible == false)
            {
                DrawViewExtentPolygon();
                _overviewMap.Invalidate();
                _overviewMapForm.Show(Shell);
                _overviewMapForm.Focus();
            }
            else
            {
                _overviewMapForm.Hide();
            }
        }
    }
}
