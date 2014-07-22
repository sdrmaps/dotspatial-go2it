using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections;
using DotSpatial.Data;
using DotSpatial.SDR.Controls;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Spatial;
using Lucene.Net.Spatial.Prefix;
using Lucene.Net.Spatial.Prefix.Tree;
using Lucene.Net.Spatial.Queries;
using Lucene.Net.Store;
using Spatial4n.Core.Context.Nts;
using SdrConfig = SDR.Configuration;
using System.Windows.Forms;
using DotSpatial.Controls;
using SDR.Data.Database;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using Version = Lucene.Net.Util.Version;
using Directory = Lucene.Net.Store.Directory;

namespace DotSpatial.SDR.Plugins.Search
{
    /// <summary>
    /// A MapFunction that allows searching of various configured layers
    /// </summary>
    public class MapFunctionSearch : MapFunction
    {
        // overall tab docking control for selecting map and tool tabs
        // (used to swap active map panels on searches)
        internal TabDockingControl TabDockingControl;

        private SearchPanel _searchPanel;
        private readonly DataGridView _dataGridView; // dgv to populate our results of query to
        private static IndexSearcher _indexSearcher = null;
        private static IndexReader _indexReader = null;

        private string _indexType;
        private string[] _columnNames;

        internal const string FID = "FID";
        internal const string LYRNAME = "LYRNAME";
        internal const string GEOSHAPE = "GEOSHAPE";

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
            SetupIndexReaderWriter();
        }

        public static IndexSearcher IndexSearcher
        {
            get { return _indexSearcher; }
        }

        public static IndexReader IndexReader
        {
            get { return _indexReader; }
        }

        private void Configure()
        {
            YieldStyle = YieldStyles.AlwaysOn;
            HandleSearchPanelEvents();
            Name = "MapFunctionSearch";
        }

        private void HandleSearchPanelEvents()
        {
            _searchPanel.SearchModeChanged += SearchPanelOnSearchModeChanged;
            _searchPanel.SearchesCleared += SearchPanelOnSearchesCleared;
            _searchPanel.HydrantLocate += SearchPanelOnHydrantLocate;
            _searchPanel.PerformSearch += SearchPanelOnPerformSearch;
            _searchPanel.OnRowDoublelicked += SearchPanelOnRowDoublelicked;
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
                //  TODO: need to seperate all these?
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
            }
        }

        private void SearchPanelOnSearchModeChanged(object sender, EventArgs eventArgs)
        {
            SetSearchVariables();
            SetupIndexReaderWriter();
        }

        private void SearchPanelOnSearchesCleared(object sender, EventArgs eventArgs)
        {
            // unbind the column order index stuff
            _dataGridView.ColumnDisplayIndexChanged -= DataGridViewOnColumnDisplayIndexChanged;
            // now clear the datagridview
            _dataGridView.Rows.Clear();
            _dataGridView.Columns.Clear();

            // TODO: this is so fucking slow, have to find a better approach
            // clear any map selections as well
            // IEnvelope env;
            // Map.MapFrame.ClearSelection(out env);
        }

        private void SearchPanelOnPerformSearch(object sender, EventArgs eventArgs)
        {
            if (_searchPanel.SearchQuery.Length <= 0) return;
            var q = _searchPanel.SearchQuery;
            // if its an intersection and there are two terms do a location zoom, otherwise find intersections
            if (_searchPanel.SearchMode == SearchMode.Intersection)
            {
                var arr = q.Split('|');
                if (arr[1].Length > 0) // zoom to intersection of the two features
                {
                    ZoomToIntersection(arr[0], arr[1]);
                    return;
                } // else look for intersections on this feature and populate combos and dgv
                q = arr[0];
            }
            // all other queries require a lucene query and populate combos and datagridview
            _searchPanel.ClearSearches();  // clear any existing searches
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
                    if (!fl1.Shape.Contains(c)) continue;
                    if (!coordsList.Contains(c))
                    {
                        coordsList.Add(c);
                    }
                }
            }
            return coordsList.ToArray();
        }

        private void ZoomToIntersection(string ft1, FeatureLookup fl2)
        {
            var fts1Lookup = FetchIntersectingLookups(ft1, fl2);
            var coords = GetIntersectionCoordinate(fts1Lookup, fl2);
            if (coords.Count() != 1)
            {
                // TODO: finish this crap
                Debug.WriteLine("major major fuckup here");
            }
            else
            {
                Debug.WriteLine(coords.First());
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
                foreach (var c in coords)
                {
                    if (!coordsList.Contains(c))
                    {
                        coordsList.Add(c);
                    }   
                }
            }
            if (coordsList.Count != 1)
            {
                // TODO: finish this crap
                Debug.WriteLine("major major fuckup here");
            }
            else
            {
                Debug.WriteLine(coordsList[0]);
            }
        }

        private IEnumerable<FeatureLookup> FetchIntersectingLookups(string ft1, FeatureLookup ft2)
        {
            if (ft1.Length <= 0 || ft2 == null) return null;
            var ctx = NtsSpatialContext.GEO; // using NTS context (provides polygon/line/point models)
            Spatial4n.Core.Shapes.Shape shp2 = ctx.ReadShape(ft2.Shape);  // load ft2 shape
            // find all other possible features that match ft1 name
            var fts1Lookup = new List<FeatureLookup>();
            ScoreDoc[] hits = ExecuteExactRoadQuery(ft1);
            foreach (var hit in hits)
            {
                var doc = _indexSearcher.Doc(hit.Doc);
                // load a possible intersecting feature shape
                Spatial4n.Core.Shapes.Shape shp1 = ctx.ReadShape(doc.Get(GEOSHAPE));
                if (shp1.Relate(shp2).Equals(Spatial4n.Core.Shapes.SpatialRelation.INTERSECTS))  // validate relation
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

                // TODO: maybe check shape type and buffer around it or something?
                mapLayer.SelectByAttribute("[FID] =" + ftLookup.Fid);
                mapLayer.ZoomToSelectedFeatures();
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

            var lookup = new FeatureLookup
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
                        ZoomToIntersection(arr[0], lookup);
                    }
                    break;
                default:  // normal operation is to go to the chosen feature
                    ZoomToFeature(lookup);
                    break;
            }
        }

        private void SearchPanelOnHydrantLocate(object sender, EventArgs eventArgs)
        {
            MessageBox.Show("locate hydrant");
        }
        #endregion

        #region Methods

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
            // TODO: update these so they are all seperate and not shared
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

        private void SetupIndexReaderWriter()
        {
            if (_indexSearcher != null)
            {
                _indexSearcher.Dispose();
                _indexReader.Directory().Dispose();
                _indexReader.Dispose();
            }
            // get the index directory for this mode
            Directory idxDir = GetLuceneIndexDirectory();  
            _indexReader = IndexReader.Open(idxDir, true);  // readonly for performance
            _indexSearcher = new IndexSearcher(_indexReader);
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
            }
        }

        private ScoreDoc[] ExecuteScoredAddressQuery(string q)
        {
            // parse our input address into a valid streetaddress object
            // TODO: perform check to determine if user is using pretypes fields
            // TODO: Not sure how to best handle this, what if they mix types??
            StreetAddress streetAddress = StreetAddressParser.Parse(q, true);
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
            if (streetAddress.PreType != null)
            {
                values.Add(streetAddress.PreType);
                fields.Add("Pre Type");
                occurs.Add(Occur.SHOULD);
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
            TopDocs docs = _indexSearcher.Search(query, _indexReader.NumDocs());
            // return our results
            return docs.ScoreDocs;
        }

        private ScoreDoc[] ExecuteScoredRoadQuery(string q)
        {
            // parse our input road into a streetaddress object for analysis
            // TODO: perform check to determine if user is using pretypes fields
            // TODO: Not sure how to best handle this, what if they mix types??
            StreetAddress streetAddress = StreetAddressParser.Parse(q, true);
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
            if (streetAddress.PreType != null)
            {
                values.Add(streetAddress.PreType);
                fields.Add("Pre Type");
                occurs.Add(Occur.SHOULD);
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
            TopDocs docs = _indexSearcher.Search(query, _indexReader.NumDocs());
            // return our results
            return docs.ScoreDocs;
        }

        private ScoreDoc[] ExecuteExactRoadQuery(string q)
        {
            // parse our input address into a valid streetaddress object
            // TODO: perform check to determine if user is using pretypes fields
            // TODO: Not sure how to best handle this, what if they mix types??
            StreetAddress streetAddress = StreetAddressParser.Parse(q, true);
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
            if (streetAddress.PreType != null)
            {
                values.Add(streetAddress.PreType);
                fields.Add("Pre Type");
                occurs.Add(Occur.MUST);
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
            TopDocs docs = _indexSearcher.Search(query, _indexReader.NumDocs());
            // return our results
            return docs.ScoreDocs;
        }

        public Directory GetLuceneIndexDirectory()
        {
            var db = SQLiteHelper.GetSQLiteFileName(SdrConfig.Settings.Instance.ProjectRepoConnectionString);
            var d = Path.GetDirectoryName(db);
            if (d == null) return null;

            var path = Path.Combine(d, "indexes", _indexType);
            if (!System.IO.Directory.Exists(path)) return null;

            Directory idxDir = FSDirectory.Open(new DirectoryInfo(path));
            return idxDir;
        }

        private ScoreDoc[] ExecuteGetIntersectionsQuery(string q)
        {
            if (q.Length <= 0) return null;
            // get the name of the street passed in so it is removed from results returned
            var sa = StreetAddressParser.Parse(q, true);
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
                Spatial4n.Core.Shapes.Shape shp = ctx.ReadShape(strShp);  // read the wkt string into an actual shape object
                // prepare spatial query
                var args = new SpatialArgs(SpatialOperation.Intersects, shp);
                Query sq = strategy.MakeQuery(args);
                // create overall boolean query to pass to indexsearcher
                var query = new BooleanQuery {{sq, Occur.MUST}, {tq, Occur.MUST_NOT}};
                // execute a query to find all features that intersect each passed in feature
                TopDocs topDocs = _indexSearcher.Search(query, _indexReader.NumDocs());
                ScoreDoc[] hits = topDocs.ScoreDocs;
                // results for cleanup after loop completes
                docs.AddRange(hits);
            }
            // remove any duplicates by street name
            var cdocs = docs.GroupBy(x => x.Doc).Select(x => x.First()).ToList<ScoreDoc>();
            return cdocs.ToArray();
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
                    dgvRow.Cells[idx].Value = val;
                }
                // add the fid and layrname textbox cells
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
                if (doc.Get("Pre Type") != null)
                {
                    val = val + doc.Get("Pre Type").Trim() + " ";
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
                if (sa.PreType != null)
                {
                    sw.WriteLine("PreType      : " + sa.PreType);
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
