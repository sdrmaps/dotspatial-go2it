using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Data;
using DotSpatial.Data.Properties;
using DotSpatial.SDR.Controls;
using DotSpatial.Symbology;
using Go2It.Properties;
using SDR.Common;
using ILog = SDR.Common.logging.ILog;
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
        // private SelectionsDisplay _selectionsDisplay;

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
           
            // TODO: add back in
            // create a selection status display panel
            // _selectionsDisplay = new SelectionsDisplay(App);
            
            // create a new lat/long display panel`
            _latLongDisplay = new CoordinateDisplay(App);

            base.Activate();
        }

        void SerializationManager_Serializing(object sender, SerializingEventArgs e)
        {
            // save the project 
            _projManager.SavingProject();
            // set shell window title
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
                // show the main form
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
            // set shell window title
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
            // TODO: add this all back in
            // display the lat/long status panel
            _latLongDisplay.ShowCoordinates = true;
            // display the selection status panel
            // _selectionsDisplay.ShowSelectionStatus = false;  // sort of pointless
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
                // remove the event binding on this map for functionmode changes
                if (map != null)  // if there is a map then remove any binding
                {
                    map.FunctionModeChanged -= MapOnFunctionModeChanged;
                }
                // update the events of the mapframe
                var mapFrame = map.MapFrame as EventMapFrame;
                if (mapFrame != null)
                {
                    // check if viewextentchanges are active, suspend them if so
                    if (!mapFrame.ViewExtentChangedSuspended)
                    {
                        // suspend any view changes while not the active tab
                        mapFrame.SuspendViewExtentChanged();
                        mapFrame.SuspendEvents();
                    }
                }
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

            // if this is a map tab we need to set map active now and also assign events for watching mapfunctionmode
            if (key.StartsWith("kMap_"))
            {
                // grab the new active map and key
                var map = (Map) dockInfo.DotSpatialDockPanel.InnerControl;
                var caption = dockInfo.DotSpatialDockPanel.Caption;
                // set them as the active map key and caption
                SdrConfig.Project.Go2ItProjectSettings.Instance.ActiveMapViewKey = key;
                SdrConfig.Project.Go2ItProjectSettings.Instance.ActiveMapViewCaption = caption;
                
                // check if there was a previously set maptab
                if (App.Map != null)
                {
                    map.FunctionMode = App.Map.FunctionMode;
                }
                else // this is a new map, load the active functionmode from user settings or default
                {
                    var funcMode = SdrConfig.User.Go2ItUserSettings.Instance.ActiveFunctionMode;
                    FunctionMode fm;
                    Enum.TryParse(funcMode, true, out fm);
                    map.FunctionMode = fm;
                }
                map.FunctionModeChanged += MapOnFunctionModeChanged;
                // update the events of the mapframe
                var mapFrame = map.MapFrame as EventMapFrame;
                if (mapFrame != null)
                {
                    // make sure that the viewextentchange event is set to active
                    if (mapFrame.ViewExtentChangedSuspended)
                    {
                        mapFrame.ResumeViewExtentChanged();
                        mapFrame.ResumeEvents();
                    }
                }
                // set the active map tab to the active application map now
                App.Map = map;
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
                    // update the active function panel being displayed and show the panel
                    SdrConfig.User.Go2ItUserSettings.Instance.ActiveFunctionPanel = key;
                    // extend the panel the appropriate height for display
                    dockControl.ExtendToolPanel(dockInfo.Height);
                }
            }
        }

        private void MapOnFunctionModeChanged(object sender, EventArgs eventArgs)
        {
            // update the user settings to reflect active functionmode
            var map = sender as Map;
            if (map != null)
            {
                SdrConfig.User.Go2ItUserSettings.Instance.ActiveFunctionMode = map.FunctionMode.ToString();
            }
        }
    }
}
