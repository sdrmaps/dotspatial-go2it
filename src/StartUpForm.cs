using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using DotSpatial.Controls;
using Go2It.Properties;
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
                CreateEmptyProject();
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
            CreateEmptyProject();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new empty project
        /// </summary>
        private void CreateEmptyProject()
        {
            _app.SerializationManager.New();
            // set the default cursor
            Cursor = Cursors.Default;
            DialogResult = DialogResult.OK;
            Close();
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
