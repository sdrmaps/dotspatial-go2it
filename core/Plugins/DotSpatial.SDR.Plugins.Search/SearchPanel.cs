using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DotSpatial.SDR.Controls;
using Lucene.Net.Search;
using SDR.Common;
using SDR.Common.logging;
using SdrConfig = SDR.Configuration;

namespace DotSpatial.SDR.Plugins.Search
{
    public partial class SearchPanel : UserControl
    {
        #region Private Variables

        private TableLayoutPanel _addressPanel;
        private TableLayoutPanel _roadPanel;
        private TableLayoutPanel _intersectionPanel;
        private TableLayoutPanel _coordinatePanel;
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

        public string CoordinateError
        {
            set
            {
                _coordinatePanel.Controls["lblCoordError"].Text = value;
                if (value.Length > 0)
                {
                    _coordinatePanel.ColumnStyles[4].SizeType = SizeType.AutoSize;
                }
                else
                {
                    _coordinatePanel.ColumnStyles[4].SizeType = SizeType.Absolute;
                    _coordinatePanel.ColumnStyles[4].Width = 0;
                }
            }
        }
        public void ActivateSearchModeButton()
        {
            switch (PluginSettings.Instance.SearchMode)
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
                case SearchMode.KeyLocation:
                    ActivateKeyLocationsSearch();
                    break;
                case SearchMode.All:
                    ActivateAllFieldsSearch();
                    break;
                case SearchMode.City:
                    ActivateCitySearch();
                    break;
                case SearchMode.Esn:
                    ActivateEsnSearch();
                    break;
                case SearchMode.CellSector:
                    ActivateCellSectorSearch();
                    break;
                case SearchMode.Parcel:
                    ActivateParcelSearch();
                    break;
                case SearchMode.Coordinate:
                    ActivateCoordinateSearch();
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
            searchKeyLocations.Checked = false;
            searchAll.Checked = false;
            searchCity.Checked = false;
            searchEsn.Checked = false;
            searchCellSector.Checked = false;
            searchParcels.Checked = false;
            searchCoordinate.Checked = false;
        }

        /// <summary>
        /// Check that there are valid indexes for each search, disable the button if not present
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="enabled"></param>
        public void EnableSearchButton(SearchMode mode, bool enabled)
        {
            switch (mode)
            {
                case SearchMode.Address:
                    searchAdds.Enabled = enabled;
                    searchName.Enabled = enabled;
                    searchPhone.Enabled = enabled;
                    break;
                case SearchMode.Road:
                    searchRoad.Enabled = enabled;
                    searchIntersection.Enabled = enabled;
                    break;
                case SearchMode.KeyLocation:
                    searchKeyLocations.Enabled = enabled;
                    break;
                case SearchMode.City:
                    searchCity.Enabled = enabled;
                    break;
                case SearchMode.Esn:
                    searchEsn.Enabled = enabled;
                    break;
                case SearchMode.CellSector:
                    searchCellSector.Enabled = enabled;
                    break;
                case SearchMode.Parcel:
                    searchParcels.Enabled = enabled;
                    break;
                case SearchMode.Hydrant:
                    searchHydrant.Enabled = enabled;
                    break;
                case SearchMode.Coordinate:
                    searchCoordinate.Enabled = enabled;
                    break;
                case SearchMode.All:
                    searchAll.Enabled = enabled;
                    break;
            }
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
            switch (PluginSettings.Instance.SearchMode)
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
                case SearchMode.KeyLocation:
                    SearchQuery = _addressPanel.Controls["txtAddressSearch"].Text;
                    break;
                case SearchMode.All:
                    SearchQuery = _addressPanel.Controls["txtAddressSearch"].Text;
                    break;
                case SearchMode.City:
                    SearchQuery = _roadPanel.Controls["cmbRoadSearch"].Text;
                    break;
                case SearchMode.Esn:
                    SearchQuery = _roadPanel.Controls["cmbRoadSearch"].Text;
                    break;
                case SearchMode.CellSector:
                    SearchQuery = _addressPanel.Controls["txtAddressSearch"].Text;
                    break;
                case SearchMode.Parcel:
                    SearchQuery = _addressPanel.Controls["txtAddressSearch"].Text;
                    break;
                case SearchMode.Coordinate:
                    SearchQuery = _coordinatePanel.Controls["txtLatitude"].Text + "|" + 
                        _coordinatePanel.Controls["txtLongitude"].Text;
                    break;
            }
            OnPerformSearch();
        }

        private void ActivateParcelSearch()
        {
            // toggle the button for this tool
            searchName.Checked = false;
            searchAdds.Checked = false;
            searchPhone.Checked = false;
            searchRoad.Checked = false;
            searchIntersection.Checked = false;
            searchKeyLocations.Checked = false;
            searchAll.Checked = false;
            searchCity.Checked = false;
            searchEsn.Checked = false;
            searchParcels.Checked = true;
            searchCellSector.Checked = false;
            searchCoordinate.Checked = false;

            // setup the search panel for this tool
            RemoveCurrentSearchPanel();
            searchLayoutPanel.Controls.Add(_addressPanel, 0, 0);
            _addressPanel.Controls["txtAddressSearch"].Text = string.Empty;
            ClearSearches(); // clears dgv
        }

        private void searchParcels_Click(object sender, EventArgs e)
        {
            if (SearchModeActivated != null) SearchModeActivated(this, EventArgs.Empty);
            if (PluginSettings.Instance.SearchMode != SearchMode.Parcel)
            {
                PluginSettings.Instance.SearchMode = SearchMode.Parcel;
                OnSearchModeChanged();
                ActivateParcelSearch();
            }
        }

        private void ActivateCellSectorSearch()
        {
            // toggle the button for this tool
            searchName.Checked = false;
            searchAdds.Checked = false;
            searchPhone.Checked = false;
            searchRoad.Checked = false;
            searchIntersection.Checked = false;
            searchKeyLocations.Checked = false;
            searchAll.Checked = false;
            searchCity.Checked = false;
            searchEsn.Checked = false;
            searchParcels.Checked = false;
            searchCellSector.Checked = true;
            searchCoordinate.Checked = false;

            // setup the search panel for this tool
            RemoveCurrentSearchPanel();
            searchLayoutPanel.Controls.Add(_addressPanel, 0, 0);
            _addressPanel.Controls["txtAddressSearch"].Text = string.Empty;
            ClearSearches(); // clears dgv
        }

        private void searchCellSector_Click(object sender, EventArgs e)
        {
            if (SearchModeActivated != null) SearchModeActivated(this, EventArgs.Empty);
            if (PluginSettings.Instance.SearchMode != SearchMode.CellSector)
            {
                PluginSettings.Instance.SearchMode = SearchMode.CellSector;
                OnSearchModeChanged();
                ActivateCellSectorSearch();
            }
        }

        private void ActivateCitySearch()
        {
            // toggle the button for this tool
            searchAdds.Checked = false;
            searchPhone.Checked = false;
            searchName.Checked = false;
            searchIntersection.Checked = false;
            searchRoad.Checked = false;
            searchKeyLocations.Checked = false;
            searchAll.Checked = false;
            searchCity.Checked = true;
            searchEsn.Checked = false;
            searchParcels.Checked = false;
            searchCellSector.Checked = false;
            searchCoordinate.Checked = false;

            // setup search panel for this tool
            RemoveCurrentSearchPanel();
            searchLayoutPanel.Controls.Add(_roadPanel, 0, 0);
            _roadPanel.Controls["cmbRoadSearch"].Text = String.Empty;
            ClearSearches();
            PopulateValuesToCombo();
        }

        private void searchCity_Click(object sender, EventArgs e)
        {
            if (SearchModeActivated != null) SearchModeActivated(this, EventArgs.Empty);
            if (PluginSettings.Instance.SearchMode != SearchMode.City)
            {
                PluginSettings.Instance.SearchMode = SearchMode.City;
                OnSearchModeChanged();
                ActivateCitySearch();
            }
        }

        private void ActivateEsnSearch()
        {
            // toggle the button for this tool
            searchAdds.Checked = false;
            searchPhone.Checked = false;
            searchName.Checked = false;
            searchIntersection.Checked = false;
            searchRoad.Checked = false;
            searchKeyLocations.Checked = false;
            searchAll.Checked = false;
            searchCity.Checked = false;
            searchEsn.Checked = true;
            searchParcels.Checked = false;
            searchCellSector.Checked = false;
            searchCoordinate.Checked = false;

            // setup search panel for this tool
            RemoveCurrentSearchPanel();
            searchLayoutPanel.Controls.Add(_roadPanel, 0, 0);
            _roadPanel.Controls["cmbRoadSearch"].Text = String.Empty;
            ClearSearches();
            PopulateValuesToCombo();
        }

        private void searchEsn_Click(object sender, EventArgs e)
        {
            if (SearchModeActivated != null) SearchModeActivated(this, EventArgs.Empty);
            if (PluginSettings.Instance.SearchMode != SearchMode.Esn)
            {
                PluginSettings.Instance.SearchMode = SearchMode.Esn;
                OnSearchModeChanged();
                ActivateEsnSearch();
            }
        }

        private void ActivateNameSearch()
        {
            // toggle the button for this tool
            searchName.Checked = true;
            searchAdds.Checked = false;
            searchPhone.Checked = false;
            searchRoad.Checked = false;
            searchIntersection.Checked = false;
            searchKeyLocations.Checked = false;
            searchAll.Checked = false;
            searchCity.Checked = false;
            searchEsn.Checked = false;
            searchParcels.Checked = false;
            searchCellSector.Checked = false;
            searchCoordinate.Checked = false;

            // setup the search panel for this tool
            RemoveCurrentSearchPanel();
            searchLayoutPanel.Controls.Add(_addressPanel, 0, 0);
            _addressPanel.Controls["txtAddressSearch"].Text = string.Empty;
            ClearSearches(); // clears dgv
        }

        private void searchName_Click(object sender, EventArgs e)
        {
            if (SearchModeActivated != null) SearchModeActivated(this, EventArgs.Empty);
            if (PluginSettings.Instance.SearchMode != SearchMode.Name)
            {
                PluginSettings.Instance.SearchMode = SearchMode.Name;
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
            searchKeyLocations.Checked = false;
            searchAll.Checked = false;
            searchCity.Checked = false;
            searchEsn.Checked = false;
            searchParcels.Checked = false;
            searchCellSector.Checked = false;
            searchCoordinate.Checked = false;

            // setup the search panel for this tool
            RemoveCurrentSearchPanel();
            searchLayoutPanel.Controls.Add(_addressPanel, 0, 0);
            _addressPanel.Controls["txtAddressSearch"].Text = string.Empty;
            ClearSearches();
        }

        private void searchPhone_Click(object sender, EventArgs e)
        {
            if (SearchModeActivated != null) SearchModeActivated(this, EventArgs.Empty);
            if (PluginSettings.Instance.SearchMode != SearchMode.Phone)
            {
                PluginSettings.Instance.SearchMode = SearchMode.Phone;
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
            searchKeyLocations.Checked = false;
            searchAll.Checked = false;
            searchCity.Checked = false;
            searchEsn.Checked = false;
            searchParcels.Checked = false;
            searchCellSector.Checked = false;
            searchCoordinate.Checked = false;

            // setup search panel for this tool
            RemoveCurrentSearchPanel();
            searchLayoutPanel.Controls.Add(_roadPanel, 0, 0);
            _roadPanel.Controls["cmbRoadSearch"].Text = String.Empty;
            ClearSearches();
            PopulateValuesToCombo();
        }

        public void SearchRoad_Click(object sender, EventArgs e)
        {
            if (SearchModeActivated != null) SearchModeActivated(this, EventArgs.Empty);
            if (PluginSettings.Instance.SearchMode != SearchMode.Road)
            {
                PluginSettings.Instance.SearchMode = SearchMode.Road;
                OnSearchModeChanged();
                ActivateRoadSearch();
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
            searchKeyLocations.Checked = false;
            searchAll.Checked = false;
            searchCity.Checked = false;
            searchEsn.Checked = false;
            searchParcels.Checked = false;
            searchCellSector.Checked = false;
            searchCoordinate.Checked = false;

            // setup the search panel for this tool
            RemoveCurrentSearchPanel();
            searchLayoutPanel.Controls.Add(_addressPanel, 0, 0);
            _addressPanel.Controls["txtAddressSearch"].Text = string.Empty;
            ClearSearches();
        }

        private void searchAdds_Click(object sender, EventArgs e)
        {
            if (SearchModeActivated != null) SearchModeActivated(this, EventArgs.Empty);
            if (PluginSettings.Instance.SearchMode != SearchMode.Address)
            {
                PluginSettings.Instance.SearchMode = SearchMode.Address;
                OnSearchModeChanged();
                ActivateAddressSearch();
            }
        }

        private void ActivateKeyLocationsSearch()
        {
            // toggle the button for this tool
            searchPhone.Checked = false;
            searchAdds.Checked = false;
            searchName.Checked = false;
            searchIntersection.Checked = false;
            searchRoad.Checked = false;
            searchKeyLocations.Checked = true;
            searchAll.Checked = false;
            searchCity.Checked = false;
            searchEsn.Checked = false;
            searchParcels.Checked = false;
            searchCellSector.Checked = false;
            searchCoordinate.Checked = false;

            // setup the search panel for this tool
            RemoveCurrentSearchPanel();
            searchLayoutPanel.Controls.Add(_addressPanel, 0, 0);
            _addressPanel.Controls["txtAddressSearch"].Text = string.Empty;
            ClearSearches();
        }

        private void searchKeyLocations_Click(object sender, EventArgs e)
        {
            if (SearchModeActivated != null) SearchModeActivated(this, EventArgs.Empty);
            if (PluginSettings.Instance.SearchMode != SearchMode.KeyLocation)
            {
                PluginSettings.Instance.SearchMode = SearchMode.KeyLocation;
                OnSearchModeChanged();
                ActivateKeyLocationsSearch();
            }
        }

        private void ActivateAllFieldsSearch()
        {
            // toggle the button for this tool
            searchPhone.Checked = false;
            searchAdds.Checked = false;
            searchName.Checked = false;
            searchIntersection.Checked = false;
            searchRoad.Checked = false;
            searchKeyLocations.Checked = false;
            searchAll.Checked = true;
            searchCity.Checked = false;
            searchEsn.Checked = false;
            searchParcels.Checked = false;
            searchCellSector.Checked = false;
            searchCoordinate.Checked = false;

            // setup the search panel for this tool
            RemoveCurrentSearchPanel();
            searchLayoutPanel.Controls.Add(_addressPanel, 0, 0);
            _addressPanel.Controls["txtAddressSearch"].Text = string.Empty;
            ClearSearches();
        }

        private void searchAll_Click(object sender, EventArgs e)
        {
            if (SearchModeActivated != null) SearchModeActivated(this, EventArgs.Empty);
            if (PluginSettings.Instance.SearchMode != SearchMode.All)
            {
                PluginSettings.Instance.SearchMode = SearchMode.All;
                OnSearchModeChanged();
                ActivateAllFieldsSearch();
            }
        }

        private void ActivateIntersectionSearch()
        {
            // toggle the button for this tool
            searchAdds.Checked = false;
            searchPhone.Checked = false;
            searchName.Checked = false;
            searchIntersection.Checked = true;
            searchRoad.Checked = false;
            searchKeyLocations.Checked = false;
            searchAll.Checked = false;
            searchCity.Checked = false;
            searchEsn.Checked = false;
            searchParcels.Checked = false;
            searchCellSector.Checked = false;
            searchCoordinate.Checked = false;

            // setup the search panel for this tool
            RemoveCurrentSearchPanel();
            searchLayoutPanel.Controls.Add(_intersectionPanel, 0, 0);
            _intersectionPanel.Controls["cmbIntSearch1"].Text = string.Empty;
            _intersectionPanel.Controls["cmbIntSearch2"].Text = string.Empty;
            ClearSearches();
            PopulateValuesToCombo();
        }

        private void searchIntersection_Click(object sender, EventArgs e)
        {
            if (SearchModeActivated != null) SearchModeActivated(this, EventArgs.Empty);
            if (PluginSettings.Instance.SearchMode != SearchMode.Intersection)
            {
                PluginSettings.Instance.SearchMode = SearchMode.Intersection;
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
            ClearSearches();
        }

        private void searchCoordinate_Click(object sender, EventArgs e)
        {
            if (SearchModeActivated != null) SearchModeActivated(this, EventArgs.Empty);
            if (PluginSettings.Instance.SearchMode != SearchMode.Coordinate)
            {
                PluginSettings.Instance.SearchMode = SearchMode.Coordinate;
                OnSearchModeChanged();
                ActivateCoordinateSearch();
            }
        }

        private void ActivateCoordinateSearch()
        {
            // toggle the button for this tool
            searchAdds.Checked = false;
            searchPhone.Checked = false;
            searchName.Checked = false;
            searchIntersection.Checked = false;
            searchRoad.Checked = false;
            searchKeyLocations.Checked = false;
            searchAll.Checked = false;
            searchCity.Checked = false;
            searchEsn.Checked = false;
            searchParcels.Checked = false;
            searchCellSector.Checked = false;
            searchCoordinate.Checked = true;

            // setup the search panel for this tool
            RemoveCurrentSearchPanel();
            searchLayoutPanel.Controls.Add(_coordinatePanel, 0, 0);
            // clear any currently set coordinates or errors displayed
            _coordinatePanel.Controls["txtLatitude"].Text = string.Empty;
            _coordinatePanel.Controls["txtLongitude"].Text = string.Empty;
            _coordinatePanel.Controls["lblCoordError"].Text = string.Empty;
            _coordinatePanel.ColumnStyles[4].SizeType = SizeType.Absolute;
            _coordinatePanel.ColumnStyles[4].Width = 0;
            ClearSearches();
        }

        private void OnSearchModeChanged()
        {
            // set the buttton label with proper text value
            if (PluginSettings.Instance.SearchMode == SearchMode.Intersection ||
                PluginSettings.Instance.SearchMode == SearchMode.Coordinate)
            {
                btnSearch.Text = @"Locate";
            }
            else
            {
                btnSearch.Text = @"Search";
            }
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
            // coordinate search tablelayoutpanel
            _coordinatePanel = new TableLayoutPanel
            {
                Name = "coordinatePanel",
                Dock = DockStyle.Fill,
                ColumnCount = 5,
                RowCount = 1
            };
            _coordinatePanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _coordinatePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 56));
            _coordinatePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            _coordinatePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 64));
            _coordinatePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            _coordinatePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 0));
            // coordinate search interface controls
            var lblLatitude = new Label { Name = "lblLatitude", Text = @"Latitude:", Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            var lblLongitude = new Label { Name = "lblLongitude", Text = @"Longitude:", Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            var txtLatitude = new TextBox { Name = "txtLatitude", Dock = DockStyle.Fill };
            var txtLongitude = new TextBox { Name = "txtLongitude", Dock = DockStyle.Fill };
            var lblCoordError = new Label { Name = "lblCoordError", Text = @"Coordinate Error", Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            // append ui controls to the tablelayoutpanel
            _coordinatePanel.Controls.Add(lblLatitude, 0, 0);
            _coordinatePanel.Controls.Add(txtLatitude, 1, 0);
            _coordinatePanel.Controls.Add(lblLongitude, 2, 0);
            _coordinatePanel.Controls.Add(txtLongitude, 3, 0);
            _coordinatePanel.Controls.Add(lblCoordError, 4, 0);
            // handle keypress event of 'enter' key same as search button click
            txtLatitude.KeyPress += delegate(object sender, KeyPressEventArgs e)
            {
                if (e.KeyChar != (char)Keys.Enter) return;
                SearchQuery = _coordinatePanel.Controls["txtLatitude"].Text + "|" +
                    _coordinatePanel.Controls["txtLongitude"].Text;
                OnPerformSearch();
            };
            txtLongitude.KeyPress += delegate(object sender, KeyPressEventArgs e)
            {
                if (e.KeyChar != (char)Keys.Enter) return;
                SearchQuery = _coordinatePanel.Controls["txtLatitude"].Text + "|" +
                    _coordinatePanel.Controls["txtLongitude"].Text;
                OnPerformSearch();
            };

            /*--------------------------------------------------------*/
            // address search tablelayoutpanel
            _addressPanel = new TableLayoutPanel
            {
                Name = "addressPanel",
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 1,
            };
            _addressPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _addressPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent));
            // address search interface controls
            var txtAddressSearch = new TextBox {Name = "txtAddressSearch", Dock = DockStyle.Fill};
            // add ui controls to tablelayoutpanel
            _addressPanel.Controls.Add(txtAddressSearch, 0, 0);
            // handle 'keypress' event for 'enter' key the same as btnSearch_Click()
            txtAddressSearch.KeyPress += delegate(object sender, KeyPressEventArgs e)
            {
                if (e.KeyChar != (char) Keys.Enter) return;
                SearchQuery = _addressPanel.Controls["txtAddressSearch"].Text;
                OnPerformSearch();
            };

            /*--------------------------------------------------------*/
            // road search tablelayoutpanel
            _roadPanel = new TableLayoutPanel
            {
                Name = "roadPanel",
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 1
            };
            _roadPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _roadPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent));
            // road search interface controls
            var cmbRoadSearch = new ComboBox {Name = "cmbRoadSearch", Dock = DockStyle.Fill};
            // add ui controls to the tablelayoutpanel
            _roadPanel.Controls.Add(cmbRoadSearch, 0, 0);
            // fixes combobox auto-suggestion when 'dropdown' event is fired
            cmbRoadSearch.DropDown += (sender, e) => cmbRoadSearch.AutoCompleteMode = AutoCompleteMode.None;
            // handle 'dropdownclosed' event the same as btnSearch_Click()
            cmbRoadSearch.DropDownClosed += delegate
            {
                cmbRoadSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                SearchQuery = _roadPanel.Controls["cmbRoadSearch"].Text;
                OnPerformSearch();
            };
            // handle 'selectedvaluechanged' event the same as btnSearch_Click()
            cmbRoadSearch.SelectedValueChanged += delegate
            {
                SearchQuery = _roadPanel.Controls["cmbRoadSearch"].Text;
                OnPerformSearch();
            };
            // handle 'keypress' event for 'enter' key the same as btnSearch_Click()
            cmbRoadSearch.KeyUp += delegate(object sender, KeyEventArgs e)
            {
                // seem redundant, but needed to handle all interface actions possible
                if (e.KeyCode != Keys.Enter) return;
                SearchQuery = _roadPanel.Controls["cmbRoadSearch"].Text;
                OnPerformSearch();
            };
            
            /*--------------------------------------------------------*/
            // intersection search tablelayoutpanel
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
            // intersection search interface controls
            var btnIntSearch = new Button {Text = @"Intersections", Name = "btnIntSearch", Dock = DockStyle.Fill};
            var cmbIntSearch1 = new ComboBox { Name = "cmbIntSearch1", Dock = DockStyle.Fill };
            var cmbIntSearch2 = new ComboBox { Name = "cmbIntSearch2", Dock = DockStyle.Fill };
            // add ui controls to the tablelayoutpanel
            _intersectionPanel.Controls.Add(cmbIntSearch1, 0, 0);
            _intersectionPanel.Controls.Add(btnIntSearch, 1, 0);
            _intersectionPanel.Controls.Add(cmbIntSearch2, 2, 0);
            // fixes combobox auto-suggestion when 'dropdown' event is fired
            cmbIntSearch1.DropDown += (sender, e) => cmbIntSearch1.AutoCompleteMode = AutoCompleteMode.None;
            cmbIntSearch1.DropDownClosed += delegate
            {
                cmbIntSearch1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                SearchQuery = _intersectionPanel.Controls["cmbIntSearch1"].Text + "|";
                OnPerformSearch();
            };
            // handle 'selectedvaluechanged' event the same as btnSearch_Click()
            cmbIntSearch1.SelectedValueChanged += delegate
            {
                SearchQuery = _intersectionPanel.Controls["cmbIntSearch1"].Text + "|";
                OnPerformSearch();
            };
            // handle 'keypress' event for 'enter' key the same as btnSearch_Click()
            cmbIntSearch1.KeyUp += delegate(object sender, KeyEventArgs e)
            {
                if (e.KeyCode != Keys.Enter) return;
                SearchQuery = _intersectionPanel.Controls["cmbIntSearch1"].Text + "|";
                OnPerformSearch();
            };
            // handle 'click' of intersection button the same as btnSearch_Click()
            btnIntSearch.Click += delegate
            {
                SearchQuery = _intersectionPanel.Controls["cmbIntSearch1"].Text + "|";
                OnPerformSearch();
            };
            // fixes combobox auto-suggestion when 'dropdown' event is fired
            cmbIntSearch2.DropDown += (sender, e) => cmbIntSearch2.AutoCompleteMode = AutoCompleteMode.None;
            cmbIntSearch2.DropDownClosed += delegate
            {
                cmbIntSearch2.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                SearchQuery = _intersectionPanel.Controls["cmbIntSearch1"].Text + "|" +
                    _intersectionPanel.Controls["cmbIntSearch2"].Text;
                OnPerformSearch();
            };
            // handle 'selectedvaluechanged' event the same as btnSearch_Click()
            cmbIntSearch2.SelectedValueChanged += delegate
            {
                SearchQuery = _intersectionPanel.Controls["cmbIntSearch1"].Text + "|" +
                    _intersectionPanel.Controls["cmbIntSearch2"].Text;
                OnPerformSearch();
            };
            // handle 'keypress' event for 'enter' key the same as btnSearch_Click()
            cmbIntSearch2.KeyUp += delegate(object sender, KeyEventArgs e)
            {
                if (e.KeyCode != Keys.Enter) return;
                SearchQuery = _intersectionPanel.Controls["cmbIntSearch1"].Text + "|" +
                    _intersectionPanel.Controls["cmbIntSearch2"].Text;
                OnPerformSearch();
            };
        }

        /// <summary>
        /// Clear Search
        /// </summary>
        public void ClearSearches()
        {
            if (SearchesCleared != null) SearchesCleared(this, EventArgs.Empty);
        }

        private void PopulateValuesToCombo()
        {
            Query query = new MatchAllDocsQuery();  // query grabs all documents
            if (MapFunctionSearch.IndexSearcher.IndexReader == null)
            {
                var log = AppContext.Instance.Get<ILog>();
                log.Info("No Search index exists");
            }
            else
            {
                TopDocs docs = MapFunctionSearch.IndexSearcher.Search(query, MapFunctionSearch.IndexSearcher.IndexReader.MaxDoc);
                ScoreDoc[] hits = docs.ScoreDocs;
                FormatQueryResultsForComboBox(hits);
            }
        }

        private void RemoveCurrentSearchPanel()
        {
            var c = searchLayoutPanel.GetControlFromPosition(0, 0);
            if (c != null)
            {
                // TODO: look into improving this with show and hide.
                // c.Hide();
                searchLayoutPanel.Controls.Remove(c);
            }
        }

        private void PopulateRoadsToCombo(ref ComboBox cmb, IEnumerable<ScoreDoc> scoreDocs)
        {
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
                if (val.Trim().Length > 0)
                {
                    if (!cmb.Items.Contains(val.Trim()))
                    {
                        cmb.Items.Add(val.Trim());
                    }
                }
            }
        }

        private void PopulateBoundariesToCombo(ref ComboBox cmb, IEnumerable<ScoreDoc> scoreDocs)
        {
            foreach (var hit in scoreDocs)
            {
                // snatch the ranked document
                var doc = MapFunctionSearch.IndexSearcher.Doc(hit.Doc);
                var val = string.Empty;

                // populate each value to the combobox
                if (doc.Get("Name") != null)
                {
                    val = doc.Get("Name").Trim();
                }
                if (val.Length > 0)
                {
                    if (!cmb.Items.Contains(val))
                    {
                        cmb.Items.Add(val);
                    }
                }
            }
        }

        private void FormatQueryResultsForComboBox(IEnumerable<ScoreDoc> hits)
        {
            var scoreDocs = hits as ScoreDoc[] ?? hits.ToArray();
            if (!scoreDocs.Any()) return;
            // snag the combobox of the search mode
            var cmb = new ComboBox();
            switch (PluginSettings.Instance.SearchMode)
            {
                case SearchMode.Road:
                    cmb = _roadPanel.Controls["cmbRoadSearch"] as ComboBox;
                    break;
                case SearchMode.Intersection:
                    cmb = _intersectionPanel.Controls["cmbIntSearch1"] as ComboBox;
                    break;
                case SearchMode.City:
                    cmb = _roadPanel.Controls["cmbRoadSearch"] as ComboBox;
                    break;
                case SearchMode.Esn:
                    cmb = _roadPanel.Controls["cmbRoadSearch"] as ComboBox;
                    break;
            }
            if (cmb == null) return;
            cmb.Items.Clear();
            // populate the combo box based in the searchmode type
            if (PluginSettings.Instance.SearchMode == SearchMode.Road || PluginSettings.Instance.SearchMode == SearchMode.Intersection)
            {
                PopulateRoadsToCombo(ref cmb, scoreDocs);
            }
            else if (PluginSettings.Instance.SearchMode == SearchMode.City || PluginSettings.Instance.SearchMode == SearchMode.Esn)
            {
                PopulateBoundariesToCombo(ref cmb, scoreDocs);
            }
            cmb.Sorted = true;
            cmb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmb.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        #endregion
    }
}
