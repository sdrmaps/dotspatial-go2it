using System.ComponentModel.Composition;
using System.Windows.Forms;
using DotSpatial.Controls;
using SDR.Common;
using SDR.Common.logging;
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
            var log = AppContext.Instance.Get<ILog>();
            AppManager = new AppManager
            {
                // map placeholder on app manager, used to swap out active map views using tab interface
                Map = new Map
                {
                    Dock = DockStyle.Fill,
                    Visible = true
                }
            };
            log.Info("Startup -> AppManager.Map.Projection: " + AppManager.Map.Projection.ToProj4String());
            log.Info("Startup -> AppManager.Map.MapFrame.ProjectionString: " + AppManager.Map.MapFrame.ProjectionString);
            log.Info("Startup -> AppManager.Map.Extent: " + AppManager.Map.Extent);
            log.Info("Startup -> AppManager.Map.ViewExtents: " + AppManager.Map.ViewExtents);
            _shell = this;
            AppManager.LoadExtensions();
        }
    }
}