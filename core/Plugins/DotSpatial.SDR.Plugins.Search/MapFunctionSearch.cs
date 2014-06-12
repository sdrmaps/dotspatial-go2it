using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Collections;
using System.Runtime.Remoting.Messaging;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using SdrConfig = SDR.Configuration;
using System.Windows.Forms;
using DotSpatial.Controls;
using SDR.Data.Database;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;
using Directory = Lucene.Net.Store.Directory;


namespace DotSpatial.SDR.Plugins.Search
{
    /// <summary>
    /// A MapFunction that allows searching of various configured layers
    /// </summary>
    public class MapFunctionSearch : MapFunction
    {
        private SearchPanel _searchPanel;
        private SearchMode _searchMode;
        private readonly DataGridView _dataGridView;
        private string _indexType;
        private string[] _columnNames;
        private const string Fid = "FID";
        private const string Lyrname = "LYRNAME";

        #region Constructors
        
        /// <summary>
        /// Creates a new instance of MapMeasureFunction, with panel
        /// </summary>
        /// <param name="sp"></param>
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

        private int GetColumnIndex(string headerText)
        {

            for (int i = 0; i <= _dataGridView.ColumnCount - 1; i++)
            {
                if (_dataGridView.Columns[i].HeaderText == headerText)
                {
                    return i;
                }
            }
            return -1;
        }

        private void SearchPanelOnRowDoublelicked(object sender, EventArgs eventArgs)
        {
            var evnt = eventArgs as DataGridViewCellEventArgs;
            if (evnt != null)
            {
                DataGridViewRow dgvr = _dataGridView.Rows[evnt.RowIndex];
                int fidIdx = GetColumnIndex(Fid);
                int lyrIdx = GetColumnIndex(Lyrname);
                // string fid = dgvr.Cells[idx].Value.ToString();
            }
        }

        private void SearchPanelOnPerformSearch(object sender, EventArgs eventArgs)
        {
            _searchPanel.ClearSearches();
            var searchQuery = _searchPanel.SearchQuery;
            // create all the datagridview columns (field names)
            foreach (var columnName in _columnNames)
            {
                var txtCol = new DataGridViewTextBoxColumn
                {
                    HeaderText = columnName,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                _dataGridView.Columns.Add(txtCol);
            }
            var fidCol = new DataGridViewTextBoxColumn()
            {
                HeaderText = Fid,
                Visible = false
            };
            _dataGridView.Columns.Add(fidCol);
            var lyrCol = new DataGridViewTextBoxColumn()
            {
                HeaderText = Lyrname,
                Visible = false
            };
            _dataGridView.Columns.Add(lyrCol);
            ExecuteLuceneQuery(searchQuery);
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
            return SQLiteHelper.GetResultsAsArray(conn, sql);
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
                    // TODO:
                    break;
                case SearchMode.Phone:
                    // TODO:
                    break;
                case SearchMode.Road:
                    // TODO:
                    break;
            }
        }

        private void SearchPanelOnSearchModeChanged(object sender, EventArgs eventArgs)
        {
            SetSearchVariables();
        }

        private void SearchPanelOnSearchesCleared(object sender, EventArgs eventArgs)
        {
            _dataGridView.Rows.Clear();
            _dataGridView.Columns.Clear();
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

        private static Query ConstructAddressQuery(string search)
        {
            // parse our input address into a valid streetaddress object
            StreetAddress streetAddress = StreetAddressParser.Parse(search);
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
                var dgvRow = new DataGridViewRow();  // our new row to add to the datagridview
                var doc = searcher.Doc(hit.Doc);  // snatch the ranked document
                foreach (var field in _columnNames)
                {
                    var val = doc.Get(field);
                    if (val != null)
                    {
                        var dgvCell = new DataGridViewTextBoxCell { Value = val };
                        dgvRow.Cells.Add(dgvCell);
                    }
                    else
                    {
                        // TODO:
                        var x = "this is null";
                    }
                }
                // add the fid and layrname textbox cells
                var fidCell = new DataGridViewTextBoxCell {Value = doc.Get(Fid)};
                dgvRow.Cells.Add(fidCell);
                var lyrCell = new DataGridViewTextBoxCell {Value = doc.Get(Lyrname)};
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
            var path = Path.Combine(d, "indexes", _indexType);

            Directory idxDir = FSDirectory.Open(new DirectoryInfo(path));
            IndexReader reader = IndexReader.Open(idxDir, true);
            Searcher searcher = new IndexSearcher(reader);
            Query query = ConstructLuceneQuery(searchString);
            
            TopDocs docs = searcher.Search(query, reader.MaxDoc);
            ScoreDoc[] hits = docs.ScoreDocs;
            idxDir.Dispose();  // wipe the directory ref out now

            FormatQueryResults(hits, searcher);
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
