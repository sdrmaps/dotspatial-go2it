using System.ComponentModel.Composition;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.SDR.Controls;
using SdrConfig = SDR.Configuration;

namespace Go2It
{
    /// <summary>
    /// Main form to which all mapping controls are assigned
    /// </summary>
    public partial class MainForm : Form
    {
        [Export("Shell", typeof(ContainerControl))]
        private static ContainerControl _shell;

        /// <summary>
        /// Gets or Sets the AppManager
        /// </summary>
        public AppManager AppManager { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            // load any custom set hotkeys from the app db | else hotkeys load from assemblies in extension load
            HotKeyManager.LoadHotKeys();

            // create our application manager
            AppManager = new AppManager();
            // replace the default SerializationManager with our custom ProjectManager
            var projManager = new ProjectManager(AppManager);
            AppManager.SerializationManager = (ProjectManager) projManager;

            _shell = this;
            // load any extensions/plugins now
            AppManager.LoadExtensions();
            AppManager.ProgressHandler.Progress("", 0, "Starting Go2It...");
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // static hotkeymanager for communicating with all tools and controls loaded via extensions
            // returns a bool to indicate if the hotkey event was handled or not
            return HotKeyManager.FireHotKeyEvent(ref msg, keyData) || base.ProcessCmdKey(ref msg, keyData);
        }
    }
}