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
using GeoAPI.Geometries;
using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries;
using Point = System.Drawing.Point;

namespace DotSpatial.SDR.Plugins.Measure
{
    /// <summary>
    /// A MapFunction that allows measuring the distance on the map.
    /// </summary>
    public class MapFunctionMeasure : MapFunction
    {
        private const double RadiusOfEarth = 111319.5;
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
        
        // private bool _doubleClick;

        #region Constructors

        /// <summary>
        /// Creates a new instance of MapMeasureFunction, with panel
        /// </summary>
        /// <param name="mp"></param>
        public MapFunctionMeasure(MeasurePanel mp)
        {
            Name = "MapFunctionMeasure";
            YieldStyle = YieldStyles.LeftButton | YieldStyles.RightButton;

            _measurePanel = mp;
            _areaMode = (_measurePanel.MeasureMode == MeasureMode.Area);
            _previousParts = new List<List<Coordinate>>();

            HandleMeasurePanelEvents();
        }
        
        private void HandleMeasurePanelEvents()
        {
            _measurePanel.MeasureModeChanged += MeasurePanel_MeasureModeChanged;
            _measurePanel.MeasurementsCleared += MeasurePanel_MeasurementsCleared;
            _measurePanel.MeasureModeActivated += MeasurePanelOnMeasureModeActivated;
        }

        private void MeasurePanelOnMeasureModeActivated(object sender, EventArgs eventArgs)
        {
            // redundant check, but prevents multiple events from firing when not needed
            if (Map.FunctionMode != FunctionMode.None)
            {
                Map.FunctionMode = FunctionMode.None;
            }
        }

        private void MeasurePanel_MeasurementsCleared(object sender, EventArgs e)
        {
            _previousParts.Clear();
            if (_coordinates != null)
                _coordinates.Clear();
            _previousDistance = 0;
            _currentDistance = 0;
            _currentArea = 0;
            if (Map != null) Map.MapFrame.Invalidate();
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
                HandleMeasurePanelEvents();
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

            Pen bluePen = new Pen(Color.Blue, 2F);
            Pen redPen = new Pen(Color.Red, 3F);
            Brush redBrush = new SolidBrush(Color.Red);

            List<Point> points = new List<Point>();
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Brush blue = new SolidBrush(Color.FromArgb(60, 0, 0, 255));

            if (_previousParts != null && _previousParts.Count > 0)
            {
                GraphicsPath previous = new GraphicsPath();
                previous.FillMode = FillMode.Winding;
                List<Point> allPoints = new List<Point>();
                foreach (List<Coordinate> part in _previousParts)
                {
                    List<Point> prt = new List<Point>();
                    foreach (Coordinate c in part)
                    {
                        prt.Add(Map.ProjToPixel(c));
                    }
                    previous.AddLines(prt.ToArray());
                    allPoints.AddRange(prt);
                    if (_areaMode) previous.CloseFigure();
                    previous.StartFigure();
                }
                if (_areaMode && _coordinates != null)
                {
                    List<Point> fillPts = new List<Point>();
                    if ((!_standBy && _coordinates.Count > 2) || _coordinates.Count > 3)
                    {
                        foreach (Coordinate c in _coordinates)
                        {
                            fillPts.Add(Map.ProjToPixel(c));
                        }
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
                foreach (Coordinate coord in _coordinates)
                {
                    points.Add(Map.ProjToPixel(coord));
                }

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
                    dist = dist * RadiusOfEarth;
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

        private double GetArea(Coordinate[] tempPolygon)
        {
            double area = Math.Abs(CGAlgorithms.SignedArea(tempPolygon));
            if (_previousParts == null || _previousParts.Count == 0)
            {
                _firstPartIsCounterClockwise = CGAlgorithms.IsCCW(tempPolygon);
            }
            else
            {
                if (CGAlgorithms.IsCCW(tempPolygon) != _firstPartIsCounterClockwise)
                {
                    area = -area;
                }
            }
            if (Map.Projection != null)
            {
                if (Map.Projection.IsLatLon)
                {
                    // this code really assumes the location is near the equator
                    area *= RadiusOfEarth * RadiusOfEarth;
                }
                else
                {
                    area *= Map.Projection.Unit.Meters * Map.Projection.Unit.Meters;
                }
            }
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
                if (!c1.Equals2D(_coordinates[_coordinates.Count - 1])) tempPolygon.Add(c1); //don't add the current coordinate again if it was added by mouse click
                if (tempPolygon.Count < 3)
                {
                    if (tempPolygon.Count > 1)
                    {
                        Rectangle r = Map.ProjToPixel(new LineString(tempPolygon.ToArray()).EnvelopeInternal.ToExtent());
                        r.Inflate(20, 20);
                        Map.Invalidate(r);
                    }
                    _mousePosition = e.Location;
                    return;
                }
                tempPolygon.Add(_coordinates[0]); //changed by jany_ (2016-06-09) close the polygon, because they must be closed by definition
                Polygon pg = new Polygon(new LinearRing(tempPolygon.ToArray()));

                double area = GetArea(tempPolygon.ToArray());

                _measurePanel.TotalArea = area;
                Rectangle rr = Map.ProjToPixel(pg.EnvelopeInternal.ToExtent());
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
                        //changed by jany_ (2016-06-09) close the polygon to get the correct area
                        List<Coordinate> tempPolygon = _coordinates.ToList();
                        tempPolygon.Add(_coordinates[0]);

                        double area = GetArea(tempPolygon.ToArray());
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

        //private void FinalizeCalculation()
        //{
        //    if (_coordinates.Count > 1)
        //    {
        //        _previousParts.Add(_coordinates);
        //        if (_areaMode)
        //        {
        //            _measurePanel.TotalArea = _currentArea;
        //        }
        //        else
        //        {
        //            _previousDistance += _currentDistance;
        //            _currentDistance = 0;
        //            _currentArea = 0;
        //            _measurePanel.Distance = 0;
        //            _measurePanel.TotalDistance = _previousDistance;
        //        }
        //    }
        //    _coordinates = new List<Coordinate>();
        //    Map.Invalidate();
        //}

        private void MeasurePanel_MeasureModeChanged(object sender, EventArgs e)
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
    }
}