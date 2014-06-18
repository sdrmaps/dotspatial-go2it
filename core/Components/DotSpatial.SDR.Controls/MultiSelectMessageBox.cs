using System;
using System.Linq;
using System.Windows.Forms;

namespace DotSpatial.SDR.Controls
{
    public partial class MultiSelectMessageBox : Form
    {
        public string Title
        {
            get { return Text;  }
            set { Text = value; }
        }

        public string Message
        {
            get { return messageLabel.ToString(); }
            set { messageLabel.Text = value; }
        }

        public string[] Selections
        {
            get
            {
                return (from object selection in selectionList.Items select selection.ToString()).ToArray();
            }
            set
            {
                selectionList.Items.Clear();
                foreach (var s in value)
                {
                    selectionList.Items.Add(s);
                }
            }
        }

        public string Result
        {
            get { return selectionList.SelectedItem.ToString(); }
        }

        public MultiSelectMessageBox(string title, string message, string[] selections)
        {
            InitializeComponent();
            Title = title;
            Message = message;
            Selections = selections;
            selectionList.DoubleClick += SelectionListOnDoubleClick;
        }

        private void SelectionListOnDoubleClick(object sender, EventArgs eventArgs)
        {
            Close();
        }
    }
}
