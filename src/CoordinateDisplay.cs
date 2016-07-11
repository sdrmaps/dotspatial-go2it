using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using DotSpatial.Projections;
using DotSpatial.SDR.Controls;
using SdrConfig = SDR.Configuration;

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
        private readonly ComboBoxStatusPanel _latLonComboBoxPanel;
        private bool _isWgs84 = true;
        private bool _showCoordinates;

        private readonly object[] _includeLocal;
        private readonly object[] _removeLocal;

        public CoordinateDisplay(AppManager app)
        {
            _includeLocal = new object[]
            {
                "Deg/Min/Sec",
                "Decimal Degrees",
                "Local Coords"
            };
            _removeLocal = new object[]
            {
                "Deg/Min/Sec",
                "Decimal Degrees"
            };

            _latLonStatusPanel = new StatusPanel
            {
                Width = 400
            };
            _latLonComboBoxPanel = new ComboBoxStatusPanel
            {
                Width = 300,
                Items = _includeLocal,
                SelectedItem = "Deg/Min/Sec"  // default, this is updated with user set values on show
            };
            _latLonComboBoxPanel.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                var item = sender as ComboBoxStatusPanel;
                if (item == null) return;

                if (e.PropertyName == "SelectedItem")
                {
                    // update the user settings on chosen mode
                    SDR.Configuration.User.Go2ItUserSettings.Instance.CoordinateDisplayMode =
                        item.SelectedItem as string;
                    // reset the panel caption
                    _latLonStatusPanel.Caption = String.Empty;
                }
            };
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

            // check if the map projection is strictly a latlong display
            if (_map.Projection.IsLatLon)
            {
                _latLonComboBoxPanel.Items = _removeLocal;
                if ((string) _latLonComboBoxPanel.SelectedItem == "Local Coords")
                {
                    _latLonComboBoxPanel.SelectedItem = "Deg/Min/Sec";
                }
            }
            else
            {
                _latLonComboBoxPanel.Items = _includeLocal;
            }
            // setup all the events for this coordinate display on the map
            _map.MouseMove += map_MouseMove;
            _map.ProjectionChanged += map_ProjectionChanged;
        }

        void map_ProjectionChanged(object sender, EventArgs e)
        {
            var mapProjEsriString = _map.Projection.ToEsriString();
            _isWgs84 = (mapProjEsriString == KnownCoordinateSystems.Geographic.World.WGS1984.ToEsriString());
            _currentMapProjection = ProjectionInfo.FromEsriString(mapProjEsriString);

            // check if the map projection is strictly a latlong display
            if (_map.Projection.IsLatLon)
            {
                _latLonComboBoxPanel.Items = _removeLocal;
                if ((string)_latLonComboBoxPanel.SelectedItem == "Local Coords")
                {
                    _latLonComboBoxPanel.SelectedItem = "Deg/Min/Sec";
                }
            }
            else
            {
                _latLonComboBoxPanel.Items = _includeLocal;
            }

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
                    // handled by StatusControl.cs
                    _appManager.ProgressHandler.Remove(_latLonStatusPanel);
                    _appManager.ProgressHandler.Remove(_latLonComboBoxPanel);
                }
                else
                {
                    // handled by StatusControl.cs
                    _appManager.ProgressHandler.Add(_latLonStatusPanel);
                    _appManager.ProgressHandler.Add(_latLonComboBoxPanel);
                }
                _latLonStatusPanel.Caption = String.Empty;

                // saving the state of the coordinate display mode to the user settings
                if (SDR.Configuration.User.Go2ItUserSettings.Instance.CoordinateDisplayMode == string.Empty)
                {
                    SDR.Configuration.User.Go2ItUserSettings.Instance.CoordinateDisplayMode =
                        _latLonComboBoxPanel.SelectedItem as string;
                }
                _latLonComboBoxPanel.SelectedItem =
                    SDR.Configuration.User.Go2ItUserSettings.Instance.CoordinateDisplayMode;
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

        private string CleanCoordinateUnitLabel()
        {
            var unit = _map.Projection.Unit.Name.Replace("_", " ");  // replace any underscores
            var builder = new StringBuilder();
           	foreach (string s in unit.Split(' '))
	        {
                switch (s.ToLower())
                {
                    case "foot":
                        builder.Append("Feet ");
                        break;
                    case "mile":
                        builder.Append("Miles ");
                        break;
                    case "meter":
                    case "metre":
                        builder.Append("Meters ");
                        break;
                    case "kilometer":
                    case "kilometre":
                        builder.Append("Kilometers ");
                        break;
                    default:
                        builder.Append(s + " ");
                        break;
                }
	        }
	        return builder.ToString();
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

            if (_latLonComboBoxPanel.SelectedItem == "Local Coords")
            {
                _latLonStatusPanel.Caption = xy[0] + ",  " + xy[1] + " " + CleanCoordinateUnitLabel();
                return;
            }

            if (!_isWgs84)
            {
                Reproject.ReprojectPoints(xy, new double[1], _currentMapProjection, _wgs84Projection, 0, 1);
            }

            if ((string) _latLonComboBoxPanel.SelectedItem == "Decimal Degrees")
            {
                _latLonStatusPanel.Caption = "Longitude: " + xy[0] + ", " + "Latitude: " + xy[1];
                return;
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

            // update the status panel label caption accordingly
            _latLonStatusPanel.Caption = "Longitude: " + d[0] + "°" + m[0].ToString("00") + "'" + s[0].ToString("00") + "\"" + Long + ", Latitude: " + d[1] + "°" + m[1].ToString("00") + "'" + s[1].ToString("00") + "\"" + lat;
        }

        #endregion Coordinate Display
    }
}
