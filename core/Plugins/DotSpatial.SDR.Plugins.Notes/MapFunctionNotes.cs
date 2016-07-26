using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Data;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using SdrConfig = SDR.Configuration;

namespace DotSpatial.SDR.Plugins.Notes
{
    /// <summary>
    /// A MapFunction that allows creation, editing and deletion to the "notes" layer
    /// </summary>
    public class MapFunctionNotes : MapFunction
    {
        [Import("Shell")]
        internal ContainerControl Shell { get; set; }

        #region Constructors

        public MapFunctionNotes()
        {
            Name = "MapFunctionNotes";
            ActiveForm = new NotesForm();
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

        private IFeature ActiveFeature { get; set; }
        private NotesForm ActiveForm { get; set; }
        private Coordinate ActiveCoordinate { get; set; }

        #endregion

        public enum FormState
        {
            /// <summary>
            /// Save the note to the notes featureclass
            /// </summary>
            Save,
            /// <summary>
            /// Take no action to save or delete
            /// </summary>
            Cancel,
            /// <summary>
            /// Delete the active note feature from the featureclass
            /// </summary>
            Delete
        }

        public void HotKeyDeleteNote()
        {
            if (ActiveForm.Visible)
            {
                ActiveForm.ActionState = FormState.Delete;
                ActiveForm.Close();
            }
        }

        protected override void OnMouseDown(GeoMouseArgs e)
        {
            if (ActiveForm.Visible) return;

            ActiveCoordinate = e.GeographicLocation;
            Envelope env = new Envelope(ActiveCoordinate);
            env.ExpandBy(25); // arbitrary unit size expansion (to generate an extent)
            IFeatureSet fs = NotesLayer.DataSet;
            var fl = fs.Select(env.ToExtent()); // select any feature within the extent
            ActiveFeature = fl.Count > 0 ? fl[0] : null;

            ActiveForm = new NotesForm();
            ActiveForm.NotesTable.RowCount = NotesFields.Count;
            ActiveForm.FormClosing += NotesOnFormClosing;

            if (ActiveFeature != null)
            {
                ActiveForm.DisplayDeleteButton = true;
                ActiveForm.SaveButtonText = "Update";
                ActiveForm.Text = @"Update an existing note";
            }
            else
            {
                ActiveForm.DisplayDeleteButton = false;
                ActiveForm.SaveButtonText = "Create";
                ActiveForm.Text = @"Create a new note";
            }
            for (var i = 0; i <= NotesFields.Count - 1; i++)
            {
                var f = NotesFields[i];
                ActiveForm.NotesTable.Controls.Add(
                    new Label {Text = f, AutoSize = true, Padding = new Padding(0, 8, 0, 0)}, 0, i);
                ActiveForm.NotesTable.Controls.Add(
                    ActiveFeature != null
                        ? new TextBox {Name = f, Text = ActiveFeature.DataRow[f].ToString(), Dock = DockStyle.Fill, Multiline = true}
                        : new TextBox {Name = f, Text = string.Empty, Dock = DockStyle.Fill, Multiline = true}, 1, i);
            }
            ActiveForm.Show(Shell);

            base.OnMouseDown(e);
        }

        private void NotesOnFormClosing(object sender, FormClosingEventArgs formClosingEventArgs)
        {
            var frm = sender as NotesForm;
            if (frm != null && frm.ActionState == FormState.Save)
            {
                if (ActiveFeature != null)
                {
                    // update the attributes of an existing notes feature
                    for (var j = 0; j <= NotesFields.Count - 1; j++)
                    {
                        var i = frm.NotesTable.Controls.IndexOfKey(NotesFields[j]);
                        ActiveFeature.DataRow[NotesFields[j]] = frm.NotesTable.Controls[i].Text;
                    }
                    NotesLayer.DataSet.Save();
                    ActiveFeature = null;
                }
                else
                {
                    // create new point and assign attributes to notes feature
                    var pt = new Point(ActiveCoordinate);
                    var ft = NotesLayer.DataSet.AddFeature(pt);
                    var cnt = 0;
                    for (var j = 0; j <= NotesFields.Count - 1; j++)
                    {
                        var i = frm.NotesTable.Controls.IndexOfKey(NotesFields[j]);
                        ft.DataRow[NotesFields[j]] = frm.NotesTable.Controls[i].Text;
                        cnt = cnt + frm.NotesTable.Controls[i].Text.Length;
                    }
                    if (cnt > 0) // check to validate the user input anything into the form
                    {

                        NotesLayer.DataSet.UpdateExtent();
                        NotesLayer.DataSet.InitializeVertices();
                        NotesLayer.DataSet.Save();
                        NotesLayer.AssignFastDrawnStates();
                        Map.MapFrame.Invalidate();
                    }
                    else
                    {
                        NotesLayer.DataSet.Features.Remove(ft);
                    }
                }
            } 
            else if (frm != null && frm.ActionState == FormState.Delete)
            {
                if (ActiveFeature == null) return;

                // delete the active notes feature
                NotesLayer.DataSet.Features.Remove(ActiveFeature);
                NotesLayer.DataSet.Save();
                Map.MapFrame.Invalidate();
                ActiveFeature = null;
            }
        }
    }
}
