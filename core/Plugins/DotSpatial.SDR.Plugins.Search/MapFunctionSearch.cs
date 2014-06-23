﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections;
using DotSpatial.Data;
using DotSpatial.SDR.Controls;
using DotSpatial.Topology;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
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
        internal TabDockingControl TabDockingControl;

        private SearchPanel _searchPanel;
        private SearchMode _searchMode;
        private readonly DataGridView _dataGridView;
        private string _indexType;
        private string[] _columnNames;
        private const string FID = "FID";
        private const string LYRNAME = "LYRNAME";

        #region Constructors
        
        /// <summary>
        /// Creates a new instance of MapMeasureFunction, with panel
        /// </summary>
        /// <param name="sp">Search Panel</param>
        public MapFunctionSearch(SearchPanel sp)
        {
            _searchPanel = sp;
            _dataGridView = sp.DataGridDisplay;
            Configure();
            SetSearchVariables();
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
            _searchPanel.RowDoubleClicked += SearchPanelOnRowDoublelicked;
        }

        private int GetColumnDisplayIndex(string name)
        {
            for (int i = 0; i <= _dataGridView.ColumnCount - 1; i++)
            {
                if (_dataGridView.Columns[i].Name == name)
                {
                    return _dataGridView.Columns[i].DisplayIndex;
                }
            }
            return -1;
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

        private void SearchPanelOnRowDoublelicked(object sender, EventArgs eventArgs)
        {
            var evnt = eventArgs as DataGridViewCellEventArgs;
            if (evnt != null)
            {
                DataGridViewRow dgvr = _dataGridView.Rows[evnt.RowIndex];
                int fidIdx = GetColumnDisplayIndex(FID);
                int lyrIdx = GetColumnDisplayIndex(LYRNAME);
                string fid = dgvr.Cells[fidIdx].Value.ToString();
                string lyr = dgvr.Cells[lyrIdx].Value.ToString();

                Dictionary<string, string> mapPanels = GetMapTabKeysContainingLayer(lyr);
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
                    else
                    {
                        return; // something bad happened here
                    }
                }

                // now we cycle through layers of our active map and find the layer we want
                var layers = Map.GetFeatureLayers();
                foreach (IMapFeatureLayer mapLayer in layers)
                {
                    if (mapLayer != null && String.IsNullOrEmpty(Path.GetFileNameWithoutExtension((mapLayer.DataSet.Filename)))) return;
                    if (mapLayer == null) continue;

                    IFeatureSet fs = FeatureSet.Open(mapLayer.DataSet.Filename);
                    if (fs == null || fs.Name != lyr) continue;

                    mapLayer.SelectByAttribute("[FID] =" + fid);
                    mapLayer.ZoomToSelectedFeatures();
                }
            }
        }

        private Dictionary<string, string> GetIndexColumnsOrder()
        {
            switch (_searchMode)
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

        private void SearchPanelOnPerformSearch(object sender, EventArgs eventArgs)
        {
            _searchPanel.ClearSearches();
            var searchQuery = _searchPanel.SearchQuery;
            var colArr = new DataGridViewColumn[_columnNames.Count()];
            // check for any sort order the user may have set
            var orderDict = GetIndexColumnsOrder();
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
                    var s = orderDict[columnName];
                    var j = -1;
                    int.TryParse(s, out j);
                    if (j < 0) continue;

                    txtCol.DisplayIndex = j;
                    colArr[j] = txtCol;
                }
                else
                {
                    colArr[i] = txtCol;
                }
            }
            _dataGridView.Columns.AddRange(colArr);
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
            // perform the query and display that shit
            ExecuteLuceneQuery(searchQuery);
            // setup event binding if user reorders columns, save user settings
            _dataGridView.ColumnDisplayIndexChanged += DataGridViewOnColumnDisplayIndexChanged;
        }

        private void DataGridViewOnColumnDisplayIndexChanged(object sender, DataGridViewColumnEventArgs dataGridViewColumnEventArgs)
        {
            var newOrder = new Dictionary<string, string>();
            foreach (DataGridViewColumn col in _dataGridView.Columns)
            {
                var i = GetColumnDisplayIndex(col.Name);
                newOrder.Add(col.Name, i.ToString(CultureInfo.InvariantCulture));
            }
            switch (_searchMode)
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
            }
        }

        private void SearchPanelOnHydrantLocate(object sender, EventArgs eventArgs)
        {
            MessageBox.Show("locate hydrant");
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

        private void SetSearchVariables()
        {
            _searchMode = _searchPanel.SearchMode;
            switch (_searchMode)
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

        private void SearchPanelOnSearchModeChanged(object sender, EventArgs eventArgs)
        {
            SetSearchVariables();
        }

        private void SearchPanelOnSearchesCleared(object sender, EventArgs eventArgs)
        {
            // unbind the column order index stuff
            _dataGridView.ColumnDisplayIndexChanged -= DataGridViewOnColumnDisplayIndexChanged;
            // now clear the datagridview
            _dataGridView.Rows.Clear();
            _dataGridView.Columns.Clear();
            // clear any map selections as well
            IEnvelope env;
            Map.MapFrame.ClearSelection(out env);
        }

        #endregion

        #region Methods

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

        private static void LogSearchQuery(string q, StreetAddress sa)
        {
            var p = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\SDR\\" +
                       SdrConfig.Settings.Instance.ApplicationName;
            var d = new DirectoryInfo(p);
            if (!d.Exists)
            {
                d.Create();
            }
            var f = p + "\\parsed_searches.txt";
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

        private static Query ConstructAddressQuery(string search)
        {
            // parse our input address into a valid streetaddress object
            // TODO: perform check to determine if user is using pretypes fields
            StreetAddress streetAddress = StreetAddressParser.Parse(search, true);
            LogSearchQuery(search, streetAddress);

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
            var vals = (string[])values.ToArray(typeof(string));
            var flds = (string[])fields.ToArray(typeof(string));
            var ocrs = (Occur[])occurs.ToArray(typeof(Occur));
            // setup the query search cursor
            Query query = MultiFieldQueryParser.Parse(
                Version.LUCENE_30,
                vals,
                flds,
                ocrs,
                new StandardAnalyzer(Version.LUCENE_30)
            );
            return query;
        }

        private Query ConstructLuceneQuery(string searchQuery)
        {
            switch (_searchMode)
            {
                case SearchMode.Address:
                    return ConstructAddressQuery(searchQuery);
                case SearchMode.Name:
                    return ConstructNameQuery(searchQuery);
                case SearchMode.Phone:
                    return ConstructPhoneQuery(searchQuery);
            }
            return null;
        }

        private void FormatAddressIndexQueryResults(IEnumerable<ScoreDoc> hits, Searcher searcher)
        {
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
                var doc = searcher.Doc(hit.Doc);
                foreach (var field in _columnNames)
                {
                    var idx = GetColumnDisplayIndex(field);
                    var val = doc.Get(field);
                    dgvRow.Cells[idx].Value = val;
                }
                // add the fid and layrname textbox cells
                var fidCell = new DataGridViewTextBoxCell {Value = doc.Get(FID)};
                dgvRow.Cells.Add(fidCell);
                var lyrCell = new DataGridViewTextBoxCell {Value = doc.Get(LYRNAME)};
                dgvRow.Cells.Add(lyrCell);
                _dataGridView.Rows.Add(dgvRow);
            }
            searcher.Dispose();
        }

        private void FormatQueryResults(IEnumerable<ScoreDoc> hits, Searcher searcher)
        {
            switch (_searchMode)
            {
                case SearchMode.Address:
                    FormatAddressIndexQueryResults(hits, searcher);
                    break;
                case SearchMode.Name:
                    break;
                case SearchMode.Phone:
                    break;
            }
        }

        private void ExecuteLuceneQuery(string searchString)
        {
            var db = SQLiteHelper.GetSQLiteFileName(SdrConfig.Settings.Instance.ProjectRepoConnectionString);
            var d = Path.GetDirectoryName(db);
            if (d == null) return;

            var path = Path.Combine(d, "indexes", _indexType);
            Directory idxDir = FSDirectory.Open(new DirectoryInfo(path));
            IndexReader reader = IndexReader.Open(idxDir, true);

            Searcher searcher = new IndexSearcher(reader);
            Query query = ConstructLuceneQuery(searchString);
            
            TopDocs docs = searcher.Search(query, reader.MaxDoc);
            ScoreDoc[] hits = docs.ScoreDocs;
            idxDir.Dispose();  // wipe the directory ref out now
            // prep the results for display to the datagridview
            FormatQueryResults(hits, searcher);
            // TODO? should se dispose of the searcher here not inside?
        }

        private static Query ConstructPhoneQuery(string searchQuery)
        {
            return null;
        }

        private static Query ConstructNameQuery(string searchQuery)
        {
            return null;
        }

        #endregion
    }
}
