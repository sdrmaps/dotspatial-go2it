using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using DotSpatial.Controls;
using DotSpatial.SDR.Controls;
using Go2It.Properties;
using SdrConfig = SDR.Configuration;

namespace Go2It
{
    public partial class StartUpForm : Form
    {
        private readonly List<ProjectFileInfo> _recentProjectFiles;
        private readonly AppManager _app;

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

        /// <summary>
        /// The list of recent project files
        /// </summary>
        public List<ProjectFileInfo> RecentProjectFiles
        {
            get { return _recentProjectFiles; }
        }

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

        /// <summary>
        /// Creates a new empty project
        /// </summary>
        private void NoProjectLoaded()
        {
            Cursor = Cursors.Default;  // set the default cursor
            DialogResult = DialogResult.OK;
            Close();
        }

        private Map CreateLoadMap()
        {
            // basically this map loads all the layers that exists on all tabs using default dotspatial
            // load routines, we then create our map tabs seperately and populate as needed from this map
            var map = new Map();
            var mapframe = new EventMapFrame();

            // suspend all events associated with load map (do not fire ANY events EVER, EVAR, EVA!!)
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
                    // TODO: do we need this here?
                    _app.Map = CreateLoadMap();  // a base level map used to load all layers
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
                    if (!existingRecentFiles.Contains(recentFile)) // add to list only if it does not exist
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
