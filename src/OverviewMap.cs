using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using DotSpatial.Data;
using DotSpatial.SDR.Controls;
using DotSpatial.Symbology;
using GeoAPI.Geometries;
using Go2It.Properties;
using NetTopologySuite.Geometries;

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
            App.HeaderControl.Add(new SimpleActionItem(HomeMenuKey, "Overview", ToggleOverviewTool_Click)
            {
                GroupCaption = "Overview_Toggle",
                ToolTipText = "Toggle Overview Map Visibility",
                SmallImage = Resources.legend_16,
                LargeImage = Resources.overview_32
            });

            ProjectManager = (ProjectManager)App.SerializationManager;
            App.DockManager.ActivePanelChanged += DockManagerOnActivePanelChanged;
            App.DockManager.PanelHidden += DockManagerOnPanelHidden;

            _marker = new FeatureSet(FeatureType.Line);
            _markerLayer = new MapLineLayer(_marker) {Symbolizer = new LineSymbolizer(Color.Yellow, 2)};

            base.Activate();
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

            mapFrame.ViewExtentsChanged -= MapFrameOnViewExtentsChanged;
            if (_overviewMap != null)
            {
                _overviewMap.MapFrame.DrawingLayers.Remove(_markerLayer);
            }
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

            Map overViewMap;
            if (!_thumbMaps.TryGetValue(key, out overViewMap))
            {
                overViewMap = ProjectManager.CreateNewMap(key + "_thumb");
                overViewMap.Layers.AddRange(map.Layers);
                overViewMap.Dock = DockStyle.Fill;
                _thumbMaps.Add(key, overViewMap);
            }
            _overviewMap = overViewMap;

            if (_overviewMapForm == null || _overviewMapForm.IsDisposed)
            {
                _overviewMapForm = new OverviewMapForm();
                _overviewMapForm.ResizeEnd += OverviewMapFormOnResize;
                _overviewMapForm.Closing += OverviewMapFormOnClosing;
            }

            if (_overviewMapForm.Visible)
            {
                AttachOverviewMap();
                DrawViewExtentPolygon();
            }
        }

        private void MapFrameOnViewExtentsChanged(object sender, ExtentArgs extentArgs)
        {
            if (_overviewMapForm == null) return;
            if (_overviewMapForm.Visible)
            {
                DrawViewExtentPolygon();
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

            var coords = new Coordinate[]
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
            if (_overviewMapForm == null) return;
            if (_overviewMapForm.Visible) // make sure the overview form is visible
            {
                // zoom to max extent
                var map = (Map) _overviewMapForm.Controls[0];
                map.ZoomToMaxExtent();
            }
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

        private void AttachOverviewMap()
        {
            _overviewMap.Height = _overviewMapForm.Height;
            _overviewMap.Width = _overviewMapForm.Width;
            _overviewMap.MapFrame.DrawingLayers.Add(_markerLayer);
            _overviewMapForm.Text = SDR.Configuration.Project.Go2ItProjectSettings.Instance.ActiveMapViewCaption + @" Overview";
            _overviewMapForm.Controls.Clear();
            _overviewMapForm.Controls.Add(_overviewMap);
            _overviewMap.ZoomToMaxExtent();
        }

        private void ToggleOverviewTool_Click(object sender, EventArgs e)
        {
            if (App.Map == null) return;

            if (_overviewMapForm == null || _overviewMapForm.IsDisposed)
            {
                _overviewMapForm = new OverviewMapForm();
                _overviewMapForm.ResizeEnd += OverviewMapFormOnResize;
                _overviewMapForm.Closing += OverviewMapFormOnClosing;
            }
            
            if (_overviewMapForm.Visible == false)
            {
                AttachOverviewMap();
                DrawViewExtentPolygon();
                _overviewMapForm.Show(Shell);
                _overviewMapForm.Focus();
            }
            else
            {
                _overviewMapForm.Close();
            }
        }

        private void OverviewMapFormOnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            _overviewMapForm.Controls.Remove(_overviewMap);
        }
    }
}
