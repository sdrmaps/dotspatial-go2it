using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotSpatial.Data;

namespace DotSpatial.SDR.Plugins.Notes
{
    public partial class NotesForm : Form
    {
        private MapFunctionNotes.FormState _frmState;

        public NotesForm()
        {
            InitializeComponent();
            _frmState = MapFunctionNotes.FormState.Cancel;
        }

        public string SaveButtonText
        {
            set { btnSave.Text = value; }    
        }

        public bool DisplayDeleteButton
        {
            set { btnDelete.Visible = value; }    
        }

        public MapFunctionNotes.FormState ActionState
        {
            get { return _frmState;}

        }
        public TableLayoutPanel NotesTable
        {
            get { return notesTable; }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _frmState = MapFunctionNotes.FormState.Save;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _frmState = MapFunctionNotes.FormState.Cancel;
            Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            _frmState = MapFunctionNotes.FormState.Delete;
            Close();
        }
    }
}
