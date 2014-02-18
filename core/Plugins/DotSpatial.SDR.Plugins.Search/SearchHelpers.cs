using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;
using Directory = Lucene.Net.Store.Directory;
using System.IO;
using SdrConfig = SDR.Configuration;
using SDR.Data.Database;
using System.Collections;

namespace DotSpatial.SDR.Plugins.Search
{
    public class SearchHelpers
    {
        public static List<DataGridViewRow> ExecuteLuceneQuery(string searchType, string searchString, string idxType, string idxQuery)
        {
            Directory idxDir = FSDirectory.Open(new DirectoryInfo(SdrConfig.Settings.Instance.CurrentProjectDirectory + "\\indexes\\" + idxType));
            IndexReader reader = IndexReader.Open(idxDir, true);
            Searcher searcher = new IndexSearcher(reader);
            Query query = ConstructLuceneQuery(searchType, searchString);
            TopDocs docs = searcher.Search(query, reader.MaxDoc);
            ScoreDoc[] hits = docs.ScoreDocs;
            idxDir.Dispose();  // wipe the directory ref out now
            // format the results accordingly
            switch (searchType)
            {
                case "address":
                    return FormatAddressIndexQueryResults(hits, searcher, idxQuery);
                case "name":
                    return FormatAddressIndexQueryResults(hits, searcher, idxQuery);
                case "phone":
                    return FormatAddressIndexQueryResults(hits, searcher, idxQuery);
            }
            return null;
        }

        public static List<DataGridViewTextBoxColumn> GetQueryColumns(string query)
        {
            string conn = SdrConfig.Settings.Instance.ProjectRepoConnectionString;
            DataTable table = SQLiteHelper.GetDataTable(conn, query);
            var columns = new List<DataGridViewTextBoxColumn>();
            // lets see what key/ValueType pairs have been set
            for (int i = 0; i < table.Rows.Count; i++)
            {
                object[] row = table.Rows[i].ItemArray;
                // verify that a value exists for a key before adding it
                if (row[1].ToString().Length > 0) {
                    var txtCol = new DataGridViewTextBoxColumn
                    {
                        HeaderText = row[0].ToString(),
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                        SortMode = DataGridViewColumnSortMode.Automatic
                    };
                    columns.Add(txtCol);
                }
            }
            return columns;
        }

        private static List<DataGridViewRow> FormatAddressIndexQueryResults(IEnumerable<ScoreDoc> hits, Searcher searcher, string idxQuery)
        {
            var rowList = new List<DataGridViewRow>();
            // get the names of all the indexed fields now
            string conn = SdrConfig.Settings.Instance.ProjectRepoConnectionString;
            DataTable fieldsTable = SQLiteHelper.GetDataTable(conn, idxQuery);
            var fieldsList = new List<string>();
            for (int i = 0; i < fieldsTable.Rows.Count; i++)
            {
                object[] row = fieldsTable.Rows[i].ItemArray;
                if (row[1].ToString().Length > 0)
                {
                    fieldsList.Add(row[0].ToString());
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
                rowList.Add(dgvRow);
            }
            searcher.Dispose();
            return rowList;
        }

        private static Query ConstructLuceneQuery(string searchType, string searchQuery)
        {
            switch (searchType)
            {
                case "address":
                    return ConstructAddressQuery(searchQuery);
                case "name":
                    return ConstructNameQuery(searchQuery);
                case "phone":
                    return ConstructPhoneQuery(searchQuery);
            }
            return null;
        }

        private static Query ConstructPhoneQuery(string searchQuery)
        {
            var values = new ArrayList();
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
            return query;
        }

        private static Query ConstructNameQuery(string searchQuery)
        {
            var values = new ArrayList();
            var fields = new ArrayList();
            var occurs = new ArrayList();

            // strip all non alpha numerics now
            char[] arr = searchQuery.Where(c => (char.IsLetterOrDigit(c) ||
                                         char.IsWhiteSpace(c) ||
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
            return query;
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
    }
}
