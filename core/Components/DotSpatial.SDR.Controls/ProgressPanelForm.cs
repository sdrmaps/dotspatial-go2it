using System;
using System.Windows.Forms;

namespace DotSpatial.SDR.Controls
{
    public partial class ProgressPanelForm : Form
    {
        public static ProgressPanelForm LoadForm = null;

        public static ProgressPanelForm Instance()
        {
            return LoadForm ?? (LoadForm = new ProgressPanelForm());
        }

        public ProgressPanelForm()
        {
            InitializeComponent();
        }

        public ProgressPanelForm(string label)
        {
            InitializeComponent();
            loadingLabel.Text = label;
            Text = label;
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private void LoadingForm_Load(object sender, EventArgs e)
        {
            Refresh();
        }
    }
}
