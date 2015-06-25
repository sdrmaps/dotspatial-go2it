using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Data;
using DotSpatial.Data.Properties;
using DotSpatial.Extensions;
using DotSpatial.SDR.Controls;
using DotSpatial.Serialization;
using DotSpatial.Symbology;
using SDR.Configuration.Project;
using SDR.Configuration.User;
using SDR.Data.Database;
using SdrConfig = SDR.Configuration;

namespace Go2It
{
    public class ProjectManager : SerializationManager
    {
        public AppManager App { get; private set; }
        public DockingControl Dock { get; private set; }

        // lookup for all layers that exist on any map tab (populated on "save" and "open")
        public readonly Dictionary<string, IMapLayer> AllLayersLookup = new Dictionary<string, IMapLayer>();

        // const variables for project layer types, used in the project settings dbase
        public const string LayerTypeAddress = "t_address";
        public const string LayerTypeRoad = "t_road";
        public const string LayerTypeNote = "t_note";
        public const string LayerTypeCityLimit = "t_citylimit";
        public const string LayerTypeCellSector = "t_cellsector";
        public const string LayerTypeEsn = "t_esn";
        public const string LayerTypeParcel = "t_parcel";
        public const string LayerTypeHydrant = "t_hydrant";
        public const string LayerTypeKeyLocation = "t_keylocation";

        public ProjectManager(AppManager mainApp) : base(mainApp)
        {
            App = mainApp;
            Dock = (DockingControl) mainApp.DockManager;
        }

        public string GetProjectShortName()
        {
            return Path.GetFileName(base.CurrentProjectFile);
        }

        public Map CreateNewMap(String mapName)
        {
            return CreateNewMap(mapName, Go2ItProjectSettings.Instance.MapBgColor);
        }

        public Map CreateNewMap(String mapName, Color bgColor)
        {
            var map = new Map(); // new map 
            var mapframe = new EventMapFrame(); // evented map frame so we can disable visualextent events
            mapframe.SuspendViewExtentChanged();  // suspend view-extents while map is not active
            // TODO: investigate this further (don't think they are applicable)
            // mapframe.SuspendChangeEvent();
            // mapframe.SuspendEvents();

            map.MapFrame = mapframe;  // set the new evented mapframe to the map mapframe
            map.BackColor = bgColor;
            map.Visible = true;  // set visibility to false if it is the _basemap
            map.Dock = DockStyle.Fill;

            return map;
        }

        /// <summary>
        ///     Creates a new 'empty' project
        /// </summary>
        public void CreateNewProject()
        {
            if (App.Map != null)
            {
                App.Map.ClearLayers();
            }
            // clear any current project directory
            SetCurrentProjectDirectory(String.Empty);
            // reset all project settings to default
            Go2ItProjectSettings.Instance.ResetProjectSettings();

            // reset all dock controller map tabs and layerlookup dict
            if (Dock == null)
            {
                Dock = (DockingControl) App.DockManager;
            }
            else
            {
                Dock.ResetLayout();
            }
            // setup a temp database to hold our settings
            SetupDatabase();

            IsDirty = false;  // and finally set to 'not dirty'
        }

        public void OpenExistingProject(string projectFile)
        {
            if (App.Map != null)
            {
                App.Map.ClearLayers();
            }
            // clear any current project directory
            SetCurrentProjectDirectory(projectFile);
            // reset all project settings to default
            Go2ItProjectSettings.Instance.ResetProjectSettings();

            // reset all dock controller map tabs and layerlookup dict
            if (Dock == null)
            {
                Dock = (DockingControl)App.DockManager;
            }
            else
            {
                Dock.ResetLayout();
            }
            // setup a temp database to hold our settings
            // TODO: assign db lookup configs here
            // SetupDatabase();
            string conString1 = SQLiteHelper.GetSQLiteConnectionString(projectFile);
            SdrConfig.Settings.Instance.ProjectRepoConnectionString = conString1;


            IsDirty = false;  // and finally set to 'not dirty'
        }

        /// <summary>
        /// Filter text for a save project as dialog
        /// </summary>
        public new string SaveDialogFilterText
        {
            get
            {
                return String.Format(SaveDialogFilterFormat, "ProjectFile");
            }
        }

        /// <summary>
        /// Gets the save dialog filter format.
        /// </summary>
        public new string SaveDialogFilterFormat
        {
            get
            {
                return "{0} (*.sqlite)|*.sqlite" + AggregateProviderExtensions(base.SaveProjectFileProviders);
            }
        }

        private static string AggregateProviderExtensions(IEnumerable<IProjectFileProvider> providers)
        {
            return providers.Aggregate(
                new StringBuilder(),
                (sb, p) => sb
                               .AppendFormat("|{1} (*{0})|*{0}", p.Extension, p.FileTypeDescription),
                s => s.ToString());
        }

        private void SetCurrentProjectDirectory(string fileName)
        {
            // we set the working directory to the location of the project file. All filenames will be relative to this path.
            if (String.IsNullOrEmpty(fileName))
            {
                base.CurrentProjectFile = CurrentProjectDirectory = String.Empty;
            }
            else
            {
                Directory.SetCurrentDirectory(Path.GetDirectoryName(fileName));
                base.CurrentProjectFile = fileName;
                base.CurrentProjectDirectory = Path.GetDirectoryName(fileName);
            }
        }

        private string CreateSerializedGraph(Map map)
        {
            var graph = new object[] { map };
            var s = new XmlSerializer();
            string xml = s.Serialize(graph);
            return xml;
        }

        private Map LoadSerializedGraph(string xmlString, string mapName)
        {
            var map = CreateNewMap(mapName);
            var graph = new object[] { map };
            var d = new XmlDeserializer();
            d.Deserialize(graph, xmlString);
            return (Map)graph[0];
        }

        private void CreateProjectDatabase()
        {
            string projectFilePath = App.SerializationManager.CurrentProjectFile;
            string projectFileName = Path.GetFileNameWithoutExtension(projectFilePath);

            // check for a project level database
            string newDbPath = Path.ChangeExtension(projectFilePath, ".sqlite");
            string newDirPath = Path.GetDirectoryName(newDbPath) + "\\" + projectFileName + "_indexes";

            string currentDbPath = SQLiteHelper.GetSQLiteFileName(SdrConfig.Settings.Instance.ProjectRepoConnectionString);
            string d = Path.GetDirectoryName(currentDbPath);
            if (d != null)
            {

                if (newDbPath != currentDbPath)
                {
                    // copy db to new path. If no db exists, create new db in the new location
                    if (SQLiteHelper.DatabaseExists(currentDbPath))
                    {
                        File.Copy(currentDbPath, newDbPath, true);

                        string currentDirPath = Path.Combine(d, "_indexes");
                        // check if there are any index files available copy them if so
                        if (Directory.Exists(currentDirPath))
                        {
                            DirectoryCopy(currentDirPath, newDirPath, true);
                        }
                    }
                    else
                    {
                        CreateNewDatabase(newDbPath);
                    }
                    // update application level database configuration settings
                    SdrConfig.Settings.Instance.ProjectRepoConnectionString =
                        SQLiteHelper.GetSQLiteConnectionString(newDbPath);
                }
            }
        }

        public void SaveProjectSettings()
        {
            // get the project settings db connection string
            string conn = SdrConfig.Settings.Instance.ProjectRepoConnectionString;
            // set  basic project level settings at this point
            var d = new Dictionary<string, string>
            {
                {"addresses_type", Go2ItProjectSettings.Instance.AddressesProjectType},
                {"keylocations_type", Go2ItProjectSettings.Instance.KeyLocationsProjectType},
                {"map_bgcolor", Go2ItProjectSettings.Instance.MapBgColor.ToArgb().ToString(CultureInfo.InvariantCulture)},
                {"active_map_key", Go2ItProjectSettings.Instance.ActiveMapViewKey},
                {"active_map_caption", Go2ItProjectSettings.Instance.ActiveMapViewCaption},
                {"use_pretypes", Go2ItProjectSettings.Instance.UsePretypes.ToString(CultureInfo.InvariantCulture)},
                {"search_zoom_factor", Go2ItProjectSettings.Instance.SearchZoomFactor.ToString(CultureInfo.InvariantCulture)},
                {"search_buffer_distance",Go2ItProjectSettings.Instance.SearchBufferDistance.ToString(CultureInfo.InvariantCulture)},
                {"search_hydrant_count", Go2ItProjectSettings.Instance.HydrantSearchCount.ToString(CultureInfo.InvariantCulture)},
                {"search_hydrant_distance", Go2ItProjectSettings.Instance.HydrantSearchDistance.ToString(CultureInfo.InvariantCulture)}
            };
            // there can only be a single project settings row in the table
            SQLiteHelper.Update(conn, "ProjectSettings", d, "key = 1");
            var g = new Dictionary<string, string>
            {
                {"point_color", Go2ItProjectSettings.Instance.GraphicPointColor.ToArgb().ToString(CultureInfo.InvariantCulture)},
                {"point_style", Go2ItProjectSettings.Instance.GraphicPointStyle},
                {"point_size", Go2ItProjectSettings.Instance.GraphicPointSize.ToString(CultureInfo.InvariantCulture)},
                {"line_color", Go2ItProjectSettings.Instance.GraphicLineColor.ToArgb().ToString(CultureInfo.InvariantCulture)},
                {"line_border_color", Go2ItProjectSettings.Instance.GraphicLineBorderColor.ToArgb().ToString(CultureInfo.InvariantCulture)},
                {"line_size", Go2ItProjectSettings.Instance.GraphicLineSize.ToString(CultureInfo.InvariantCulture)},
                {"line_cap", Go2ItProjectSettings.Instance.GraphicLineCap},
                {"line_style", Go2ItProjectSettings.Instance.GraphicLineStyle}
            };
            // there can only be a single graphics settings row in the table
            SQLiteHelper.Update(conn, "GraphicSettings", g, "key = 1");
            // clear any (base map) layers currently set in the table and reset them now
            SQLiteHelper.ClearTable(conn, "Layers");
            // cycle and save all layer types to settings
            SaveLayerCollection(Go2ItProjectSettings.Instance.AddressLayers, LayerTypeAddress, conn);
            SaveLayerCollection(Go2ItProjectSettings.Instance.RoadLayers, LayerTypeRoad, conn);
            SaveLayerCollection(Go2ItProjectSettings.Instance.NotesLayer, LayerTypeNote, conn);
            SaveLayerCollection(Go2ItProjectSettings.Instance.CityLimitsLayer, LayerTypeCityLimit, conn);
            SaveLayerCollection(Go2ItProjectSettings.Instance.CellSectorsLayer, LayerTypeCellSector, conn);
            SaveLayerCollection(Go2ItProjectSettings.Instance.EsnsLayer, LayerTypeEsn, conn);
            SaveLayerCollection(Go2ItProjectSettings.Instance.ParcelsLayer, LayerTypeParcel, conn);
            SaveLayerCollection(Go2ItProjectSettings.Instance.HydrantsLayer, LayerTypeHydrant, conn);
            SaveLayerCollection(Go2ItProjectSettings.Instance.KeyLocationLayers, LayerTypeKeyLocation, conn);
        }

        /// <summary>
        /// True if there are some unsaved changes in the current project,
        /// False otherwise
        /// </summary>
        public new bool IsDirty
        {
            get { return base.IsDirty;  }
            set { base.IsDirty = value;  }
        }

        private void SaveMapTabs()
        {
            // get the project settings db connection string
            string conn = SdrConfig.Settings.Instance.ProjectRepoConnectionString;
            SQLiteHelper.ClearTable(conn, "MapTabs");
            DockingControl dc = Dock;

            foreach (var dpi in dc.DockPanelLookup)
            {
                if (!dpi.Key.Trim().StartsWith("kMap")) continue;
                var map = (Map)dpi.Value.DotSpatialDockPanel.InnerControl;


                var xmlMap = CreateSerializedGraph(map);

                var mapFrame = map.MapFrame as MapFrame;
                IMapLayerCollection layers = map.Layers; // get the layers
                string txtLayers = string.Empty; // text block to store layers

                foreach (IMapLayer mapLayer in layers)
                {
                    string layName;
                    if (mapLayer.GetType().Name == "MapImageLayer")
                    {
                        var mImage = (IMapImageLayer)mapLayer;
                        layName = Path.GetFileNameWithoutExtension(mImage.Image.Filename);
                    }
                    else
                    {
                        var ftLayer = (FeatureLayer)mapLayer;
                        var mpLayer = (IMapFeatureLayer) mapLayer;
                        layName = ftLayer.Name;
                        // assign this to our layer lookup now
                        var fileName = Path.GetFileNameWithoutExtension(ftLayer.DataSet.Filename);
                        if (fileName != null) AllLayersLookup.Add(fileName, mpLayer);
                    }
                    txtLayers = txtLayers + layName + "|";
                }
                if (txtLayers.Length != 0)
                {
                    txtLayers = txtLayers.Remove(txtLayers.Length - 1, 1);
                }
                // send this all to a dict to save to dbase as a maptab
                if (mapFrame == null) continue;
                var dd = new Dictionary<string, string>
                {
                    {"lookup", dpi.Key},
                    {"caption", dpi.Value.DotSpatialDockPanel.Caption},
                    {"zorder", dpi.Value.SortOrder.ToString(CultureInfo.InvariantCulture)},
                    {"extent", mapFrame.Extent.ToString()},
                    {"viewextent", mapFrame.ViewExtents.ToString()},
                    {"bounds", map.Bounds.ToString()},
                    {"layers", txtLayers},
                    {"map_xml", xmlMap}
                };
                SQLiteHelper.Insert(conn, "MapTabs", dd);
            }
        }

        public new void SaveProject(string fileName)
        {
            Contract.Requires(!String.IsNullOrEmpty(fileName), "fileName is null or empty.");
            Contract.Requires(App.Map != null);

            SetCurrentProjectDirectory(fileName);

            // App.ProgressHandler.Progress("Saving Project " + projectFileName, 0, "");
            // Application.DoEvents();

            CreateProjectDatabase();

            // OnSerializing(new SerializingEventArgs());

            // save all the generic project settings
            
            // serialize each map that has been created for each tab
            SaveMapTabs();
            SaveProjectSettings();

            SdrConfig.Settings.Instance.AddFileToRecentFiles(fileName);

            IsDirty = false;
            //OnIsDirtyChanged();

            // App.ProgressHandler.Progress(String.Empty, 0, String.Empty);
        }

        /// <summary>
        /// Deserializes the map from a file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public new void OpenProject(string fileName)
        {
            Contract.Requires(!String.IsNullOrEmpty(fileName), "fileName is null or empty.");
            // Contract.Requires(App.Map != null);

            if (App.Map != null)
            {
                App.Map.ClearLayers();
            }
            this.SetCurrentProjectDirectory(fileName);

            string extension = Path.GetExtension(fileName);

            bool isProviderPresent = false;
            foreach (var provider in OpenProjectFileProviders)
            {
                if (String.Equals(provider.Extension, extension, StringComparison.OrdinalIgnoreCase))
                {
                    provider.Open(fileName);
                    isProviderPresent = true;
                }
            }

            if (!isProviderPresent)
            {
                OpenProjectFile(fileName);
            }

            
            SdrConfig.Settings.Instance.AddFileToRecentFiles(fileName);
            
            //_changeTracker.Map = _applicationManager.Map;
            IsDirty = false;
            OnIsDirtyChanged();

            OnDeserializing(new SerializingEventArgs());
        }

        private void AssignLayerSymbologies(IMapFrame mapFrame)
        {
            foreach (ILayer layer in mapFrame.GetAllLayers())
            {
                IMapLineLayer lineLayer = layer as IMapLineLayer;
                if (lineLayer != null)
                {
                    ILineScheme original = lineLayer.Symbology;
                    if (original != null)
                    {
                        ILineScheme newScheme = original.Clone() as ILineScheme;
                        original.CopyProperties(newScheme);
                        original.ResumeEvents();
                    }
                }

                //to correctly draw categories:
                IMapFeatureLayer featureLayer = layer as IMapFeatureLayer;
                if (featureLayer != null)
                {
                    if (featureLayer.Symbology.NumCategories > 1)
                    {
                        featureLayer.DataSet.FillAttributes();
                        featureLayer.ApplyScheme(featureLayer.Symbology);
                    }
                }
            }
        }
        private void AssignParentGroups(IGroup parentGroup, IMapFrame parentMapFrame)
        {
            //this method will assign the parent groups.
            //it needs to be applied after opening project so that none of
            //the parent groups are NULL.
            foreach (ILayer child in parentGroup.GetLayers())
            {
                IGroup childGroup = child as IGroup;
                if (childGroup != null)
                {
                    AssignParentGroups(childGroup, parentMapFrame);
                }
                child.SetParentItem(parentGroup);
                child.MapFrame = parentMapFrame;
            }
        }

        private void OpenProjectFile(string fileName)
        {
            OpenExistingProject(fileName);


            string repoConnStr = SQLiteHelper.GetSQLiteConnectionString(fileName);
            SdrConfig.Settings.Instance.ProjectRepoConnectionString = repoConnStr;

            const string tabsQuery = "SELECT * FROM MapTabs";
            DataTable tabsTable = SQLiteHelper.GetDataTable(repoConnStr, tabsQuery);
            // cycle through all map tabs and generate maps and tabs as needed
            foreach (DataRow row in tabsTable.Rows)
            {
                string txtMapXml = row["map_xml"].ToString();
                string txtKey = row["lookup"].ToString();
                string txtCaption = row["caption"].ToString();
                string txtLayers = row["layers"].ToString();
                string txtViewExtent = row["viewextent"].ToString();
                string zorder = row["zorder"].ToString();
                string txtExtent = row["extent"].ToString();
                string txtBounds = row["bounds"].ToString(); // unused

                Map graph = LoadSerializedGraph(txtMapXml, txtKey);


                // create new dockable panel to hold the map
                var dp = new DockablePanel(txtKey, txtCaption, graph, DockStyle.Fill);
               
                
                // add the new tab view to the main form
                Dock.Add(dp);
                // select the map now to activate plugin bindings
                Dock.SelectPanel(txtKey);
                ResetMapProjection();

                //_applicationManager.Map.Invalidate();

                ////temporary fix by Jiri to properly assign the parent groups
                //if (_applicationManager.Map.MapFrame != null)
                //{
                //    AssignParentGroups(_applicationManager.Map.MapFrame, _applicationManager.Map.MapFrame);
                //    AssignLayerSymbologies(_applicationManager.Map.MapFrame);
                //}
                //end temporary fix
                 
            }



        }

        /// <summary>
        ///     Check if the path is a valid SQLite database
        ///     This function returns false, if the SQLite db
        ///     file doesn't exist or if the file size is 0 Bytes
        /// </summary>
        public static bool DatabaseExists(string dbPath)
        {
            return SQLiteHelper.DatabaseExists(dbPath);
        }

        /// <summary>
        ///     To get the SQLite database path given the SQLite connection string
        /// </summary>
        public static string GetSQLiteFileName(string sqliteConnString)
        {
            return SQLiteHelper.GetSQLiteFileName(sqliteConnString);
        }

        /// <summary>
        ///     To get the full SQLite connection string given the SQLite database path
        /// </summary>
        public static string GetSQLiteConnectionString(string dbFileName)
        {
            return SQLiteHelper.GetSQLiteConnectionString(dbFileName);
        }

        /// <summary>
        ///     Create the default .SQLITE database in the user-specified path
        /// </summary>
        /// <returns>true if database was created, false otherwise</returns>
        public static Boolean CreateNewDatabase(string dbPath)
        {
            // to create the default.sqlite database file using the SQLiteHelper method
            return SQLiteHelper.CreateSQLiteDatabase(dbPath, "Go2It.Resources.defaultDatabase.sqlite");
        }

        public void OpeningProject()
        {
            if (App.SerializationManager.CurrentProjectFile == null) return;
            // add the file to recent files list
            SdrConfig.Settings.Instance.AddFileToRecentFiles(App.SerializationManager.CurrentProjectFile);
            // reset the project settings back to default for repopulation
            Go2ItProjectSettings.Instance.ResetProjectSettings();
            // now clear any map panels that maybe present
            Dock.ResetLayout();
            // go ahead and set the database to proper location
            string dbFileName = Path.ChangeExtension(App.SerializationManager.CurrentProjectFile, "sqlite");
            // check if it exists, create a default db if not (this really shouldnt be happening -> user lost the db)
            if (!DatabaseExists(dbFileName))
            {
                MessageBox.Show(@"Database is missing, a new default database was created");
                CreateNewDatabase(dbFileName);
            }
            // cycle through all the map layers that were added and assign to our baselayerlookup dict
            foreach (IMapLayer layer in App.Map.Layers)
            {
                var fl = layer as IMapFeatureLayer;
                if (fl != null && String.IsNullOrEmpty(Path.GetFileNameWithoutExtension((fl.DataSet.Filename)))) return;
                if (fl != null)
                {
                    IFeatureSet fs = FeatureSet.Open(fl.DataSet.Filename);
                    // add this as a lookup to the dict
                    // if (fs != null) Dock.BaseLayerLookup.Add(fs.Name, layer);
                }
            }
            // do a basic query to get a key and caption for the panel tab lookup and selection
            string repoConnStr = SQLiteHelper.GetSQLiteConnectionString(dbFileName);
            SdrConfig.Settings.Instance.ProjectRepoConnectionString = repoConnStr;
            // do basic project configuration query
            const string psQuery = "SELECT * FROM ProjectSettings";
            DataTable psTable = SQLiteHelper.GetDataTable(repoConnStr, psQuery);
            DataRow psR = psTable.Rows[0]; // there is only one row for project settings
            // setup all project level type lookups and keys
            Go2ItProjectSettings.Instance.KeyLocationsProjectType = psR["keylocations_type"].ToString();
            Go2ItProjectSettings.Instance.AddressesProjectType = psR["addresses_type"].ToString();
            Go2ItProjectSettings.Instance.MapBgColor = Color.FromArgb(Convert.ToInt32(psR["map_bgcolor"].ToString()));
            Go2ItProjectSettings.Instance.ActiveMapViewKey = psR["active_map_key"].ToString();
            Go2ItProjectSettings.Instance.ActiveMapViewCaption = psR["active_map_caption"].ToString();
            Go2ItProjectSettings.Instance.UsePretypes = bool.Parse(psR["use_pretypes"].ToString());
            Go2ItProjectSettings.Instance.HydrantSearchCount = int.Parse(psR["search_hydrant_count"].ToString());
            Go2ItProjectSettings.Instance.HydrantSearchDistance = int.Parse(psR["search_hydrant_distance"].ToString());
            Go2ItProjectSettings.Instance.SearchBufferDistance = int.Parse(psR["search_buffer_distance"].ToString());
            Go2ItProjectSettings.Instance.SearchZoomFactor = decimal.Parse(psR["search_zoom_factor"].ToString());
            // setup all project level graphics settings
            const string gsQuery = "SELECT * FROM GraphicSettings";
            DataTable gsTable = SQLiteHelper.GetDataTable(repoConnStr, gsQuery);
            DataRow gsR = gsTable.Rows[0]; // there is only one row for graphics settings
            // setup all project level type lookups and keys
            Go2ItProjectSettings.Instance.GraphicPointColor =
                Color.FromArgb(Convert.ToInt32(gsR["point_color"].ToString()));
            Go2ItProjectSettings.Instance.GraphicPointStyle = gsR["point_style"].ToString();
            Go2ItProjectSettings.Instance.GraphicPointSize = Convert.ToInt32(gsR["point_size"]);
            Go2ItProjectSettings.Instance.GraphicLineBorderColor =
                Color.FromArgb(Convert.ToInt32(gsR["line_border_color"].ToString()));
            Go2ItProjectSettings.Instance.GraphicLineColor =
                Color.FromArgb(Convert.ToInt32(gsR["line_color"].ToString()));
            Go2ItProjectSettings.Instance.GraphicLineSize = Convert.ToInt32(gsR["line_size"]);
            Go2ItProjectSettings.Instance.GraphicLineStyle = gsR["line_style"].ToString();
            Go2ItProjectSettings.Instance.GraphicLineCap = gsR["line_cap"].ToString();
            // now lets setup all the layers to proper types
            const string lyrQuery = "SELECT * FROM Layers";
            DataTable lyrTable = SQLiteHelper.GetDataTable(repoConnStr, lyrQuery);
            foreach (DataRow row in lyrTable.Rows)
            {
                string name = row["name"].ToString();
                string lyrType = row["layerType"].ToString(); // unused
                string projType = row["projectType"].ToString();
                switch (projType)
                {
                    case "t_address":
                        Go2ItProjectSettings.Instance.AddAddressLayer(name);
                        break;
                    case "t_road":
                        Go2ItProjectSettings.Instance.AddRoadLayer(name);
                        break;
                    case "t_note":
                        Go2ItProjectSettings.Instance.NotesLayer = name;
                        break;
                    case "t_citylimit":
                        Go2ItProjectSettings.Instance.CityLimitsLayer = name;
                        break;
                    case "t_cellsector":
                        Go2ItProjectSettings.Instance.CellSectorsLayer = name;
                        break;
                    case "t_esn":
                        Go2ItProjectSettings.Instance.EsnsLayer = name;
                        break;
                    case "t_parcel":
                        Go2ItProjectSettings.Instance.ParcelsLayer = name;
                        break;
                    case "t_hydrant":
                        Go2ItProjectSettings.Instance.HydrantsLayer = name;
                        break;
                    case "t_keylocation":
                        Go2ItProjectSettings.Instance.AddKeyLocationLayer(name);
                        break;
                }
            }
            // run a query to get all the map tabs that this project has set
            const string tabsQuery = "SELECT * FROM MapTabs";
            DataTable tabsTable = SQLiteHelper.GetDataTable(repoConnStr, tabsQuery);
            // cycle through all map tabs and generate maps and tabs as needed
            foreach (DataRow row in tabsTable.Rows)
            {
                string txtKey = row["lookup"].ToString();
                string txtCaption = row["caption"].ToString();
                string txtLayers = row["layers"].ToString();
                string txtViewExtent = row["viewextent"].ToString();
                string zorder = row["zorder"].ToString();
                string txtExtent = row["extent"].ToString();
                string txtBounds = row["bounds"].ToString(); // unused

                var nMap = new Map
                {
                    BackColor = Go2ItProjectSettings.Instance.MapBgColor,
                    Visible = true,
                    Dock = DockStyle.Fill,
                    MapFrame = new EventMapFrame(),
                    // TODO: investigate best way to handle projections per maptab?
                    Projection = App.Map.Projection,
                };

                // TODO: remove out map logging event binding
                // LogMapEvents(nMap, txtCaption);  // log all map events
                var nMapFrame = nMap.MapFrame as EventMapFrame;
                // LogMapFrameEvents(nMapFrame, txtCaption);  // log all mapframe events

                // TODO: can also suspend map reprojection msgbox and no projection warning
                nMapFrame.SuspendViewExtentChanged(); // suspend all view extent changes events
                nMapFrame.SuspendEvents(); // suspend all layer events
                nMapFrame.ExtentsInitialized = true; // set the extents manually below

                nMap.MapFrame = nMapFrame;

                // parse the layers string and cycle through all layers add as needed
                string[] lyrs = txtLayers.Split('|');
                foreach (IMapLayer ml in App.Map.Layers)
                {
                    if (ml.GetType().Name == "MapImageLayer")
                    {
                        var mImageLyr = (IMapImageLayer) ml;
                        if (mImageLyr != null &&
                            String.IsNullOrEmpty(Path.GetFileNameWithoutExtension(mImageLyr.Image.Filename))) return;
                        string fName = Path.GetFileNameWithoutExtension(mImageLyr.Image.Filename);
                        foreach (string lyr in lyrs)
                        {
                            // layer names match add them to the new map tab
                            if (lyr == fName)
                            {
                                nMap.Layers.Add(mImageLyr);
                            }
                        }
                    }
                    else
                    {
                        var mFeatureLyr = ml as IMapFeatureLayer;
                        if (mFeatureLyr != null &&
                            String.IsNullOrEmpty(Path.GetFileNameWithoutExtension((mFeatureLyr.DataSet.Filename))))
                            return;
                        IFeatureSet fs = FeatureSet.Open(mFeatureLyr.DataSet.Filename);
                        if (fs != null)
                        {
                            foreach (string lyr in lyrs)
                            {
                                // layers match add them to the new map tab
                                if (lyr == fs.Name)
                                {
                                    nMap.Layers.Add(mFeatureLyr);
                                    break;
                                }
                            }
                        }
                    }
                }

                string er;
                Extent eExt;
                Extent.TryParse(txtExtent, out eExt, out er);
                if (er == "Y")
                {
                    nMap.MapFrame.Extent.SetValues(eExt.MinX, eExt.MinY, eExt.MaxX, eExt.MaxY);
                }
                string vr;
                Extent vExt;
                Extent.TryParse(txtViewExtent, out vExt, out vr);
                if (vr == "Y")
                {
                    nMap.MapFrame.ViewExtents.SetValues(vExt.MinX, vExt.MinY, vExt.MaxX, vExt.MaxY);
                }

                // create new dockable panel and stow that shit yo!
                var dp = new DockablePanel(txtKey, txtCaption, nMap, DockStyle.Fill)
                {
                    DefaultSortOrder = Convert.ToInt16(zorder)
                };
                App.DockManager.Add(dp);
            }
            App.Map = null; // kill the load map
            App.DockManager.SelectPanel(Go2ItProjectSettings.Instance.ActiveMapViewKey);
            App.DockManager.SelectPanel(Go2ItUserSettings.Instance.ActiveFunctionPanel);
        }


        /// <summary>
        ///     This method sets up the default databases.
        ///     By default these are created in the temporary directory.
        /// </summary>
        public void SetupDatabase()
        {
            // the 'default' database path is a temporary db file, it is not for use on actual projects
            string unqTmpId = string.Format("{0}_{1}{2}", DateTime.Now.Date.ToString("yyyy-MM-dd"), DateTime.Now.Hour,
                DateTime.Now.Minute);
            string dataRepositoryTempFile = unqTmpId + ".sqlite";

            // find or create a temp directory to hold the db and any possible indexes
            string tempDir = SdrConfig.ConfigurationHelper.FindOrCreateTempDirectory(
                SdrConfig.Settings.Instance.ApplicationName + "\\" + unqTmpId);

            // setup the basic data repo path
            string dataRepositoryPath = Path.Combine(tempDir, dataRepositoryTempFile);
            // validate we can write to temp access directory
            if (HasWriteAccessToFolder(tempDir))
            {
                // create (copy) a new dataRepository Db
                SQLiteHelper.CreateSQLiteDatabase(dataRepositoryPath, "Go2It.Resources.defaultDatabase.sqlite");
                string conString1 = SQLiteHelper.GetSQLiteConnectionString(dataRepositoryPath);
                SdrConfig.Settings.Instance.ProjectRepoConnectionString = conString1;
            }
        }

        private static bool HasWriteAccessToFolder(string folderPath)
        {
            try
            {
                // Attempt to get a list of security permissions from the folder.
                // This will raise an exception if the path is read only or do not have access to view the permissions.
                // System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(folderPath);
                Directory.GetAccessControl(folderPath);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        private void SaveLayerCollection(StringCollection sc, string projectType, string conn)
        {
            if (sc == null) return;
            foreach (string layName in sc)
            {
                Dictionary<string, string> d = CreateLayerDictionary(layName, projectType);
                if (d != null)
                {
                    SQLiteHelper.Insert(conn, "Layers", d);
                }
            }
        }

        private void SaveLayerCollection(string layName, string projectType, string conn)
        {
            Dictionary<string, string> d = CreateLayerDictionary(layName, projectType);
            if (d != null)
            {
                SQLiteHelper.Insert(conn, "Layers", d);
            }
        }

        private Dictionary<string, string> CreateLayerDictionary(string layName, string projType)
        {
            if (layName.Length == 0) return null;
            if (projType.Length == 0) return null;

            var d = new Dictionary<string, string>();

            foreach (var keyValuePair in AllLayersLookup)
            {
                var fl = keyValuePair.Value as IMapFeatureLayer;
                if (fl != null)
                {
                    if (keyValuePair.Key == layName)
                    {
                        var ftLayer = (FeatureLayer)fl;
                        d.Add("name", ftLayer.Name);
                        d.Add("layerType", ftLayer.DataSet.FeatureType.ToString());
                        d.Add("projectType", projType);
                        return d; // send me home now
                    }
                }
            }
            return null;
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory could not be located: "
                    + sourceDirName);
            }
            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }
            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }
            // If copying subdirectories, copy them and their contents to new location. 
            if (!copySubDirs) return;
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath, true);
            }
        }
    }
}