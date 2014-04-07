using System;
using System.ComponentModel.Composition;
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

        private StartUpForm _startUpForm;
        private ProjectManager _projManager;
        private CoordinateDisplay _latLongDisplay;
        private SelectionStatusDisplay _selectionStatusDisplay;

        public override void Activate()
        {
            App.DockManager.ActivePanelChanged += DockManager_ActivePanelChanged;
            App.DockManager.PanelHidden += DockManagerOnPanelHidden;

            // set the application wide project manager now
            _projManager = new ProjectManager(App);

            // setup new project save and open project serialization events
            App.SerializationManager.NewProjectCreated += SerializationManagerOnNewProjectCreated;
            App.SerializationManager.Deserializing += SerializationManager_Deserializing;
            App.SerializationManager.Serializing += SerializationManager_Serializing;

            // activate all available extensions now
            App.ExtensionsActivated += App_ExtensionsActivated;
           
            // create a selection status display panel
            _selectionStatusDisplay = new SelectionStatusDisplay(App);

            // create a new lat/long display panel
            _latLongDisplay = new CoordinateDisplay(App);

            base.Activate();
        }

        void SerializationManager_Serializing(object sender, SerializingEventArgs e)
        {
            // save the project 
            _projManager.SavingProject();

            Shell.Text = string.Format("{0} - {1}", Resources.AppName, GetProjectShortName());
            if (App.Map.Projection != null)
            {
                _latLongDisplay.MapProjectionString = App.Map.Projection.ToEsriString();
            }
        }

        private void SerializationManagerOnNewProjectCreated(object sender, SerializingEventArgs serializingEventArgs)
        {
            _projManager.CreateEmptyProject();
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
            App.SerializationManager.NewProjectCreated -= SerializationManagerOnNewProjectCreated;
            App.SerializationManager.Deserializing -= SerializationManager_Deserializing;
            App.SerializationManager.Serializing -= SerializationManager_Serializing;
            base.Deactivate();
        }

        #region IPartImportsSatisfiedNotification Members

        public void OnImportsSatisfied()
        {
            var mainForm = Shell as Form;
            if (mainForm != null)
            {
                mainForm.Shown += mainForm_Shown;
            }
        }

        void mainForm_Shown(object sender, EventArgs e)
        {
            // displays the initial startup dialog for projects
            if (string.IsNullOrEmpty(App.SerializationManager.CurrentProjectFile))
            {
                ShowStartupScreen();
            }
            else
            {
                try
                {
                    // project is being opened currently do not show startup form
                    SerializationManager_Deserializing(null, null);
                }
                catch (IOException)
                {
                    MessageBox.Show(String.Format(Resources.CouldNotOpenMapFile, App.SerializationManager.CurrentProjectFile), Resources.CouldNotOpenMapFile,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (XmlException)
                {
                    MessageBox.Show(String.Format(Resources.CouldNotReadMapFile, App.SerializationManager.CurrentProjectFile), Resources.CouldNotReadMapFile,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (ArgumentException)
                {
                    MessageBox.Show(String.Format(Resources.CouldNotReadAPortionMapFile, App.SerializationManager.CurrentProjectFile), Resources.CouldNotReadAPortionMapFile,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        private string GetProjectShortName()
        {
            return Path.GetFileName(App.SerializationManager.CurrentProjectFile);
        }

        void SerializationManager_Deserializing(object sender, SerializingEventArgs e)
        {
            // open up the project and assign all attributes and properties
            _projManager.OpeningProject();

            Shell.Text = string.Format("{0} - {1}", Resources.AppName, GetProjectShortName());
            if (App.Map.Projection != null)
            {
                _latLongDisplay.MapProjectionString = App.Map.Projection.ToEsriString();
            }
        }

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
            // display the selection status panel
            _selectionStatusDisplay.ShowSelectionStatus = false;  // sort of pointless
            // set focus to the main application window
            Shell.Focus();
        }

        private void DockManagerOnPanelHidden(object sender, DockablePanelEventArgs e)
        {
            var dockControl = (DockingControl)sender;
            var key = e.ActivePanelKey;

            DockPanelInfo dockInfo;
            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;

            // check if this is a maptab being deselected
            if (key.StartsWith("kMap_"))
            {
                var map = (Map) dockInfo.DotSpatialDockPanel.InnerControl;
                // update the events of the mapframe
                var mapFrame = map.MapFrame as EventMapFrame;
                if (mapFrame != null)
                {
                    if (!mapFrame.ViewExtentChangedSuspended)
                    {
                        // suspend any view changes while not active tab
                        mapFrame.SuspendViewExtentChanged();
                        mapFrame.SuspendEvents();
                    }
                }
            }
            else if (key.StartsWith("kPanel_"))
            {
                if (key == "kPanel_Clear") return;  // controls the panel collapse
                dockControl.CollapseToolPanel();
            }
        }

        void DockManager_ActivePanelChanged(object sender, DockablePanelEventArgs e)
        {
            var dockControl = (DockingControl)sender;
            var key = e.ActivePanelKey;
            DockPanelInfo dockInfo;
            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;

            // if this is a map tab we need to set map active now
            if (key.StartsWith("kMap_"))
            {
                // grab the new active map and key
                var map = (Map) dockInfo.DotSpatialDockPanel.InnerControl;
                var caption = dockInfo.DotSpatialDockPanel.Caption;
                // set them as the active map key and caption
                SdrConfig.Project.Go2ItProjectSettings.Instance.ActiveMapViewKey = key;
                SdrConfig.Project.Go2ItProjectSettings.Instance.ActiveMapViewCaption = caption;
                // set the active function mode from previous map tab
                map.FunctionMode = App.Map.FunctionMode;
                // update the events of the mapframe
                var mapFrame = map.MapFrame as EventMapFrame;
                if (mapFrame != null)
                {
                    // activate view extent changes event for mapframe
                    mapFrame.ResumeViewExtentChanged();
                    mapFrame.ResumeEvents();
                }
                // set the active map tab to the active application map now
                App.Map = map;
                App.Map.Invalidate(); // force a refresh of the map
            }
            else if (key.StartsWith("kPanel_"))
            {
                if (key == "kPanel_Clear") return;  // controls the panel collapse
                dockControl.ExtendToolPanel();
            }
        }
    }
}
