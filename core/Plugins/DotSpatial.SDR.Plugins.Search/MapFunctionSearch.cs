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
        private DataGridView _dataGridView;
        private string _indexType;
        private string _indexQuery;

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
                int idx = GetColumnIndex("FID");
                string fid = dgvr.Cells[idx].Value.ToString();

                //switch (_searchMode)
                //{

                //    SdrConfig.Project.Go2ItProjectSettings.Instance.AddressLayers
                //}
            }
        }

        private void SearchPanelOnPerformSearch(object sender, EventArgs eventArgs)
        {
            _searchPanel.ClearSearches();
            var searchQuery = _searchPanel.SearchQuery;

            List<DataGridViewTextBoxColumn> cols = GetQueryColumns(_indexQuery);
            foreach (DataGridViewTextBoxColumn col in cols)
            {
                _dataGridView.Columns.Add(col);
            }
            List<DataGridViewRow> rows = ExecuteLuceneQuery(searchQuery);
            foreach (DataGridViewRow row in rows)
            {
                _dataGridView.Rows.Add(row);
            }
        }

        private void SearchPanelOnHydrantLocate(object sender, EventArgs eventArgs)
        {
            MessageBox.Show("locate hydrant");
        }

        private void SetSearchVariables()
        {
            _searchMode = _searchPanel.SearchMode;
            switch (_searchMode)
            {
                case SearchMode.Address:
                    _indexType = "AddressIndex";
                    _indexQuery = "SELECT * FROM " + _indexType;
                    break;
                case SearchMode.Intersection:
                    // _indexType = "AddressIndex";
                    // _indexQuery = "SELECT * FROM " + _indexType;
                    break;
                case SearchMode.Name:
                    // _indexType = "AddressIndex";
                    // _indexQuery = "SELECT * FROM " + _indexType;
                    break;
                case SearchMode.Phone:
                    // _indexType = "AddressIndex";
                    // _indexQuery = "SELECT * FROM " + _indexType;
                    break;
                case SearchMode.Road:
                    // _indexType = "AddressIndex";
                    // _indexQuery = "SELECT * FROM " + _indexType;
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

        private static List<DataGridViewTextBoxColumn> GetQueryColumns(string query)
        {
            string conn = SdrConfig.Settings.Instance.ProjectRepoConnectionString;
            DataTable table = SQLiteHelper.GetDataTable(conn, query);
            var columns = new List<DataGridViewTextBoxColumn>();
            // lets see what key/ValueType pairs have been set
            for (int i = 0; i < table.Rows.Count; i++)
            {
                object[] row = table.Rows[i].ItemArray;
                // verify that a value exists for a key before adding it
                if (row[1].ToString().Length > 0)
                {
                    var txtCol = new DataGridViewTextBoxColumn
                    {
                        HeaderText = row[2].ToString(),
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                        SortMode = DataGridViewColumnSortMode.Automatic
                    };
                    columns.Add(txtCol);
                }
            }
            var fidCol = new DataGridViewTextBoxColumn
            {
                HeaderText = "FID"
            };
            fidCol.Visible = false;
            columns.Add(fidCol);
            return columns;
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

        private List<DataGridViewRow> FormatAddressIndexQueryResults(IEnumerable<ScoreDoc> hits, Searcher searcher)
        {
            var rowList = new List<DataGridViewRow>();
            // get the names of all the indexed fields now
            string conn = SdrConfig.Settings.Instance.ProjectRepoConnectionString;
            DataTable fieldsTable = SQLiteHelper.GetDataTable(conn, _indexQuery);
            var fieldsList = new List<string>();
            for (int i = 0; i < fieldsTable.Rows.Count; i++)
            {
                object[] row = fieldsTable.Rows[i].ItemArray;
                if (row[1].ToString().Length > 0)
                {
                    fieldsList.Add(row[1].ToString());
                }
            }
            foreach (var hit in hits)
            {
                var dgvRow = new DataGridViewRow();  // our new row to add to the datagridview
                var doc = searcher.Doc(hit.Doc);  // snatch the ranked document
                foreach (string field in fieldsList)
                {
                    var dgvCell = new DataGridViewTextBoxCell { Value = doc.Get(field) };
                    dgvRow.Cells.Add(dgvCell);
                }
                // make sure we have a FID lookup value
                var fidCell = new DataGridViewTextBoxCell {Value = doc.Get("FID")};
                dgvRow.Cells.Add(fidCell);
                rowList.Add(dgvRow);
            }
            searcher.Dispose();
            return rowList;
        }

        private List<DataGridViewRow> FormatQueryResults(IEnumerable<ScoreDoc> hits, Searcher searcher)
        {
            switch (_searchMode)
            {
                case SearchMode.Address:
                    return FormatAddressIndexQueryResults(hits, searcher);
                case SearchMode.Name:
                    return null;
                    // return FormatAddressIndexQueryResults(hits, searcher, idxQuery);
                case SearchMode.Phone:
                    return null;
                    // return FormatAddressIndexQueryResults(hits, searcher, idxQuery);
            }
            return null;
            
        }

        private List<DataGridViewRow> ExecuteLuceneQuery(string searchString)
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

            return FormatQueryResults(hits, searcher);
        }


        private static Query ConstructPhoneQuery(string searchQuery)
        {
            /*var values = new ArrayList();
            var fields = new ArrayList();
            var occurs = new ArrayList();
            // lets strip everything except actual digits
            // string clean_query = new String(search_query.Where(Char.IsDigit).ToArray());
            values.Add(searchQuery);
            fields.Add("Phone");
            occurs.Add(Occur.MUST);
            // values.Add(search_query);
            // fields.Add("Aux. Phone");
            // occurs.Add(Occur.SHOULD);

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
            return query;*/
            return null;
        }

        private static Query ConstructNameQuery(string searchQuery)
        {
            /*var values = new ArrayList();
            var fields = new ArrayList();
            var occurs = new ArrayList();

            // strip all non alpha numerics now
            char[] arr = searchQuery.Where(c => (Char.IsLetterOrDigit(c) ||
                                                 Char.IsWhiteSpace(c) ||
                                                 c == '-')).ToArray();
            var cleanQuery = new string(arr);
            var search = cleanQuery.Split();

            foreach (string t in search)
            {
                values.Add(t);
                fields.Add("First Name");
                occurs.Add(Occur.SHOULD);
                values.Add(t);
                fields.Add("Last Name");
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
            return query;*/
            return null;
        }

        #endregion
    }
}
