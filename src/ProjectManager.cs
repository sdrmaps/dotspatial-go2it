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
using DotSpatial.Extensions;
using DotSpatial.SDR.Controls;
using DotSpatial.Serialization;
using DotSpatial.Symbology;
using SDR.Common;
using SDR.Common.UserMessage;
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

        // lookup for any layer that may exist on any or multiple maptabs (used to ease serialization of types below)
        // Populated with: 'PopulateAllLayerLookup()' & used by CreateLayerDictionary()
        private readonly Dictionary<string, IMapLayer> _allLayersLookup = new Dictionary<string, IMapLayer>();
        
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

        public Dictionary<string, IMapLayer> GetLayerLookup
        {
            get { return _allLayersLookup; }
        }

        public string GetProjectShortName()
        {
            return Path.GetFileName(CurrentProjectFile);
        }

        public Map CreateNewMap(String mapName)
        {
            return CreateNewMap(mapName, Go2ItProjectSettings.Instance.MapBgColor);
        }

        public Map CreateNewMap(String mapName, Color bgColor)
        {
            // generate a new empty map frame to assign to the map
            var mapframe = new EventMapFrame();
            var map = new Map
            {
                MapFrame = mapframe,
                BackColor = bgColor,
                Visible = true,
                Dock = DockStyle.Fill
            }; 
            return map;
        }

        public void CreateNewProject()
        {
            LoadExistingProject(string.Empty);
        }

        private void LoadExistingProject(string file)
        {
            _allLayersLookup.Clear();
            if (App.Map != null)
            {
                App.Map.ClearLayers();
            }
            // set the new project directory and reset all project settings preparing for load
            SetCurrentProjectDirectory(file);
            Go2ItProjectSettings.Instance.ResetProjectSettings();
            // reset or create a dockcontroller to handle the map tabs
            if (Dock == null)
            {
                Dock = (DockingControl) App.DockManager;
            }
            else
            {
                Dock.ResetLayout();
            }
            IsDirty = false;  // and finally set project state to 'not dirty'
            OnIsDirtyChanged(); // and then fire off the dirty changed event
        }

        /// <summary>
        /// Filter text for a save project as dialog
        /// </summary>
        public new string SaveDialogFilterText
        {
            get
            {
                return String.Format(SaveDialogFilterFormat, "Project File");
            }
        }

        /// <summary>
        /// Gets the save dialog filter format.
        /// </summary>
        public new string SaveDialogFilterFormat
        {
            get
            {
                return "{0} (*.sqlite)|*.sqlite" + AggregateProviderExtensions(SaveProjectFileProviders);
            }
        }

        private static string AggregateProviderExtensions(IEnumerable<IProjectFileProvider> providers)
        {
            return providers.Aggregate(
                new StringBuilder(),
                (sb, p) => sb.AppendFormat("|{1} (*{0})|*{0}", 
                    p.Extension, 
                    p.FileTypeDescription), 
                    s => s.ToString());
        }

        private void SetCurrentProjectDirectory(string fileName)
        {
            // we set the working directory to the location of the project file.
            // all filenames will be relative to this path once set
            if (String.IsNullOrEmpty(fileName))
            {
                CurrentProjectFile = CurrentProjectDirectory = String.Empty;
            }
            else
            {
                var dn = Path.GetDirectoryName(fileName);
                if (dn == null) return;
                Directory.SetCurrentDirectory(dn);
                CurrentProjectFile = fileName;
                CurrentProjectDirectory = Path.GetDirectoryName(fileName);
            }
        }

        private string CreateSerializedMap(Map map)
        {
            var graph = new object[] { map };
            var s = new XmlSerializer();
            var xml = s.Serialize(graph);
            return xml;
        }

        private Map LoadSerializedMap(string xmlString, string mapName)
        {
            var map = CreateNewMap(mapName);
            var graph = new object[] { map };
            var d = new XmlDeserializer();
            d.Deserialize(graph, xmlString);
            return (Map)graph[0];
        }

        private void CreateProjectDatabase()
        {
            // create a default project database if needed
            if (!SQLiteHelper.DatabaseExists(CurrentProjectFile))
            {
                if (HasWriteAccessToFolder(CurrentProjectDirectory))
                {
                    CreateNewDatabase(CurrentProjectFile);
                }
                else
                {
                    var msg = AppContext.Instance.Get<IUserMessage>();
                    msg.Error("Attempt to write to " + CurrentProjectDirectory + " failed, please check permissions");
                }
            }
        }

        private static void LoadLayerCollections(DataTable lyrTable)
        {
            foreach (DataRow row in lyrTable.Rows)
            {
                string name = row["name"].ToString();
                string lyrType = row["layerType"].ToString(); // currently unused
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
        }

        private void SaveProjectSettings()
        {
            var conn = SQLiteHelper.GetSQLiteConnectionString(CurrentProjectFile);

            SQLiteHelper.ClearTable(conn, "ProjectSettings");
            var d = new Dictionary<string, string>
            {
                {"addresses_type", Go2ItProjectSettings.Instance.AddressesProjectType},
                {"keylocations_type", Go2ItProjectSettings.Instance.KeyLocationsProjectType},
                {"map_bgcolor", Go2ItProjectSettings.Instance.MapBgColor.ToArgb().ToString(CultureInfo.InvariantCulture)},
                {"active_map_key", Go2ItProjectSettings.Instance.ActiveMapViewKey},
                {"active_map_caption", Go2ItProjectSettings.Instance.ActiveMapViewCaption},
                {"search_query_logging", Go2ItProjectSettings.Instance.SearchQueryParserLogging.ToString(CultureInfo.InvariantCulture)},
                {"search_use_pretypes", Go2ItProjectSettings.Instance.SearchUsePretypes.ToString(CultureInfo.InvariantCulture)},
                {"search_zoom_factor", Go2ItProjectSettings.Instance.SearchZoomFactor.ToString(CultureInfo.InvariantCulture)},
                {"search_buffer_distance",Go2ItProjectSettings.Instance.SearchBufferDistance.ToString(CultureInfo.InvariantCulture)},
                {"search_hydrant_count", Go2ItProjectSettings.Instance.HydrantSearchCount.ToString(CultureInfo.InvariantCulture)},
                {"search_hydrant_distance", Go2ItProjectSettings.Instance.HydrantSearchDistance.ToString(CultureInfo.InvariantCulture)}
            };
            SQLiteHelper.Insert(conn, "ProjectSettings", d);

            SQLiteHelper.ClearTable(conn, "GraphicSettings");
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
            SQLiteHelper.Insert(conn, "GraphicSettings", g);

            SQLiteHelper.ClearTable(conn, "Layers");
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

        private void LoadProjectSettings()
        {
            var conn = SQLiteHelper.GetSQLiteConnectionString(CurrentProjectFile);
            
            const string psQuery = "SELECT * FROM ProjectSettings";
            DataTable psTable = SQLiteHelper.GetDataTable(conn, psQuery);
            DataRow psR = psTable.Rows[0]; // there is only one row for project settings

            Go2ItProjectSettings.Instance.KeyLocationsProjectType = psR["keylocations_type"].ToString();
            Go2ItProjectSettings.Instance.AddressesProjectType = psR["addresses_type"].ToString();
            Go2ItProjectSettings.Instance.MapBgColor = Color.FromArgb(Convert.ToInt32(psR["map_bgcolor"].ToString()));
            Go2ItProjectSettings.Instance.ActiveMapViewKey = psR["active_map_key"].ToString();
            Go2ItProjectSettings.Instance.ActiveMapViewCaption = psR["active_map_caption"].ToString();
            Go2ItProjectSettings.Instance.SearchUsePretypes = bool.Parse(psR["search_use_pretypes"].ToString());
            Go2ItProjectSettings.Instance.HydrantSearchCount = int.Parse(psR["search_hydrant_count"].ToString());
            Go2ItProjectSettings.Instance.HydrantSearchDistance = int.Parse(psR["search_hydrant_distance"].ToString());
            Go2ItProjectSettings.Instance.SearchBufferDistance = int.Parse(psR["search_buffer_distance"].ToString());
            Go2ItProjectSettings.Instance.SearchZoomFactor = decimal.Parse(psR["search_zoom_factor"].ToString());
            Go2ItProjectSettings.Instance.SearchQueryParserLogging = bool.Parse(psR["search_query_logging"].ToString());

            const string gsQuery = "SELECT * FROM GraphicSettings";
            DataTable gsTable = SQLiteHelper.GetDataTable(conn, gsQuery);
            DataRow gsR = gsTable.Rows[0]; // there is only one row for graphics settings

            Go2ItProjectSettings.Instance.GraphicPointColor = Color.FromArgb(Convert.ToInt32(gsR["point_color"].ToString()));
            Go2ItProjectSettings.Instance.GraphicPointStyle = gsR["point_style"].ToString();
            Go2ItProjectSettings.Instance.GraphicPointSize = Convert.ToInt32(gsR["point_size"]);
            Go2ItProjectSettings.Instance.GraphicLineBorderColor = Color.FromArgb(Convert.ToInt32(gsR["line_border_color"].ToString()));
            Go2ItProjectSettings.Instance.GraphicLineColor = Color.FromArgb(Convert.ToInt32(gsR["line_color"].ToString()));
            Go2ItProjectSettings.Instance.GraphicLineSize = Convert.ToInt32(gsR["line_size"]);
            Go2ItProjectSettings.Instance.GraphicLineStyle = gsR["line_style"].ToString();
            Go2ItProjectSettings.Instance.GraphicLineCap = gsR["line_cap"].ToString();

            const string lyrQuery = "SELECT * FROM Layers";  // assign all layers to their proper lookup 'type'
            DataTable lyrTable = SQLiteHelper.GetDataTable(conn, lyrQuery);
            LoadLayerCollections(lyrTable);
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

        private string PopulateAllLayersLookup(IEnumerable<IMapLayer> layers)
        {
            if (layers == null) return string.Empty;

            var txtLayers = string.Empty;
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
                    var mpLayer = (IMapFeatureLayer)mapLayer;
                    layName = ftLayer.Name;
                    // assign this to our layer lookup now
                    var fileName = Path.GetFileNameWithoutExtension(ftLayer.DataSet.Filename);
                    // store all featurelayers to lookup, to ease type serialization on settings save
                    if (fileName != null)
                    {
                        if (!_allLayersLookup.ContainsKey(fileName))
                        {
                            _allLayersLookup.Add(fileName, mpLayer);
                        }
                    }
                }
                txtLayers = txtLayers + layName + "|";
            }
            if (txtLayers.Length != 0)
            {
                txtLayers = txtLayers.Remove(txtLayers.Length - 1, 1);
            }
            return txtLayers;
        }

        private void SaveMapTabs()
        {
            var conn = SQLiteHelper.GetSQLiteConnectionString(CurrentProjectFile);
            SQLiteHelper.ClearTable(conn, "MapTabs");  // clear any existing tabs saved

            DockingControl dc = Dock;
            foreach (var dpi in dc.DockPanelLookup)
            {
                if (!dpi.Key.Trim().StartsWith("kMap")) continue;

                var map = (Map)dpi.Value.DotSpatialDockPanel.InnerControl;
                if (map == null) continue;
                var mapFrame = map.MapFrame as MapFrame;
                if (mapFrame == null) continue;

                var xmlMap = CreateSerializedMap(map);
                IMapLayerCollection layers = map.Layers; // get the layers
                string txtLayers = PopulateAllLayersLookup(layers);

                // store it all to a dict for storage in a sqlite db
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
            // TODO: are these actually functional??
            Contract.Requires(!String.IsNullOrEmpty(fileName), "fileName is null or empty.");
            Contract.Requires(App.Map != null);

            SetCurrentProjectDirectory(fileName);

            App.ProgressHandler.Progress("project_save_" + fileName, 0, "Saving Project: " + fileName);
            Application.DoEvents();  // TODO: investigate this further

            CreateProjectDatabase();  // either create new sqlite db or clear an existing one
            SaveMapTabs();  // serialize all the map tabs xml
            SaveProjectSettings();  // now save the general project settings: bgcolor, default activeTabs, etc

            SdrConfig.Settings.Instance.AddFileToRecentFiles(fileName);

            IsDirty = false;
            OnIsDirtyChanged();
            OnSerializing(new SerializingEventArgs());  // event for other plugins to listen for

            App.ProgressHandler.Progress(String.Empty, 0, String.Empty);  // clear the progress display
        }


        private void OpenProjectFile(string fileName)
        {
            LoadExistingProject(fileName);  // resets everything to default and sets CurrentProjectFile etc.
            LoadProjectSettings();
            LoadMapTabs();

            Dock.SelectPanel(Go2ItProjectSettings.Instance.ActiveMapViewKey);
            Dock.SelectPanel(Go2ItUserSettings.Instance.ActiveFunctionPanel);

            ResetMapProjection();
            App.Map.Invalidate();
        }

        public new void OpenProject(string fileName)
        {
            // TODO: again further research here
            Contract.Requires(!String.IsNullOrEmpty(fileName), "fileName is null or empty.");

            SetCurrentProjectDirectory(fileName);

            App.ProgressHandler.Progress("project_open_" + fileName, 0, "Opening Project: " + fileName);
            Application.DoEvents();  // TODO: investigate this further

            // check if there is an additional provider avaiable to open this type
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
            
            IsDirty = false;
            OnIsDirtyChanged();
            OnDeserializing(new SerializingEventArgs());

            App.ProgressHandler.Progress(String.Empty, 0, String.Empty);  // clear the progress display
        }

        private void AssignLayerSymbologies(IMapFrame mapFrame)
        {
            foreach (ILayer layer in mapFrame.GetAllLayers())
            {
                var lineLayer = layer as IMapLineLayer;
                if (lineLayer != null)
                {
                    ILineScheme original = lineLayer.Symbology;
                    if (original != null)
                    {
                        var newScheme = original.Clone() as ILineScheme;
                        original.CopyProperties(newScheme);
                        original.ResumeEvents();
                    }
                }
                // to correctly draw categories:
                var featureLayer = layer as IMapFeatureLayer;
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
            // this method will assign the parent groups.
            // it needs to be applied after opening project so that none of
            // the parent groups are NULL.
            foreach (ILayer child in parentGroup.GetLayers())
            {
                var childGroup = child as IGroup;
                if (childGroup != null)
                {
                    AssignParentGroups(childGroup, parentMapFrame);
                }
                child.SetParentItem(parentGroup);
                child.MapFrame = parentMapFrame;
            }
        }

        private void LoadMapTabs()
        {
            var conn = SQLiteHelper.GetSQLiteConnectionString(CurrentProjectFile);

            // snag all the maptab records from the db: deserialize and populate tabs
            const string tabsQuery = "SELECT * FROM MapTabs";
            DataTable tabsTable = SQLiteHelper.GetDataTable(conn, tabsQuery);

            foreach (DataRow row in tabsTable.Rows)
            {
                string txtMapXml = row["map_xml"].ToString();
                string txtKey = row["lookup"].ToString();
                string txtCaption = row["caption"].ToString();
                // TODO: implement these settings as needed (zorder is really the only one)
                string txtLayers = row["layers"].ToString();
                string txtViewExtent = row["viewextent"].ToString();
                string zorder = row["zorder"].ToString();
                string txtExtent = row["extent"].ToString();
                string txtBounds = row["bounds"].ToString();

                Map map = LoadSerializedMap(txtMapXml, txtKey);
                if (map != null)
                {
                    // properly parent the mapframe groups and assign symbology
                    if (map.MapFrame != null)
                    {
                        // TODO: dont think this is needed anymore.
                        string er;
                        Extent eExt;
                        Extent.TryParse(txtExtent, out eExt, out er);
                        if (er == "Y")
                        {
                            map.MapFrame.Extent.SetValues(eExt.MinX, eExt.MinY, eExt.MaxX, eExt.MaxY);
                        }
                        string vr;
                        Extent vExt;
                        Extent.TryParse(txtViewExtent, out vExt, out vr);
                        if (vr == "Y")
                        {
                            map.MapFrame.ViewExtents.SetValues(vExt.MinX, vExt.MinY, vExt.MaxX, vExt.MaxY);
                        }
                        // TODO: not sure if we need these??
                        AssignParentGroups(map.MapFrame, map.MapFrame);
                        AssignLayerSymbologies(map.MapFrame);
                    }
                    PopulateAllLayersLookup(map.Layers);
                }


                // create new dockable panel to hold the new map
                var dp = new DockablePanel(txtKey, txtCaption, map, DockStyle.Fill);
                Dock.Add(dp);  // add the new tab view to the main form
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

            foreach (var keyValuePair in _allLayersLookup)
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
    }
}