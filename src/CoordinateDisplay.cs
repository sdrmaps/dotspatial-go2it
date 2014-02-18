using System;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using DotSpatial.Projections;
using DotSpatial.SDR.Controls;
using DotSpatial.Topology;

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

        readonly ProjectionInfo _wgs84Projection = ProjectionInfo.FromEsriString(Properties.Resources.wgs_84_esri_string);
        ProjectionInfo _currentMapProjection;
        readonly StatusPanel _latLonStatusPanel;
        bool _isWgs84 = true;
        bool _showCoordinates;

        public CoordinateDisplay(AppManager app)
        {
            _latLonStatusPanel = new StatusPanel {Width = 400};
            app.ProgressHandler.Add(_latLonStatusPanel);
            // set the application manager and the panel changed event to update coords
            _appManager = app;
            _appManager.DockManager.ActivePanelChanged += DockManagerOnActivePanelChanged;
        }

        private void DockManagerOnActivePanelChanged(object sender, DockablePanelEventArgs dockablePanelEventArgs)
        {
            var dockControl = (DockingControl)sender;
            var key = dockablePanelEventArgs.ActivePanelKey;
            DockPanelInfo dockInfo;
            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;
            // if this is a map tab update the map events for display of lat/long
            if (!dockInfo.DotSpatialDockPanel.Key.StartsWith("kMap_")) return;
            // first check if there is already a map, if so then remove the events on it
            if (_map != null)
            {
                _map.MouseMove -= map_MouseMove;
                _map.ProjectionChanged -= map_ProjectionChanged;
            }
            // grab the active map from the dockinginfo object
            _map = (Map)dockInfo.DotSpatialDockPanel.InnerControl;
            // grab the projection string of this map panel for events
            string mapProjEsriString = _map.Projection.ToEsriString();
            _isWgs84 = (mapProjEsriString == Properties.Resources.wgs_84_esri_string);
            _currentMapProjection = ProjectionInfo.FromEsriString(mapProjEsriString);
            // setup all the events for this coordinate display on the map
            _map.MouseMove += map_MouseMove;
            _map.ProjectionChanged += map_ProjectionChanged;
        }

        void map_ProjectionChanged(object sender, EventArgs e)
        {
            var mapProjEsriString = _map.Projection.ToEsriString();
            _isWgs84 = (mapProjEsriString == Properties.Resources.wgs_84_esri_string);
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
                _isWgs84 = (_currentMapProjection.ToEsriString() == Properties.Resources.wgs_84_esri_string);
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
            Coordinate projCor = _map.PixelToProj(mouseLocation);

            var xy = new double[2];
            xy[0] = projCor.X;
            xy[1] = projCor.Y;

            var z = new double[1];
            if (!_isWgs84)
            {
                Reproject.ReprojectPoints(xy, z, _currentMapProjection, _wgs84Projection, 0, 1);
            }

            //Convert to Degrees Minutes Seconds
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
