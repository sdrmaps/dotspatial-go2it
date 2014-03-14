using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using DotSpatial.SDR.Controls;
using Go2It.Properties;
using SdrConfig = SDR.Configuration;

namespace Go2It
{
    public class MenuBarControl : Extension
    {
        [Import("Shell")]
        internal ContainerControl Shell { get; set; }

        private LoginForm _loginForm;
        private AdminForm _adminForm;
        private AppManager _appManager;

        private bool _adminUser;
        private readonly List<PermissionedActionItem> _listPermissionedItems = new List<PermissionedActionItem>();

        #region Constants and Fields

        private const string FileMenuKey = "kMenu_File";
        private const string AdminMenuKey = "kMenu_Admin";
        
        #endregion

        #region Public Methods
        public override void Activate()
        {
            _appManager = App;
            // get the grid header control
            IHeaderControl header = App.HeaderControl;
            // add the file menu
            header.Add(new RootItem(FileMenuKey, "File") { SortOrder = -20 });
            header.Add(new SimpleActionItem(FileMenuKey, "Open Project", OpenProject_Click) { GroupCaption = DotSpatial.Controls.Header.HeaderControl.ApplicationMenuKey, SortOrder = 10, SmallImage = Resources.open_file_16, LargeImage = Resources.open_file_32 });
            // header.Add(new SimpleActionItem(FileMenuKey, Msg.File_Print, PrintLayout_Click) { GroupCaption = DotSpatial.Controls.Header.HeaderControl.ApplicationMenuKey, SortOrder = 40, SmallImage = Resources.printer_16, LargeImage = Resources.printer_32 });
            header.Add(new SimpleActionItem(FileMenuKey, "New Project", NewLayout_Click) { GroupCaption = DotSpatial.Controls.Header.HeaderControl.ApplicationMenuKey, SortOrder = 200, SmallImage = Resources.filenew_16, LargeImage = Resources.filenew_32 });
            header.Add(new SimpleActionItem(FileMenuKey, "Map Info", Info_Click) { GroupCaption = DotSpatial.Controls.Header.HeaderControl.ApplicationMenuKey, SortOrder = 2000, });
            header.Add(new SimpleActionItem(FileMenuKey, "Exit", Exit_Click) { GroupCaption = DotSpatial.Controls.Header.HeaderControl.ApplicationMenuKey, SortOrder = 5000, });
            // add the admin menu
            header.Add(new RootItem(AdminMenuKey, "Administration") {SortOrder = -10});
            _listPermissionedItems.Add(new PermissionedActionItem(AdminMenuKey, "Log In", !_adminUser, Log_In_Click) { GroupCaption = DotSpatial.Controls.Header.HeaderControl.ApplicationMenuKey, SmallImage = Resources.login_16, LargeImage = Resources.login_32 });
            _listPermissionedItems.Add(new PermissionedActionItem(AdminMenuKey, "Log Out", _adminUser, Log_Out_Click) { GroupCaption = DotSpatial.Controls.Header.HeaderControl.ApplicationMenuKey, SmallImage = Resources.logout_16, LargeImage = Resources.logout_32 });
            _listPermissionedItems.Add(new PermissionedActionItem(AdminMenuKey, "Configuration", _adminUser, Configure_Click) );
            foreach (PermissionedActionItem item in _listPermissionedItems)
            {
                header.Add(item);
            }
            // App.DockManager.ActivePanelChanged += DockManager_ActivePanelChanged;
            base.Activate();
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            base.Deactivate();
        }
        #endregion

        #region Methods
        private void ToggleLogin()
        {
            foreach (PermissionedActionItem item in _listPermissionedItems)
            {
                if (item.Caption == "Log In")
                {
                    item.Logout();
                } else
                {
                    item.Login();
                }
            }
        }

        private void ToggleLogout()
        {
            foreach (PermissionedActionItem item in _listPermissionedItems)
            {
                if (item.Caption == "Log In")
                {
                    item.Login();
                }
                else
                {
                    item.Logout();
                }
            }
        }

        private void Configure_Click(object sender, EventArgs e)
        {
            ShowAdminForm();
        }

        private void Log_In_Click(object sender, EventArgs e)
        {
            _loginForm = new LoginForm
            {
                StartPosition = FormStartPosition.CenterScreen,
                Owner = Shell as Form,
                TopMost = true
            };
            _loginForm.FormLogin += loginForm_Login;
            _loginForm.FormLogout += loginForm_Logout;

            int x = Shell.Location.X + Shell.Width / 2 - _loginForm.Width / 2;
            int y = Shell.Location.Y + Shell.Height / 2 - _loginForm.Height / 2;
            _loginForm.Location = new System.Drawing.Point(x, y);

            App.CompositionContainer.ComposeParts(_loginForm);

            _loginForm.Show(Shell);
            _loginForm.Focus();
        }

        private void loginForm_Login(object sender, EventArgs e)
        {
            _adminUser = true;
            ToggleLogin();
            ShowAdminForm();
        }

        private void ShowAdminForm()
        {
            _adminForm = new AdminForm(_appManager)
            {
                StartPosition = FormStartPosition.CenterScreen,
                Owner = Shell as Form
            };
            _adminForm.FormClosing += adminForm_FormClosing;

            int x = Shell.Location.X + Shell.Width / 2 - _adminForm.Width / 2;
            int y = Shell.Location.Y + Shell.Height / 2 - _adminForm.Height / 2;
            _adminForm.Location = new System.Drawing.Point(x, y);

            App.CompositionContainer.ComposeParts(_adminForm);

            _adminForm.Show(Shell);
            _adminForm.Focus();
        }

        void adminForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!e.Cancel)
            {
                // form is closing so set the focus on the main application, otherwise ignore
                Shell.Focus();
            }
        }

        private void loginForm_Logout(object sender, EventArgs e)
        {
            // in this case the user has chosen to hit cancel, as such do nothing
        }

        private void Log_Out_Click(object sender, EventArgs e)
        {
            _adminUser = false;
            ToggleLogout();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Info_Click(object sender, EventArgs e)
        {
            // AppManager Map
            Debug.WriteLine("_appManager.Map.Bounds: " + _appManager.Map.Bounds);
            Debug.WriteLine("_appManager.Map.Extent: " + _appManager.Map.Extent);
            Debug.WriteLine("_appManager.Map.GetMaxExtent: " + _appManager.Map.GetMaxExtent());
            Debug.WriteLine("_appManager.Map.Projection.ToProj4String: " + _appManager.Map.Projection.ToProj4String());
            Debug.WriteLine("_appManager.Map.Projection: " + _appManager.Map.Projection);
            Debug.WriteLine("_appManager.Map.ViewExtents: " + _appManager.Map.ViewExtents);
            // AppManager MapFrame
            Debug.WriteLine("_appManager.Map.MapFrame.Extent: " + _appManager.Map.MapFrame.Extent);
            Debug.WriteLine("_appManager.Map.MapFrame.GeographicExtents: " + _appManager.Map.MapFrame.GeographicExtents);
            Debug.WriteLine("_appManager.Map.MapFrame.Projection.ToProj4String: " + _appManager.Map.MapFrame.Projection.ToProj4String());
            Debug.WriteLine("_appManager.Map.MapFrame.Projection: " + _appManager.Map.MapFrame.Projection);
            Debug.WriteLine("_appManager.Map.MapFrame.ProjectionString: " + _appManager.Map.MapFrame.ProjectionString);
            Debug.WriteLine("_appManager.Map.MapFrame.View: " + _appManager.Map.MapFrame.View);
            Debug.WriteLine("_appManager.Map.MapFrame.ViewExtents: " + _appManager.Map.MapFrame.ViewExtents);
        }

        private void OpenProject_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = App.SerializationManager.OpenDialogFilterText;
                if (dlg.ShowDialog() != DialogResult.OK)
                    return;
                try
                {
                    App.SerializationManager.OpenProject(dlg.FileName);
                    // App.Map.Invalidate();
                }
                catch (IOException)
                {
                    MessageBox.Show(String.Format(Resources.CouldNotOpenMapFile, dlg.FileName), Resources.CouldNotOpenMapFile,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (XmlException)
                {
                    MessageBox.Show(String.Format(Resources.CouldNotReadMapFile, dlg.FileName), Resources.CouldNotReadMapFile,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (ArgumentException)
                {
                    MessageBox.Show(String.Format(Resources.CouldNotReadAPortionMapFile, dlg.FileName), Resources.CouldNotReadAPortionMapFile,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the PrintLayout control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void PrintLayout_Click(object sender, EventArgs e)
        {
            using (var layout = new LayoutForm())
            {
                layout.MapControl = App.Map as Map;
                layout.ShowDialog();
            }
        }

        private void NewLayout_Click(object sender, EventArgs e)
        {
            App.SerializationManager.New();
        }
        #endregion
    }
}
