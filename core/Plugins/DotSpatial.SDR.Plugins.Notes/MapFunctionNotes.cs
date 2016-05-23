using System;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
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
        [Import("Shell")]
        internal ContainerControl Shell { get; set; }

        #region Constructors

        public MapFunctionNotes()
        {
            Name = "MapFunctionNotes";
        }

        public IMapFeatureLayer NotesLayer
        {
            get; set;
        }

        public Coordinate NoteCoordinate
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

        private IFeature _activeFeature;
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

        protected override void OnMouseDown(GeoMouseArgs e)
        {
            NoteCoordinate = e.GeographicLocation;
            IEnvelope env = new Envelope(NoteCoordinate);
            env.ExpandBy(25);  // arbitrary unit size expansion (to generate an extent)
            IFeatureSet fs = NotesLayer.DataSet;
            var fl = fs.Select(env.ToExtent());  // select any feature within the extent
            _activeFeature = fl.Count > 0 ? fl[0] : null;

            var nf = new NotesForm();
            nf.NotesTable.RowCount = NotesFields.Count;
            nf.FormClosing += NotesOnFormClosing;

            if (_activeFeature != null)
            {
                nf.DisplayDeleteButton = true;
                nf.SaveButtonText = "Update";
                nf.Text = @"Update an existing note";
            }
            else
            {
                nf.DisplayDeleteButton = false;
                nf.SaveButtonText = "Create";
                nf.Text = @"Create a new note";
            }
            for (var i = 0; i <= NotesFields.Count - 1; i++)
            {
                var f = NotesFields[i];
                nf.NotesTable.Controls.Add(new Label { Text = f, AutoSize = true, Padding = new Padding(0, 8, 0, 0) }, 0, i);
                nf.NotesTable.Controls.Add(
                    _activeFeature != null
                        ? new TextBox {Name = f, Text = _activeFeature.DataRow[f].ToString(), Dock = DockStyle.Fill, Multiline = true}
                        : new TextBox {Name = f, Text = string.Empty, Dock = DockStyle.Fill, Multiline = true}, 1, i);
            }
            nf.Show(Shell);

            base.OnMouseDown(e);
        }

        private void NotesOnFormClosing(object sender, FormClosingEventArgs formClosingEventArgs)
        {
            var frm = sender as NotesForm;
            if (frm != null && frm.ActionState == FormState.Save)
            {
                if (_activeFeature != null)
                {
                    // update the attributes of an existing notes feature
                    for (var j = 0; j <= NotesFields.Count - 1; j++)
                    {
                        var i = frm.NotesTable.Controls.IndexOfKey(NotesFields[j]);
                        _activeFeature.DataRow[NotesFields[j]] = frm.NotesTable.Controls[i].Text;
                    }
                    NotesLayer.DataSet.Save();
                    _activeFeature = null;
                }
                else
                {
                    // create new point and assign attributes to notes feature
                    var pt = new Point(NoteCoordinate);
                    var ft = NotesLayer.DataSet.AddFeature(new Feature(pt));
                    for (var j = 0; j <= NotesFields.Count - 1; j++)
                    {
                        var i = frm.NotesTable.Controls.IndexOfKey(NotesFields[j]);
                        ft.DataRow[NotesFields[j]] = frm.NotesTable.Controls[i].Text;
                    }
                    NotesLayer.DataSet.UpdateExtent();
                    NotesLayer.DataSet.InitializeVertices();
                    NotesLayer.DataSet.Save();
                    NotesLayer.AssignFastDrawnStates();
                    Map.MapFrame.Invalidate();
                }
            } 
            else if (frm != null && frm.ActionState == FormState.Delete)
            {
                if (_activeFeature == null) return;

                NotesLayer.DataSet.Features.Remove(_activeFeature);
                NotesLayer.DataSet.Save();
                Map.MapFrame.Invalidate();
                _activeFeature = null;
            }
        }
    }
}
