using System;
using System.Windows.Forms;
using SdrConfig = SDR.Configuration;

namespace DotSpatial.SDR.Plugins.Search
{
    public partial class SearchPanel : UserControl
    {
        #region Private Variables

        private string _searchQuery;
        private SearchMode _searchMode;

        #endregion

        #region Constructors

        public SearchPanel()
        {
            InitializeComponent();
            _searchQuery = string.Empty;
            DataGridDisplay = searchDGV;
            _searchMode = SearchMode.Address;
        }

        #endregion

        #region Properties

        public void ClearSearches()
        {
            if (SearchesCleared != null) SearchesCleared(this, EventArgs.Empty);
        }

        #endregion

        /// <summary>
        /// Gets or sets which type of search to perfrom
        /// </summary>
        public SearchMode SearchMode
        {
            get { return _searchMode; }
            set
            {
                _searchMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the datagrid view for query display
        /// </summary>
        public DataGridView DataGridDisplay { get; set; }

        public string SearchQuery
        {
            get { return _searchQuery; }
            set
            {
                _searchQuery = value;
            }
        }

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
        /// Occurs when a row has been double clicked
        /// </summary>
        public event EventHandler RowDoubleClicked;

        private void btnSearch_Click(object sender, EventArgs e)
        {
            // make sure we have a valid query to submit
            if (txtSearch.Text.Length < 0) return;  
            _searchQuery = txtSearch.Text;
            OnPerformSearch();
        }

        private void searchName_Click(object sender, EventArgs e)
        {
            _searchMode = SearchMode.Name;
            searchName.Checked = true;
            searchAdds.Checked = false;
            searchPhone.Checked = false;
            searchRoad.Checked = false;
            searchIntersection.Checked = false;
            OnSearchModeChanged();
        }

        private void searchAdds_Click(object sender, EventArgs e)
        {
            _searchMode = SearchMode.Address;
            searchAdds.Checked = true;
            searchPhone.Checked = false;
            searchName.Checked = false;
            searchIntersection.Checked = false;
            searchRoad.Checked = false;
            OnSearchModeChanged();
        }

        private void searchPhone_Click(object sender, EventArgs e)
        {
            _searchMode = SearchMode.Phone;
            searchPhone.Checked = true;
            searchAdds.Checked = false;
            searchName.Checked = false;
            searchIntersection.Checked = false;
            searchRoad.Checked = false;
            OnSearchModeChanged();
        }

        private void searchRoad_Click(object sender, EventArgs e)
        {
            _searchMode = SearchMode.Road;
            searchAdds.Checked = false;
            searchPhone.Checked = false;
            searchName.Checked = false;
            searchIntersection.Checked = false;
            searchRoad.Checked = true;
            OnSearchModeChanged();
        }

        private void searchIntersection_Click(object sender, EventArgs e)
        {
            _searchMode = SearchMode.Intersection;
            searchAdds.Checked = false;
            searchPhone.Checked = false;
            searchName.Checked = false;
            searchIntersection.Checked = true;
            searchRoad.Checked = false;
            OnSearchModeChanged();
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
           if (SearchModeChanged != null) SearchModeChanged(this, new EventArgs());
        }

        private void OnPerformSearch()
        {
            if (PerformSearch != null) PerformSearch(this, new EventArgs());
        }

        private void searchDGV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (RowDoubleClicked != null) RowDoubleClicked(this, e);
        }
    }
}
