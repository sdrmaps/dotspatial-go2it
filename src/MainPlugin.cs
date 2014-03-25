using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
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
        // private CoordinateDisplay _latLongDisplay;
        // private SelectionStatusDisplay selectionDisplay;

        public override void Activate()
        {
            App.DockManager.ActivePanelChanged += DockManager_ActivePanelChanged;
            // set the application wide project manager now
            _projManager = new ProjectManager(App);
            // setup new project save and open project serialization events
            App.SerializationManager.NewProjectCreated += SerializationManagerOnNewProjectCreated;
            App.SerializationManager.Deserializing += SerializationManager_Deserializing;
            App.SerializationManager.Serializing += SerializationManager_Serializing;

            // activate all available extensions now
            App.ExtensionsActivated += App_ExtensionsActivated;
           
            // show selection status display
            // selectionDisplay = new SelectionStatusDisplay(App);

            //show latitude, longitude coordinate display
            // _latLongDisplay = new CoordinateDisplay(App);
            base.Activate();
        }

        void SerializationManager_Serializing(object sender, SerializingEventArgs e)
        {
            // save the project 
            _projManager.SavingProject();

            Shell.Text = string.Format("{0} - {1}", Resources.AppName, GetProjectShortName());
            if (App.Map.Projection != null)
            {
                // _latLongDisplay.MapProjectionString = App.Map.Projection.ToEsriString();
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
                // _latLongDisplay.MapProjectionString = App.Map.Projection.ToEsriString();
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
            // check the map for a coordinate/projection system
            //setup the lat, long coordinate display
            // _latLongDisplay.ShowCoordinates = true;
            // set focus to the main application window
            Shell.Focus();
        }

        void DockManager_ActivePanelChanged(object sender, DockablePanelEventArgs e)
        {
            var dockControl = (DockingControl)sender;
            var key = e.ActivePanelKey;
            DockPanelInfo dockInfo;
            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;
            // if this is a map tab we need to set map active now
            if (!dockInfo.DotSpatialDockPanel.Key.StartsWith("kMap_")) return;
            // store the new active map panel caption and key
            var map = (Map)dockInfo.DotSpatialDockPanel.InnerControl;
            var caption = dockInfo.DotSpatialDockPanel.Caption;

            MapFrame mf = map.MapFrame as MapFrame;
            var k = mf.CanZoomToPrevious();


            // save the current active map tab view to settings
            SdrConfig.Project.Go2ItProjectSettings.Instance.ActiveMapViewKey = key;
            SdrConfig.Project.Go2ItProjectSettings.Instance.ActiveMapViewCaption = caption;
            // set the active function mode from previous map tab
            map.FunctionMode = App.Map.FunctionMode;
            // set the active map tab to the active application map now
            App.Map = map;            
            App.Map.Invalidate(); // force a refresh of the map

            MapFrame amf = App.Map.MapFrame as MapFrame;
            var l = amf.CanZoomToPrevious();
        }

        /* void HeaderControl_RootItemSelected(object sender, RootItemEventArgs e)
        {
            Boolean showCoordinates = false;

            if (e.SelectedRootKey == SharedConstants.SearchRootkey || e.SelectedRootKey == HeaderControl.HomeRootItemKey)
            {
                App.SerializationManager.SetCustomSetting("SearchRootClicked", true);
                App.DockManager.SelectPanel("kMap");
                App.DockManager.SelectPanel("kLegend");
                App.DockManager.ShowPanel(SharedConstants.SeriesViewKey);
                showCoordinates = true;
            }
            else if (e.SelectedRootKey == "RootRibbonHydroModeler")
            {
                //hide panels
                App.DockManager.HidePanel("kLegend");
                App.DockManager.HidePanel(HydroDesktop.Common.SharedConstants.SeriesViewKey);
                App.DockManager.SelectPanel("kHydroModelerDock");
            }
            else if (e.SelectedRootKey == "kHydroGraph_01" || e.SelectedRootKey == SharedConstants.TableRootKey || e.SelectedRootKey == "kHydroEditView" || e.SelectedRootKey == "kHydroR")
            {
                App.DockManager.SelectPanel(HydroDesktop.Common.SharedConstants.SeriesViewKey);
                App.DockManager.ShowPanel("kLegend");
            }

            if (e.SelectedRootKey == "kHydroSearchV3")
                showCoordinates = true;
            else
                App.SerializationManager.SetCustomSetting("SearchRootClicked", false);

            if (latLongDisplay != null)
                latLongDisplay.ShowCoordinates = showCoordinates;
        }*/
    }
}
