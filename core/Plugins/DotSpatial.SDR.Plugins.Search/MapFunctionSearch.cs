using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections;
using System.Text.RegularExpressions;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.SDR.Controls;
using DotSpatial.Symbology;
using DotSpatial.Topology;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Search.Function;
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
        // (used to swap active map panels on searches) (this is set on init)
        internal TabDockingControl TabDockingControl;

        private SearchPanel _searchPanel;
        private readonly DataGridView _dataGridView; // dgv to populate our results of query to
        private static IndexSearcher _indexSearcher = null;
        private static IndexReader _indexReader = null;

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

        #region Constructors

        /// <summary>
        /// Creates a new instance of MapFunctionSearch, with panel
        /// </summary>
        /// <param name="sp">Search Panel</param>
        public MapFunctionSearch(SearchPanel sp)
        {
            _searchPanel = sp;
            _dataGridView = sp.DataGridDisplay;
            Configure();
            SetSearchVariables();
            SetupIndexReaderWriter(_indexType);
        }

        private void Configure()
        {
            YieldStyle = YieldStyles.AlwaysOn;
            HandleSearchPanelEvents();
            Name = "MapFunctionSearch";
        }

        private void SetSearchVariables()
        {
            switch (_searchPanel.SearchMode)
            {
                case SearchMode.Address:
                    _indexType = "AddressIndex";
                    _columnNames = GetColumnNames(); // uses the _indexType variable
                    break;
                case SearchMode.Intersection:
                    _indexType = "RoadIndex";
                    _columnNames = GetColumnNames();
                    break;
                case SearchMode.Name:
                    _indexType = "AddressIndex";
                    _columnNames = GetColumnNames();
                    break;
                case SearchMode.Phone:
                    _indexType = "AddressIndex";
                    _columnNames = GetColumnNames();
                    break;
                case SearchMode.Road:
                    _indexType = "RoadIndex";
                    _columnNames = GetColumnNames();
                    break;
                case SearchMode.Key_Locations:
                    _indexType = "KeyLocationsIndex";
                    _columnNames = GetColumnNames();
                    break;
                case SearchMode.City:
                    _indexType = "CityLimitIndex";
                    _columnNames = GetColumnNames();
                    break;
                case SearchMode.Esn:
                    _indexType = "EsnIndex";
                    _columnNames = GetColumnNames();
                    break;
                case SearchMode.Cell_Sector:
                    _indexType = "CellSectorIndex";
                    _columnNames = GetColumnNames();
                    break;
                case SearchMode.Parcel:
                    _indexType = "ParcelIndex";
                    _columnNames = GetColumnNames();
                    break;
                case SearchMode.All:
                    // TODO: not sure what we want to do here yet.
                    break;
            }
        }

        public static IndexSearcher IndexSearcher
        {
            get { return _indexSearcher; }
        }

        public static IndexReader IndexReader
        {
            get { return _indexReader; }
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

        private void SearchPanelOnSearchModeActivated(object sender, EventArgs eventArgs)
        {
            // redundant check, but prevents multiple events from firing when not needed
            if (Map.FunctionMode != FunctionMode.None)
            {
                Map.FunctionMode = FunctionMode.None;
            }
        }

        protected override void OnActivate()
        {
            if (_searchPanel == null || _searchPanel.IsDisposed)
            {
                _searchPanel = new SearchPanel();
                HandleSearchPanelEvents();
            }
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
            var newOrder = new Dictionary<string, string>();
            foreach (DataGridViewColumn col in _dataGridView.Columns)
            {
                var i = GetColumnDisplayIndex(col.Name, _dataGridView);
                newOrder.Add(col.Name, i.ToString(CultureInfo.InvariantCulture));
            }
            switch (_searchPanel.SearchMode)
            {
                case SearchMode.Address:
                    SdrConfig.User.Go2ItUserSettings.Instance.AddressIndexColumnOrder = newOrder;
                    break;
                case SearchMode.Intersection:
                    SdrConfig.User.Go2ItUserSettings.Instance.RoadIndexColumnOrder = newOrder;
                    break;
                case SearchMode.Name:
                    SdrConfig.User.Go2ItUserSettings.Instance.AddressIndexColumnOrder = newOrder;
                    break;
                case SearchMode.Phone:
                    SdrConfig.User.Go2ItUserSettings.Instance.AddressIndexColumnOrder = newOrder;
                    break;
                case SearchMode.Road:
                    SdrConfig.User.Go2ItUserSettings.Instance.RoadIndexColumnOrder = newOrder;
                    break;
                case SearchMode.Key_Locations:
                    SdrConfig.User.Go2ItUserSettings.Instance.KeyLocationIndexColumnOrder = newOrder;
                    break;
                case SearchMode.Parcel:
                    SdrConfig.User.Go2ItUserSettings.Instance.ParcelIndexColumnOrder = newOrder;
                    break;
                case SearchMode.Cell_Sector:
                    SdrConfig.User.Go2ItUserSettings.Instance.CellSectorIndexColumnOrder = newOrder;
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
            if (_searchPanel.SearchMode == SearchMode.Intersection)
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
            else if (_searchPanel.SearchMode == SearchMode.City || _searchPanel.SearchMode == SearchMode.Esn)
            {
                // all other queries require a lucene query and populate combos and datagridview
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
                    ZoomToFeature(lookup);
                    return;
                }
            }
            // all other queries require a lucene query and populate combos and datagridview
            _searchPanel.ClearSearches();  // clear any existing searches (fires the event above this one actually)
            // setup columns, ordering, etc for results datagridview
            PrepareDataGridView();
            // execute our lucene query
            var hits = ExecuteLuceneQuery(q);
            FormatQueryResults(hits);
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
            double[] xy = new double[2];
            xy[0] = x;
            xy[1] = y;
            // reproject the point if need be
            if (Map.Projection.ToProj4String() != KnownCoordinateSystems.Geographic.World.WGS1984.ToProj4String())
            {
                Reproject.ReprojectPoints(
                    xy, new double[1], KnownCoordinateSystems.Geographic.World.WGS1984, Map.Projection, 0, 1);
            }
            CreatePointGraphic(new Coordinate(xy[0], xy[1]));
            var envelope = CreateBufferGraphic(new DotSpatial.Topology.Point(new Coordinate(xy[0], xy[1])));

            double zoomInFactor = (double) SdrConfig.Project.Go2ItProjectSettings.Instance.SearchZoomFactor; // fixed zoom-in by 10% - 5% on each side
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
            ScoreDoc[] hits = ExecuteExactRoadQuery(ft2);
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
            ScoreDoc[] hits = ExecuteExactRoadQuery(ft1);
            foreach (var hit in hits)
            {
                var doc = _indexSearcher.Doc(hit.Doc);
                // load a possible intersecting feature shape
                Spatial4n.Core.Shapes.Shape shp1 = ctx.ReadShape(doc.Get(GEOSHAPE));
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

                LineCap lineCap = new LineCap();
                LineCap.TryParse(SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicLineStyle, true, out lineCap);
                DashStyle lineStyle = new DashStyle();
                DashStyle.TryParse(SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicLineCap, true, out lineStyle);
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

        private void CreatePointGraphic(Coordinate c)
        {
            if (_pointGraphicsLayer == null)
            {
                _pointGraphics = new FeatureSet(FeatureType.Point);
                _pointGraphicsLayer = new MapPointLayer(_pointGraphics);
                PointShape pointShape = new PointShape();
                PointShape.TryParse(SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicLineStyle, true, out pointShape);
                _pointGraphicsLayer.Symbolizer = new PointSymbolizer(
                    SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicPointColor,
                    pointShape,
                    SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicPointSize);
                Map.MapFrame.DrawingLayers.Add(_pointGraphicsLayer);
            }
            var point = new Point(c);
            _pointGraphics.AddFeature(point);
        }

        private void ZoomToFeature(FeatureLookup ftLookup)
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

                // TODO: actually selecting something on the map cause tons of redraws, lets use a graphic instead
                // mapLayer.SelectByAttribute("[FID] =" + ftLookup.Fid);

                // grab the feature and use the shape to generate a buffer around it
                var ft = fs.GetFeature(Convert.ToInt32(ftLookup.Fid));

                if (ft.Coordinates.Count == 1)
                {
                    CreatePointGraphic(ft.Coordinates[0]);
                }
                IEnvelope buffEnv = CreateBufferGraphic(ft.BasicGeometry as Geometry);

                double zoomInFactor = (double)SdrConfig.Project.Go2ItProjectSettings.Instance.SearchZoomFactor;
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

            switch (_searchPanel.SearchMode)
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
                    ZoomToFeature(_activeLookup);
                    break;
            }
        }

        private void SearchPanelOnHydrantLocate(object sender, EventArgs eventArgs)
        {
            if (_activeLookup == null)
            {
                MessageBox.Show(@"No feature or location is active");
                return;
            }
            IMapPointLayer hydrantLayer = null;
            var layers = Map.GetPointLayers();
            foreach (IMapPointLayer ptLayer in layers)
            {
                if (ptLayer != null && String.IsNullOrEmpty(Path.GetFileNameWithoutExtension((ptLayer.DataSet.Filename)))) return;
                IFeatureSet fs = FeatureSet.Open(ptLayer.DataSet.Filename);
                if (fs != null && fs.Name != SdrConfig.Project.Go2ItProjectSettings.Instance.HydrantsLayer)
                {
                    hydrantLayer = ptLayer;
                    break;
                }
            }
            var idxType = _indexType;  // store the current index type | reset when operation is complete
            SetupIndexReaderWriter("HydrantIndex");
            var hits = ExecuteScoredHydrantQuery(_activeLookup);
            for (int i = 0; i <= SdrConfig.Project.Go2ItProjectSettings.Instance.HydrantSearchCount -1; i++)
            {
                var hit = hits[i];
                var doc = _indexSearcher.Doc(hit.Doc);
                hydrantLayer.SelectByAttribute("[FID] =" + doc.Get(FID));
	        }
            SetupIndexReaderWriter(idxType);  // set the index searcher back

            IEnvelope hydrantEnv = hydrantLayer.Selection.Envelope;
            double zoomInFactor = (double)SdrConfig.Project.Go2ItProjectSettings.Instance.SearchZoomFactor;
            var newExtentWidth = Map.ViewExtents.Width * zoomInFactor;
            var newExtentHeight = Map.ViewExtents.Height * zoomInFactor;
            hydrantEnv.ExpandBy(newExtentWidth, newExtentHeight);

            Map.ViewExtents = hydrantEnv.ToExtent();

            // hydrantLayer.ZoomToSelectedFeatures();
        }
        #endregion

        #region Methods

        private ScoreDoc[] ExecuteScoredHydrantQuery(FeatureLookup activeLookup)
        {
            var ctx = NtsSpatialContext.GEO; // using NTS (provides polygon/line/point models)
            Spatial4n.Core.Shapes.Shape shp = ctx.ReadShape(activeLookup.Shape);
            Spatial4n.Core.Shapes.Point centerPt = shp.GetCenter();

            SpatialStrategy strategy = new RecursivePrefixTreeStrategy(new GeohashPrefixTree(ctx, 24), GEOSHAPE);
            var args = new SpatialArgs(SpatialOperation.Intersects,
                ctx.MakeCircle(centerPt.GetX(), centerPt.GetY(),
                    DistanceUtils.Dist2Degrees(10, DistanceUtils.EARTH_MEAN_RADIUS_KM)));

            Filter filter = strategy.MakeFilter(args);
            ValueSource valueSource = strategy.MakeDistanceValueSource(centerPt);
            ValueSourceQuery query = new ValueSourceQuery(valueSource);
            Sort sort = new Sort(new SortField("DISTANCE", SortField.SCORE, true));

            TopDocs docs = _indexSearcher.Search(query, filter, 10, sort);
            return docs.ScoreDocs;
        }

        private IEnumerable<ScoreDoc> ExecuteLuceneQuery(string sq)
        {
            ScoreDoc[] hits = null;
            switch (_searchPanel.SearchMode)
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
                case SearchMode.Key_Locations:
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
                case SearchMode.Cell_Sector:
                    hits = ExecuteScoredCellSectorsQuery(sq);
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
            var conn = SdrConfig.Settings.Instance.ProjectRepoConnectionString;
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
            switch (_searchPanel.SearchMode)
            {
                case SearchMode.Address:
                    return SdrConfig.User.Go2ItUserSettings.Instance.AddressIndexColumnOrder;
                case SearchMode.Intersection:
                    return SdrConfig.User.Go2ItUserSettings.Instance.RoadIndexColumnOrder;
                case SearchMode.Name:
                    return SdrConfig.User.Go2ItUserSettings.Instance.AddressIndexColumnOrder;
                case SearchMode.Phone:
                    return SdrConfig.User.Go2ItUserSettings.Instance.AddressIndexColumnOrder;
                case SearchMode.Road:
                    return SdrConfig.User.Go2ItUserSettings.Instance.RoadIndexColumnOrder;
                case SearchMode.Key_Locations:
                    return SdrConfig.User.Go2ItUserSettings.Instance.KeyLocationIndexColumnOrder;
                case SearchMode.Parcel:
                    return SdrConfig.User.Go2ItUserSettings.Instance.ParcelIndexColumnOrder;
                case SearchMode.Cell_Sector:
                    return SdrConfig.User.Go2ItUserSettings.Instance.CellSectorIndexColumnOrder;
                case SearchMode.All:
                    // TODO:
                    return null;
            }
            return null;
        }

        private string[] GetColumnNames()
        {
            var conn = SdrConfig.Settings.Instance.ProjectRepoConnectionString;
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

        private void SetupIndexReaderWriter(string idxType)
        {
            if (_indexSearcher != null)
            {
                _indexSearcher.Dispose();
                _indexReader.Directory().Dispose();
                _indexReader.Dispose();
            }
            // get the index directory for this mode
            Directory idxDir = GetLuceneIndexDirectory(idxType);
            if (idxDir != null)
            {
                _indexReader = IndexReader.Open(idxDir, true);  // readonly for performance
                _indexSearcher = new IndexSearcher(_indexReader);
            }
        }

        public Directory GetLuceneIndexDirectory(string indexType)
        {
            var db = SQLiteHelper.GetSQLiteFileName(SdrConfig.Settings.Instance.ProjectRepoConnectionString);
            var d = Path.GetDirectoryName(db);
            if (d == null) return null;

            var path = Path.Combine(d, "indexes", indexType);
            if (!System.IO.Directory.Exists(path)) return null;

            Directory idxDir = FSDirectory.Open(new DirectoryInfo(path));
            return idxDir;
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
                Version.LUCENE_30,
                vals,
                flds,
                ocrs,
                new StandardAnalyzer(Version.LUCENE_30)
                );
            if (_indexReader == null)
            {
                MessageBox.Show("Search index not found");
                return new ScoreDoc[0];
            }
            TopDocs docs = _indexSearcher.Search(query, _indexReader.NumDocs());
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
                Version.LUCENE_30,
                vals,
                flds,
                ocrs,
                new StandardAnalyzer(Version.LUCENE_30)
                );
            if (_indexReader == null)
            {
                MessageBox.Show("Search index not found");
                return new ScoreDoc[0];
            }
            TopDocs docs = _indexSearcher.Search(query, _indexReader.NumDocs());
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
                Version.LUCENE_30,
                vals,
                flds,
                ocrs,
                new StandardAnalyzer(Version.LUCENE_30)
                );
            if (_indexReader == null)
            {
                MessageBox.Show("Search index not found");
                return new ScoreDoc[0];
            }
            TopDocs docs = _indexSearcher.Search(query, _indexReader.NumDocs());
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
                Version.LUCENE_30,
                vals,
                flds,
                ocrs,
                new StandardAnalyzer(Version.LUCENE_30)
                );
            if (_indexReader == null)
            {
                MessageBox.Show("Search index not found");
                return new ScoreDoc[0];
            }
            TopDocs docs = _indexSearcher.Search(query, _indexReader.NumDocs());
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
                Version.LUCENE_30,
                vals,
                flds,
                ocrs,
                new StandardAnalyzer(Version.LUCENE_30)
                );
            if (_indexReader == null)
            {
                MessageBox.Show("Search index not found");
                return new ScoreDoc[0];
            }
            TopDocs docs = _indexSearcher.Search(query, _indexReader.NumDocs());
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
                Version.LUCENE_30,
                vals,
                flds,
                ocrs,
                new StandardAnalyzer(Version.LUCENE_30)
                );
            if (_indexReader == null)
            {
                MessageBox.Show("Search index not found");
                return new ScoreDoc[0];
            }
            TopDocs docs = _indexSearcher.Search(query, _indexReader.NumDocs());
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
                Version.LUCENE_30,
                vals,
                flds,
                ocrs,
                new StandardAnalyzer(Version.LUCENE_30)
                );

            if (_indexReader == null)
            {
                MessageBox.Show("Search index not found");
                return new ScoreDoc[0];
            }
            TopDocs docs = _indexSearcher.Search(query, _indexReader.NumDocs());
            // return our results
            return docs.ScoreDocs;
        }

        private ScoreDoc[] ExecuteScoredAddressQuery(string q)
        {
            // parse our input address into a valid streetaddress object
            StreetAddress streetAddress = StreetAddressParser.Parse(q, SdrConfig.Project.Go2ItProjectSettings.Instance.UsePretypes);
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
            if (SdrConfig.Project.Go2ItProjectSettings.Instance.UsePretypes)
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
                Version.LUCENE_30,
                vals,
                flds,
                ocrs,
                new StandardAnalyzer(Version.LUCENE_30)
                );

            if (_indexReader == null)
            {
                MessageBox.Show("Search index not found");
                return new ScoreDoc[0];
            }
            TopDocs docs = _indexSearcher.Search(query, _indexReader.NumDocs());
            // return our results
            return docs.ScoreDocs;
        }

        private ScoreDoc[] ExecuteScoredRoadQuery(string q)
        {
            // parse our input road into a streetaddress object for analysis
            StreetAddress streetAddress = StreetAddressParser.Parse(q, SdrConfig.Project.Go2ItProjectSettings.Instance.UsePretypes);
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
            if (SdrConfig.Project.Go2ItProjectSettings.Instance.UsePretypes)
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
                Version.LUCENE_30,
                vals,
                flds,
                ocrs,
                new StandardAnalyzer(Version.LUCENE_30)
                );
            if (_indexReader == null)
            {
                MessageBox.Show("Search index not found");
                return new ScoreDoc[0];
            }
            TopDocs docs = _indexSearcher.Search(query, _indexReader.NumDocs());
            // return our results
            return docs.ScoreDocs;
        }

        private ScoreDoc[] ExecuteExactRoadQuery(string q)
        {
            // parse our input address into a valid streetaddress object
            StreetAddress streetAddress = StreetAddressParser.Parse(q, SdrConfig.Project.Go2ItProjectSettings.Instance.UsePretypes);
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
            if (SdrConfig.Project.Go2ItProjectSettings.Instance.UsePretypes)
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
                Version.LUCENE_30,
                vals,
                flds,
                ocrs,
                new StandardAnalyzer(Version.LUCENE_30)
                );
            if (_indexReader == null)
            {
                MessageBox.Show("Search index not found");
                return new ScoreDoc[0];
            }
            TopDocs docs = _indexSearcher.Search(query, _indexReader.NumDocs());
            // return our results
            return docs.ScoreDocs;
        }

        private ScoreDoc[] ExecuteGetIntersectionsQuery(string q)
        {
            if (q.Length <= 0) return null;
            // get the name of the street passed in so it is removed from results returned
            var sa = StreetAddressParser.Parse(q, SdrConfig.Project.Go2ItProjectSettings.Instance.UsePretypes);
            var docs = new List<ScoreDoc>();  // total docs for return
            ScoreDoc[] qHits = ExecuteExactRoadQuery(q);
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
                if (_indexReader == null)
                {
                    MessageBox.Show("Search index not found");
                    return new ScoreDoc[0];
                }
                TopDocs topDocs = _indexSearcher.Search(query, _indexReader.NumDocs());
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

        private void FormatQueryResultsForDataGridView(IEnumerable<ScoreDoc> hits)
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

        private void FormatQueryResults(IEnumerable<ScoreDoc> hits)
        {
            if (hits == null) return;
            switch (_searchPanel.SearchMode)
            {
                case SearchMode.Intersection:
                    FormatQueryResultsForDataGridView(hits);
                    UpdateIntersectedFeatures(hits);
                    break;
                default:
                    FormatQueryResultsForDataGridView(hits);
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
                if (SdrConfig.Project.Go2ItProjectSettings.Instance.UsePretypes)
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
            if (!SdrConfig.Project.Go2ItProjectSettings.Instance.EnableQueryParserLogging) return;
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
                if (SdrConfig.Project.Go2ItProjectSettings.Instance.UsePretypes)
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
