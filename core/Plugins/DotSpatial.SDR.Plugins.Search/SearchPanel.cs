using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DotSpatial.SDR.Controls;
using DotSpatial.SDR.Plugins.Search.Properties;
using Lucene.Net.Search;
using SDR.Common;
using SDR.Common.logging;
using SdrConfig = SDR.Configuration;

namespace DotSpatial.SDR.Plugins.Search
{
    public partial class SearchPanel : UserControl
    {
        #region Private Variables

        private SearchMode _searchMode;
        private TableLayoutPanel _addressPanel;
        private TableLayoutPanel _roadPanel;
        private TableLayoutPanel _intersectionPanel;
        private readonly EventedArrayList _intersectedFeatures;

        #endregion

        #region Constructors

        public SearchPanel()
        {
            InitializeComponent();
            SearchQuery = string.Empty;
            CreateQueryPanels();
            _intersectedFeatures = new EventedArrayList();
            _intersectedFeatures.ListChanged += IntersectedFeaturesOnListChanged;
            _searchMode = SearchMode;
        }

        private void IntersectedFeaturesOnListChanged(object sender, EventArgs eventArgs)
        {
            if (_intersectedFeatures.Count <= 0) return;
            var cmb = (ComboBox) _intersectionPanel.Controls["cmbIntSearch2"];
            cmb.Items.Clear();
            // make sure to remove duplicates
            cmb.Items.AddRange(_intersectedFeatures.ToArray().Distinct().ToArray());
            cmb.Sorted = true;
            cmb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmb.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets which type of search to perfrom
        /// </summary>
        /// <summary>
        /// Gets or sets whether to display the distances or areas.
        /// </summary>
        public SearchMode SearchMode
        {
            get
            {
                var funcMode = UserSettings.Default.SearchMode;
                if (funcMode.Length <= 0) return SearchMode.Address;
                SearchMode sm;
                Enum.TryParse(funcMode, true, out sm);
                return sm;
            }
            set
            {
                _searchMode = value;  // update the searchmode for local reference
                UserSettings.Default.SearchMode = value.ToString();
                UserSettings.Default.Save();
            }
        }


        /// <summary>
        /// Gets or sets the datagrid view for query display
        /// </summary>
        public DataGridView DataGridDisplay
        {
            get { return searchDGV; }
        }
        /// <summary>
        /// Gets or sets the search query
        /// </summary>
        public string SearchQuery { get; set; }

        /// <summary>
        /// Sets the intersected features to display intersections search combobox
        /// </summary>
        public ArrayList IntersectedFeatures
        {
            set
            {
                _intersectedFeatures.Clear();
                _intersectedFeatures.AddRange(value);
            }
        }

        public void ActivateSearchModeButton()
        {
            switch (_searchMode)
            {
                case SearchMode.Address:
                    ActivateAddressSearch();
                    break;
                case SearchMode.Road:
                    ActivateRoadSearch();
                    break;
                case SearchMode.Intersection:
                    ActivateIntersectionSearch();
                    break;
                case SearchMode.Name:
                    ActivateNameSearch();
                    break;
                case SearchMode.Phone:
                    ActivatePhoneSearch();
                    break;
            }
        }

        public void DeactivateSearchModeButtons()
        {
            searchName.Checked = false;
            searchAdds.Checked = false;
            searchPhone.Checked = false;
            searchRoad.Checked = false;
            searchIntersection.Checked = false;
        }

        #endregion

        #region Event Handlers
        /// <summary>
        /// Occurs when the search mode has been changed.
        /// </summary>
        public event EventHandler SearchModeChanged;
        /// <summary>
        /// Occurs when the clear button has been pressed.
        /// </summary>
        public event EventHandler SearchesCleared;
        /// <summary>
        /// Occurs when the hydrant button has been pressed.
        /// </summary>
        public event EventHandler HydrantLocate;
        /// <summary>
        /// Occurs when the search button has been pressed.
        /// </summary>
        public event EventHandler PerformSearch;
        /// <summary>
        /// Occurs when a feature is to be zoomed to
        /// </summary>
        public event EventHandler OnRowDoublelicked;
        /// <summary>
        /// Occurs when the search mode has been activated
        /// </summary>
        public event EventHandler SearchModeActivated;

        #endregion

        #region Events

        private void btnSearch_Click(object sender, EventArgs e)
        {
            switch (_searchMode)
            {
                case SearchMode.Address:
                    SearchQuery = _addressPanel.Controls["txtAddressSearch"].Text;
                    break;
                case SearchMode.Road:
                    SearchQuery = _roadPanel.Controls["cmbRoadSearch"].Text;
                    break;
                case SearchMode.Intersection:
                    SearchQuery = _intersectionPanel.Controls["cmbIntSearch1"].Text + "|" +
                        _intersectionPanel.Controls["cmbIntSearch2"].Text;
                    break;
                case SearchMode.Name:
                    SearchQuery = _addressPanel.Controls["txtAddressSearch"].Text;
                    break;
                case SearchMode.Phone:
                    SearchQuery = _addressPanel.Controls["txtAddressSearch"].Text;
                    break;
            }
            OnPerformSearch();
        }

        private void ActivateNameSearch()
        {
            // toggle the button for this tool
            searchName.Checked = true;
            searchAdds.Checked = false;
            searchPhone.Checked = false;
            searchRoad.Checked = false;
            searchIntersection.Checked = false;
            // setup the search panel for this tool
            RemoveCurrentSearchPanel();
            searchLayoutPanel.Controls.Add(_addressPanel, 0, 0);
            _addressPanel.Controls["txtAddressSearch"].Text = string.Empty;
            ClearSearches(); // clears dgv
        }

        private void searchName_Click(object sender, EventArgs e)
        {
            if (SearchModeActivated != null) SearchModeActivated(this, EventArgs.Empty);
            if (_searchMode != SearchMode.Name)
            {
                SearchMode = SearchMode.Name;
                OnSearchModeChanged();
                ActivateNameSearch();
            }
        }

        private void ActivatePhoneSearch()
        {
            // toggle the button for this tool
            searchPhone.Checked = true;
            searchAdds.Checked = false;
            searchName.Checked = false;
            searchIntersection.Checked = false;
            searchRoad.Checked = false;
            // setup the search panel for this tool
            RemoveCurrentSearchPanel();
            searchLayoutPanel.Controls.Add(_addressPanel, 0, 0);
            _addressPanel.Controls["txtAddressSearch"].Text = string.Empty;
            ClearSearches();
        }

        private void searchPhone_Click(object sender, EventArgs e)
        {
            if (SearchModeActivated != null) SearchModeActivated(this, EventArgs.Empty);
            if (_searchMode != SearchMode.Phone)
            {
                SearchMode = SearchMode.Phone;
                OnSearchModeChanged();
                ActivatePhoneSearch();
            }
        }

        private void ActivateRoadSearch()
        {
            // toggle the button for this tool
            searchAdds.Checked = false;
            searchPhone.Checked = false;
            searchName.Checked = false;
            searchIntersection.Checked = false;
            searchRoad.Checked = true;
            // setup search panel for this tool
            RemoveCurrentSearchPanel();
            searchLayoutPanel.Controls.Add(_roadPanel, 0, 0);
            _roadPanel.Controls["cmbRoadSearch"].Text = String.Empty;
            ClearSearches();
            PopulateRoadsToCombo();
        }

        private void searchRoad_Click(object sender, EventArgs e)
        {
            if (SearchModeActivated != null) SearchModeActivated(this, EventArgs.Empty);
            if (_searchMode != SearchMode.Road)
            {
                SearchMode = SearchMode.Road;
                OnSearchModeChanged();
                ActivateRoadSearch();
            }
        }

        private void searchAdds_Click(object sender, EventArgs e)
        {
            if (SearchModeActivated != null) SearchModeActivated(this, EventArgs.Empty);
            if (_searchMode != SearchMode.Address)
            {
                SearchMode = SearchMode.Address;
                OnSearchModeChanged();
                ActivateAddressSearch();
            }
        }

        private void ActivateAddressSearch()
        {
            // toggle the button for this tool
            searchAdds.Checked = true;
            searchPhone.Checked = false;
            searchName.Checked = false;
            searchIntersection.Checked = false;
            searchRoad.Checked = false;
            // setup the search panel for this tool
            RemoveCurrentSearchPanel();
            searchLayoutPanel.Controls.Add(_addressPanel, 0, 0);
            _addressPanel.Controls["txtAddressSearch"].Text = string.Empty;
            ClearSearches();
        }

        private void ActivateIntersectionSearch()
        {
            // toggle the button for this tool
            searchAdds.Checked = false;
            searchPhone.Checked = false;
            searchName.Checked = false;
            searchIntersection.Checked = true;
            searchRoad.Checked = false;
            // setup the search panel for this tool
            RemoveCurrentSearchPanel();
            searchLayoutPanel.Controls.Add(_intersectionPanel, 0, 0);
            _intersectionPanel.Controls["cmbIntSearch1"].Text = string.Empty;
            _intersectionPanel.Controls["cmbIntSearch2"].Text = string.Empty;
            ClearSearches();
            PopulateRoadsToCombo();
        }

        private void searchIntersection_Click(object sender, EventArgs e)
        {
            if (SearchModeActivated != null) SearchModeActivated(this, EventArgs.Empty);
            if (_searchMode != SearchMode.Intersection)
            {
                SearchMode = SearchMode.Intersection;
                OnSearchModeChanged();
                ActivateIntersectionSearch();
            }
        }

        private void searchHydrant_Click(object sender, EventArgs e)
        {
            if (HydrantLocate != null) HydrantLocate(this, EventArgs.Empty);
        }

        private void searchClear_Click(object sender, EventArgs e)
        {
            if (SearchesCleared != null) SearchesCleared(this, EventArgs.Empty);
        }

        private void OnSearchModeChanged()
        {
            btnSearch.Text = _searchMode == SearchMode.Intersection ? @"Locate" : @"Search";
            if (SearchModeChanged != null) SearchModeChanged(this, new EventArgs());
        }

        private void OnPerformSearch()
        {
            if (PerformSearch != null) PerformSearch(this, new EventArgs());
        }

        private void searchDGV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (OnRowDoublelicked != null) OnRowDoublelicked(this, e);
        }

        #endregion

        # region Methods

        /// <summary>
        /// Create all the tablepanels for search types input
        /// </summary>
        private void CreateQueryPanels()
        {
            /*--------------------------------------------------------*/
            // address search panel
            _addressPanel = new TableLayoutPanel
            {
                Name = "addressPanel",
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 1
            };
            _addressPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _addressPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent));
            var txtAddressSearch = new TextBox {Name = "txtAddressSearch", Dock = DockStyle.Fill};
            _addressPanel.Controls.Add(txtAddressSearch, 0, 0);

            /*--------------------------------------------------------*/
            // road search panel
            _roadPanel = new TableLayoutPanel
            {
                Name = "roadPanel",
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 1
            };
            _roadPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _roadPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent));
            var cmbRoadSearch = new ComboBox {Name = "cmbRoadSearch", Dock = DockStyle.Fill};
            // fixes auto suggestion when the list is dropped down
            cmbRoadSearch.DropDown += (sender, e) => cmbRoadSearch.AutoCompleteMode = AutoCompleteMode.None;
            cmbRoadSearch.DropDownClosed += delegate
            {
                cmbRoadSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                SearchQuery = _roadPanel.Controls["cmbRoadSearch"].Text;
                OnPerformSearch();
            };
            cmbRoadSearch.SelectedValueChanged += delegate
            {
                SearchQuery = _roadPanel.Controls["cmbRoadSearch"].Text;
                OnPerformSearch();
            };
            _roadPanel.Controls.Add(cmbRoadSearch, 0, 0);
            
            /*--------------------------------------------------------*/
            // intersection search panel
            _intersectionPanel = new TableLayoutPanel
            {
                Name = "intersectionPanel",
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1
            };
            _intersectionPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _intersectionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            _intersectionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 85));
            _intersectionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            var btnIntSearch = new Button {Text = @"Intersections", Name = "btnIntSearch", Dock = DockStyle.Fill};
            btnIntSearch.Click += delegate
            {
                SearchQuery = _intersectionPanel.Controls["cmbIntSearch1"].Text + "|";
                OnPerformSearch();
            };
            var cmbIntSearch1 = new ComboBox {Name = "cmbIntSearch1", Dock = DockStyle.Fill};
            // fixes auto suggestion when the list is dropped down
            cmbIntSearch1.DropDown += (sender, e) => cmbIntSearch1.AutoCompleteMode = AutoCompleteMode.None;
            cmbIntSearch1.DropDownClosed += delegate
            {
                cmbIntSearch1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                SearchQuery = _intersectionPanel.Controls["cmbIntSearch1"].Text + "|";
                OnPerformSearch();
            };
            cmbIntSearch1.SelectedValueChanged += delegate
            {
                SearchQuery = _intersectionPanel.Controls["cmbIntSearch1"].Text + "|";
                OnPerformSearch();
            };
            var cmbIntSearch2 = new ComboBox {Name = "cmbIntSearch2", Dock = DockStyle.Fill};
            // fixes auto suggestion when the list is dropped down
            cmbIntSearch2.DropDown += (sender, e) => cmbIntSearch2.AutoCompleteMode = AutoCompleteMode.None;
            cmbIntSearch2.DropDownClosed += delegate
            {
                cmbIntSearch2.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                SearchQuery = _intersectionPanel.Controls["cmbIntSearch1"].Text + "|" +
                    _intersectionPanel.Controls["cmbIntSearch2"].Text;
                OnPerformSearch();
            };
            cmbIntSearch2.SelectedValueChanged += delegate
            {
                SearchQuery = _intersectionPanel.Controls["cmbIntSearch1"].Text + "|" +
                    _intersectionPanel.Controls["cmbIntSearch2"].Text;
                OnPerformSearch();
            };
            _intersectionPanel.Controls.Add(cmbIntSearch1, 0, 0);
            _intersectionPanel.Controls.Add(btnIntSearch, 1, 0);
            _intersectionPanel.Controls.Add(cmbIntSearch2, 2, 0);
        }

        /// <summary>
        /// Clear Search
        /// </summary>
        public void ClearSearches()
        {
            if (SearchesCleared != null) SearchesCleared(this, EventArgs.Empty);
        }

        private void PopulateRoadsToCombo()
        {
            Query query = new MatchAllDocsQuery();  // query grabs all documents
            var log = AppContext.Instance.Get<ILog>();
            if (MapFunctionSearch.IndexSearcher == null)
            {
                log.Info("PopulateRoadsToCombo in SearchPanel failed to load, valid _indexSearcher not found");
            }
            else
            {
                TopDocs docs = MapFunctionSearch.IndexSearcher.Search(query, MapFunctionSearch.IndexReader.MaxDoc);
                ScoreDoc[] hits = docs.ScoreDocs;
                FormatQueryResultsForComboBox(hits);
            }
        }

        private void RemoveCurrentSearchPanel()
        {
            var c = searchLayoutPanel.GetControlFromPosition(0, 0);
            if (c != null)
            {
                searchLayoutPanel.Controls.Remove(c);
            }
        }

        private void FormatQueryResultsForComboBox(IEnumerable<ScoreDoc> hits)
        {
            var scoreDocs = hits as ScoreDoc[] ?? hits.ToArray();
            if (!scoreDocs.Any()) return;
            // snag the combobox of the search mode
            var cmb = new ComboBox();
            switch (_searchMode)
            {
                case SearchMode.Road:
                    cmb = _roadPanel.Controls["cmbRoadSearch"] as ComboBox;
                    break;
                case SearchMode.Intersection:
                    cmb = _intersectionPanel.Controls["cmbIntSearch1"] as ComboBox;
                    break;
            }
            if (cmb == null) return;

            cmb.Items.Clear();

            int count = 0;
            foreach (var hit in scoreDocs)
            {
                // snatch the ranked document
                var doc = MapFunctionSearch.IndexSearcher.Doc(hit.Doc);
                var val = string.Empty;
                // create the full string and add to combobox
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
                if (val.Trim().Length > 0)
                {
                    if (!cmb.Items.Contains(val.Trim()))
                    {
                        cmb.Items.Add(val.Trim());
                        count++;
                    }
                }
            }
            cmb.Sorted = true;
            cmb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmb.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        #endregion
    }
}
