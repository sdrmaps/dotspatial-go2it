using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.SDR.Controls;
using DotSpatial.Symbology;
using DotSpatial.Topology;
using Go2It.Properties;
using SdrConfig = SDR.Configuration;
using SDR.Data.Database;

namespace Go2It
{
    public class ProjectManager
    {
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

        /// <summary>
        /// The main app manager
        /// </summary>
        public AppManager App { get; private set; }
        public DockingControl Dock { get; private set; }

        /// <summary>
        /// Creates a new instance of the project manager
        /// </summary>
        /// <param name="mainApp"></param>
        public ProjectManager(AppManager mainApp)
        {
            App = mainApp;
            Dock = (DockingControl) mainApp.DockManager;
        }

        public static ProjectionInfo DefaultProjection { get { return KnownCoordinateSystems.Projected.World.WebMercator; } }

        // sets the map extent to continental U.S
        public void SetDefaultMapExtents()
        {
            App.Map.ViewExtents = DefaultMapExtents().ToExtent();
        }

        public Envelope DefaultMapExtents()
        {
            var defaultMapExtent = new Envelope(-130, -60, 10, 55);
            var xy = new double[4];
            xy[0] = defaultMapExtent.Minimum.X;
            xy[1] = defaultMapExtent.Minimum.Y;
            xy[2] = defaultMapExtent.Maximum.X;
            xy[3] = defaultMapExtent.Maximum.Y;
            var z = new double[] { 0, 0 };
            ProjectionInfo wgs84 = KnownCoordinateSystems.Geographic.World.WGS1984;
            Reproject.ReprojectPoints(xy, z, wgs84, DefaultProjection, 0, 2);
            return new Envelope(xy[0], xy[2], xy[1], xy[3]);
        }

        /// <summary>
        /// Check if the path is a valid SQLite database
        /// This function returns false, if the SQLite db
        /// file doesn't exist or if the file size is 0 Bytes
        /// </summary>
        public static bool DatabaseExists(string dbPath)
        {
            return SQLiteHelper.DatabaseExists(dbPath);
        }

        /// <summary>
        /// To get the SQLite database path given the SQLite connection string
        /// </summary>
        public static string GetSQLiteFileName(string sqliteConnString)
        {
            return SQLiteHelper.GetSQLiteFileName(sqliteConnString);
        }

        /// <summary>
        /// To get the full SQLite connection string given the SQLite database path
        /// </summary>
        public static string GetSQLiteConnectionString(string dbFileName)
        {
            return SQLiteHelper.GetSQLiteConnectionString(dbFileName);
        }

        /// <summary>
        /// Create the default .SQLITE database in the user-specified path
        /// </summary>
        /// <returns>true if database was created, false otherwise</returns>
        public static Boolean CreateNewDatabase(string dbPath)
        {
            // to create the default.sqlite database file using the SQLiteHelper method
            return SQLiteHelper.CreateSQLiteDatabase(dbPath, "Go2It.Resources.defaultDatabase.sqlite");
        }

        /// <summary>
        /// Opens a project and updates the maps
        /// </summary>
        // public void OpenProject(string projectFileName)
        //{
            // App.ProgressHandler.Progress("Opening Project", 0, "Opening Project");
            // OpenProject will also call the overriden method OpeningProject directly below
            // App.SerializationManager.OpenProject(projectFileName);
            // SdrConfig.Settings.Instance.AddFileToRecentFiles(projectFileName);
            // App.ProgressHandler.Progress("Project opened", 0, "");
        // }

        public void OpeningProject()
        {
            if (App.SerializationManager.CurrentProjectFile == null) return;
            // add the file to recent files list
            SdrConfig.Settings.Instance.AddFileToRecentFiles(App.SerializationManager.CurrentProjectFile);
            // attempt to reset the projection of the application map
            if (App.Map.MapFrame.Projection != KnownCoordinateSystems.Projected.World.WebMercator)
            {
                MapFrameProjectionHelper.ReprojectMapFrame(App.Map.MapFrame, KnownCoordinateSystems.Projected.World.WebMercator.ToEsriString());
            }
            // reset the project settings back to default for repopulation
            SdrConfig.Project.Go2ItProjectSettings.Instance.ResetProjectSettings();
            // now clear any map panels that maybe present
            Dock.ResetLayout();
            // go ahead and set the database to proper location
            string dbFileName = Path.ChangeExtension(App.SerializationManager.CurrentProjectFile, "sqlite");
            // check if it exists, create a default db if not (this really shouldnt be happening -> user lost the db)
            if (!DatabaseExists(dbFileName))
            {
                MessageBox.Show(@"Database is missing, a new default DB has been created");
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
                    if (fs != null) Dock.BaseLayerLookup.Add(fs.Name, layer);
                }
            }
            // do a basic query to get a key and caption for the panel tab lookup and selection
            var repoConnStr = SQLiteHelper.GetSQLiteConnectionString(dbFileName);
            SdrConfig.Settings.Instance.ProjectRepoConnectionString = repoConnStr;
            // do basic project configuration query
            const string psQuery = "SELECT keylocations_type, addresses_type, map_bgcolor, active_map_key, active_map_caption FROM ProjectSettings";
            DataTable psTable = SQLiteHelper.GetDataTable(repoConnStr, psQuery);
            var psR = psTable.Rows[0];  // there is only one row for project settings
            // setup all project level type lookups and keys
            SdrConfig.Project.Go2ItProjectSettings.Instance.KeyLocationsProjectType = psR["keylocations_type"].ToString();
            SdrConfig.Project.Go2ItProjectSettings.Instance.AddressesProjectType = psR["addresses_type"].ToString();
            SdrConfig.Project.Go2ItProjectSettings.Instance.MapBgColor = Color.FromArgb(Convert.ToInt32(psR["map_bgcolor"].ToString()));
            SdrConfig.Project.Go2ItProjectSettings.Instance.ActiveMapViewKey = psR["active_map_key"].ToString();
            SdrConfig.Project.Go2ItProjectSettings.Instance.ActiveMapViewCaption= psR["active_map_caption"].ToString();
            // now lets setup all the layers to proper types
            const string lyrQuery = "SELECT name, layerType, projectType FROM Layers";
            DataTable lyrTable = SQLiteHelper.GetDataTable(repoConnStr, lyrQuery);
            foreach (DataRow row in lyrTable.Rows)
            {
                var name = row["name"].ToString();
                // var lyrType = row["layerType"].ToString();
                var projType = row["projectType"].ToString();
                switch (projType)
                {
                    case "t_address":
                        SdrConfig.Project.Go2ItProjectSettings.Instance.AddAddressLayer(name);
                        break;
                    case "t_road":
                        SdrConfig.Project.Go2ItProjectSettings.Instance.AddRoadLayer(name);
                        break;
                    case "t_note":
                        SdrConfig.Project.Go2ItProjectSettings.Instance.NotesLayer = name;
                        break;
                    case "t_citylimit":
                        SdrConfig.Project.Go2ItProjectSettings.Instance.CityLimitsLayer = name;
                        break;
                    case "t_cellsector":
                        SdrConfig.Project.Go2ItProjectSettings.Instance.CellSectorsLayer = name;
                        break;
                    case "t_esn":
                        SdrConfig.Project.Go2ItProjectSettings.Instance.EsnsLayer = name;
                        break;
                    case "t_parcel":
                        SdrConfig.Project.Go2ItProjectSettings.Instance.ParcelsLayer = name;
                        break;
                    case "t_hydrant":
                        SdrConfig.Project.Go2ItProjectSettings.Instance.HydrantsLayer = name;
                        break;
                    case "t_keylocation":
                        SdrConfig.Project.Go2ItProjectSettings.Instance.AddKeyLocationLayer(name);
                        break;
                }
            }
            // run a query to get all the map tabs that this project has set
            const string tabsQuery = "SELECT layers, extent, bounds, zorder, lookup, caption, viewextent FROM MapTabs";
            DataTable tabsTable = SQLiteHelper.GetDataTable(repoConnStr, tabsQuery);
            // cycle through all map tabs and generate maps and tabs as needed
            foreach (DataRow row in tabsTable.Rows)
            {
                var txtKey = row["lookup"].ToString();
                var txtCaption  = row["caption"].ToString();
                var txtLayers = row["layers"].ToString();
                var txtExtent = row["extent"].ToString();
                // var txtBounds = row["bounds"].ToString();
                var txtViewExtent = row["viewextent"].ToString();
                var zorder = row["zorder"].ToString();
                var nMap = new Map
                {
                    BackColor = SdrConfig.Project.Go2ItProjectSettings.Instance.MapBgColor,
                    Visible = true
                    // TODO: really need to verify if we have to pass in map functions
                    // should also set the projection sometime in here i think
                    // MapFunctions = Dock.BaseMap.MapFunctions
                };
                Extent ext;
                string r;
                Extent.TryParse(txtExtent, out ext, out r);
                if (r == "Y")
                {
                    // nMap.MapFrame.Extent
                    nMap.Extent.SetValues(ext.MinX, ext.MinY, ext.MaxX, ext.MinY);
                }
                Extent vext;
                Extent.TryParse(txtViewExtent, out vext, out r);
                if (r == "Y")
                {
                    nMap.ViewExtents.SetValues(vext.MinX, ext.MinY, ext.MaxX, ext.MinY);
                }
                // parse the layers string and cycle through all layers add as needed
                string[] lyrs = txtLayers.Split('|');
                foreach (IMapLayer ml in App.Map.Layers)
                {
                    if (ml.GetType().Name == "MapImageLayer")
                    {
                        var mImageLyr = (IMapImageLayer) ml;
                        if (mImageLyr != null && String.IsNullOrEmpty(Path.GetFileNameWithoutExtension(mImageLyr.Image.Filename))) return;
                        var fName = Path.GetFileNameWithoutExtension(mImageLyr.Image.Filename);
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
                        if (mFeatureLyr != null && String.IsNullOrEmpty(Path.GetFileNameWithoutExtension((mFeatureLyr.DataSet.Filename)))) return;
                        IFeatureSet fs = FeatureSet.Open(mFeatureLyr.DataSet.Filename);
                        if (fs != null)
                        {
                            foreach (string lyr in lyrs)
                            {
                                // layers match add them to the new map tab
                                if (lyr == fs.Name)
                                {
                                    nMap.Layers.Add(mFeatureLyr);
                                }
                            }
                        }
                    }
                }
                // create new dockable panel and stow that shit yo!
                var dp = new DockablePanel(txtKey, txtCaption, nMap, DockStyle.Fill)
                {
                    DefaultSortOrder = Convert.ToInt16(zorder)
                };
                App.DockManager.Add(dp);
            }
            // select the active map tab via the key and go go go
            App.DockManager.SelectPanel(SdrConfig.Project.Go2ItProjectSettings.Instance.ActiveMapViewKey);
        }


        /// <summary>
        /// Creates a new 'empty' project
        /// </summary>
        public void CreateEmptyProject()
        {
            // clear all active map layers
            App.Map.Layers.Clear();
            // reset extents to default
            SetDefaultMapExtents();
            // reset all project settings to default
            SdrConfig.Project.Go2ItProjectSettings.Instance.ResetProjectSettings();
            // reset all dock controller map tabs and layerlookup dict
            Dock.ResetLayout();
            // setup a temp database to hold our settings
            SetupDatabase();
        }

        /// <summary>
        /// This method sets up the default databases.
        /// By default these are created in the temporary directory.
        /// </summary>
        public void SetupDatabase()
        {
            // the 'default' database path is a temporary db file, it is not for use on actual projects
            var unqTmpId = string.Format("{0}_{1}{2}", DateTime.Now.Date.ToString("yyyy-MM-dd"), DateTime.Now.Hour, DateTime.Now.Minute);
            string dataRepositoryTempFile = unqTmpId + ".sqlite";

            // find or create a temp directory to hold the db and any possible indexes
            string tempDir = SdrConfig.ConfigurationHelper.FindOrCreateTempDirectory(
                SdrConfig.Settings.Instance.ApplicationName + "\\" + unqTmpId);

            // setup the basic datarepo path
            string dataRepositoryPath = Path.Combine(tempDir, dataRepositoryTempFile);
            // validate we can write to temp access directory
            if (HasWriteAccessToFolder(tempDir))
            {
                // create (copy) a new dataRepositoryDb
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

        public void SaveProjectSettings()
        {
            // get the project settings db connection string
            var conn = SdrConfig.Settings.Instance.ProjectRepoConnectionString;
            // set  basic project level settings at this point
            var d = new Dictionary<string, string>
            {
                { "addresses_type", SdrConfig.Project.Go2ItProjectSettings.Instance.AddressesProjectType },
                { "keylocations_type", SdrConfig.Project.Go2ItProjectSettings.Instance.KeyLocationsProjectType },
                { "map_bgcolor", SdrConfig.Project.Go2ItProjectSettings.Instance.MapBgColor.ToArgb().ToString(CultureInfo.InvariantCulture) },
                { "active_map_key", SdrConfig.Project.Go2ItProjectSettings.Instance.ActiveMapViewKey },
                { "active_map_caption", SdrConfig.Project.Go2ItProjectSettings.Instance.ActiveMapViewCaption }
            };
            // there can only be a single project settings row in the table
            SQLiteHelper.Update(conn, "ProjectSettings", d, "key = 1");
            // clear any (base map) layers currently set in the table and reset them now
            SQLiteHelper.ClearTable(conn, "Layers");
            // cycle and save all layer types to settings
            SaveLayerCollection(SdrConfig.Project.Go2ItProjectSettings.Instance.AddressLayers, LayerTypeAddress, conn);
            SaveLayerCollection(SdrConfig.Project.Go2ItProjectSettings.Instance.RoadLayers, LayerTypeRoad, conn);
            SaveLayerCollection(SdrConfig.Project.Go2ItProjectSettings.Instance.NotesLayer, LayerTypeNote, conn);
            SaveLayerCollection(SdrConfig.Project.Go2ItProjectSettings.Instance.CityLimitsLayer, LayerTypeCityLimit, conn);
            SaveLayerCollection(SdrConfig.Project.Go2ItProjectSettings.Instance.CellSectorsLayer, LayerTypeCellSector, conn);
            SaveLayerCollection(SdrConfig.Project.Go2ItProjectSettings.Instance.EsnsLayer, LayerTypeEsn, conn);
            SaveLayerCollection(SdrConfig.Project.Go2ItProjectSettings.Instance.ParcelsLayer, LayerTypeParcel, conn);
            SaveLayerCollection(SdrConfig.Project.Go2ItProjectSettings.Instance.HydrantsLayer, LayerTypeHydrant, conn);
            SaveLayerCollection(SdrConfig.Project.Go2ItProjectSettings.Instance.KeyLocationLayers, LayerTypeKeyLocation, conn);
            // check for any map tabs and save them as needed
            SQLiteHelper.ClearTable(conn, "MapTabs");
            var dc = Dock;
            foreach (KeyValuePair<string, DockPanelInfo> dpi in dc.DockPanelLookup)
            {
                var map = (Map)dpi.Value.DotSpatialDockPanel.InnerControl;
                var layers = map.Layers;  // get the layers
                var txtLayers = string.Empty;  // text block to store layers

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
                        layName = ftLayer.Name;
                    }
                    txtLayers = txtLayers + layName + "|";
                }
                
                txtLayers = txtLayers.Remove(txtLayers.Length - 1, 1);
                // send this all to a dict to save to dbase as a maptab
                var dd = new Dictionary<string, string>
                {
                    {"lookup", dpi.Key},
                    {"caption", dpi.Value.DotSpatialDockPanel.Caption},
                    {"zorder", dpi.Value.SortOrder.ToString(CultureInfo.InvariantCulture)},
                    {"extent", map.Extent.ToString()},
                    {"viewextent",map.ViewExtents.ToString()},
                    {"bounds", map.Bounds.ToString()},
                    {"layers", txtLayers}
                };
                SQLiteHelper.Insert(conn, "MapTabs", dd);
            }
        }

        private void SaveLayerCollection(StringCollection sc, string projectType, string conn)
        {
            if (sc == null) return;
            foreach (var layName in sc)
            {
                var d = CreateLayerDictionary(layName, projectType);
                if (d != null)
                {
                    SQLiteHelper.Insert(conn, "Layers", d);
                }
            }
        }

        private void SaveLayerCollection(string layName, string projectType, string conn)
        {
            var d = CreateLayerDictionary(layName, projectType);
            if (d != null)
            {
                SQLiteHelper.Insert(conn, "Layers", d);
            }
        }

        private static Predicate<IMapFeatureLayer> ByName(string name)
        {
            return delegate(IMapFeatureLayer mapLayer)
            {
                var ftLayer = (FeatureLayer)mapLayer;
                return ftLayer.Name == name;
            };
        }

        private Dictionary<string, string> CreateLayerDictionary(string layName, string projType)
        {
            if (layName.Length == 0) return null;
            if (projType.Length == 0) return null;

            var d = new Dictionary<string, string>();
            foreach (KeyValuePair<string, IMapLayer> keyValuePair in Dock.BaseLayerLookup)
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
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
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
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public void SavingProject()
        {
            string projectFileName = App.SerializationManager.CurrentProjectFile;
            SdrConfig.Settings.Instance.AddFileToRecentFiles(projectFileName);

            // App.ProgressHandler.Progress("Saving Project " + projectFileName, 0, "");
            Application.DoEvents();

            // check for a project level database
            string newDbPath = Path.ChangeExtension(projectFileName, ".sqlite");
            string newDirPath = Path.GetDirectoryName(newDbPath) + "\\" + "indexes";

            string currentDbPath = SQLiteHelper.GetSQLiteFileName(SdrConfig.Settings.Instance.ProjectRepoConnectionString);
            var d = Path.GetDirectoryName(currentDbPath);
            var currentDirPath = Path.Combine(d, "indexes");

            if (newDbPath != currentDbPath)
            {
                // copy db to new path. If no db exists, create new db in the new location
                if (SQLiteHelper.DatabaseExists(currentDbPath))
                {
                    File.Copy(currentDbPath, newDbPath, true);
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
                SdrConfig.Settings.Instance.ProjectRepoConnectionString = SQLiteHelper.GetSQLiteConnectionString(newDbPath);
            }
            SaveProjectSettings();
            // App.ProgressHandler.Progress(String.Empty, 0, String.Empty);
        }
    }
}
