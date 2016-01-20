using System;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using DotSpatial.Projections;
using DotSpatial.SDR.Controls;

namespace Go2It
{
    /// <summary>
    /// This class is responsible for
    /// displaying the Lat, Lon coordinates
    /// in the status bar
    /// </summary>
    public class CoordinateDisplay
    {
        private Map _map;
        private readonly AppManager _appManager;
        private readonly ProjectionInfo _wgs84Projection = KnownCoordinateSystems.Geographic.World.WGS1984;
        private ProjectionInfo _currentMapProjection;
        private readonly StatusPanel _latLonStatusPanel;
        private bool _isWgs84 = true;
        private bool _showCoordinates;

        public CoordinateDisplay(AppManager app)
        {
            _latLonStatusPanel = new StatusPanel { Width = 400 };
            // set the application manager and the panel changed event to update coords
            _appManager = app;
            _appManager.DockManager.ActivePanelChanged += DockManagerOnActivePanelChanged;
            _appManager.DockManager.PanelHidden += DockManagerOnPanelHidden;
        }

        private void DockManagerOnPanelHidden(object sender, DockablePanelEventArgs e)
        {
            DockPanelInfo dockInfo;
            var dockControl = (TabDockingControl)sender;
            var key = e.ActivePanelKey;

            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;
            if (!key.StartsWith("kMap_")) return;
            if (_map == null) return;

            // remove active map tab binding
            _map.MouseMove -= map_MouseMove;
            _map.ProjectionChanged -= map_ProjectionChanged;
        }

        private void DockManagerOnActivePanelChanged(object sender, DockablePanelEventArgs dockablePanelEventArgs)
        {
            DockPanelInfo dockInfo;
            var dockControl = (DockingControl)sender;
            var key = dockablePanelEventArgs.ActivePanelKey;

            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;
            if (!key.StartsWith("kMap_")) return;

            _map = (Map)dockInfo.DotSpatialDockPanel.InnerControl;
            if (_map == null) return;

            var mapProjEsriString = _map.Projection.ToEsriString();
            _isWgs84 = (mapProjEsriString == KnownCoordinateSystems.Geographic.World.WGS1984.ToEsriString());
            _currentMapProjection = ProjectionInfo.FromEsriString(mapProjEsriString);

            // setup all the events for this coordinate display on the map
            _map.MouseMove += map_MouseMove;
            _map.ProjectionChanged += map_ProjectionChanged;
        }

        void map_ProjectionChanged(object sender, EventArgs e)
        {
            var mapProjEsriString = _map.Projection.ToEsriString();
            _isWgs84 = (mapProjEsriString == KnownCoordinateSystems.Geographic.World.WGS1984.ToEsriString());
            _currentMapProjection = ProjectionInfo.FromEsriString(mapProjEsriString);
        }

        public bool ShowCoordinates
        {
            get
            {
                return _showCoordinates;
            }
            set
            {
                _showCoordinates = value;
                if (_showCoordinates == false)
                {
                    _appManager.ProgressHandler.Remove(_latLonStatusPanel);
                }
                else
                {
                    _appManager.ProgressHandler.Add(_latLonStatusPanel);
                }
                _latLonStatusPanel.Caption = String.Empty;
            }
        }

        public string MapProjectionString
        {
            get { return _currentMapProjection.ToEsriString(); }
            set
            {
                _currentMapProjection = ProjectionInfo.FromEsriString(value);
                _isWgs84 = (_currentMapProjection.ToEsriString() == KnownCoordinateSystems.Geographic.World.WGS1984.ToEsriString());
            }
        }

        #region Coordinate Display

        private void map_MouseMove(object sender, MouseEventArgs e)
        {
            if (!ShowCoordinates)
            {
                return;
            }

            var mouseLocation = new System.Drawing.Point {X = e.X, Y = e.Y};
            var projCor = _map.PixelToProj(mouseLocation);

            var xy = new double[2];
            xy[0] = projCor.X;
            xy[1] = projCor.Y;

            var z = new double[1];
            if (!_isWgs84)
            {
                Reproject.ReprojectPoints(xy, z, _currentMapProjection, _wgs84Projection, 0, 1);
            }

            // convert to degrees, minutes, seconds
            var coord = new double[2];
            coord[0] = Math.Abs(xy[0]);
            coord[1] = Math.Abs(xy[1]);

            var d = new double[2];
            var m = new double[2];
            var s = new double[2];

            d[0] = Math.Floor(coord[0]);
            coord[0] -= d[0];
            coord[0] *= 60;

            m[0] = Math.Floor(coord[0]);
            coord[0] -= m[0];
            coord[0] *= 60;

            s[0] = Math.Floor(coord[0]);

            d[1] = Math.Floor(coord[1]);
            coord[1] -= d[1];
            coord[1] *= 60;

            m[1] = Math.Floor(coord[1]);
            coord[1] -= m[1];
            coord[1] *= 60;

            s[1] = Math.Floor(coord[1]);

            string Long;
            string lat;

            if (projCor.X > 0) Long = "E";
            else if (projCor.X < 0) Long = "W";
            else Long = " ";

            if (projCor.Y > 0) lat = "N";
            else if (projCor.Y < 0) lat = "S";
            else lat = " ";

            _latLonStatusPanel.Caption = "Longitude: " + d[0] + "°" + m[0].ToString("00") + "'" + s[0].ToString("00") + "\"" + Long + ", Latitude: " + d[1] + "°" + m[1].ToString("00") + "'" + s[1].ToString("00") + "\"" + lat;
        }

        #endregion Coordinate Display
    }
}
