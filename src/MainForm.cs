using System.ComponentModel.Composition;
using System.Windows.Forms;
using DotSpatial.Controls;
using SdrConfig = SDR.Configuration;

namespace Go2It
{
    /// <summary>
    /// A Form to add all the main mapping controls to
    /// </summary>
    public partial class MainForm : Form
    {
        [Export("Shell", typeof(ContainerControl))]
        private static ContainerControl _shell;

        /// <summary>
        /// Gets or sets the appManager
        /// </summary>
        public AppManager AppManager { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            // create the application level app manager and assign base map and mapframe
            AppManager = new AppManager
            {
                // map placeholder on app manager, used to swap out active map views using tab interface
                Map = new Map
                {
                    Dock = DockStyle.Fill,
                    Visible = false,
                }
            };
            _shell = this;
            AppManager.LoadExtensions();
        }
    }
}