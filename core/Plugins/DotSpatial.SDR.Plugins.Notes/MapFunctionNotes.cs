using System.Collections.Specialized;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Topology;
using SdrConfig = SDR.Configuration;

namespace DotSpatial.SDR.Plugins.Notes
{
    /// <summary>
    /// A MapFunction that allows creation, editing and deletion to the "notes" layer
    /// </summary>
    public class MapFunctionNotes : MapFunction
    {
        #region Constructors

        public MapFunctionNotes()
        {
            Name = "MapFunctionNotes";
        }

        public IMapFeatureLayer NotesLayer
        {
            get; set;
        }

        private static StringCollection NotesFields
        {
            get
            {
                return SdrConfig.Project.Go2ItProjectSettings.Instance.NoteFields;
            }
        }
        #endregion

        protected override void OnMouseDown(GeoMouseArgs e)
        {
            var coord = e.GeographicLocation;

            IEnvelope env = new Envelope(coord);
            env.ExpandBy(5);  // arbitrary unit size expansion (to generate an extent)

            IFeatureSet fs = NotesLayer.DataSet;
            var fl = fs.Select(env.ToExtent());
            IFeature ft = fl.Count > 0 ? fl[0] : null;
            var p = new Padding(0, 8, 0, 0);  // padding to move text down a bit
            var nf = new NotesForm();
            nf.NotesTable.RowCount = NotesFields.Count;
            for (var i = 0; i <= NotesFields.Count - 1; i++)
            {
                var f = NotesFields[i];
                nf.NotesTable.Controls.Add(new Label { Text = f, AutoSize = true, Padding = p }, 0, i);
                if (ft != null)
                {
                    var v = ft.DataRow[f].ToString();
                    nf.NotesTable.Controls.Add(new TextBox { Text = v, Dock = DockStyle.Fill }, 1, i);
                }
                else
                {
                    nf.NotesTable.Controls.Add(new TextBox { Text = string.Empty, Dock = DockStyle.Fill }, 1, i);
                }
            }
            nf.Show();

            base.OnMouseDown(e);
        }
    }
}
