using System.ComponentModel.Composition;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.SDR.Controls;
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
            // load any custom set hotkeys from the app db / else hotkeys load on extension assembly load
            HotKeyManager.LoadHotKeys();
            // create the application level app manager and assign base map and mapframe
            AppManager = new AppManager
            {

            };
                            // map placeholder on app manager, used to swap out active map views using tab interface
            //Map map = new Map
            //{
            //    Dock = DockStyle.Fill,
            //    Visible = false,
            //};

            // AppManager.Map = map;

            // TODO: remove this
            AppManager.MapChanged += delegate(object sender, MapChangedEventArgs args)
            {
                var log = AppContext.Instance.Get<ILog>();
                log.Info("AppManager.MapChanged -- MainForm");
            };


            _shell = this;
            AppManager.LoadExtensions();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // custom static hotkeymanager for communicating with tools in extensions
            // returns a bool to indicate if the hotkey event was handled or not
            return HotKeyManager.FireHotKeyEvent(ref msg, keyData) || base.ProcessCmdKey(ref msg, keyData);
        }
    }
}