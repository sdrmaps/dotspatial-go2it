using System;
using System.Windows.Forms;

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
            set { _frmState = value; }
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
