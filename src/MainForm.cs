using System.ComponentModel.Composition;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.SDR.Controls;
using Lucene.Net.Search;
using SDR.Common;
using SDR.Common.logging;
using SdrConfig = SDR.Configuration;

namespace Go2It
{
    /// <summary>
    /// Main form to which all mapping controls are assigned
    /// </summary>
    public partial class MainForm : Form
    {
        // the main form is exported so that the IHeaderControl plug-in can add the menu or the
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
            // load any custom set hotkeys from the app db | else hotkeys load from assemblies in extension load
            // TODO: Add back in HotKey Manager
            // HotKeyManager.LoadHotKeys();
            // create our application manager
            AppManager = new AppManager();

            _shell = this;
            // load any extensions/plugins now
            AppManager.LoadExtensions();
            // TODO: do we want a startup message
            // AppManager.ProgressHandler.Progress("", 0, "Handy Startup Message!");
        }

        // TODO: Add HotKey Manager Events back
        /*protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // static hotkeymanager for communicating with all tools and controls loaded via extensions
            // returns a bool to indicate if the hotkey event was handled or not
            return HotKeyManager.FireHotKeyEvent(ref msg, keyData) || base.ProcessCmdKey(ref msg, keyData);
        }*/
    }
}