using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SdrConfig = SDR.Configuration;


namespace DotSpatial.SDR.Plugins.Search
{
    public partial class SearchPanel : UserControl
    {
        public SearchPanel()
        {
            InitializeComponent();
        }
        // determine the type of query and go..
        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text.Length < 0) return;  // make sure a query has even been submitted
            // clear any existing data shown on the datagridview currently
            searchDGV.Rows.Clear();
            searchDGV.Columns.Clear();
            // lists for storing the columns and rows to populate dgv
            string searchType = string.Empty;
            string idxType = string.Empty;
            var idxQuery = string.Empty;
            // now set the settings as required
            if (searchAdds.Checked)
            {
                searchType = "address";
                idxType = "Address_Index";
                idxQuery = "SELECT * FROM " + idxType;
            }
            else if (searchName.Checked)
            {
                searchType = "name";
                idxType = "Address_Index";
                idxQuery = "SELECT * FROM " + idxType;
            } 
            else if (searchPhone.Checked)
            {
                searchType = "phone";
                idxType = "Address_Index";
                idxQuery = "SELECT * FROM " + idxType;
            }
            else if (searchRoad.Checked)
            {
            }
            else if (searchIntersection.Checked)
            {
            }
            // complete the datagridview population
            List<DataGridViewTextBoxColumn> cols = SearchHelpers.GetQueryColumns(idxQuery);
            foreach(DataGridViewTextBoxColumn col in cols)
            {
                searchDGV.Columns.Add(col);
            }
            List<DataGridViewRow> rows = SearchHelpers.ExecuteLuceneQuery(searchType, txtSearch.Text, idxType, idxQuery);
            foreach(DataGridViewRow row in rows) 
            {
                searchDGV.Rows.Add(row);
            }
        }

        private void searchName_Click(object sender, EventArgs e)
        {
            searchName.Checked = true;
            searchAdds.Checked = false;
            searchPhone.Checked = false;
            searchRoad.Checked = false;
            searchIntersection.Checked = false;
        }

        private void searchAdds_Click(object sender, EventArgs e)
        {
            searchAdds.Checked = true;
            searchPhone.Checked = false;
            searchName.Checked = false;
            searchIntersection.Checked = false;
            searchRoad.Checked = false;
        }

        private void searchPhone_Click(object sender, EventArgs e)
        {
            searchPhone.Checked = true;
            searchAdds.Checked = false;
            searchName.Checked = false;
            searchIntersection.Checked = false;
            searchRoad.Checked = false;
        }

        private void searchHydrant_Click(object sender, EventArgs e)
        {

        }

        private void searchClear_Click(object sender, EventArgs e)
        {

        }

        private void searchRoad_Click(object sender, EventArgs e)
        {
            searchAdds.Checked = false;
            searchPhone.Checked = false;
            searchName.Checked = false;
            searchIntersection.Checked = false;
            searchRoad.Checked = true;
        }

        private void searchIntersection_Click(object sender, EventArgs e)
        {
            searchAdds.Checked = false;
            searchPhone.Checked = false;
            searchName.Checked = false;
            searchIntersection.Checked = true;
            searchRoad.Checked = false;
        }
    }
}
