// ********************************************************************************************************
// Product Name: DotSpatial.Controls.dll
// Description:  The Windows Forms user interface controls like the map, legend, toolbox, ribbon and others.
// ********************************************************************************************************
// The contents of this file are subject to the MIT License (MIT)
// you may not use this file except in compliance with the License. You may obtain a copy of the License at
// http://dotspatial.codeplex.com/license
//
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF
// ANY KIND, either expressed or implied. See the License for the specific language governing rights and
// limitations under the License.
//
// The Original Code is from MapWindow.dll version 6.0
//
// The Initial Developer of this Original Code is Ted Dunsford. Created 11/19/2009 10:59:47 AM
//
// Contributor(s): (Open source contributors should list themselves and their modifications here).
//
// ********************************************************************************************************

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using DotSpatial.Topology;
using DotSpatial.Topology.Algorithm;
using Point = System.Drawing.Point;

namespace DotSpatial.SDR.Plugins.Measure
{
    /// <summary>
    /// A MapFunction that allows measuring the distance on the map.
    /// </summary>
    public class MapFunctionMeasure : MapFunction
    {
        private bool _areaMode;
        private List<Coordinate> _coordinates;
        private double _currentArea;
        private double _currentDistance;
        private bool _firstPartIsCounterClockwise;
        private MeasurePanel _measurePanel;
        private Point _mousePosition;
        private double _previousDistance;
        private List<List<Coordinate>> _previousParts;

        private bool _standBy;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the MapFunctionMeasure class.
        /// </summary>
        public MapFunctionMeasure()
        {
            Configure();
        }

        /// <summary>
        /// Creates a new instance of AddShapeFunction, but specifies
        /// the Map that this function should be applied to.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="mp"></param>
        public MapFunctionMeasure(IMap map, MeasurePanel mp)
            : base(map)
        {
            _measurePanel = mp;
            Configure();
        }

        /// <summary>
        /// Creates a new instance of AddShapeFunction, but specifies
        /// the Map that this function should be applied to.
        /// </summary>
        /// <param name="map"></param>
        public MapFunctionMeasure(IMap map)
            : base(map)
        {
            Configure();
        }

        private void Configure()
        {
            _previousParts = new List<List<Coordinate>>();
            YieldStyle = YieldStyles.LeftButton | YieldStyles.RightButton;
            HandleMeasureDialogEvents();

            var map = Map as Control;
            if (map != null) map.MouseLeave += map_MouseLeave;
            Name = "MapFunctionMeasure";
        }

        private void map_MouseLeave(object sender, EventArgs e)
        {
            Map.Invalidate();
        }

        private void HandleMeasureDialogEvents()
        {
            _measurePanel.MeasureModeChanged += MeasureDialogMeasureModeChanged;
            // _measurePanel.FormClosing += CoordinateDialogFormClosing;
            _measurePanel.MeasurementsCleared += MeasureDialog_MeasurementsCleared;
        }

        private void MeasureDialog_MeasurementsCleared(object sender, EventArgs e)
        {
            _previousParts.Clear();
            if (_coordinates != null)
                _coordinates.Clear();
            _previousDistance = 0;
            _currentDistance = 0;
            _currentArea = 0;
            Map.MapFrame.Invalidate();
            Map.Invalidate();
            _measurePanel.Distance = 0;
            _measurePanel.TotalDistance = 0;
            _measurePanel.TotalArea = 0;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Forces this function to begin collecting points for building a new shape.
        /// </summary>
        protected override void OnActivate()
        {
            if (_measurePanel == null || _measurePanel.IsDisposed)
            {
                _measurePanel = new MeasurePanel();
                HandleMeasureDialogEvents();
            }
            _measurePanel.Show();
            if (_standBy == false)
            {
                _previousParts = new List<List<Coordinate>>();
                _coordinates = new List<Coordinate>();
            }

            _standBy = false;
            base.OnActivate();
        }

        /// <summary>
        /// Allows for new behavior during deactivation.
        /// </summary>
        protected override void OnDeactivate()
        {
            if (_standBy)
            {
                return;
            }
            // Don't completely deactivate, but rather go into standby mode
            // where we draw only the content that we have actually locked in.
            _standBy = true;
            Map.Invalidate();
        }

        /// <summary>
        /// Handles drawing of editing features
        /// </summary>
        /// <param name="e">The drawing args</param>
        protected override void OnDraw(MapDrawArgs e)
        {
            Point mouseTest = Map.PointToClient(Control.MousePosition);

            bool hasMouse = Map.ClientRectangle.Contains(mouseTest);

            var bluePen = new Pen(Color.Blue, 2F);
            var redPen = new Pen(Color.Red, 3F);
            var redBrush = new SolidBrush(Color.Red);

            var points = new List<Point>();
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Brush blue = new SolidBrush(Color.FromArgb(60, 0, 0, 255));

            if (_previousParts != null && _previousParts.Count > 0)
            {
                var previous = new GraphicsPath {FillMode = FillMode.Winding};
                var allPoints = new List<Point>();
                foreach (List<Coordinate> part in _previousParts)
                {
                    var prt = part.Select(c => Map.ProjToPixel(c)).ToList();
                    previous.AddLines(prt.ToArray());
                    allPoints.AddRange(prt);
                    if (_areaMode) previous.CloseFigure();
                    previous.StartFigure();
                }
                if (_areaMode && _coordinates != null)
                {
                    var fillPts = new List<Point>();
                    if ((!_standBy && _coordinates.Count > 2) || _coordinates.Count > 3)
                    {
                        fillPts.AddRange(_coordinates.Select(c => Map.ProjToPixel(c)));
                        if (!_standBy && hasMouse)
                        {
                            fillPts.Add(_mousePosition);
                        }

                        previous.AddLines(fillPts.ToArray());
                        previous.CloseFigure();
                    }
                }
                if (allPoints.Count > 1)
                {
                    e.Graphics.DrawPath(bluePen, previous);
                    if (_areaMode)
                    {
                        e.Graphics.FillPath(blue, previous);
                    }
                }

                foreach (Point pt in allPoints)
                {
                    e.Graphics.FillRectangle(redBrush, new Rectangle(pt.X - 2, pt.Y - 2, 4, 4));
                }
            }

            if (_coordinates != null)
            {
                points.AddRange(_coordinates.Select(coord => Map.ProjToPixel(coord)));

                if (points.Count > 1)
                {
                    e.Graphics.DrawLines(bluePen, points.ToArray());
                    foreach (Point pt in points)
                    {
                        e.Graphics.FillRectangle(redBrush, new Rectangle(pt.X - 2, pt.Y - 2, 4, 4));
                    }
                }

                if (points.Count > 0 && _standBy == false && hasMouse)
                {
                    e.Graphics.DrawLine(redPen, points[points.Count - 1], _mousePosition);
                    if (_areaMode && points.Count > 1)
                    {
                        e.Graphics.DrawLine(redPen, points[0], _mousePosition);
                    }
                }
                if (points.Count > 1 && _areaMode && (_previousParts == null || _previousParts.Count == 0))
                {
                    if (hasMouse && !_standBy)
                    {
                        points.Add(_mousePosition);
                    }

                    if (points.Count > 2)
                    {
                        e.Graphics.FillPolygon(blue, points.ToArray());
                    }
                }
            }
            bluePen.Dispose();
            redPen.Dispose();
            redBrush.Dispose();
            blue.Dispose();
            base.OnDraw(e);
        }

        private double GetDist(Coordinate c1)
        {
            Coordinate c2 = _coordinates[_coordinates.Count - 1];
            double dx = Math.Abs(c2.X - c1.X);
            double dy = Math.Abs(c2.Y - c1.Y);
            double dist;
            if (Map.Projection != null)
            {
                if (Map.Projection.IsLatLon)
                {
                    double y = (c2.Y + c1.Y) / 2;
                    double factor = Math.Cos(y * Math.PI / 180);
                    dx *= factor;
                    dist = Math.Sqrt(dx * dx + dy * dy);
                    dist = dist * 111319.5;
                }
                else
                {
                    dist = Math.Sqrt(dx * dx + dy * dy);
                    dist *= Map.Projection.Unit.Meters;
                }
            }
            else
            {
                dist = Math.Sqrt(dx * dx + dy * dy);
            }
            _measurePanel.Distance = dist;
            return dist;
        }

        private double GetArea(List<Coordinate> tempPolygon)
        {
            double area = Math.Abs(CgAlgorithms.SignedArea(tempPolygon));
            if (_previousParts == null || _previousParts.Count == 0)
            {
                _firstPartIsCounterClockwise = CgAlgorithms.IsCounterClockwise(tempPolygon);
            }
            else
            {
                if (CgAlgorithms.IsCounterClockwise(tempPolygon) != _firstPartIsCounterClockwise)
                {
                    area = -area;
                }
            }
            if (Map.Projection != null)
            {
                if (Map.Projection.IsLatLon)
                {
                    // this code really assumes the location is near the equator
                    const double radiusOfEarth = 111319.5;
                    area *= radiusOfEarth * radiusOfEarth;
                }
                else
                {
                    area *= Map.Projection.Unit.Meters * Map.Projection.Unit.Meters;
                }
            }
            // _measurePanel.Area = area;
            return area;
        }

        /// <summary>
        /// updates the auto-filling X and Y coordinates
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(GeoMouseArgs e)
        {
            if (_standBy)
            {
                return;
            }
            if (_coordinates == null || _coordinates.Count == 0)
            {
                return;
            }
            Coordinate c1 = e.GeographicLocation;
            if (_measurePanel.MeasureMode == MeasureMode.Distance)
            {
                double dist = GetDist(c1);
                _measurePanel.TotalDistance = _previousDistance + _currentDistance + dist;
            }
            else
            {
                List<Coordinate> tempPolygon = _coordinates.ToList();
                tempPolygon.Add(c1);
                if (tempPolygon.Count < 3)
                {
                    if (tempPolygon.Count == 2)
                    {
                        Rectangle r = Map.ProjToPixel(new LineString(tempPolygon).Envelope.ToExtent());
                        r.Inflate(20, 20);
                        Map.Invalidate(r);
                    }
                    _mousePosition = e.Location;
                    return;
                }
                var pg = new Polygon(new LinearRing(tempPolygon));

                double area = GetArea(tempPolygon);

                _measurePanel.TotalArea = area;
                Rectangle rr = Map.ProjToPixel(pg.Envelope.ToExtent());
                rr.Inflate(20, 20);
                Map.Invalidate(rr);
                _mousePosition = e.Location;
            }

            if (_coordinates.Count > 0)
            {
                List<Point> points = _coordinates.Select(coord => Map.ProjToPixel(coord)).ToList();
                Rectangle oldRect = SymbologyGlobal.GetRectangle(_mousePosition, points[points.Count - 1]);
                Rectangle newRect = SymbologyGlobal.GetRectangle(e.Location, points[points.Count - 1]);
                Rectangle invalid = Rectangle.Union(newRect, oldRect);
                invalid.Inflate(20, 20);
                Map.Invalidate(invalid);
            }
            _mousePosition = e.Location;
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Handles the Mouse-Up situation
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(GeoMouseArgs e)
        {
            if (_standBy)
            {
                return;
            }
            // Add the current point to the featureset

            if (e.Button == MouseButtons.Right)
            {
                if (_coordinates.Count > 1)
                {
                    _previousParts.Add(_coordinates);
                    if (_areaMode)
                    {
                        _measurePanel.TotalArea = _currentArea;
                    }
                    else
                    {
                        _previousDistance += _currentDistance;
                        _currentDistance = 0;
                        _currentArea = 0;
                        _measurePanel.Distance = 0;
                        _measurePanel.TotalDistance = _previousDistance;
                    }
                }

                _coordinates = new List<Coordinate>();
                Map.Invalidate();
            }
            else
            {
                if (_coordinates == null)
                {
                    _coordinates = new List<Coordinate>();
                }

                if (_coordinates.Count > 0)
                {
                    if (_measurePanel.MeasureMode == MeasureMode.Distance)
                    {
                        Coordinate c1 = e.GeographicLocation;
                        double dist = GetDist(c1);
                        _measurePanel.TotalDistance = _previousDistance + dist;
                        _currentDistance += dist;
                    }
                }
                _coordinates.Add(e.GeographicLocation);
                if (_areaMode)
                {
                    if (_coordinates.Count >= 3)
                    {
                        double area = GetArea(_coordinates);
                        _currentArea = area;
                    }
                }
                Map.Invalidate();
            }

            base.OnMouseUp(e);
        }

        /// <summary>
        /// Occurs when this function is removed.
        /// </summary>
        protected override void OnUnload()
        {
            if (Enabled)
            {
                _coordinates = null;
                _previousParts = null;
                _measurePanel.Hide();
            }

            Map.Invalidate();
        }

        /* private void CoordinateDialogFormClosing(object sender, FormClosingEventArgs e1)
        {
            // This signals that we are done with editing, and should therefore close up shop
            Enabled = false;
            Map.Invalidate();
        } */

        private void MeasureDialogMeasureModeChanged(object sender, EventArgs e)
        {
            _previousParts.Clear();

            _areaMode = (_measurePanel.MeasureMode == MeasureMode.Area);
            if (_coordinates != null)
            {
                _coordinates = new List<Coordinate>();
            }
            Map.Invalidate();
        }

        #endregion

        /// <summary>
        /// Gets or sets the featureset to modify
        /// </summary>
        public IFeatureSet FeatureSet { get; set; }
    }
}