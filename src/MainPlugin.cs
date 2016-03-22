using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.SDR.Controls;
using Go2It.Properties;
using SdrConfig = SDR.Configuration;

namespace Go2It
{
    public class MainPlugin : Extension, IPartImportsSatisfiedNotification
    {
        [Import("Shell")]
        internal ContainerControl Shell { get; set; }

        /// <summary>
        /// Gets or sets the ProjectManager (SDR replacement for SerializationManager)
        /// </summary>
        private ProjectManager ProjectManager { get; set; }

        private StartUpForm _startUpForm;
        private CoordinateDisplay _latLongDisplay;
        private MapTipsPopup _mapTipsPopup;
        // TODO: add the selections display message back to application
        // private SelectionsDisplay _selectionsDisplay;

        public override void Activate()
        {
            App.DockManager.ActivePanelChanged += DockManager_ActivePanelChanged;
            App.DockManager.PanelHidden += DockManagerOnPanelHidden;

            // setup new project save and open project serialization events
            ProjectManager = (ProjectManager) App.SerializationManager;
            ProjectManager.Deserializing += ProjectManagerOnDeserializing;
            ProjectManager.Serializing += ProjectManagerOnSerializing;

            // activate all available extensions now
            App.ExtensionsActivated += App_ExtensionsActivated;
           
            // TODO: create a selection status display panel
            // _selectionsDisplay = new SelectionsDisplay(App);
            
            // create a new lat/long display panel
            _latLongDisplay = new CoordinateDisplay(App);
            _mapTipsPopup = new MapTipsPopup(App);
            // make sure the admin mode is set to false on startup
            SdrConfig.User.Go2ItUserSettings.Instance.AdminModeActive = false;
            base.Activate();
        }

        private void ProjectManagerOnSerializing(object sender, SerializingEventArgs serializingEventArgs)
        {
            // set shell window title
            Shell.Text = string.Format("{0} - {1}", Resources.AppName, ProjectManager.GetProjectShortName());

            if (App.Map.Projection != null)
            {
                _latLongDisplay.MapProjectionString = App.Map.Projection.ToEsriString();
            }
        }

        private void ProjectManagerOnDeserializing(object sender, SerializingEventArgs serializingEventArgs)
        {
            // set shell window title
            Shell.Text = string.Format("{0} - {1}", Resources.AppName, ProjectManager.GetProjectShortName());

            if (App.Map.Projection != null)
            {
                _latLongDisplay.MapProjectionString = App.Map.Projection.ToEsriString();
            }
        }

        void App_ExtensionsActivated(object sender, EventArgs e)
        {
            App.RefreshExtensions();
        }

        public override void Deactivate()
        {
            // disconnect all event binding on deactivation 
            App.ExtensionsActivated -= App_ExtensionsActivated;
            App.DockManager.PanelHidden -= DockManagerOnPanelHidden;
            App.DockManager.ActivePanelChanged -= DockManager_ActivePanelChanged;
            ProjectManager.Deserializing -= ProjectManagerOnDeserializing;
            ProjectManager.Serializing -= ProjectManagerOnSerializing;
            base.Deactivate();
        }

        #region IPartImportsSatisfiedNotification Members

        public void OnImportsSatisfied()
        {
            var mainForm = Shell as Form;
            if (mainForm != null)
            {
                // show the main form
                mainForm.Shown += mainForm_Shown;
            }
        }

        void mainForm_Shown(object sender, EventArgs e)
        {
            // displays the initial startup dialog for projects
            if (string.IsNullOrEmpty(ProjectManager.CurrentProjectFile))
            {
                ShowStartupScreen();
            }
            else
            {
                try
                {
                    // project is currently being loaded (from double click or command line) - do not show the startup form
                    ProjectManagerOnDeserializing(null, null);
                }
                catch (IOException)
                {
                    MessageBox.Show(String.Format(Resources.CouldNotOpenMapFile, ProjectManager.CurrentProjectFile), Resources.CouldNotOpenMapFile,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (XmlException)
                {
                    MessageBox.Show(String.Format(Resources.CouldNotReadMapFile, ProjectManager.CurrentProjectFile), Resources.CouldNotReadMapFile,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (ArgumentException)
                {
                    MessageBox.Show(String.Format(Resources.CouldNotReadAPortionMapFile, ProjectManager.CurrentProjectFile), Resources.CouldNotReadAPortionMapFile,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion


        private void ShowStartupScreen()
        {
            _startUpForm = new StartUpForm(App)
            {
                StartPosition = FormStartPosition.CenterScreen,
                Owner = Shell as Form,
                TopMost = true
            };
            _startUpForm.FormClosing += startUpForm_FormClosing;

            int x = Shell.Location.X + Shell.Width / 2 - _startUpForm.Width / 2;
            int y = Shell.Location.Y + Shell.Height / 2 - _startUpForm.Height / 2;
            _startUpForm.Location = new System.Drawing.Point(x, y);

            App.CompositionContainer.ComposeParts(_startUpForm);

            _startUpForm.Show(Shell);
            _startUpForm.Focus();
        }

        void startUpForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // display the lat/long status panel
            _latLongDisplay.ShowCoordinates = true;
            _mapTipsPopup.ShowMapTips = true;
            // TODO: display the selection status panel
            // _selectionsDisplay.ShowSelectionStatus = false;
            Shell.Focus();  // set focus to the main application window
        }

        private void DockManagerOnPanelHidden(object sender, DockablePanelEventArgs e)
        {
            var dockControl = (DockingControl)sender;
            var key = e.ActivePanelKey;
            DockPanelInfo dockInfo;
            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;

            if (key.StartsWith("kMap_"))
            {
                var map = (Map) dockInfo.DotSpatialDockPanel.InnerControl;
                if (map == null) return;
                // remove map functionmode change binding
                map.FunctionModeChanged -= MapOnFunctionModeChanged;
                // update the view changes/events of the mapframe
                var mapFrame = map.MapFrame as EventMapFrame;
                if (mapFrame == null) return;
                // check if viewextentchanges are active and suspend them if so
                if (mapFrame.ViewExtentChangedSuspended) return;
                mapFrame.SuspendViewExtentChanged();
                mapFrame.SuspendEvents();
            }
            else if (key.StartsWith("kPanel_"))
            {
                if (SdrConfig.User.Go2ItUserSettings.Instance.AdminModeActive)
                {
                    dockControl.CollapseToolPanel();
                }
            }
        }

        void DockManager_ActivePanelChanged(object sender, DockablePanelEventArgs e)
        {
            var dockControl = (DockingControl)sender;
            var key = e.ActivePanelKey;
            DockPanelInfo dockInfo;
            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;

            if (key.StartsWith("kMap_"))
            {
                var map = (Map)dockInfo.DotSpatialDockPanel.InnerControl;
                if (map == null) return;

                // update the new active map key and caption to settings
                SdrConfig.Project.Go2ItProjectSettings.Instance.ActiveMapViewKey = key;
                SdrConfig.Project.Go2ItProjectSettings.Instance.ActiveMapViewCaption = dockInfo.DotSpatialDockPanel.Caption;

                // check the current map function mode and set it to the new map if it exists
                if (App.Map != null)
                {
                    map.FunctionMode = App.Map.FunctionMode;
                }
                else // no current map (this is a new map), instead load the active functionmode from usersettings/default
                {
                    var funcMode = SdrConfig.User.Go2ItUserSettings.Instance.ActiveFunctionMode;
                    FunctionMode fm;
                    Enum.TryParse(funcMode, true, out fm);
                    map.FunctionMode = fm;
                }
                // assign the functionmode changed event binding
                map.FunctionModeChanged += MapOnFunctionModeChanged;

                // update the view changes/events of the mapframe
                var mapFrame = map.MapFrame as EventMapFrame;
                if (mapFrame != null)
                {
                    // check if viewextentchanges are suspended and activate them if so
                    if (mapFrame.ViewExtentChangedSuspended)
                    {
                        mapFrame.ResumeViewExtentChanged();
                        mapFrame.ResumeEvents();
                    }
                }
                App.Map = map;  // finally, set the active map tab to the active application map
                App.Map.Invalidate();  // refresh the new active map
            }
            else if (key.StartsWith("kPanel_"))
            {
                if (SdrConfig.User.Go2ItUserSettings.Instance.AdminModeActive)
                {
                    dockControl.CollapseToolPanel();
                }
                else
                {
                    if (App.Map == null) return;
                    // update the active function panel setting and extend/display the panel if needed
                    SdrConfig.User.Go2ItUserSettings.Instance.ActiveFunctionPanel = key;
                    if (App.Map.FunctionMode == FunctionMode.None)  // account for built in tools such as pan, zoomin, etc.
                    {
                        dockControl.ExtendToolPanel(dockInfo.Height);
                    }
                }
            }
        }

        private void MapOnFunctionModeChanged(object sender, EventArgs eventArgs)
        {
            // save the active functionmode state (used to persist active functionmode between map panels)
            var map = sender as Map;
            if (map != null)
            {
                if (map.FunctionMode != FunctionMode.None)
                {
                    var dockControl = (DockingControl) App.DockManager;
                    dockControl.CollapseToolPanel();
                }
                SdrConfig.User.Go2ItUserSettings.Instance.ActiveFunctionMode = map.FunctionMode.ToString();
            }
        }
    }
}
