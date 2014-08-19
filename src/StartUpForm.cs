using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using DotSpatial.Controls;
using DotSpatial.SDR.Controls;
using Go2It.Properties;
using SDR.Common;
using SDR.Common.logging;
using SdrConfig = SDR.Configuration;

namespace Go2It
{
    public partial class StartUpForm : Form
    {
        #region Private Variables

        private readonly List<ProjectFileInfo> _recentProjectFiles;
        private readonly AppManager _app;

        #endregion

        #region Constructor

        public StartUpForm(AppManager app)
        {
            InitializeComponent();
            _app = app;
            _recentProjectFiles = new List<ProjectFileInfo>();
            bsRecentFiles = new BindingSource(RecentProjectFiles, null);
            lstRecentProjects.DataSource = bsRecentFiles;
            lstRecentProjects.DoubleClick += lstRecentProjects_DoubleClick;
            FormClosing += Form_Closing;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The list of recent project files
        /// </summary>
        public List<ProjectFileInfo> RecentProjectFiles
        {
            get { return _recentProjectFiles; }
        }

        #endregion

        #region Events

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (rbOpenExistingProject.Checked)
            {
                OpenProject();
            }
            else
            {
                NoProjectLoaded();
            }
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void StartUpForm_Load(object sender, EventArgs e)
        {
            FindRecentProjectFiles();
        }

        private void btnBrowseProject_Click(object sender, EventArgs e)
        {
            rbOpenExistingProject.Checked = true;
            var fileDialog = new OpenFileDialog
            {
                Filter = @"Go2It Project File|*.dspx",
                Title = @"Select the Project File to Open"
            };
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                OpenExistingProject(fileDialog.FileName);
            }
        }

        private void lstRecentProjects_DoubleClick(object sender, EventArgs e)
        {
            OpenProject();
        }

        private void lstRecentProjects_Click(object sender, EventArgs e)
        {
            rbOpenExistingProject.Checked = true;
        }

        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            // Create an empty project if the x button is clicked
            if (DialogResult == DialogResult.OK) return;
            if (!String.IsNullOrEmpty(_app.SerializationManager.CurrentProjectFile)) return;
            e.Cancel = true;
            NoProjectLoaded();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new empty project
        /// </summary>
        private void NoProjectLoaded()
        {
            Cursor = Cursors.Default;  // set the default cursor
            DialogResult = DialogResult.OK;
            Close();
        }

        private void LogMapEvents(IMap map, string name)
        {
            var log = AppContext.Instance.Get<ILog>();
            map.FinishedRefresh += (sender, args) => log.Info(name + " FinishedRefresh");
            map.FunctionModeChanged += (sender, args) => log.Info(name + " FunctionModeChanged");
            map.LayerAdded += (sender, args) => log.Info(name + " LayerAdded");
            map.SelectionChanged += (sender, args) => log.Info(name + " SelectionChanged");
            map.Resized += (sender, args) => log.Info(name + " Resized");
            map.MapFrame.BufferChanged += (sender, args) => log.Info(name + " MapFrame.BufferChanged");
            map.MapFrame.EnvelopeChanged += (sender, args) => log.Info(name + " MapFrame.EnvelopeChanged");
            map.MapFrame.FinishedLoading += (sender, args) => log.Info(name + " MapFrame.FinishedLoading");
            map.MapFrame.FinishedRefresh += (sender, args) => log.Info(name + " MapFrame.FinishedRefresh");
            map.MapFrame.Invalidated += (sender, args) => log.Info(name + " MapFrame.Invalidated");
            map.MapFrame.ItemChanged += (sender, args) => log.Info(name + " MapFrame.ItemChanged");
            map.MapFrame.LayerAdded += (sender, args) => log.Info(name + " MapFrame.LayerAdded");
            map.MapFrame.LayerRemoved += (sender, args) => log.Info(name + " MapFrame.LayerRemoved");
            map.MapFrame.LayerSelected += (sender, args) => log.Info(name + " MapFrame.LayerSelected");
            map.MapFrame.RemoveItem += (sender, args) => log.Info(name + " MapFrame.RemoveItem");
            map.MapFrame.ScreenUpdated += (sender, args) => log.Info(name + " MapFrame.ScreenUpdated");
            map.MapFrame.SelectionChanged += (sender, args) => log.Info(name + " MapFrame.SelectionChanged");
            map.MapFrame.ShowProperties += (sender, args) => log.Info(name + " MapFrame.ShowProperties");
            map.MapFrame.UpdateMap += (sender, args) => log.Info(name + " MapFrame.UpdateMap");
            map.MapFrame.ViewChanged += (sender, args) => log.Info(name + " MapFrame.ViewChanged");
            map.MapFrame.ViewExtentsChanged += (sender, args) => log.Info(name + " MapFrame.ViewExtentsChanged");
            map.MapFrame.VisibleChanged += (sender, args) => log.Info(name + " MapFrame.VisibleChanged");
        }

        private void LogMapFrameEvents(IMapFrame mapframe, string name)
        {
            var log = AppContext.Instance.Get<ILog>();
            mapframe.BufferChanged += (sender, args) => log.Info(name + " MapFrame.BufferChanged");
            mapframe.EnvelopeChanged += (sender, args) => log.Info(name + " MapFrame.EnvelopeChanged");
            mapframe.FinishedLoading += (sender, args) => log.Info(name + " MapFrame.FinishedLoading");
            mapframe.FinishedRefresh += (sender, args) => log.Info(name + " MapFrame.FinishedRefresh");
            mapframe.Invalidated += (sender, args) => log.Info(name + " MapFrame.Invalidated");
            mapframe.ItemChanged += (sender, args) => log.Info(name + " MapFrame.ItemChanged");
            mapframe.LayerAdded += (sender, args) => log.Info(name + " MapFrame.LayerAdded");
            mapframe.LayerRemoved += (sender, args) => log.Info(name + " MapFrame.LayerRemoved");
            mapframe.LayerSelected += (sender, args) => log.Info(name + " MapFrame.LayerSelected");
            mapframe.RemoveItem += (sender, args) => log.Info(name + " MapFrame.RemoveItem");
            mapframe.ScreenUpdated += (sender, args) => log.Info(name + " MapFrame.ScreenUpdated");
            mapframe.SelectionChanged += (sender, args) => log.Info(name + " MapFrame.SelectionChanged");
            mapframe.ShowProperties += (sender, args) => log.Info(name + " MapFrame.ShowProperties");
            mapframe.UpdateMap += (sender, args) => log.Info(name + " MapFrame.UpdateMap");
            mapframe.ViewChanged += (sender, args) => log.Info(name + " MapFrame.ViewChanged");
            mapframe.ViewExtentsChanged += (sender, args) => log.Info(name + " MapFrame.ViewExtentsChanged");
            mapframe.VisibleChanged += (sender, args) => log.Info(name + " MapFrame.VisibleChanged");
        }


        // todo: is this just a temp map to satisfy load needs (i dont think we even use it)
        private Map CreateLoadMap(String mapName)
        {
            var map = new Map();
            LogMapEvents(map, mapName);
            var mapframe = new EventMapFrame();
            LogMapFrameEvents(mapframe, mapName);

            // suspend all events associated with load map (do not fire tool events ever evar eva)
            mapframe.SuspendChangeEvent();
            mapframe.SuspendEvents();
            mapframe.SuspendViewExtentChanged();

            map.MapFrame = mapframe;
            map.Visible = false;
            map.Dock = DockStyle.Fill;
            map.MapFunctions.Clear();  // remove all map functions from load map
            return map;
        }

        private void OpenProject()
        {
            var selected = lstRecentProjects.SelectedValue as ProjectFileInfo;
            if (selected != null)
            {
                OpenExistingProject(selected.FullPath);
            }
        }

        private void OpenExistingProject(string projectFileName)
        {
            // setup wait cursor while project is opening
            Cursor = Cursors.WaitCursor;
            // open the project up using the default open routines
            try
            {
                if (_app.Map == null)
                {
                    _app.Map = CreateLoadMap("_loadMap");  // a base map used to load all layers
                }
                _app.SerializationManager.OpenProject(projectFileName);
            }
            catch (IOException)
            {
                MessageBox.Show(String.Format(Resources.CouldNotOpenMapFile, projectFileName), Resources.CouldNotOpenMapFile,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (XmlException)
            {
                MessageBox.Show(String.Format(Resources.CouldNotReadMapFile, projectFileName), Resources.CouldNotReadMapFile,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ArgumentException)
            {
                MessageBox.Show(String.Format(Resources.CouldNotReadAPortionMapFile, projectFileName), Resources.CouldNotReadAPortionMapFile,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // set the cursor back to default now
            Cursor = Cursors.Default;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void FindRecentProjectFiles()
        {
            RecentProjectFiles.Clear();
            var existingRecentFiles = new List<string>();
            foreach (string recentFile in SdrConfig.Settings.Instance.RecentProjectFiles)
            {
                if (File.Exists(recentFile))
                {
                    if (!existingRecentFiles.Contains(recentFile)) //add to list only if not exists
                    {
                        existingRecentFiles.Add(recentFile);
                    }
                }
            }
            SdrConfig.Settings.Instance.RecentProjectFiles.Clear();
            foreach (string recentFile in existingRecentFiles)
            {
                SdrConfig.Settings.Instance.RecentProjectFiles.Add(recentFile);
                RecentProjectFiles.Add(new ProjectFileInfo(recentFile));
            }
            bsRecentFiles.ResetBindings(false);
            lstRecentProjects.SelectedIndex = -1;
        }
        #endregion
    }

    public class ProjectFileInfo
    {
        public ProjectFileInfo(string fullPath)
        {
            FullPath = fullPath;
        }

        public string FullPath { get; private set; }

        public string Name
        {
            get
            {
                return Path.GetFileNameWithoutExtension(FullPath);
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ProjectFileInfo);
        }

        public bool Equals(ProjectFileInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || string.Equals(Name, other.Name);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}
