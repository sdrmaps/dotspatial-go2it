using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.SDR.Controls;
using DotSpatial.Symbology;
using DotSpatial.Topology;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Search.Function;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Spatial;
using Lucene.Net.Spatial.Prefix;
using Lucene.Net.Spatial.Prefix.Tree;
using Lucene.Net.Spatial.Queries;
using Lucene.Net.Store;
using Spatial4n.Core.Context.Nts;
using Spatial4n.Core.Distance;
using SdrConfig = SDR.Configuration;
using System.Windows.Forms;
using DotSpatial.Controls;
using SDR.Data.Database;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using Version = Lucene.Net.Util.Version;
using Directory = Lucene.Net.Store.Directory;
using Field = Lucene.Net.Documents.Field;
using PointShape = DotSpatial.Symbology.PointShape;
using Shape = Spatial4n.Core.Shapes.Shape;

namespace DotSpatial.SDR.Plugins.Search
{
    /// <summary>
    /// A MapFunction that allows searching of various configured layers
    /// </summary>
    public class MapFunctionSearch : MapFunction
    {
        // overall tab docking control for selecting map and tool tabs
        // as well as serialization manager to get project file info sqlite strings etc
        // (used to swap active map panels on searches) (these are set on init)
        internal TabDockingControl TabDockingControl { get; set; }
        private readonly string _currentProjectFile;

        private SearchPanel _searchPanel;
        private readonly DataGridView _dataGridView; // dgv to populate our results of query to
        private static IndexSearcher _indexSearcher;

        // drawing layers used by this tool
        private FeatureSet _pointGraphics;
        private MapPointLayer _pointGraphicsLayer;
        private FeatureSet _polylineGraphics;
        private MapLineLayer _polylineGraphicsLayer;

        private string _indexType;
        private string[] _columnNames;
        private FeatureLookup _activeLookup;

        internal const string FID = "FID";
        internal const string LYRNAME = "LYRNAME";
        internal const string GEOSHAPE = "GEOSHAPE";

        private static readonly Regex DigitsOnly = new Regex(@"[^\d]");
        // private static readonly Regex ParseCoordinates = new Regex("(-?\\d{1,3})[\\.\\,°]{0,1}\\s*(\\d{0,2})[\\.\\,\']{0,1}\\s*(\\d*)[\\.\\,°]{0,1}\\s*([NSnsEeWw]?)");
        private static readonly Regex ParseCoordinates = new Regex("(-?\\d{1,3})[\\.\\,\\°]{0,1}\\s*(\\d{0,2})[\\.\\,\']{0,1}\\s*(\\d*)[\\.\\,\"]*\\d*[\\s\\.\"]*([NSnsEeWw]?)");

        #region Constructors

        /// <summary>
        /// Creates a new instance of MapFunctionSearch, with panel
        /// </summary>
        /// <param name="sp">Search Panel</param>
        /// <param name="currentProjectFile">Project File Path</param>
        public MapFunctionSearch(SearchPanel sp, string currentProjectFile)
        {
            Name = "MapFunctionSearch";
            YieldStyle = YieldStyles.AlwaysOn;

            _currentProjectFile = currentProjectFile;
            _searchPanel = sp;
            _dataGridView = sp.DataGridDisplay;

            HandleSearchPanelEvents();
            SetSearchVariables();
            SetupIndexReaderWriter(_indexType);
        }

        private void HandleSearchPanelEvents()
        {
            _searchPanel.SearchModeChanged += SearchPanelOnSearchModeChanged;
            _searchPanel.SearchesCleared += SearchPanelOnSearchesCleared;
            _searchPanel.HydrantLocate += SearchPanelOnHydrantLocate;
            _searchPanel.PerformSearch += SearchPanelOnPerformSearch;
            _searchPanel.OnRowDoublelicked += SearchPanelOnRowDoublelicked;
            _searchPanel.SearchModeActivated += SearchPanelOnSearchModeActivated;
        }

        private int SetSearchButtonsState(SearchMode m)
        {
            var d = GetLuceneIndexDirectory(m + "Index");
            _searchPanel.EnableSearchButton(m, d != null);
            return d != null ? 1 : 0;
        }

        public void EnableSearchModes()
        {
            int i = 0;  // track total count of lucene search modes active
            // check each search type for existence of index directories, activate accordingly
            i += SetSearchButtonsState(SearchMode.Address);
            i += SetSearchButtonsState(SearchMode.Road);
            i += SetSearchButtonsState(SearchMode.KeyLocation);
            i += SetSearchButtonsState(SearchMode.City);
            i += SetSearchButtonsState(SearchMode.CellSector);
            i += SetSearchButtonsState(SearchMode.Parcel);
            i += SetSearchButtonsState(SearchMode.Esn);
            i += SetSearchButtonsState(SearchMode.Hydrant);
            // coord search (no need for lucene index)
            _searchPanel.EnableSearchButton(SearchMode.Coordinate, Map != null);
            // set the state of the search all button (make sure we have at least one search mode available
            _searchPanel.EnableSearchButton(SearchMode.All, i >= 1);
        }

        private void SetSearchVariables()
        {
            switch (PluginSettings.Instance.SearchMode)
            {
                case SearchMode.Address:
                    _indexType = SearchMode.Address + "Index";
                    _columnNames = GetColumnNames(); // uses the _indexType variable
                    break;
                case SearchMode.Intersection:
                    _indexType = SearchMode.Road + "Index";
                    _columnNames = GetColumnNames();
                    break;
                case SearchMode.Name:
                    _indexType = SearchMode.Address + "Index";
                    _columnNames = GetColumnNames();
                    break;
                case SearchMode.Phone:
                    _indexType = SearchMode.Address + "Index";
                    _columnNames = GetColumnNames();
                    break;
                case SearchMode.Road:
                    _indexType = SearchMode.Road + "Index";
                    _columnNames = GetColumnNames();
                    break;
                case SearchMode.KeyLocation:
                    _indexType = SearchMode.KeyLocation + "Index";
                    _columnNames = GetColumnNames();
                    break;
                case SearchMode.City:
                    _indexType = SearchMode.City + "Index";
                    _columnNames = GetColumnNames();
                    break;
                case SearchMode.Esn:
                    _indexType = SearchMode.Esn + "Index";
                    _columnNames = GetColumnNames();
                    break;
                case SearchMode.CellSector:
                    _indexType = SearchMode.CellSector + "Index";
                    _columnNames = GetColumnNames();
                    break;
                case SearchMode.Parcel:
                    _indexType = SearchMode.Parcel + "Index";
                    _columnNames = GetColumnNames();
                    break;
                case SearchMode.Coordinate:
                    _indexType = string.Empty;
                    _columnNames = new string[0];
                    break;
                case SearchMode.All:
                    _indexType = SearchMode.All + "Index";
                    _columnNames = new[] {"Layer Name", "Field Name", "Field Value"};
                    break;
            }
        }

        public static IndexSearcher IndexSearcher
        {
            get { return _indexSearcher; }
        }

        private static Version LuceneVersion
        {
            get { return Version.LUCENE_30; }
        }

        private static Analyzer LuceneAnalyzer
        {
            get { return new StandardAnalyzer(LuceneVersion); }
        }

        private void SearchPanelOnSearchModeActivated(object sender, EventArgs eventArgs)
        {
            // redundant check, but prevents multiple events from firing when not needed
            if (Map.FunctionMode != FunctionMode.None)
            {
                Map.FunctionMode = FunctionMode.None;
                EnableSearchModes();
            }
        }

        protected override void OnActivate()
        {
            if (_searchPanel == null || _searchPanel.IsDisposed)
            {
                _searchPanel = new SearchPanel();
                HandleSearchPanelEvents();
            }
            EnableSearchModes();
            _searchPanel.Show();
            base.OnActivate();
        }

        #endregion

        #region Events

        /// <summary>
        /// Set and Store column ordering preferences for the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="dataGridViewColumnEventArgs"></param>
        private void DataGridViewOnColumnDisplayIndexChanged(object sender, DataGridViewColumnEventArgs dataGridViewColumnEventArgs)
        {
            // todo handle any missing cases
            var newOrder = new Dictionary<string, string>();
            foreach (DataGridViewColumn col in _dataGridView.Columns)
            {
                var i = GetColumnDisplayIndex(col.Name, _dataGridView);
                newOrder.Add(col.Name, i.ToString(CultureInfo.InvariantCulture));
            }
            switch (PluginSettings.Instance.SearchMode)
            {
                case SearchMode.Address:
                    PluginSettings.Instance.AddressIndexColumnOrder = newOrder;
                    break;
                case SearchMode.Intersection:
                    PluginSettings.Instance.RoadIndexColumnOrder = newOrder;
                    break;
                case SearchMode.Name:
                    PluginSettings.Instance.AddressIndexColumnOrder = newOrder;
                    break;
                case SearchMode.Phone:
                    PluginSettings.Instance.AddressIndexColumnOrder = newOrder;
                    break;
                case SearchMode.Road:
                    PluginSettings.Instance.RoadIndexColumnOrder = newOrder;
                    break;
                case SearchMode.KeyLocation:
                    PluginSettings.Instance.KeyLocationIndexColumnOrder = newOrder;
                    break;
                case SearchMode.Parcel:
                    PluginSettings.Instance.ParcelIndexColumnOrder = newOrder;
                    break;
                case SearchMode.CellSector:
                    PluginSettings.Instance.CellSectorIndexColumnOrder = newOrder;
                    break;
            }
        }

        private void SearchPanelOnSearchModeChanged(object sender, EventArgs eventArgs)
        {
            SetSearchVariables();
            SetupIndexReaderWriter(_indexType);
        }

        private void SearchPanelOnSearchesCleared(object sender, EventArgs eventArgs)
        {
            _activeLookup = null;
            // unbind the column order index stuff
            _dataGridView.ColumnDisplayIndexChanged -= DataGridViewOnColumnDisplayIndexChanged;
            // now clear the datagridview
            _dataGridView.Rows.Clear();
            _dataGridView.Columns.Clear();

            if (Map != null && Map.MapFrame.DrawingLayers.Contains(_pointGraphicsLayer))
            {
                Map.MapFrame.DrawingLayers.Remove(_pointGraphicsLayer);
                _pointGraphicsLayer = null;
                _pointGraphics = null;
            }
            if (Map != null && Map.MapFrame.DrawingLayers.Contains(_polylineGraphicsLayer))
            {
                Map.MapFrame.DrawingLayers.Remove(_polylineGraphicsLayer);
                _polylineGraphicsLayer = null;
                _polylineGraphics = null;
            }
            if (Map != null) Map.MapFrame.Invalidate();
        }

        private void SearchPanelOnPerformSearch(object sender, EventArgs eventArgs)
        {
            if (_searchPanel.SearchQuery.Length <= 0) return;
            var q = _searchPanel.SearchQuery;
            // if its an intersection and there are two terms do a location zoom, otherwise find intersections
            if (PluginSettings.Instance.SearchMode == SearchMode.Intersection)
            {
                if (_searchPanel.SearchQuery.Length <= 1) return;  // only the | is present (blank search)
                var arr = q.Split('|');
                if (arr[1].Length > 0) // zoom to intersection of the two features
                {
                    ZoomToIntersection(arr[0], arr[1]);
                    return;
                } // else look for intersections on this feature and populate combos and dgv
                q = arr[0];
            }
            else if (PluginSettings.Instance.SearchMode == SearchMode.City || PluginSettings.Instance.SearchMode == SearchMode.Esn)
            {
                _searchPanel.ClearSearches();  // clear any existing searches (fires the event above this one actually)
                var hitz = ExecuteLuceneQuery(q);
                if (hitz.Count() != 0)
                {
                    var hit = hitz.First();
                    var doc = _indexSearcher.Doc(hit.Doc);
                    var lookup = new FeatureLookup
                    {
                        Fid = doc.Get(FID),
                        Layer = doc.Get(LYRNAME),
                        Shape = doc.Get(GEOSHAPE)
                    };
                    ZoomToFeatureLookup(lookup);
                    return;
                }
            }
            else if (PluginSettings.Instance.SearchMode == SearchMode.Coordinate)
            {
                if (_searchPanel.SearchQuery.Length <= 1) return;  // only the | (internal separator) is present (empty search)
                var arr = q.Split('|');
                if (arr.Length != 2) return; // lat and long have not been passed in
                if (arr[0].Length > 0 && arr[1].Length > 0)
                {
                    _searchPanel.ClearSearches();  // clear any existing searches
                    ZoomToCoordinates(arr[0], arr[1]);
                    return;
                }
            }
            /*         
             * all other query types are processed and proceed from this point ....
             * each requires a lucene query and populates combos and datagridview    
             */
            _searchPanel.ClearSearches();  // clear any existing searches (fires the event above this one actually)
            // setup columns, ordering, etc for results datagridview
            PrepareDataGridView();
            // execute our lucene query
            var hits = ExecuteLuceneQuery(q);
            FormatQueryResults(hits);
        }

        private void ZoomToCoordinates(string strLat, string strLng)
        {
            var lat = new double[3];
            var lng = new double[3];

            var latCheck = ValidateCoordinates(lat, strLat);
            var lonCheck = ValidateCoordinates(lng, strLng);

            if (!latCheck)
            {
                _searchPanel.CoordinateError = "Invalid Latitude (Valid example: \"41.1939 N\")";
                return;
            }
            if (!lonCheck)
            {
                _searchPanel.CoordinateError = "Invalid Longitude (Valid example: \"19.4908 E\")";
                return;
            }

            var latCoor = LoadCoordinates(lat);
            var lonCoor = LoadCoordinates(lng);

            var xy = new double[2];

            // Now convert from Lat-Long to x,y coordinates that App.Map.ViewExtents can use to pan to the correct location.
            xy = LatLonReproject(lonCoor, latCoor);

            // Get extent where center is desired X,Y coordinate.
            
            var width = Map.ViewExtents.Width;
            var height = Map.ViewExtents.Height;
            Map.ViewExtents.X = (xy[0] - (width / 2));
            Map.ViewExtents.Y = (xy[1] + (height / 2));
            var ex = Map.ViewExtents;

            //Set App.Map.ViewExtents to new extent that centers on desired LatLong.
            Map.ViewExtents = ex;

        }

        private double[] LatLonReproject(double x, double y)
        {
            var xy = new[] { x, y };

            //Change y coordinate to be less than 90 degrees to prevent an issue.
            if (xy[1] >= 90) xy[1] = 89.9;
            if (xy[1] <= -90) xy[1] = -89.9;

            //Need to convert points to proper projection. Currently describe WGS84 points which may or may not be accurate.

            var wgs84String = "GEOGCS[\"GCS_WGS_1984\",DATUM[\"D_WGS_1984\",SPHEROID[\"WGS_1984\",6378137,298.257223562997]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.0174532925199433]]";
            var mapProjEsriString = Map.Projection.ToEsriString();
            var isWgs84 = (mapProjEsriString.Equals(wgs84String));

            //If the projection is not WGS84, then convert points to properly describe desired location.
            if (!isWgs84)
            {
                var z = new double[1];
                var wgs84Projection = ProjectionInfo.FromEsriString(wgs84String);
                var currentMapProjection = ProjectionInfo.FromEsriString(mapProjEsriString);
                Reproject.ReprojectPoints(xy, z, wgs84Projection, currentMapProjection, 0, 1);
            }

            //Return array with 1 x and 1 y value.
            return xy;
        }

        // Take Degrees-Minutes-Seconds from ParseCoordinates and turn them into doubles.
        private static double LoadCoordinates(IList<double> values)
        {
            //Convert Degrees, Minutes, Seconds to x, y coordinates for both lat and long.
            var coor = values[2] / 100;
            coor += values[1];
            coor = coor / 100;
            coor += Math.Abs(values[0]);

            //Change signs to get to the right quadrant.
            if (values[0] < 0) { coor *= -1; }

            return coor;
        }

        // ConvertCoordinates will understand lat-lon coordinates in a variety of
        // formats and separate them into Degrees, Minutes, and Seconds.
        private bool ValidateCoordinates(IList<double> values, String text)
        {
            // TODO: work on decimal minutes as input, it tends to fail their
            var match = ParseCoordinates.Match(text);
            var groups = match.Groups;

            try
            {
                values[0] = Double.Parse(groups[1].ToString());
                if (groups[2].Length > 0)
                {
                    values[1] = Double.Parse(groups[2].ToString());
                    if (groups[2].Length == 1)
                    {
                        values[1] *= 10;
                    }
                }
                if (groups[3].Length > 0)
                {
                    values[2] = Double.Parse(groups[3].ToString());
                    if (groups[3].Length == 1)
                    {
                        values[2] *= 10;
                    }
                }
            }
            catch
            {
                return false;
            }

            if ((groups[4].ToString().Equals("S", StringComparison.OrdinalIgnoreCase)
                || groups[4].ToString().Equals("W", StringComparison.OrdinalIgnoreCase))
                && values[0] > 0)
            {
                values[0] *= -1;
            }
            return true;
        }

        private void ActivateMapPanelWithLayer(string layer)
        {
            if (layer.Length <= 0) return;

            Dictionary<string, string> mapPanels = GetMapTabKeysContainingLayer(layer);
            if (!mapPanels.ContainsKey(SdrConfig.Project.Go2ItProjectSettings.Instance.ActiveMapViewKey))
            {
                if (mapPanels.Count == 1)
                {
                    TabDockingControl.SelectPanel(mapPanels.ElementAt(0).Key);
                }
                else if (mapPanels.Count > 1)
                {
                    var v = mapPanels.Values;
                    var msgBox = new MultiSelectMessageBox(
                        "Multiple Map Tabs",
                        "Multiple map tabs contain this feature, please select the tab to map the feature below.",
                        v.ToArray());
                    msgBox.ShowDialog();
                    var key = mapPanels.FirstOrDefault(x => x.Value == msgBox.Result).Key;
                    TabDockingControl.SelectPanel(key);
                }
            }
        }

        private string[] StripWktString(string wkt)
        {
            var strip = wkt.Replace("LINESTRING", "").Replace("POLYGON", "").Replace("POINT", "").Replace("(", "").Replace(")", "").Trim();
            return strip.Split(',');
        }

        private IEnumerable<string> GetIntersectionCoordinate(IEnumerable<FeatureLookup> fl1s, FeatureLookup fl2)
        {
            var coordsList = new List<string>();
            // strip all the wkt crap from coords and look for matches
            var coords = StripWktString(fl2.Shape);
            // now look for shared coordinates (the intersection point)
            foreach (var fl1 in fl1s)
            {
                foreach (var c in coords)
                {
                    if (!fl1.Shape.Contains(c.Trim())) continue;
                    if (!coordsList.Contains(c.Trim()))
                    {
                        coordsList.Add(c.Trim());
                    }
                }
            }
            return coordsList.ToArray();
        }

        private void CreateIntersectionGraphic(string coords)
        {
            var coord = coords.Split(' ');
            double x, y;
            double.TryParse(coord[0], out x);
            double.TryParse(coord[1], out y);
            var xy = new double[2];
            xy[0] = x;
            xy[1] = y;
            // reproject the point if need be
            if (Map.Projection.ToProj4String() != KnownCoordinateSystems.Geographic.World.WGS1984.ToProj4String())
            {
                Reproject.ReprojectPoints(
                    xy, new double[1], KnownCoordinateSystems.Geographic.World.WGS1984, Map.Projection, 0, 1);
            }
            CreatePointHighlightGraphic(new Coordinate(xy[0], xy[1]));
            var envelope = CreateBufferGraphic(new Point(new Coordinate(xy[0], xy[1])));

            var zoomInFactor = (double) SdrConfig.Project.Go2ItProjectSettings.Instance.SearchZoomFactor; // fixed zoom-in by 10% - 5% on each side
            var newExtentWidth = envelope.Width * zoomInFactor;
            var newExtentHeight = envelope.Height * zoomInFactor;
            envelope.ExpandBy(newExtentWidth, newExtentHeight);
            Map.ViewExtents = envelope.ToExtent();
        }

        private void ZoomToIntersection(string ft1, FeatureLookup fl2)
        {
            // snag all featurelookups that intersect with featurelookup fl2
            var fts1Lookup = FetchIntersectingLookups(ft1, fl2);
            // find the coordinate that is the intersection
            var coords = GetIntersectionCoordinate(fts1Lookup, fl2);
            var enumerable = coords as string[] ?? coords.ToArray();
            if (enumerable.Count() == 1)
            {
                CreateIntersectionGraphic(enumerable.First().Trim());
            }
        }

        private void ZoomToIntersection(string ft1, string ft2)
        {
            if (ft1.Length <= 0 || ft2.Length <= 0) return;

            var coordsList = new List<string>();
            // search for all features that match feature2 exactly
            IEnumerable<ScoreDoc> hits = ExecuteExactRoadQuery(ft2);
            foreach (var hit in hits)
            {
                var doc = _indexSearcher.Doc(hit.Doc);
                var fl2 = new FeatureLookup  // generate a new ftlookup for each hit
                {
                    Shape = doc.Get(GEOSHAPE),
                    Fid = doc.Get(FID),
                    Layer = doc.Get(LYRNAME)
                };
                var fts1Lookup = FetchIntersectingLookups(ft1, fl2);
                var coords = GetIntersectionCoordinate(fts1Lookup, fl2);
                foreach (var c in coords) // add to our overall list for final checking
                {
                    if (!coordsList.Contains(c))
                    {
                        coordsList.Add(c);
                    }   
                }
            }
            if (coordsList.Count == 1)
            {
                CreateIntersectionGraphic(coordsList.First().Trim());
            }
        }

        private IEnumerable<FeatureLookup> FetchIntersectingLookups(string ft1, FeatureLookup ft2)
        {
            if (ft1.Length <= 0 || ft2 == null) return null;
            var ctx = NtsSpatialContext.GEO; // using NTS context (provides polygon/line/point models)
            Shape shp2 = ctx.ReadShape(ft2.Shape);  // load ft2 shape
            // find all other possible features that match ft1 name
            var fts1Lookup = new List<FeatureLookup>();
            IEnumerable<ScoreDoc> hits = ExecuteExactRoadQuery(ft1);
            foreach (var hit in hits)
            {
                var doc = _indexSearcher.Doc(hit.Doc);
                // load a possible intersecting feature shape
                Shape shp1 = ctx.ReadShape(doc.Get(GEOSHAPE));
                // validate relation
                if (shp1.Relate(shp2).Equals(Spatial4n.Core.Shapes.SpatialRelation.INTERSECTS))  
                {
                    var fl1 = new FeatureLookup
                    {
                        Shape = doc.Get(GEOSHAPE),
                        Fid = doc.Get(FID),
                        Layer = doc.Get(LYRNAME)
                    };
                    if (!fts1Lookup.Contains(fl1))
                    {
                        fts1Lookup.Add(fl1);
                    }
                }
            }
            return fts1Lookup.ToArray();
        }

        private IEnvelope CreateBufferGraphic(IGeometry ft)
        {
            if (_polylineGraphicsLayer == null)
            {
                _polylineGraphics = new FeatureSet(FeatureType.Line);
                _polylineGraphicsLayer = new MapLineLayer(_polylineGraphics);

                LineCap lineCap;
                Enum.TryParse(SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicLineStyle, true, out lineCap);
                DashStyle lineStyle;
                Enum.TryParse(SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicLineCap, true, out lineStyle);
                _polylineGraphicsLayer.Symbolizer = new LineSymbolizer(
                    SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicLineColor,
                    SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicLineBorderColor,
                    SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicLineSize,
                    lineStyle,
                    lineCap);
                Map.MapFrame.DrawingLayers.Add(_polylineGraphicsLayer);
            }
            var buffer = ft.Buffer(SdrConfig.Project.Go2ItProjectSettings.Instance.SearchBufferDistance);
            _polylineGraphics.AddFeature(buffer);
            return buffer.Envelope;
        }

        private void CreatePointHighlightGraphic(Coordinate c)
        {
            if (_pointGraphicsLayer == null)
            {
                _pointGraphics = new FeatureSet(FeatureType.Point);
                _pointGraphicsLayer = new MapPointLayer(_pointGraphics);
                PointShape pointShape;
                Enum.TryParse(SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicPointStyle, true, out pointShape);

                _pointGraphicsLayer.Symbolizer = new PointSymbolizer(
                    SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicPointColor,
                    pointShape,
                    SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicPointSize);

                // TODO: I do not recall what I was planning to do here
                //  Symbol = new CharacterSymbol()
                // _pointGraphicsLayer.Symbolizer.Symbols.Add();

                Map.MapFrame.DrawingLayers.Add(_pointGraphicsLayer);
            }
            var point = new Point(c);
            _pointGraphics.AddFeature(point);
        }

        private void ZoomToFeatureLookup(FeatureLookup ftLookup)
        {
            if (ftLookup == null) return;

            // make sure the active map panel has the layer available for display
            ActivateMapPanelWithLayer(ftLookup.Layer);
            // cycle through layers of the active map panel and find the one we need
            var layers = Map.GetFeatureLayers();
            foreach (IMapFeatureLayer mapLayer in layers)
            {
                if (mapLayer != null &&
                    String.IsNullOrEmpty(Path.GetFileNameWithoutExtension((mapLayer.DataSet.Filename)))) return;
                if (mapLayer == null) continue;

                IFeatureSet fs = FeatureSet.Open(mapLayer.DataSet.Filename);
                if (fs == null || fs.Name != ftLookup.Layer) continue;

                // grab the feature and use the shape to generate a buffer around it
                var ft = fs.GetFeature(Convert.ToInt32(ftLookup.Fid));

                if (ft.Coordinates.Count == 1)
                {
                    CreatePointHighlightGraphic(ft.Coordinates[0]);
                }
                IEnvelope buffEnv = CreateBufferGraphic(ft.BasicGeometry as Geometry);

                var zoomInFactor = (double)SdrConfig.Project.Go2ItProjectSettings.Instance.SearchZoomFactor;
                var newExtentWidth = Map.ViewExtents.Width * zoomInFactor;
                var newExtentHeight = Map.ViewExtents.Height * zoomInFactor;
                buffEnv.ExpandBy(newExtentWidth, newExtentHeight);
                Map.ViewExtents = buffEnv.ToExtent();
            }
        }

        private void SearchPanelOnRowDoublelicked(object sender, EventArgs eventArgs)
        {
            var evnt = eventArgs as DataGridViewCellEventArgs;
            if (evnt == null) return;

            var dgvr = _dataGridView.Rows[evnt.RowIndex];
            var fidIdx = GetColumnDisplayIndex(FID, _dataGridView);
            var lyrIdx = GetColumnDisplayIndex(LYRNAME, _dataGridView);
            var shpIdx = GetColumnDisplayIndex(GEOSHAPE, _dataGridView);

            _activeLookup = new FeatureLookup
            {
                Fid = dgvr.Cells[fidIdx].Value.ToString(),
                Layer = dgvr.Cells[lyrIdx].Value.ToString(),
                Shape = dgvr.Cells[shpIdx].Value.ToString()
            };

            switch (PluginSettings.Instance.SearchMode)
            {
                case SearchMode.Intersection:
                    if (_searchPanel.SearchQuery.Length <= 0) return;
                    var arr = _searchPanel.SearchQuery.Split('|');
                    if (arr.Length > 0 && arr[0].Length > 0)
                    {
                        ZoomToIntersection(arr[0], _activeLookup);
                    }
                    break;
                default:  // normal operation is to go to the chosen feature
                    ZoomToFeatureLookup(_activeLookup);
                    break;
            }
        }

        private void SearchPanelOnHydrantLocate(object sender, EventArgs eventArgs)
        {
            if (_activeLookup == null)
            {
                MessageBox.Show(@"No feature or location is currently active");
                return;
            }
            var layers = Map.GetPointLayers();
            IFeatureSet hydrantfs = null;
            foreach (IMapPointLayer ptLayer in layers)
            {
                if (ptLayer != null && String.IsNullOrEmpty(Path.GetFileNameWithoutExtension((ptLayer.DataSet.Filename)))) return;
                var fs = FeatureSet.Open(ptLayer.DataSet.Filename);
                if (fs != null && fs.Name == SdrConfig.Project.Go2ItProjectSettings.Instance.HydrantsLayer)
                {
                    hydrantfs = fs;
                    break;
                }
            }
            if (hydrantfs == null) return;
            // store the current index type | reset when operation is complete
            var idxType = _indexType;  
            SetupIndexReaderWriter(SearchMode.Hydrant.ToString());
            var hits = ExecuteScoredHydrantQuery(_activeLookup);
            if (hits.Length == 0)
            {
                SetupIndexReaderWriter(idxType);  // set the index searcher back
                MessageBox.Show(@"No hydrants within the search area");
            }
            // create a graphic for each hydrant to be displayed
            for (int i = 0; i <= SdrConfig.Project.Go2ItProjectSettings.Instance.HydrantSearchCount -1; i++)
            {
                if (hits.Length >= i + 1)
                {
                    var hit = hits[i];
                    var doc = _indexSearcher.Doc(hit.Doc);
                    var ft = hydrantfs.GetFeature(Convert.ToInt32(doc.Get(FID)));
                    if (ft.Coordinates.Count == 1)
                    {
                        CreatePointHighlightGraphic(ft.Coordinates[0]);
                    }
                }
            }

            SetupIndexReaderWriter(idxType);  // set the index searcher back
            var zoomFactor = (double)SdrConfig.Project.Go2ItProjectSettings.Instance.SearchZoomFactor;

            var newExtentWidth = _pointGraphics.Extent.Width * zoomFactor;
            var newExtentHeight = _pointGraphics.Extent.Height * zoomFactor;

            _pointGraphics.Extent.ExpandBy(newExtentWidth, newExtentHeight);

            var env = _pointGraphics.Extent.ToEnvelope();
            Map.ViewExtents = env.ToExtent();
        }
        #endregion

        #region Methods

        private ScoreDoc[] ExecuteScoredHydrantQuery(FeatureLookup activeLookup)
        {
            var ctx = NtsSpatialContext.GEO; // using NTS (provides polygon/line/point models)
            Shape shp = ctx.ReadShape(activeLookup.Shape);
            Spatial4n.Core.Shapes.Point centerPt = shp.GetCenter();

            SpatialStrategy strategy = new RecursivePrefixTreeStrategy(new GeohashPrefixTree(ctx, 24), GEOSHAPE);
            var args = new SpatialArgs(SpatialOperation.Intersects,
                ctx.MakeCircle(centerPt.GetX(), centerPt.GetY(),
                    DistanceUtils.Dist2Degrees(SdrConfig.Project.Go2ItProjectSettings.Instance.HydrantSearchDistance, DistanceUtils.EARTH_MEAN_RADIUS_KM)));

            Filter filter = strategy.MakeFilter(args);
            ValueSource valueSource = strategy.MakeDistanceValueSource(centerPt);
            var query = new ValueSourceQuery(valueSource);
            var sort = new Sort(new SortField("DISTANCE", SortField.SCORE, true));

            TopDocs docs = _indexSearcher.Search(query, filter, 10, sort);
            return docs.ScoreDocs;
        }

        private IEnumerable<ScoreDoc> ExecuteLuceneQuery(string sq)
        {
            // todo: handle any missing cases remaining
            ScoreDoc[] hits = null;
            switch (PluginSettings.Instance.SearchMode)
            {
                case SearchMode.Address:
                    hits = ExecuteScoredAddressQuery(sq);
                    break;
                case SearchMode.Road:
                    hits = ExecuteScoredRoadQuery(sq);
                    break;
                case SearchMode.Intersection:
                    hits = ExecuteGetIntersectionsQuery(sq);
                    break;
                case SearchMode.Name:
                    hits = ExecuteScoredNameQuery(sq);
                    break;
                case SearchMode.Phone:
                    hits = ExecuteScoredPhoneQuery(sq);
                    break;
                case SearchMode.KeyLocation:
                    hits = ExecuteScoredKeyLocationsQuery(sq);
                    break;
                case SearchMode.Parcel:
                    hits = ExecuteScoredParcelsQuery(sq);
                    break;
                case SearchMode.Esn:
                    hits = ExecuteScoredEsnQuery(sq);
                    break;
                case SearchMode.City:
                    hits = ExecuteScoredCityQuery(sq);
                    break;
                case SearchMode.CellSector:
                    hits = ExecuteScoredCellSectorsQuery(sq);
                    break;
                case SearchMode.All:
                    hits = ExecuteScoredAllIndexesQuery(sq);
                    break;
            }
            return hits;
        }

        private static int GetColumnDisplayIndex(string name, DataGridView dgv)
        {
            for (int i = 0; i <= dgv.ColumnCount - 1; i++)
            {
                if (dgv.Columns[i].Name == name)
                {
                    return dgv.Columns[i].DisplayIndex;
                }
            }
            return -1;
        }

        private void PrepareDataGridView()
        {
            // setup the columns and ordering
            var colArr = new DataGridViewColumn[_columnNames.Count()];
            // check for any sort order the user may have set
            var orderDict = GetIndexColumnsOrder();
            var tList = new List<DataGridViewColumn>();
            // add our columns to the datagridview 
            for (var i = 0; i < _columnNames.Count(); i++)
            {
                var columnName = _columnNames[i];
                var txtCol = new DataGridViewTextBoxColumn
                {
                    HeaderText = columnName,
                    Name = columnName,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                // check if there is any sort order set
                if (orderDict != null)
                {
                    if (orderDict.ContainsKey(columnName))
                    {
                        var s = orderDict[columnName];
                        var j = -1;
                        int.TryParse(s, out j);
                        if (j < 0) continue;

                        txtCol.DisplayIndex = j;
                        colArr[j] = txtCol;
                    }
                    else
                    {
                        // field doesnt exists in column order
                        // save it temp and append after others have been "set"
                        tList.Add(txtCol);
                    }
                }
                else
                {
                    colArr[i] = txtCol;
                }
            }
            // check if we added any cols to temp list for later appending
            foreach (var tCol in tList)
            {
                for (var x = 0; x < colArr.Length; x++)
                {
                    if (colArr[x] != null) continue;
                    colArr[x] = tCol;
                    break;
                }
            }
            _dataGridView.Columns.AddRange(colArr);
            // add in our columns for data feature lookup
            var fidCol = new DataGridViewTextBoxColumn
            {
                HeaderText = FID,
                Name = FID,
                Visible = false
            };
            _dataGridView.Columns.Add(fidCol);
            var lyrCol = new DataGridViewTextBoxColumn
            {
                HeaderText = LYRNAME,
                Name = LYRNAME,
                Visible = false
            };
            _dataGridView.Columns.Add(lyrCol);
            var shpCol = new DataGridViewTextBoxColumn
            {
                HeaderText = GEOSHAPE,
                Name = GEOSHAPE,
                Visible = false
            };
            _dataGridView.Columns.Add(shpCol);
            // setup event binding if user reorders columns, save user settings
            _dataGridView.ColumnDisplayIndexChanged += DataGridViewOnColumnDisplayIndexChanged;
        }

        private Dictionary<string, string> GetMapTabKeysContainingLayer(string lyrName)
        {
            var conn = SQLiteHelper.GetSQLiteConnectionString(_currentProjectFile);

            const string sql = "SELECT layers, lookup, caption FROM MapTabs";
            DataTable mapTabsTable = SQLiteHelper.GetDataTable(conn, sql);
            var mapPanelsLookup = new Dictionary<string, string>();
            foreach (DataRow row in mapTabsTable.Rows)
            {
                // parse the layers string into layer names
                var lyrs = row["layers"].ToString().Split('|');
                var lyrMatch = lyrs.Any(lyr => lyr == lyrName);
                if (lyrMatch)
                {
                    mapPanelsLookup.Add(row["lookup"].ToString(), row["caption"].ToString());
                }
            }
            return mapPanelsLookup;
        }

        private Dictionary<string, string> GetIndexColumnsOrder()
        {
            switch (PluginSettings.Instance.SearchMode)
            {
                case SearchMode.Address:
                    return PluginSettings.Instance.AddressIndexColumnOrder;
                case SearchMode.Intersection:
                    return PluginSettings.Instance.RoadIndexColumnOrder;
                case SearchMode.Name:
                    return PluginSettings.Instance.AddressIndexColumnOrder;
                case SearchMode.Phone:
                    return PluginSettings.Instance.AddressIndexColumnOrder;
                case SearchMode.Road:
                    return PluginSettings.Instance.RoadIndexColumnOrder;
                case SearchMode.KeyLocation:
                    return PluginSettings.Instance.KeyLocationIndexColumnOrder;
                case SearchMode.Parcel:
                    return PluginSettings.Instance.ParcelIndexColumnOrder;
                case SearchMode.CellSector:
                    return PluginSettings.Instance.CellSectorIndexColumnOrder;
                case SearchMode.All:
                    // TODO:
                    return null;
            }
            return null;
        }

        private string[] GetColumnNames()
        {
            if (_currentProjectFile.Length == 0)
            {
                return new string[0];
            }
            var conn = SQLiteHelper.GetSQLiteConnectionString(_currentProjectFile);
            var tblNames = SQLiteHelper.GetAllTableNames(conn);
            var sql = string.Empty;

            foreach (string tblName in tblNames.Where(tblName => tblName.StartsWith(_indexType)))
            {
                if (sql.Length == 0)
                {
                    sql = "SELECT lookup FROM " + tblName;
                }
                else
                {
                    sql = sql + " UNION SELECT lookup FROM " + tblName;
                }
            }
            return sql.Length == 0 ? new string[0] : SQLiteHelper.GetResultsAsArray(conn, sql);
        }

        private Directory FetchSearchDirectory(SearchMode m)
        {
            return GetLuceneIndexDirectory(m + "Index");
        }

        private Directory[] FetchAllIndexDirectories()
        {
            // array list of all available directories (used by the multireader on search all)
            var idxList = new List<Directory>
            {
                // cycle through all available index directories and populate to arraylist
                FetchSearchDirectory(SearchMode.Address),
                FetchSearchDirectory(SearchMode.Road),
                FetchSearchDirectory(SearchMode.KeyLocation),
                FetchSearchDirectory(SearchMode.City),
                FetchSearchDirectory(SearchMode.CellSector),
                FetchSearchDirectory(SearchMode.Parcel),
                FetchSearchDirectory(SearchMode.Esn),
                FetchSearchDirectory(SearchMode.Hydrant)
            };
            // remove any null values and return result as an array
            return idxList.Where(d => d != null).ToArray();
        }

        private void SetupIndexReaderWriter(string idxType)
        {
            // reset the indexsearcher if it has been initialized
            if (_indexSearcher != null)
            {
                _indexSearcher.Dispose();
                _indexSearcher.IndexReader.Dispose();
            }
            // if no index type is set then this is a coordinate based search
            if (_indexType == string.Empty) return;
            // setup the reader and searchers for the indexType being searched
            if (_indexType == SearchMode.All + "Index")
            {
                Directory[] indexDirs = FetchAllIndexDirectories();
                // searching across all indexes requires an array of indexreaders
                var idxReaders = new IndexReader[indexDirs.Length];
                for (int i = 0; i < indexDirs.Length; i++)
                {
                    idxReaders[i] = IndexReader.Open(indexDirs[i], true);  // readonly for performance
                }
                _indexSearcher = new IndexSearcher(new MultiReader(idxReaders));
            }
            else
            {
                // single index type search initiated
                Directory idxDir = GetLuceneIndexDirectory(idxType);
                if (idxDir != null)
                {
                    var idxReader = IndexReader.Open(idxDir, true);  // readonly for performance
                    _indexSearcher = new IndexSearcher(idxReader);
                }
            }
        }

        public Directory GetLuceneIndexDirectory(string indexType)
        {
            if (_currentProjectFile.Length == 0)
            {
                return null;
            }

            var conn = SQLiteHelper.GetSQLiteConnectionString(_currentProjectFile);
            var db = SQLiteHelper.GetSQLiteFileName(conn);

            var projectName = Path.GetFileNameWithoutExtension(db);

            var d = Path.GetDirectoryName(db);
            if (d == null) return null;

            var path = Path.Combine(d, projectName + "_indexes", indexType);
            if (!System.IO.Directory.Exists(path)) return null;

            Directory idxDir = FSDirectory.Open(new DirectoryInfo(path));
            return idxDir;
        }

        private Query GetSearchAllQuery(string q)
        {
            // fetch all the avilable field names that are searchable regardless of index
            var fldList = _indexSearcher.IndexReader.GetFieldNames(IndexReader.FieldOption.INDEXED);
            // arrays for storing all the values to pass into the index search
            var values = new ArrayList();
            var fields = new ArrayList();
            var occurs = new ArrayList();

            string[] qTermArray = q.Split(' ');
            foreach (string fld in fldList)
            {
                // no need to search these fields as they are for internal use only
                if (fld == "FID" || fld == "LYRNAME" || fld == "GEOSHAPE") continue;
                foreach (var qTerm in qTermArray)
                {
                    values.Add(qTerm);
                    fields.Add(fld);
                    occurs.Add(Occur.SHOULD);
                }
            }
            // turn the array lists into static arrays
            var vals = (string[])values.ToArray(typeof(string));
            var flds = (string[])fields.ToArray(typeof(string));
            var ocrs = (Occur[])occurs.ToArray(typeof(Occur));

            // create lucene query from query string arrays
            Query query = MultiFieldQueryParser.Parse(
                LuceneVersion,
                vals,
                flds,
                ocrs,
                LuceneAnalyzer
            );
            return query;
        }

        private ScoreDoc[] ExecuteScoredAllIndexesQuery(string q)
        {
            if (_indexSearcher.IndexReader == null)
            {
                return new ScoreDoc[0];
            }
            TopDocs docs = _indexSearcher.Search(GetSearchAllQuery(q), _indexSearcher.IndexReader.NumDocs());
            // return our results
            return docs.ScoreDocs;
        }

        private ScoreDoc[] ExecuteScoredCellSectorsQuery(string q)
        {
            // arrays for storing all the values to pass into the index search
            var values = new ArrayList();
            var fields = new ArrayList();
            var occurs = new ArrayList();

            string[] namesArray = q.Split(' ');
            foreach (var name in namesArray)
            {
                values.Add(name);
                fields.Add("Sector ID");
                occurs.Add(Occur.SHOULD);

                values.Add(name);
                fields.Add("Tower ID");
                occurs.Add(Occur.SHOULD);

                values.Add(name);
                fields.Add("Company ID");
                occurs.Add(Occur.SHOULD);
            }
            var vals = (string[])values.ToArray(typeof(string));
            var flds = (string[])fields.ToArray(typeof(string));
            var ocrs = (Occur[])occurs.ToArray(typeof(Occur));
            // create lucene query from query string arrays
            Query query = MultiFieldQueryParser.Parse(
                LuceneVersion,
                vals,
                flds,
                ocrs,
                LuceneAnalyzer
                );
            if (_indexSearcher.IndexReader == null)
            {
                return new ScoreDoc[0];
            }
            TopDocs docs = _indexSearcher.Search(query, _indexSearcher.IndexReader.NumDocs());
            // return our results
            return docs.ScoreDocs;
        }

        private ScoreDoc[] ExecuteScoredKeyLocationsQuery(string q)
        {
            // arrays for storing all the values to pass into the index search
            var values = new ArrayList();
            var fields = new ArrayList();
            var occurs = new ArrayList();

            string[] namesArray = q.Split(' ');
            foreach (var name in namesArray)
            {
                values.Add(name);
                fields.Add("Name");
                occurs.Add(Occur.SHOULD);

                values.Add(name);
                fields.Add("Type");
                occurs.Add(Occur.SHOULD);

                values.Add(name);
                fields.Add("Description");
                occurs.Add(Occur.SHOULD);
            }
            var vals = (string[])values.ToArray(typeof(string));
            var flds = (string[])fields.ToArray(typeof(string));
            var ocrs = (Occur[])occurs.ToArray(typeof(Occur));
            // create lucene query from query string arrays
            Query query = MultiFieldQueryParser.Parse(
                LuceneVersion,
                vals,
                flds,
                ocrs,
                LuceneAnalyzer
                );
            if (_indexSearcher.IndexReader == null)
            {
                return new ScoreDoc[0];
            }
            TopDocs docs = _indexSearcher.Search(query, _indexSearcher.IndexReader.NumDocs());
            // return our results
            return docs.ScoreDocs;
        }

        private ScoreDoc[] ExecuteScoredEsnQuery(string q)
        {
            // arrays for storing all the values to pass into the index search
            var values = new ArrayList();
            var fields = new ArrayList();
            var occurs = new ArrayList();

            string[] namesArray = q.Split(' ');
            foreach (var name in namesArray)
            {
                values.Add(name);
                fields.Add("Name");
                occurs.Add(Occur.SHOULD);
            }
            var vals = (string[])values.ToArray(typeof(string));
            var flds = (string[])fields.ToArray(typeof(string));
            var ocrs = (Occur[])occurs.ToArray(typeof(Occur));
            // create lucene query from query string arrays
            Query query = MultiFieldQueryParser.Parse(
                LuceneVersion,
                vals,
                flds,
                ocrs,
                LuceneAnalyzer
                );
            if (_indexSearcher.IndexReader == null)
            {
                return new ScoreDoc[0];
            }
            TopDocs docs = _indexSearcher.Search(query, _indexSearcher.IndexReader.NumDocs());
            // return our results
            return docs.ScoreDocs;
        }

        private ScoreDoc[] ExecuteScoredCityQuery(string q)
        {
            // arrays for storing all the values to pass into the index search
            var values = new ArrayList();
            var fields = new ArrayList();
            var occurs = new ArrayList();

            string[] namesArray = q.Split(' ');
            foreach (var name in namesArray)
            {
                values.Add(name);
                fields.Add("Name");
                occurs.Add(Occur.SHOULD);
            }
            var vals = (string[])values.ToArray(typeof(string));
            var flds = (string[])fields.ToArray(typeof(string));
            var ocrs = (Occur[])occurs.ToArray(typeof(Occur));
            // create lucene query from query string arrays
            Query query = MultiFieldQueryParser.Parse(
                LuceneVersion,
                vals,
                flds,
                ocrs,
                LuceneAnalyzer
                );
            if (_indexSearcher.IndexReader == null)
            {
                return new ScoreDoc[0];
            }
            TopDocs docs = _indexSearcher.Search(query, _indexSearcher.IndexReader.NumDocs());
            // return our results
            return docs.ScoreDocs;
        }

        private ScoreDoc[] ExecuteScoredParcelsQuery(string q)
        {
            // arrays for storing all the values to pass into the index search
            var values = new ArrayList();
            var fields = new ArrayList();
            var occurs = new ArrayList();

            string[] namesArray = q.Split(' ');
            foreach (var name in namesArray)
            {
                values.Add(name);
                fields.Add("Parcel ID");
                occurs.Add(Occur.SHOULD);

                values.Add(name);
                fields.Add("Owner Name");
                occurs.Add(Occur.SHOULD);

                values.Add(name);
                fields.Add("Other 1");
                occurs.Add(Occur.SHOULD);

                values.Add(name);
                fields.Add("Other 2");
                occurs.Add(Occur.SHOULD);
            }
            var vals = (string[])values.ToArray(typeof(string));
            var flds = (string[])fields.ToArray(typeof(string));
            var ocrs = (Occur[])occurs.ToArray(typeof(Occur));
            // create lucene query from query string arrays
            Query query = MultiFieldQueryParser.Parse(
                LuceneVersion,
                vals,
                flds,
                ocrs,
                LuceneAnalyzer
                );
            if (_indexSearcher.IndexReader == null)
            {
                return new ScoreDoc[0];
            }
            TopDocs docs = _indexSearcher.Search(query, _indexSearcher.IndexReader.NumDocs());
            // return our results
            return docs.ScoreDocs;
        }

        private ScoreDoc[] ExecuteScoredNameQuery(string q)
        {
            // arrays for storing all the values to pass into the index search
            var values = new ArrayList();
            var fields = new ArrayList();
            var occurs = new ArrayList();

            string[] namesArray = q.Split(' ');
            foreach (var name in namesArray)
            {
                values.Add(name);
                fields.Add("First Name");
                occurs.Add(Occur.SHOULD);

                values.Add(name);
                fields.Add("Last Name");
                occurs.Add(Occur.SHOULD);
            }
            var vals = (string[])values.ToArray(typeof(string));
            var flds = (string[])fields.ToArray(typeof(string));
            var ocrs = (Occur[])occurs.ToArray(typeof(Occur));
            // create lucene query from query string arrays
            Query query = MultiFieldQueryParser.Parse(
                LuceneVersion,
                vals,
                flds,
                ocrs,
                LuceneAnalyzer
                );
            if (_indexSearcher.IndexReader == null)
            {
                return new ScoreDoc[0];
            }
            TopDocs docs = _indexSearcher.Search(query, _indexSearcher.IndexReader.NumDocs());
            // return our results
            return docs.ScoreDocs;
        }

        private ScoreDoc[] ExecuteScoredPhoneQuery(string q)
        {
            var num = DigitsOnly.Replace(q, "");

            // arrays for storing all the values to pass into the index search
            var values = new ArrayList();
            var fields = new ArrayList();
            var occurs = new ArrayList();

            values.Add(num);
            fields.Add("Phone");
            occurs.Add(Occur.SHOULD);

            values.Add(num);
            fields.Add("Aux. Phone");
            occurs.Add(Occur.SHOULD);

            var vals = (string[])values.ToArray(typeof(string));
            var flds = (string[])fields.ToArray(typeof(string));
            var ocrs = (Occur[])occurs.ToArray(typeof(Occur));
            // create lucene query from query string arrays
            Query query = MultiFieldQueryParser.Parse(
                LuceneVersion,
                vals,
                flds,
                ocrs,
                LuceneAnalyzer
                );

            if (_indexSearcher.IndexReader == null)
            {
                return new ScoreDoc[0];
            }
            TopDocs docs = _indexSearcher.Search(query, _indexSearcher.IndexReader.NumDocs());
            // return our results
            return docs.ScoreDocs;
        }

        private ScoreDoc[] ExecuteScoredAddressQuery(string q)
        {
            // parse our input address into a valid streetaddress object
            StreetAddress streetAddress = StreetAddressParser.Parse(q, SdrConfig.Project.Go2ItProjectSettings.Instance.SearchUsePretypes);
            LogStreetAddressParsedQuery(q, streetAddress);
            // arrays for storing all the values to pass into the index search
            var values = new ArrayList();
            var fields = new ArrayList();
            var occurs = new ArrayList();
            // assemble our query string now
            if (streetAddress.Number != null)
            {
                values.Add(streetAddress.Number);
                fields.Add("Structure Number");
                occurs.Add(Occur.MUST);
            }
            if (streetAddress.Predirectional != null)
            {
                values.Add(streetAddress.Predirectional);
                fields.Add("Pre Directional");
                occurs.Add(Occur.SHOULD);
            }
            if (SdrConfig.Project.Go2ItProjectSettings.Instance.SearchUsePretypes)
            {
                if (streetAddress.PreType != null)
                {
                    values.Add(streetAddress.PreType);
                    fields.Add("Pre Type");
                    occurs.Add(Occur.SHOULD);
                }
            }
            if (streetAddress.StreetName != null)
            {
                values.Add(streetAddress.StreetName);
                fields.Add("Street Name");
                occurs.Add(Occur.MUST);
            }
            if (streetAddress.StreetType != null)
            {
                values.Add(streetAddress.StreetType);
                fields.Add("Street Type");
                occurs.Add(Occur.SHOULD);
            }
            if (streetAddress.Postdirectional != null)
            {
                values.Add(streetAddress.Postdirectional);
                fields.Add("Post Directional");
                occurs.Add(Occur.SHOULD);
            }
            if (streetAddress.SubUnitType != null)
            {
                values.Add(streetAddress.SubUnitType);
                fields.Add("Sub Unit Type");
                occurs.Add(Occur.SHOULD);
            }
            if (streetAddress.SubUnitValue != null)
            {
                values.Add(streetAddress.SubUnitValue);
                fields.Add("Sub Unit Designation");
                occurs.Add(Occur.SHOULD);
            }
            var vals = (string[]) values.ToArray(typeof (string));
            var flds = (string[]) fields.ToArray(typeof (string));
            var ocrs = (Occur[]) occurs.ToArray(typeof (Occur));
            // create lucene query from query string arrays
            Query query = MultiFieldQueryParser.Parse(
                LuceneVersion,
                vals,
                flds,
                ocrs,
                LuceneAnalyzer
                );

            if (_indexSearcher.IndexReader == null)
            {
                return new ScoreDoc[0];
            }
            TopDocs docs = _indexSearcher.Search(query, _indexSearcher.IndexReader.NumDocs());
            // return our results
            return docs.ScoreDocs;
        }

        private ScoreDoc[] ExecuteScoredRoadQuery(string q)
        {
            // parse our input road into a streetaddress object for analysis
            StreetAddress streetAddress = StreetAddressParser.Parse(q, SdrConfig.Project.Go2ItProjectSettings.Instance.SearchUsePretypes);
            // any values parsed to the structure number should be moved to street name because this is a road search
            if (streetAddress.Number != null)
            {
                // TODO: decide if this is needed
                //if (streetAddress.StreetName == streetAddress.StreetType)
                //{
                //    streetAddress.StreetName = streetAddress.Number;
                //}
                //else
                //{
                    streetAddress.StreetName = streetAddress.Number + " " + streetAddress.StreetName;
                // }
                streetAddress.Number = null;
                streetAddress.StreetName = streetAddress.StreetName.Trim();
            }
            LogStreetAddressParsedQuery(q, streetAddress);
            // arrays for storing all the values to pass into the index search
            var values = new ArrayList();
            var fields = new ArrayList();
            var occurs = new ArrayList();
            // assemble our query string now
            if (streetAddress.Predirectional != null)
            {
                values.Add(streetAddress.Predirectional);
                fields.Add("Pre Directional");
                occurs.Add(Occur.SHOULD);
            }
            if (SdrConfig.Project.Go2ItProjectSettings.Instance.SearchUsePretypes)
            {
                if (streetAddress.PreType != null)
                {
                    values.Add(streetAddress.PreType);
                    fields.Add("Pre Type");
                    occurs.Add(Occur.SHOULD);
                }
            }
            if (streetAddress.StreetName != null)
            {
                values.Add(streetAddress.StreetName);
                fields.Add("Street Name");
                occurs.Add(Occur.MUST);
            }
            if (streetAddress.StreetType != null)
            {
                values.Add(streetAddress.StreetType);
                fields.Add("Street Type");
                occurs.Add(Occur.SHOULD);
            }
            if (streetAddress.Postdirectional != null)
            {
                values.Add(streetAddress.Postdirectional);
                fields.Add("Post Directional");
                occurs.Add(Occur.SHOULD);
            }
            var vals = (string[])values.ToArray(typeof(string));
            var flds = (string[])fields.ToArray(typeof(string));
            var ocrs = (Occur[])occurs.ToArray(typeof(Occur));
            // create lucene query from query string arrays
            Query query = MultiFieldQueryParser.Parse(
                LuceneVersion,
                vals,
                flds,
                ocrs,
                LuceneAnalyzer
                );
            if (_indexSearcher.IndexReader == null)
            {
                return new ScoreDoc[0];
            }
            TopDocs docs = _indexSearcher.Search(query, _indexSearcher.IndexReader.NumDocs());
            // return our results
            return docs.ScoreDocs;
        }

        private IEnumerable<ScoreDoc> ExecuteExactRoadQuery(string q)
        {
            // parse our input address into a valid streetaddress object
            StreetAddress streetAddress = StreetAddressParser.Parse(q, SdrConfig.Project.Go2ItProjectSettings.Instance.SearchUsePretypes);
            // any values parsed to the structure number should be moved to street name because this is a road search
            if (streetAddress.Number != null)
            {
                // TODO: decide if this is needed
                //if (streetAddress.StreetName == streetAddress.StreetType)
                //{
                //    streetAddress.StreetName = streetAddress.Number;
                //}
                //else
                //{
                    streetAddress.StreetName = streetAddress.Number + " " + streetAddress.StreetName;
                // }
                streetAddress.Number = null;
                streetAddress.StreetName = streetAddress.StreetName.Trim();
            }
            LogStreetAddressParsedQuery(q, streetAddress);
            // arrays for storing all the values to pass into the index search
            var values = new ArrayList();
            var fields = new ArrayList();
            var occurs = new ArrayList();
            // assemble our query string now
            if (streetAddress.Predirectional != null)
            {
                values.Add(streetAddress.Predirectional);
                fields.Add("Pre Directional");
                occurs.Add(Occur.MUST);
            }
            if (SdrConfig.Project.Go2ItProjectSettings.Instance.SearchUsePretypes)
            {
                if (streetAddress.PreType != null)
                {
                    values.Add(streetAddress.PreType);
                    fields.Add("Pre Type");
                    occurs.Add(Occur.SHOULD);
                }
            }
            if (streetAddress.StreetName != null)
            {
                values.Add(streetAddress.StreetName);
                fields.Add("Street Name");
                occurs.Add(Occur.MUST);
            }
            if (streetAddress.StreetType != null)
            {
                values.Add(streetAddress.StreetType);
                fields.Add("Street Type");
                occurs.Add(Occur.MUST);
            }
            if (streetAddress.Postdirectional != null)
            {
                values.Add(streetAddress.Postdirectional);
                fields.Add("Post Directional");
                occurs.Add(Occur.MUST);
            }
            var vals = (string[])values.ToArray(typeof(string));
            var flds = (string[])fields.ToArray(typeof(string));
            var ocrs = (Occur[])occurs.ToArray(typeof(Occur));
            // create lucene query from query string arrays
            Query query = MultiFieldQueryParser.Parse(
                LuceneVersion,
                vals,
                flds,
                ocrs,
                LuceneAnalyzer
                );
            if (_indexSearcher.IndexReader == null)
            {
                return new ScoreDoc[0];
            }
            TopDocs docs = _indexSearcher.Search(query, _indexSearcher.IndexReader.NumDocs());
            // return our results
            return docs.ScoreDocs;
        }

        private ScoreDoc[] ExecuteGetIntersectionsQuery(string q)
        {
            if (q.Length <= 0) return null;
            // get the name of the street passed in so it is removed from results returned
            var sa = StreetAddressParser.Parse(q, SdrConfig.Project.Go2ItProjectSettings.Instance.SearchUsePretypes);
            var docs = new List<ScoreDoc>();  // total docs for return
            IEnumerable<ScoreDoc> qHits = ExecuteExactRoadQuery(q);
            // setup a spatial query to find all features that intersect with our results
            var ctx = NtsSpatialContext.GEO; // using NTS (provides polygon/line/point models)
            SpatialStrategy strategy = new RecursivePrefixTreeStrategy(new GeohashPrefixTree(ctx, 24), GEOSHAPE);
            // term query to remove features with the same name, ie segments of the road
            var saTerm = new Term("Street Name", sa.StreetName.ToLower());
            Query tq = new TermQuery(saTerm);
            foreach (var qHit in qHits)
            {
                var doc = _indexSearcher.Doc(qHit.Doc);  // snag the current doc for additional spatial queries
                var strShp = doc.Get(GEOSHAPE);  // get the string representation of the feature shape
                Shape shp = ctx.ReadShape(strShp);  // read the wkt string into an actual shape object
                // prepare spatial query
                var args = new SpatialArgs(SpatialOperation.Intersects, shp);
                Query sq = strategy.MakeQuery(args);
                // create overall boolean query to pass to indexsearcher
                var query = new BooleanQuery {{sq, Occur.MUST}, {tq, Occur.MUST_NOT}};
                // execute a query to find all features that intersect each passed in feature
                if (_indexSearcher.IndexReader == null)
                {
                    return new ScoreDoc[0];
                }
                TopDocs topDocs = _indexSearcher.Search(query, _indexSearcher.IndexReader.NumDocs());
                ScoreDoc[] hits = topDocs.ScoreDocs;
                // add to results for cleanup after loop completes
                docs.AddRange(hits);
            }
            // remove any duplicates by street name, ignoring scoring
            var cdocs = docs.GroupBy(x => x.Doc).Select(x => x.First()).ToList<ScoreDoc>();
            return cdocs.ToArray();
        }

        private string FormatPhoneNumber(string p)
        {
            switch (p.Length)
            {
                case 11:
                    return p.Substring(0, 1) + "-" + p.Substring(1, 3) + "-" + p.Substring(4, 3) + "-" + p.Substring(7, 4);
                case 10:
                    return p.Substring(0, 3) + "-" + p.Substring(3, 3) + "-" + p.Substring(6, 4);
                case 7:
                    return p.Substring(0, 3) + "-" + p.Substring(3, 4);
                default:
                    return p;
            }
        }

        private void PopulateSingleQueryResultsToDgv(IEnumerable<ScoreDoc> hits)
        {
            if (hits == null) return;
            foreach (var hit in hits)
            {
                // generate all the empty cells we need for a full row
                var newCells = new DataGridViewCell[_columnNames.Length];
                for (int i = 0; i <= _columnNames.Length - 1; i++)
                {
                    var txtCell = new DataGridViewTextBoxCell();
                    newCells[i] = txtCell;
                }
                // create the row and populate it
                var dgvRow = new DataGridViewRow();
                dgvRow.Cells.AddRange(newCells);
                // snatch the ranked document
                var doc = _indexSearcher.Doc(hit.Doc);
                foreach (var field in _columnNames)
                {
                    var idx = GetColumnDisplayIndex(field, _dataGridView);
                    var val = doc.Get(field);

                    if (field == "Phone" || field == "Aux. Phone")
                    {
                        dgvRow.Cells[idx].Value = FormatPhoneNumber(val);
                    }
                    else
                    {
                        dgvRow.Cells[idx].Value = val;
                    }
                }
                // add the fid and layername textbox cells
                var fidCell = new DataGridViewTextBoxCell {Value = doc.Get(FID)};
                dgvRow.Cells.Add(fidCell);
                var lyrCell = new DataGridViewTextBoxCell {Value = doc.Get(LYRNAME)};
                dgvRow.Cells.Add(lyrCell);
                var shpCell = new DataGridViewTextBoxCell { Value = doc.Get(GEOSHAPE) };
                dgvRow.Cells.Add(shpCell);
                _dataGridView.Rows.Add(dgvRow);
            }
        }

        private void PopulateMultiQueryResultsToDgv(IEnumerable<ScoreDoc> hits)
        {
            if (hits == null) return;

            IFragmenter fragmenter = new NullFragmenter();
            IScorer scorer = new QueryScorer(GetSearchAllQuery(_searchPanel.SearchQuery));
            Highlighter highlighter = new Highlighter(scorer) { TextFragmenter = fragmenter };

            foreach (var hit in hits)
            {
                var doc = _indexSearcher.Doc(hit.Doc);
                var fds = doc.GetFields();
                foreach (var fld in fds)
                {
                    // check if this field is a match
                    var f = highlighter.GetBestFragment(LuceneAnalyzer, fld.Name, fld.StringValue);
                    if (f != null)
                    {
                        // generate the empty cells required for a full row
                        var newCells = new DataGridViewCell[_columnNames.Length];
                        for (int i = 0; i <= _columnNames.Length - 1; i++)
                        {
                            newCells[i] = new DataGridViewTextBoxCell();
                        }
                        // create the row and populate it
                        var dgvRow = new DataGridViewRow();
                        dgvRow.Cells.AddRange(newCells);

                        // default columns displayed on an all index search
                        var idx = GetColumnDisplayIndex("Layer Name", _dataGridView);
                        dgvRow.Cells[idx].Value = doc.Get(LYRNAME);
                        idx = GetColumnDisplayIndex("Field Name", _dataGridView);
                        dgvRow.Cells[idx].Value = fld.Name;
                        idx = GetColumnDisplayIndex("Field Value", _dataGridView);
                        dgvRow.Cells[idx].Value = fld.StringValue;

                        // add the fid and layername textbox cells (used by various functions)
                        var fidCell = new DataGridViewTextBoxCell { Value = doc.Get(FID) };
                        dgvRow.Cells.Add(fidCell);
                        var lyrCell = new DataGridViewTextBoxCell { Value = doc.Get(LYRNAME) };
                        dgvRow.Cells.Add(lyrCell);
                        var shpCell = new DataGridViewTextBoxCell { Value = doc.Get(GEOSHAPE) };
                        dgvRow.Cells.Add(shpCell);
                        _dataGridView.Rows.Add(dgvRow);
                    }
                }
            }
        }

        private void FormatQueryResults(IEnumerable<ScoreDoc> hits)
        {
            if (hits == null) return;
            switch (PluginSettings.Instance.SearchMode)
            {
                case SearchMode.Intersection:
                    PopulateSingleQueryResultsToDgv(hits);
                    UpdateIntersectedFeatures(hits);
                    break;
                case SearchMode.All:
                    PopulateMultiQueryResultsToDgv(hits);
                    break;
                default:
                    PopulateSingleQueryResultsToDgv(hits);
                    break;
            }
        }

        private void UpdateIntersectedFeatures(IEnumerable<ScoreDoc> hits)
        {
            if (!hits.Any() || hits == null) return;
            var arrList = new ArrayList();
            foreach (var hit in hits)
            {
                var doc = _indexSearcher.Doc(hit.Doc);
                var val = string.Empty;
                // create the full string for combobox population
                if (doc.Get("Pre Directional") != null)
                {
                    val = val + doc.Get("Pre Directional").Trim() + " ";
                }
                if (SdrConfig.Project.Go2ItProjectSettings.Instance.SearchUsePretypes)
                {
                    if (doc.Get("Pre Type") != null)
                    {
                        val = val + doc.Get("Pre Type").Trim() + " ";
                    }
                }
                if (doc.Get("Street Name") != null)
                {
                    val = val + doc.Get("Street Name").Trim() + " ";
                }
                if (doc.Get("Street Type") != null)
                {
                    val = val + doc.Get("Street Type").Trim() + " ";
                }
                if (doc.Get("Post Directional") != null)
                {
                    val = val + doc.Get("Post Directional").Trim() + " ";
                }
                if (val.Length > 0)
                {
                    arrList.Add(val.Trim());
                }
            }
            _searchPanel.IntersectedFeatures = arrList;  // fires an event on the panel to update the combobox
        }

        private void LogStreetAddressParsedQuery(string q, StreetAddress sa)
        {
            // only log if enabled
            if (!SdrConfig.Project.Go2ItProjectSettings.Instance.SearchQueryParserLogging) return;
            var p = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\SDR\\" +
                    SdrConfig.Settings.Instance.ApplicationName;
            var d = new DirectoryInfo(p);
            if (!d.Exists)
            {
                d.Create();
            }
            var f = p + "\\parsed_queries.txt";
            using (var sw = File.AppendText(f))
            {
                sw.WriteLine("Query        : " + q);
                sw.WriteLine("--------------------------------------------------------");
                if (sa.Number != null)
                {
                    sw.WriteLine("StructNum    : " + sa.Number);
                }
                if (sa.Predirectional != null)
                {
                    sw.WriteLine("PreDir       : " + sa.Predirectional);
                }
                if (SdrConfig.Project.Go2ItProjectSettings.Instance.SearchUsePretypes)
                {
                    if (sa.PreType != null)
                    {
                        sw.WriteLine("PreType      : " + sa.PreType);
                    }
                }
                if (sa.StreetName != null)
                {
                    sw.WriteLine("StreetName   : " + sa.StreetName);
                }
                if (sa.StreetType != null)
                {
                    sw.WriteLine("StreetType   : " + sa.StreetType);
                }
                if (sa.Postdirectional != null)
                {
                    sw.WriteLine("PostDir      : " + sa.Postdirectional);
                }
                if (sa.SubUnitType != null)
                {
                    sw.WriteLine("SubUnitType  : " + sa.SubUnitType);
                }
                if (sa.SubUnitValue != null)
                {
                    sw.WriteLine("SubUnitValue : " + sa.SubUnitValue);
                }
                sw.WriteLine("========================================================");
                sw.Close();
            }
        }
        #endregion
    }
}
