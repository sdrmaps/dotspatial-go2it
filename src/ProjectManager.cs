using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
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
            Go2ItProjectSettings.Instance.ResetProjectSettingsToDefaults();
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
            if (!SQLiteHelper.DatabaseFileExists(CurrentProjectFile))
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

        private string ValidateColumn(string conn, string table, string colName, string colType)
        {
            if (!SQLiteHelper.ColumnExists(conn, table, colName))
            {
                SQLiteHelper.CreateColumn(conn, table, colName, colType);
            }
            return colName;
        }

        private void SaveProjectSettings()
        {
            var conn = SQLiteHelper.GetSQLiteConnectionString(CurrentProjectFile);
            var d = new Dictionary<string, string>
            {
                { ValidateColumn(conn, "ProjectSettings", "addresses_type", "TEXT"), Go2ItProjectSettings.Instance.AddressesProjectType},
                { ValidateColumn(conn, "ProjectSettings", "keylocations_type", "TEXT"), Go2ItProjectSettings.Instance.KeyLocationsProjectType},
                { ValidateColumn(conn, "ProjectSettings", "map_bgcolor", "TEXT"), Go2ItProjectSettings.Instance.MapBgColor.ToArgb().ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "active_map_key", "TEXT"), Go2ItProjectSettings.Instance.ActiveMapViewKey},
                { ValidateColumn(conn, "ProjectSettings", "active_map_caption", "TEXT"), Go2ItProjectSettings.Instance.ActiveMapViewCaption},
                { ValidateColumn(conn, "ProjectSettings", "search_query_logging", "TEXT"), Go2ItProjectSettings.Instance.SearchQueryParserLogging.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "search_use_pretypes", "TEXT"), Go2ItProjectSettings.Instance.SearchUsePretypes.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "search_zoom_factor", "NUMERIC"), Go2ItProjectSettings.Instance.SearchZoomFactor.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "search_buffer_distance", "NUMERIC"), Go2ItProjectSettings.Instance.SearchBufferDistance.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "search_hydrant_count", "NUMERIC"), Go2ItProjectSettings.Instance.HydrantSearchCount.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "search_hydrant_distance", "NUMERIC"), Go2ItProjectSettings.Instance.HydrantSearchDistance.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "ali_mode", "TEXT"), Go2ItProjectSettings.Instance.AliMode.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "ali_enterpol_tablename", "TEXT"), Go2ItProjectSettings.Instance.AliEnterpolTableName.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "ali_enterpol_initcatalog", "TEXT"), Go2ItProjectSettings.Instance.AliEnterpolInitialCatalog.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "ali_enterpol_datasource", "TEXT"), Go2ItProjectSettings.Instance.AliEnterpolDataSource.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "ali_global_logpath", "TEXT"), Go2ItProjectSettings.Instance.AliGlobalCadLogPath.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "ali_global_archivepath", "TEXT"), Go2ItProjectSettings.Instance.AliGlobalCadArchivePath.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "ali_global_configpath", "TEXT"), Go2ItProjectSettings.Instance.AliGlobalCadConfigPath.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "ali_sdrserver_udpport", "NUMERIC"), Go2ItProjectSettings.Instance.AliSdrServerUdpPort.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "ali_sdrserver_udphost", "TEXT"), Go2ItProjectSettings.Instance.AliSdrServerUdpHost.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "ali_sdrserver_dbpath", "TEXT"), Go2ItProjectSettings.Instance.AliSdrServerDbPath.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "ali_networkfleet_udphost", "TEXT"), Go2ItProjectSettings.Instance.AliNetworkfleetUdpHost.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "ali_networkfleet_udpport", "NUMERIC"), Go2ItProjectSettings.Instance.AliNetworkfleetUdpPort.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "ali_use_networkfleet", "TEXT"), Go2ItProjectSettings.Instance.AliUseNetworkfleet.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "ali_use_enterpol_avl", "TEXT"), Go2ItProjectSettings.Instance.AliUseEnterpolAvl.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "ali_enterpol_avl_tablename", "TEXT"), Go2ItProjectSettings.Instance.AliEnterpolAvlTableName.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "ali_enterpol_avl_initcatalog", "TEXT"), Go2ItProjectSettings.Instance.AliEnterpolAvlInitialCatalog.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "ali_enterpol_avl_setcurlocproc", "TEXT"), Go2ItProjectSettings.Instance.AliEnterpolAvlSetMyLocProc.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "ali_enterpol_avl_whoamiproc", "TEXT"), Go2ItProjectSettings.Instance.AliEnterpolAvlWhoAmIProc.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "ali_enterpol_avl_updatefreq", "NUMERIC"), Go2ItProjectSettings.Instance.AliEnterpolAvlUpdateFreq.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "ali_enterpol_avl_readfreq", "NUMERIC"), Go2ItProjectSettings.Instance.AliEnterpolAvlReadFreq.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "ali_enterpol_avl_age1freq", "NUMERIC"), Go2ItProjectSettings.Instance.AliEnterpolAvlAge1Freq.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "ali_enterpol_avl_age2freq", "NUMERIC"), Go2ItProjectSettings.Instance.AliEnterpolAvlAge2Freq.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "ali_enterpol_avl_age3freq", "NUMERIC"), Go2ItProjectSettings.Instance.AliEnterpolAvlAge3Freq.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "ProjectSettings", "ali_avl_autohideinactive", "TEXT"), Go2ItProjectSettings.Instance.AliAvlAutoHideInactiveUnits.ToString(CultureInfo.InvariantCulture)},
            };
            var k = SQLiteHelper.ExecuteScalar(conn, "SELECT key FROM ProjectSettings limit 1");
            if (k.Length == 0)
            {
                SQLiteHelper.Insert(conn, "ProjectSettings", d);
            }
            else
            {
                SQLiteHelper.Update(conn, "ProjectSettings", d, "key=" + k.ToString());
            }

            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));
            string networkFleetFont = converter.ConvertToString(Go2ItProjectSettings.Instance.AliNetworkfleetFont);
            string enterpolAvlSymbolFont = converter.ConvertToString(Go2ItProjectSettings.Instance.AliEnterpolAvlSymbolFont);
            string enterpolAvlLabelFont = converter.ConvertToString(Go2ItProjectSettings.Instance.AliEnterpolAvlLabelFont);
            var g = new Dictionary<string, string>
            {
                { ValidateColumn(conn, "GraphicSettings", "point_color", "TEXT"), Go2ItProjectSettings.Instance.GraphicPointColor.ToArgb().ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "GraphicSettings", "point_style", "TEXT"), Go2ItProjectSettings.Instance.GraphicPointStyle},
                { ValidateColumn(conn, "GraphicSettings", "point_size", "NUMERIC"), Go2ItProjectSettings.Instance.GraphicPointSize.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "GraphicSettings", "line_color", "TEXT"), Go2ItProjectSettings.Instance.GraphicLineColor.ToArgb().ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "GraphicSettings", "line_border_color", "TEXT"), Go2ItProjectSettings.Instance.GraphicLineBorderColor.ToArgb().ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "GraphicSettings", "line_size", "NUMERIC"), Go2ItProjectSettings.Instance.GraphicLineSize.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "GraphicSettings", "line_cap", "TEXT"), Go2ItProjectSettings.Instance.GraphicLineCap},
                { ValidateColumn(conn, "GraphicSettings", "line_style", "TEXT"), Go2ItProjectSettings.Instance.GraphicLineStyle},
                { ValidateColumn(conn, "GraphicSettings", "ali_networkfleet_color", "TEXT"), Go2ItProjectSettings.Instance.AliNetworkfleetColor.ToArgb().ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "GraphicSettings", "ali_networkfleet_font", "TEXT"), networkFleetFont},
                { ValidateColumn(conn, "GraphicSettings", "ali_networkfleet_char", "TEXT"), Go2ItProjectSettings.Instance.AliNetworkfleetChar.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "GraphicSettings", "ali_enterpol_avl_symbol_font", "TEXT"), enterpolAvlSymbolFont},
                { ValidateColumn(conn, "GraphicSettings", "ali_enterpol_avl_label_font", "TEXT"), enterpolAvlLabelFont},
                { ValidateColumn(conn, "GraphicSettings", "ali_enterpol_avl_lecolor", "TEXT"), Go2ItProjectSettings.Instance.AliEnterpolAvlLeColor.ToArgb().ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "GraphicSettings", "ali_enterpol_avl_fdcolor", "TEXT"), Go2ItProjectSettings.Instance.AliEnterpolAvlFdColor.ToArgb().ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "GraphicSettings", "ali_enterpol_avl_emscolor", "TEXT"), Go2ItProjectSettings.Instance.AliEnterpolAvlEmsColor.ToArgb().ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "GraphicSettings", "ali_enterpol_avl_lechar", "TEXT"), Go2ItProjectSettings.Instance.AliEnterpolAvlLeChar.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "GraphicSettings", "ali_enterpol_avl_fdchar", "TEXT"), Go2ItProjectSettings.Instance.AliEnterpolAvlFdChar.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "GraphicSettings", "ali_enterpol_avl_emschar", "TEXT"), Go2ItProjectSettings.Instance.AliEnterpolAvlEmsChar.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "GraphicSettings", "ali_enterpol_avl_mycolor", "TEXT"), Go2ItProjectSettings.Instance.AliEnterpolAvlMyColor.ToArgb().ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "GraphicSettings", "ali_avl_inactivecolor", "TEXT"), Go2ItProjectSettings.Instance.AliAvlInactiveColor.ToArgb().ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "GraphicSettings", "ali_enterpol_avl_label_alignment", "TEXT"), Go2ItProjectSettings.Instance.AliEnterpolAvlLabelAlignment},
                { ValidateColumn(conn, "GraphicSettings", "ali_enterpol_avl_label_xoffset", "TEXT"), Go2ItProjectSettings.Instance.AliEnterpolAvlLabelXOffset.ToString(CultureInfo.InvariantCulture)},
                { ValidateColumn(conn, "GraphicSettings", "ali_enterpol_avl_label_yoffset", "TEXT"), Go2ItProjectSettings.Instance.AliEnterpolAvlLabelYOffset.ToString(CultureInfo.InvariantCulture)},
            };
            k = SQLiteHelper.ExecuteScalar(conn, "SELECT key FROM GraphicSettings limit 1");
            if (k.Length == 0)
            {
                SQLiteHelper.Insert(conn, "GraphicSettings", g);
            }
            else
            {
                SQLiteHelper.Update(conn, "GraphicSettings", g, "key=" + k);
            }

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

        private static string AttachSetting(string key, string setting, DataTable dt)
        {
            if (!dt.Columns.Contains(key)) return setting;
            DataRow r = dt.Rows[0]; // there is only one row for project settings
            return r[key].ToString().Length == 0 ? setting : r[key].ToString();
        }

        private static Color AttachSetting(string key, Color setting, DataTable dt)
        {
            if (!dt.Columns.Contains(key)) return setting;
            DataRow r = dt.Rows[0]; // there is only one row for project settings
            return r[key].ToString().Length == 0 ? setting : Color.FromArgb((Convert.ToInt32(r[key].ToString())));
        }

        private static bool AttachSetting(string key, bool setting, DataTable dt)
        {
            if (!dt.Columns.Contains(key)) return setting;
            DataRow r = dt.Rows[0]; // there is only one row for project settings
            return r[key].ToString().Length == 0 ? setting : bool.Parse(r[key].ToString());
        }

        private static int AttachSetting(string key, int setting, DataTable dt)
        {
            if (!dt.Columns.Contains(key)) return setting;
            DataRow r = dt.Rows[0]; // there is only one row for project settings
            return r[key].ToString().Length == 0 ? setting : int.Parse(r[key].ToString());
        }

        private static decimal AttachSetting(string key, decimal setting, DataTable dt)
        {
            if (!dt.Columns.Contains(key)) return setting;
            DataRow r = dt.Rows[0]; // there is only one row for project settings
            return r[key].ToString().Length == 0 ? setting : decimal.Parse(r[key].ToString());
        }

        private static char AttachSetting(string key, char setting, DataTable dt)
        {
            if (!dt.Columns.Contains(key)) return setting;
            DataRow r = dt.Rows[0]; // there is only one row for project settings
            return r[key].ToString().Length == 0 ? setting : char.Parse(r[key].ToString());
        }

        private static Font AttachSetting(string key, Font setting, DataTable dt)
        {
            if (!dt.Columns.Contains(key)) return setting;
            DataRow r = dt.Rows[0]; // there is only one row for project settings
            if (r[key].ToString().Length == 0)
            {
                return setting;
            }
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));
            return (Font)converter.ConvertFromString(r[key].ToString());
        }

        private void LoadProjectSettings()
        {
            var conn = SQLiteHelper.GetSQLiteConnectionString(CurrentProjectFile);
            
            const string psQuery = "SELECT * FROM ProjectSettings";
            DataTable p = SQLiteHelper.GetDataTable(conn, psQuery);
            Go2ItProjectSettings.Instance.KeyLocationsProjectType = AttachSetting("keylocations_type", Go2ItProjectSettings.Instance.KeyLocationsProjectType, p);
            Go2ItProjectSettings.Instance.AddressesProjectType = AttachSetting("addresses_type", Go2ItProjectSettings.Instance.AddressesProjectType, p);
            Go2ItProjectSettings.Instance.MapBgColor = AttachSetting("map_bgcolor", Go2ItProjectSettings.Instance.MapBgColor, p);
            Go2ItProjectSettings.Instance.ActiveMapViewKey = AttachSetting("active_map_key", Go2ItProjectSettings.Instance.ActiveMapViewKey, p);
            Go2ItProjectSettings.Instance.ActiveMapViewCaption = AttachSetting("active_map_caption", Go2ItProjectSettings.Instance.ActiveMapViewCaption, p);
            Go2ItProjectSettings.Instance.SearchUsePretypes = AttachSetting("search_use_pretypes", Go2ItProjectSettings.Instance.SearchUsePretypes, p);
            Go2ItProjectSettings.Instance.HydrantSearchCount = AttachSetting("search_hydrant_count", Go2ItProjectSettings.Instance.HydrantSearchCount, p);
            Go2ItProjectSettings.Instance.HydrantSearchDistance = AttachSetting("search_hydrant_distance", Go2ItProjectSettings.Instance.HydrantSearchDistance, p);
            Go2ItProjectSettings.Instance.SearchBufferDistance = AttachSetting("search_buffer_distance", Go2ItProjectSettings.Instance.SearchBufferDistance, p);
            Go2ItProjectSettings.Instance.SearchZoomFactor = AttachSetting("search_zoom_factor", Go2ItProjectSettings.Instance.SearchZoomFactor, p);
            Go2ItProjectSettings.Instance.SearchQueryParserLogging = AttachSetting("search_query_logging", Go2ItProjectSettings.Instance.SearchQueryParserLogging, p);

            Go2ItProjectSettings.Instance.AliEnterpolInitialCatalog = AttachSetting("ali_enterpol_initcatalog", Go2ItProjectSettings.Instance.AliEnterpolInitialCatalog, p);
            Go2ItProjectSettings.Instance.AliEnterpolTableName = AttachSetting("ali_enterpol_tablename", Go2ItProjectSettings.Instance.AliEnterpolTableName, p);
            Go2ItProjectSettings.Instance.AliEnterpolDataSource = AttachSetting("ali_enterpol_datasource", Go2ItProjectSettings.Instance.AliEnterpolDataSource, p);
            Go2ItProjectSettings.Instance.AliGlobalCadLogPath = AttachSetting("ali_global_logpath", Go2ItProjectSettings.Instance.AliGlobalCadLogPath, p);
            Go2ItProjectSettings.Instance.AliGlobalCadConfigPath = AttachSetting("ali_global_configpath", Go2ItProjectSettings.Instance.AliGlobalCadConfigPath, p);
            Go2ItProjectSettings.Instance.AliGlobalCadArchivePath = AttachSetting("ali_global_archivepath", Go2ItProjectSettings.Instance.AliGlobalCadArchivePath, p);
            Go2ItProjectSettings.Instance.AliSdrServerDbPath = AttachSetting("ali_sdrserver_dbpath", Go2ItProjectSettings.Instance.AliSdrServerDbPath, p);
            Go2ItProjectSettings.Instance.AliSdrServerUdpHost = AttachSetting("ali_sdrserver_udphost", Go2ItProjectSettings.Instance.AliSdrServerUdpHost, p);
            Go2ItProjectSettings.Instance.AliSdrServerUdpPort = AttachSetting("ali_sdrserver_udpport", Go2ItProjectSettings.Instance.AliSdrServerUdpPort, p);
            Go2ItProjectSettings.Instance.AliNetworkfleetUdpHost = AttachSetting("ali_networkfleet_udphost", Go2ItProjectSettings.Instance.AliNetworkfleetUdpHost, p);
            Go2ItProjectSettings.Instance.AliNetworkfleetUdpPort = AttachSetting("ali_networkfleet_udpport", Go2ItProjectSettings.Instance.AliNetworkfleetUdpPort, p);
            Go2ItProjectSettings.Instance.AliUseNetworkfleet = AttachSetting("ali_use_networkfleet", Go2ItProjectSettings.Instance.AliUseNetworkfleet, p);

            Go2ItProjectSettings.Instance.AliUseEnterpolAvl = AttachSetting("ali_use_enterpol_avl", Go2ItProjectSettings.Instance.AliUseEnterpolAvl, p);
            Go2ItProjectSettings.Instance.AliEnterpolAvlTableName = AttachSetting("ali_enterpol_avl_tablename", Go2ItProjectSettings.Instance.AliEnterpolAvlTableName, p);
            Go2ItProjectSettings.Instance.AliEnterpolAvlInitialCatalog = AttachSetting("ali_enterpol_avl_initcatalog", Go2ItProjectSettings.Instance.AliEnterpolAvlInitialCatalog, p);
            Go2ItProjectSettings.Instance.AliEnterpolAvlSetMyLocProc = AttachSetting("ali_enterpol_avl_setcurlocproc", Go2ItProjectSettings.Instance.AliEnterpolAvlSetMyLocProc, p);
            Go2ItProjectSettings.Instance.AliEnterpolAvlWhoAmIProc = AttachSetting("ali_enterpol_avl_whoamiproc", Go2ItProjectSettings.Instance.AliEnterpolAvlWhoAmIProc, p);
            Go2ItProjectSettings.Instance.AliEnterpolAvlUpdateFreq = AttachSetting("ali_enterpol_avl_updatefreq", Go2ItProjectSettings.Instance.AliEnterpolAvlUpdateFreq, p);
            Go2ItProjectSettings.Instance.AliEnterpolAvlReadFreq = AttachSetting("ali_enterpol_avl_readfreq", Go2ItProjectSettings.Instance.AliEnterpolAvlReadFreq, p);
            Go2ItProjectSettings.Instance.AliEnterpolAvlAge1Freq = AttachSetting("ali_enterpol_avl_age1freq", Go2ItProjectSettings.Instance.AliEnterpolAvlAge1Freq, p);
            Go2ItProjectSettings.Instance.AliEnterpolAvlAge2Freq = AttachSetting("ali_enterpol_avl_age2freq", Go2ItProjectSettings.Instance.AliEnterpolAvlAge2Freq, p);
            Go2ItProjectSettings.Instance.AliEnterpolAvlAge3Freq = AttachSetting("ali_enterpol_avl_age3freq", Go2ItProjectSettings.Instance.AliEnterpolAvlAge3Freq, p);

            Go2ItProjectSettings.Instance.AliAvlAutoHideInactiveUnits = AttachSetting("ali_avl_autohideinactive", Go2ItProjectSettings.Instance.AliAvlAutoHideInactiveUnits, p);

            // needs to be final ali setting loaded as it fires a change event
            Go2ItProjectSettings.Instance.AliMode = AttachSetting("ali_mode", Go2ItProjectSettings.Instance.AliMode, p);

            const string gsQuery = "SELECT * FROM GraphicSettings";
            DataTable g = SQLiteHelper.GetDataTable(conn, gsQuery);
            Go2ItProjectSettings.Instance.GraphicPointColor = AttachSetting("point_color", Go2ItProjectSettings.Instance.GraphicPointColor, g);
            Go2ItProjectSettings.Instance.GraphicPointStyle = AttachSetting("point_style", Go2ItProjectSettings.Instance.GraphicPointStyle, g);
            Go2ItProjectSettings.Instance.GraphicPointSize = AttachSetting("point_size",Go2ItProjectSettings.Instance.GraphicPointSize,g);
            Go2ItProjectSettings.Instance.GraphicLineBorderColor = AttachSetting("line_border_color", Go2ItProjectSettings.Instance.GraphicLineBorderColor, g);
            Go2ItProjectSettings.Instance.GraphicLineColor = AttachSetting("line_color", Go2ItProjectSettings.Instance.GraphicLineColor, g);
            Go2ItProjectSettings.Instance.GraphicLineSize = AttachSetting("line_size", Go2ItProjectSettings.Instance.GraphicLineSize, g);
            Go2ItProjectSettings.Instance.GraphicLineStyle = AttachSetting("line_style", Go2ItProjectSettings.Instance.GraphicLineStyle, g);
            Go2ItProjectSettings.Instance.GraphicLineCap = AttachSetting("line_cap", Go2ItProjectSettings.Instance.GraphicLineCap, g);

            Go2ItProjectSettings.Instance.AliNetworkfleetColor = AttachSetting("ali_networkfleet_color", Go2ItProjectSettings.Instance.AliNetworkfleetColor, g);
            Go2ItProjectSettings.Instance.AliNetworkfleetChar = AttachSetting("ali_networkfleet_char", Go2ItProjectSettings.Instance.AliNetworkfleetChar, g);
            Go2ItProjectSettings.Instance.AliNetworkfleetFont = AttachSetting("ali_networkfleet_font", Go2ItProjectSettings.Instance.AliNetworkfleetFont, g);
            Go2ItProjectSettings.Instance.AliEnterpolAvlSymbolFont = AttachSetting("ali_enterpol_avl_symbol_font", Go2ItProjectSettings.Instance.AliEnterpolAvlSymbolFont, g);
            Go2ItProjectSettings.Instance.AliEnterpolAvlLabelFont = AttachSetting("ali_enterpol_avl_label_font", Go2ItProjectSettings.Instance.AliEnterpolAvlLabelFont, g);
            Go2ItProjectSettings.Instance.AliEnterpolAvlLabelAlignment = AttachSetting("ali_enterpol_avl_label_alignment", Go2ItProjectSettings.Instance.AliEnterpolAvlLabelAlignment, g);
            Go2ItProjectSettings.Instance.AliEnterpolAvlLabelXOffset = AttachSetting("ali_enterpol_avl_label_xoffset", Go2ItProjectSettings.Instance.AliEnterpolAvlLabelXOffset, g);
            Go2ItProjectSettings.Instance.AliEnterpolAvlLabelYOffset = AttachSetting("ali_enterpol_avl_label_yoffset", Go2ItProjectSettings.Instance.AliEnterpolAvlLabelYOffset, g);

            Go2ItProjectSettings.Instance.AliEnterpolAvlFdChar = AttachSetting("ali_enterpol_avl_fdchar", Go2ItProjectSettings.Instance.AliEnterpolAvlFdChar, g);
            Go2ItProjectSettings.Instance.AliEnterpolAvlFdColor = AttachSetting("ali_enterpol_avl_fdcolor", Go2ItProjectSettings.Instance.AliEnterpolAvlFdColor, g);
            Go2ItProjectSettings.Instance.AliEnterpolAvlLeChar = AttachSetting("ali_enterpol_avl_lechar", Go2ItProjectSettings.Instance.AliEnterpolAvlLeChar, g);
            Go2ItProjectSettings.Instance.AliEnterpolAvlLeColor = AttachSetting("ali_enterpol_avl_lecolor", Go2ItProjectSettings.Instance.AliEnterpolAvlLeColor, g);
            Go2ItProjectSettings.Instance.AliEnterpolAvlEmsChar = AttachSetting("ali_enterpol_avl_emschar", Go2ItProjectSettings.Instance.AliEnterpolAvlEmsChar, g);
            Go2ItProjectSettings.Instance.AliEnterpolAvlEmsColor = AttachSetting("ali_enterpol_avl_emscolor", Go2ItProjectSettings.Instance.AliEnterpolAvlEmsColor, g);
            Go2ItProjectSettings.Instance.AliEnterpolAvlMyColor = AttachSetting("ali_enterpol_avl_mycolor", Go2ItProjectSettings.Instance.AliEnterpolAvlMyColor, g);
            Go2ItProjectSettings.Instance.AliAvlInactiveColor = AttachSetting("ali_avl_inactivecolor", Go2ItProjectSettings.Instance.AliAvlInactiveColor, g);

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
            return SQLiteHelper.DatabaseFileExists(dbPath);
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
            // copy the default database to the target location
            if (String.IsNullOrEmpty(dbPath))
            {
                throw new ArgumentException(@"dbPath is null or empty.", dbPath);
            }
            //to create the default.sqlite database file
            try
            {
                var asm = Assembly.GetCallingAssembly();
                var dirName = Path.GetDirectoryName(asm.Location);
                if (dirName != null)
                {
                    var p = dirName.ToString(CultureInfo.InvariantCulture) + "\\Resources\\defaultDatabase.sqlite";
                    File.Copy(p, dbPath);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                // TODO: replace debug with proper logging
                Debug.WriteLine("Error creating the database " + dbPath +
                    ". Please check your write permissions. " + ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                // TODO: replace debug with proper logging
                Debug.WriteLine("Error creating the default database " + dbPath +
                    ". Error details: " + ex.Message);
                return false;
            }
            return File.Exists(dbPath);
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