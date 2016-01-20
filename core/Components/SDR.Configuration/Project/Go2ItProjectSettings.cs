using System;
using System.Drawing;

namespace SDR.Configuration.Project
{
    /// <summary>
    /// Project - level settings including layerKeys, and field index information
    /// </summary>
    public class Go2ItProjectSettings
    {
        /// <summary>
        /// Access SiteStructure.Instance to get the singleton object.
        /// Then call methods on that instance.
        /// </summary>
        public static Go2ItProjectSettings Instance { get; private set; }

        /// <summary>
        /// Creates a new settings object with default values.
        /// This is a private constructor, meaning no outsiders have access.
        /// </summary>
        private Go2ItProjectSettings()
        {
        }

        static Go2ItProjectSettings()
        {
            Instance = new Go2ItProjectSettings();
        }

        public void ResetProjectSettings()
        {
            Properties.ProjectSettings.Default.AddressesLayers.Clear();
            Properties.ProjectSettings.Default.RoadsLayers.Clear();
            Properties.ProjectSettings.Default.KeyLocationsLayers.Clear();
            Properties.ProjectSettings.Default.NotesLayer = string.Empty;
            Properties.ProjectSettings.Default.CityLimitsLayer = string.Empty;
            Properties.ProjectSettings.Default.CellSectorsLayer = string.Empty;
            Properties.ProjectSettings.Default.EsnsLayer = string.Empty;
            Properties.ProjectSettings.Default.ParcelsLayer = string.Empty;
            Properties.ProjectSettings.Default.HydrantsLayer = string.Empty;
            Properties.ProjectSettings.Default.ActiveMapViewKey = "";
            Properties.ProjectSettings.Default.ActiveMapViewCaption = "";
            Properties.ProjectSettings.Default.AddressesProjectType = "POINT";
            Properties.ProjectSettings.Default.KeyLocationsProjectType = "POINT";
            Properties.ProjectSettings.Default.MapBgColor = Color.Black;
            Properties.ProjectSettings.Default.GraphicLineBorderColor = Color.Orange;
            Properties.ProjectSettings.Default.GraphicLineCap = "Flat";
            Properties.ProjectSettings.Default.GraphicLineColor = Color.Orange;
            Properties.ProjectSettings.Default.GraphicLineSize = 5;
            Properties.ProjectSettings.Default.GraphicLineStyle = "Solid";
            Properties.ProjectSettings.Default.GraphicPointColor = Color.LimeGreen;
            Properties.ProjectSettings.Default.GraphicPointSize = 18;
            Properties.ProjectSettings.Default.GraphicPointStyle = "Triangle";

            Properties.ProjectSettings.Default.HydrantSearchCount = 3;
            Properties.ProjectSettings.Default.HydrantSearchDistance = 250;
            Properties.ProjectSettings.Default.SearchUsePretypes = false;
            Properties.ProjectSettings.Default.SearchZoomFactor = (decimal) 0.05;
            Properties.ProjectSettings.Default.SearchBufferDistance = 250;
            Properties.ProjectSettings.Default.SearchQueryParserLogging = false;

            Properties.ProjectSettings.Default.AliMode = "Disabled";
            Properties.ProjectSettings.Default.AliEnterpolDataSource = string.Empty;
            Properties.ProjectSettings.Default.AliEnterpolTableName = string.Empty;
            Properties.ProjectSettings.Default.AliEnterpolInitialCatalog = "SDRCADInterface";
            Properties.ProjectSettings.Default.AliGlobalCadLogPath = string.Empty;
            Properties.ProjectSettings.Default.AliGlobalCadArchivePath = string.Empty;
            Properties.ProjectSettings.Default.AliGlobalCadConfigPath = string.Empty;
            Properties.ProjectSettings.Default.AliSdrServerDbPath = string.Empty;
            Properties.ProjectSettings.Default.AliSdrServerUdpHost = "127.0.0.1";
            Properties.ProjectSettings.Default.AliSdrServerUdpPort = 777;

            Properties.ProjectSettings.Default.AliUseNetworkfleet = false;
            Properties.ProjectSettings.Default.AliNetworkfleetUdpHost = "127.0.0.1";
            Properties.ProjectSettings.Default.AliNetworkfleetUdpPort = 1111;
            Properties.ProjectSettings.Default.AliNetworkfleetPointChar = 'n';
            Properties.ProjectSettings.Default.AliNetworkfleetPointColor = Color.DarkTurquoise;
            Properties.ProjectSettings.Default.AliNetworkfleetPointFont = new Font("ESRI Transportation & Civic", 20);

            Properties.ProjectSettings.Default.AliUseEnterpolAVL = false;
            Properties.ProjectSettings.Default.AliEnterpolAVLTableName = string.Empty;
            Properties.ProjectSettings.Default.AliEnterpolAVLInitialCatalog = "SDRActiveUnitsView";
            Properties.ProjectSettings.Default.AliEnterpolAVLSetMyLocProc = "SDR_SetMyLocation";
            Properties.ProjectSettings.Default.AliEnterpolAVLWhoAmIProc = "SDR_WhoAmI";
            Properties.ProjectSettings.Default.AliEnterpolAVLUpdateFreq = 5000;
            Properties.ProjectSettings.Default.AliEnterpolAVLAge1Freq = 2;
            Properties.ProjectSettings.Default.AliEnterpolAVLAge2Freq = 5;
            Properties.ProjectSettings.Default.AliEnterpolAVLAge3Freq = 60;
            Properties.ProjectSettings.Default.AliEnterpolAVLReadFreq = 50000;
            Properties.ProjectSettings.Default.AliEnterpolAVLFont = new Font("Microsoft Sans Serif", 8);
            Properties.ProjectSettings.Default.AliEnterpolAVLLEChar = 'p';
            Properties.ProjectSettings.Default.AliEnterpolAVLFDChar = 'f';
            Properties.ProjectSettings.Default.AliEnterpolAVLEMSChar = 'e';
            Properties.ProjectSettings.Default.AliEnterpolAVLLEColor = Color.Cyan;
            Properties.ProjectSettings.Default.AliEnterpolAVLFDColor = Color.Red;
            Properties.ProjectSettings.Default.AliEnterpolAVLEMSColor = Color.LimeGreen;
            Properties.ProjectSettings.Default.AliEnterpolAVLMyVehicleColor = Color.Fuchsia;

            Properties.ProjectSettings.Default.AliAvlAutoHideInactiveUnits = true;
        }

        /// <summary>
        /// Gets the list of address layers indexed
        /// </summary>
        public System.Collections.Specialized.StringCollection AddressLayers
        {
            get { return Properties.ProjectSettings.Default.AddressesLayers; }
            set { Properties.ProjectSettings.Default.AddressesLayers = value; }
        }

        /// <summary>
        /// Add a address layer to the current list of address layers
        /// </summary>
        public void AddAddressLayer(string layer)
        {
            if (!Properties.ProjectSettings.Default.AddressesLayers.Contains(layer))
            {
                Properties.ProjectSettings.Default.AddressesLayers.Add(layer);
            }
        }

        /// <summary>
        /// clears the list of address layers indexed
        /// </summary>
        public void ClearAddressLayers()
        {
            Properties.ProjectSettings.Default.AddressesLayers.Clear();
        }

        /// <summary>
        /// Gets the list of road layers indexed
        /// </summary>
        public System.Collections.Specialized.StringCollection RoadLayers
        {
            get { return Properties.ProjectSettings.Default.RoadsLayers; }
            set { Properties.ProjectSettings.Default.RoadsLayers = value; }
        }

        /// <summary>
        /// Add a road layer to the current list of road layers
        /// </summary>
        public void AddRoadLayer(string layer)
        {
            if (!Properties.ProjectSettings.Default.RoadsLayers.Contains(layer))
            {
                Properties.ProjectSettings.Default.RoadsLayers.Add(layer);
            }
        }

        /// <summary>
        /// clears the list of road layers indexed
        /// </summary>
        public void ClearRoadLayers()
        {
            Properties.ProjectSettings.Default.RoadsLayers.Clear();
        }

        /// <summary>
        /// Gets the list of keylocation layers indexed
        /// </summary>
        public System.Collections.Specialized.StringCollection KeyLocationLayers
        {
            get { return Properties.ProjectSettings.Default.KeyLocationsLayers; }
            set { Properties.ProjectSettings.Default.KeyLocationsLayers = value; }
        }

        /// <summary>
        /// Add a key location layer to the current list of key location layers
        /// </summary>
        public void AddKeyLocationLayer(string layer)
        {
            if (!Properties.ProjectSettings.Default.KeyLocationsLayers.Contains(layer))
            {
                Properties.ProjectSettings.Default.KeyLocationsLayers.Add(layer);
            }
        }

        /// <summary>
        /// clears the list of key location layers indexed
        /// </summary>
        public void ClearKeyLocationLayers()
        {
            Properties.ProjectSettings.Default.KeyLocationsLayers.Clear();
        }

        /// <summary>
        /// Active Map View tab caption
        /// </summary>
        public string ActiveMapViewCaption
        {
            set { Properties.ProjectSettings.Default.ActiveMapViewCaption = value; }
            get { return Properties.ProjectSettings.Default.ActiveMapViewCaption; }
        }

        /// <summary>
        /// Active Map key lookup
        /// </summary>
        public string ActiveMapViewKey
        {
            set { Properties.ProjectSettings.Default.ActiveMapViewKey = value; }
            get { return Properties.ProjectSettings.Default.ActiveMapViewKey; }
        }

        /// <summary>
        /// Use Pretypes in search
        /// </summary>
        public bool SearchUsePretypes
        {
            set { Properties.ProjectSettings.Default.SearchUsePretypes = value; }
            get { return Properties.ProjectSettings.Default.SearchUsePretypes; }
        }

        /// <summary>
        /// Enable Logging of Search Querys and Parsed Results
        /// </summary>
        public bool SearchQueryParserLogging
        {
            set { Properties.ProjectSettings.Default.SearchQueryParserLogging = value; }
            get { return Properties.ProjectSettings.Default.SearchQueryParserLogging; }
        }

        public Color MapBgColor
        {
            set { Properties.ProjectSettings.Default.MapBgColor = value; }
            get { return Properties.ProjectSettings.Default.MapBgColor; }
        }

        /// <summary>
        /// Active Notes Layer
        /// </summary>
        public string NotesLayer
        {
            set { Properties.ProjectSettings.Default.NotesLayer = value; }
            get { return Properties.ProjectSettings.Default.NotesLayer; }
        }

        /// <summary>
        /// Active City Limits Layer
        /// </summary>
        public string CityLimitsLayer
        {
            set { Properties.ProjectSettings.Default.CityLimitsLayer = value; }
            get { return Properties.ProjectSettings.Default.CityLimitsLayer; }
        }

        /// <summary>
        /// Active Cell Sectors Layer
        /// </summary>
        public string CellSectorsLayer
        {
            set { Properties.ProjectSettings.Default.CellSectorsLayer = value; }
            get { return Properties.ProjectSettings.Default.CellSectorsLayer; }
        }

        /// <summary>
        /// Active ESN Layer
        /// </summary>
        public string EsnsLayer
        {
            set { Properties.ProjectSettings.Default.EsnsLayer = value; }
            get { return Properties.ProjectSettings.Default.EsnsLayer; }
        }

        /// <summary>
        /// Active Parcels Layer
        /// </summary>
        public string ParcelsLayer
        {
            set { Properties.ProjectSettings.Default.ParcelsLayer = value; }
            get { return Properties.ProjectSettings.Default.ParcelsLayer; }
        }

        /// <summary>
        /// Active Hydrants Layer
        /// </summary>
        public string HydrantsLayer
        {
            set { Properties.ProjectSettings.Default.HydrantsLayer = value; }
            get { return Properties.ProjectSettings.Default.HydrantsLayer; }
        }

        /// <summary>
        /// Address layers are of type Polygon or Point
        /// </summary>
        public string AddressesProjectType
        {
            set { Properties.ProjectSettings.Default.AddressesProjectType = value; }
            get { return Properties.ProjectSettings.Default.AddressesProjectType; }
        }

        /// <summary>
        /// Key Location layers are of type Polygon or Point
        /// </summary>
        public string KeyLocationsProjectType
        {
            set { Properties.ProjectSettings.Default.KeyLocationsProjectType = value; }
            get { return Properties.ProjectSettings.Default.KeyLocationsProjectType; }
        }

        public Color GraphicPointColor
        {
            set { Properties.ProjectSettings.Default.GraphicPointColor = value; }
            get { return Properties.ProjectSettings.Default.GraphicPointColor; }
        }

        public string GraphicPointStyle
        {
            set { Properties.ProjectSettings.Default.GraphicPointStyle = value; }
            get { return Properties.ProjectSettings.Default.GraphicPointStyle; }
        }

        public int GraphicPointSize
        {
            set { Properties.ProjectSettings.Default.GraphicPointSize = value; }
            get { return Properties.ProjectSettings.Default.GraphicPointSize; }
        }

        public Color GraphicLineColor
        {
            set { Properties.ProjectSettings.Default.GraphicLineColor = value; }
            get { return Properties.ProjectSettings.Default.GraphicLineColor; }
        }

        public Color GraphicLineBorderColor
        {
            set { Properties.ProjectSettings.Default.GraphicLineBorderColor = value; }
            get { return Properties.ProjectSettings.Default.GraphicLineBorderColor; }
        }

        public int GraphicLineSize
        {
            set { Properties.ProjectSettings.Default.GraphicLineSize = value; }
            get { return Properties.ProjectSettings.Default.GraphicLineSize; }
        }

        public string GraphicLineCap
        {
            set { Properties.ProjectSettings.Default.GraphicLineCap = value; }
            get { return Properties.ProjectSettings.Default.GraphicLineCap; }
        }

        public string GraphicLineStyle
        {
            set { Properties.ProjectSettings.Default.GraphicLineStyle = value; }
            get { return Properties.ProjectSettings.Default.GraphicLineStyle; }
        }

        public Font AliNetworkfleetFont
        {
            set { Properties.ProjectSettings.Default.AliNetworkfleetPointFont = value; }
            get { return Properties.ProjectSettings.Default.AliNetworkfleetPointFont; }
        }

        public Char AliNetworkfleetChar
        {
            set { Properties.ProjectSettings.Default.AliNetworkfleetPointChar = value; }
            get { return Properties.ProjectSettings.Default.AliNetworkfleetPointChar; }
        }

        public Color AliNetworkfleetColor
        {
            set { Properties.ProjectSettings.Default.AliNetworkfleetPointColor = value; }
            get { return Properties.ProjectSettings.Default.AliNetworkfleetPointColor; }
        }

        public int HydrantSearchCount
        {
            set { Properties.ProjectSettings.Default.HydrantSearchCount = value; }
            get { return Properties.ProjectSettings.Default.HydrantSearchCount; }
        }

        public decimal SearchZoomFactor
        {
            set { Properties.ProjectSettings.Default.SearchZoomFactor = value; }
            get { return Properties.ProjectSettings.Default.SearchZoomFactor; }
        }

        public int SearchBufferDistance
        {
            set { Properties.ProjectSettings.Default.SearchBufferDistance = value; }
            get { return Properties.ProjectSettings.Default.SearchBufferDistance; }
        }

        public int HydrantSearchDistance
        {
            set { Properties.ProjectSettings.Default.HydrantSearchDistance = value; }
            get { return Properties.ProjectSettings.Default.HydrantSearchDistance; }
        }

        public event EventHandler AliModeChanged;
        public string AliMode
        {
            get {
                return Properties.ProjectSettings.Default.AliMode; 
            }
            set
            {
                if (Properties.ProjectSettings.Default.AliMode != value)
                {
                    Properties.ProjectSettings.Default.AliMode = value;
                    OnAliModeChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliModeChanged(EventArgs e)
        {
            if (AliModeChanged != null)
                AliModeChanged(this, e);
        }

        public event EventHandler AliSdrServerDbPathChanged;
        public string AliSdrServerDbPath
        {
            get { return Properties.ProjectSettings.Default.AliSdrServerDbPath; }
            set
            {
                if (Properties.ProjectSettings.Default.AliSdrServerDbPath != value)
                {
                    Properties.ProjectSettings.Default.AliSdrServerDbPath = value;
                    OnAliSdrServerDbPathChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliSdrServerDbPathChanged(EventArgs e)
        {
            if (AliSdrServerDbPathChanged != null)
                AliSdrServerDbPathChanged(this, e);
        }

        public event EventHandler AliSdrServerUdpHostChanged;
        public string AliSdrServerUdpHost
        {
            get { return Properties.ProjectSettings.Default.AliSdrServerUdpHost; }
            set
            {
                if (Properties.ProjectSettings.Default.AliSdrServerUdpHost != value)
                {
                    Properties.ProjectSettings.Default.AliSdrServerUdpHost = value;
                    OnAliSdrServeUdpHostChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliSdrServeUdpHostChanged(EventArgs e)
        {
            if (AliSdrServerUdpHostChanged != null)
                AliSdrServerUdpHostChanged(this, e);
        }

        public event EventHandler AliSdrServerUdpPortChanged;
        public int AliSdrServerUdpPort
        {
            get { return Properties.ProjectSettings.Default.AliSdrServerUdpPort; }
            set
            {
                if (Properties.ProjectSettings.Default.AliSdrServerUdpPort != value)
                {
                    Properties.ProjectSettings.Default.AliSdrServerUdpPort = value;
                    OnAliSdrServeUdpPortChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliSdrServeUdpPortChanged(EventArgs e)
        {
            if (AliSdrServerUdpPortChanged != null)
                AliSdrServerUdpPortChanged(this, e);
        }

        public event EventHandler AliNetworkfleetUdpHostChanged;
        public string AliNetworkfleetUdpHost
        {
            get { return Properties.ProjectSettings.Default.AliNetworkfleetUdpHost; }
            set
            {
                if (Properties.ProjectSettings.Default.AliNetworkfleetUdpHost != value)
                {
                    Properties.ProjectSettings.Default.AliNetworkfleetUdpHost = value;
                    OnAliNetworkfleetUdpHostChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliNetworkfleetUdpHostChanged(EventArgs e)
        {
            if (AliNetworkfleetUdpHostChanged != null)
                AliNetworkfleetUdpHostChanged(this, e);
        }

        public event EventHandler AliNetworkfleetUdpPortChanged;
        public int AliNetworkfleetUdpPort
        {
            get { return Properties.ProjectSettings.Default.AliNetworkfleetUdpPort; }
            set
            {
                if (Properties.ProjectSettings.Default.AliNetworkfleetUdpPort != value)
                {
                    Properties.ProjectSettings.Default.AliNetworkfleetUdpPort = value;
                    OnAliSdrServeUdpPortChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliNetworkfleetUdpPortChanged(EventArgs e)
        {
            if (AliNetworkfleetUdpPortChanged != null)
                AliNetworkfleetUdpPortChanged(this, e);
        }

        public event EventHandler AliUseNetworkfleetChanged;
        public bool AliUseNetworkfleet
        {
            get { return Properties.ProjectSettings.Default.AliUseNetworkfleet; }
            set
            {
                if (Properties.ProjectSettings.Default.AliUseNetworkfleet != value)
                {
                    Properties.ProjectSettings.Default.AliUseNetworkfleet = value;
                    OnAliUseNetworkfleetChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliUseNetworkfleetChanged(EventArgs e)
        {
            if (AliUseNetworkfleetChanged != null)
                AliUseNetworkfleetChanged(this, e);
        }

        public event EventHandler AliGlobalCadLogPathChanged;
        public string AliGlobalCadLogPath
        {
            get { return Properties.ProjectSettings.Default.AliGlobalCadLogPath; }
            set
            {
                if (Properties.ProjectSettings.Default.AliGlobalCadLogPath != value)
                {
                    Properties.ProjectSettings.Default.AliGlobalCadLogPath = value;
                    OnAliGlobalCadLogPathChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliGlobalCadLogPathChanged(EventArgs e)
        {
            if (AliGlobalCadLogPathChanged != null)
                AliGlobalCadLogPathChanged(this, e);
        }

        public event EventHandler AliGlobalCadConfigPathChanged;
        public string AliGlobalCadConfigPath
        {
            get { return Properties.ProjectSettings.Default.AliGlobalCadConfigPath; }
            set
            {
                if (Properties.ProjectSettings.Default.AliGlobalCadConfigPath != value)
                {
                    Properties.ProjectSettings.Default.AliGlobalCadConfigPath = value;
                    OnAliGlobalCadConfigPathChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliGlobalCadConfigPathChanged(EventArgs e)
        {
            if (AliGlobalCadConfigPathChanged != null)
                AliGlobalCadConfigPathChanged(this, e);
        }

        public event EventHandler AliGlobalCadArchivePathChanged;
        public string AliGlobalCadArchivePath
        {
            get { return Properties.ProjectSettings.Default.AliGlobalCadArchivePath; }
            set
            {
                if (Properties.ProjectSettings.Default.AliGlobalCadArchivePath != value)
                {
                    Properties.ProjectSettings.Default.AliGlobalCadArchivePath = value;
                    OnAliGlobalCadArchivePathChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliGlobalCadArchivePathChanged(EventArgs e)
        {
            if (AliGlobalCadArchivePathChanged != null)
                AliGlobalCadArchivePathChanged(this, e);
        }

        public event EventHandler AliEnterpolTableNameChanged;
        public string AliEnterpolTableName
        {
            get { return Properties.ProjectSettings.Default.AliEnterpolTableName; }
            set
            {
                if (Properties.ProjectSettings.Default.AliEnterpolTableName != value)
                {
                    Properties.ProjectSettings.Default.AliEnterpolTableName = value;
                    OnAliEnterpolTableNameChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliEnterpolTableNameChanged(EventArgs e)
        {
            if (AliEnterpolTableNameChanged != null)
                AliEnterpolTableNameChanged(this, e);
        }

        public event EventHandler AliEnterpolDataSourceChanged;
        public string AliEnterpolDataSource
        {
            get { return Properties.ProjectSettings.Default.AliEnterpolDataSource; }
            set
            {
                if (Properties.ProjectSettings.Default.AliEnterpolDataSource != value)
                {
                    Properties.ProjectSettings.Default.AliEnterpolDataSource = value;
                    OnAliEnterpolDataSourceChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliEnterpolDataSourceChanged(EventArgs e)
        {
            if (AliEnterpolDataSourceChanged != null)
                AliEnterpolDataSourceChanged(this, e);
        }

        public event EventHandler AliEnterpolInitialCatalogChanged;
        public string AliEnterpolInitialCatalog
        {
            get { return Properties.ProjectSettings.Default.AliEnterpolInitialCatalog; }
            set
            {
                if (Properties.ProjectSettings.Default.AliEnterpolInitialCatalog != value)
                {
                    Properties.ProjectSettings.Default.AliEnterpolInitialCatalog = value;
                    OnAliEnterpolInitialCatalogChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliEnterpolInitialCatalogChanged(EventArgs e)
        {
            if (AliEnterpolInitialCatalogChanged != null)
                AliEnterpolInitialCatalogChanged(this, e);
        }

        public event EventHandler AliUseEnterpolAvlChanged;
        public bool AliUseEnterpolAvl
        {
            get { return Properties.ProjectSettings.Default.AliUseEnterpolAVL; }
            set
            {
                if (Properties.ProjectSettings.Default.AliUseNetworkfleet != value)
                {
                    Properties.ProjectSettings.Default.AliUseEnterpolAVL = value;
                    OnAliUseEnterpolAvlChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliUseEnterpolAvlChanged(EventArgs e)
        {
            if (AliUseEnterpolAvlChanged != null)
                AliUseEnterpolAvlChanged(this, e);
        }

        public event EventHandler AliEnterpolAvlInitialCatalogChanged;
        public string AliEnterpolAvlInitialCatalog
        {
            get { return Properties.ProjectSettings.Default.AliEnterpolAVLInitialCatalog; }
            set
            {
                if (Properties.ProjectSettings.Default.AliEnterpolAVLInitialCatalog != value)
                {
                    Properties.ProjectSettings.Default.AliEnterpolAVLInitialCatalog = value;
                    OnAliEnterpolAvlInitialCatalogChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliEnterpolAvlInitialCatalogChanged(EventArgs e)
        {
            if (AliEnterpolAvlInitialCatalogChanged != null)
                AliEnterpolAvlInitialCatalogChanged(this, e);
        }

        public event EventHandler AliEnterpolAvlTableNameChanged;
        public string AliEnterpolAvlTableName
        {
            get { return Properties.ProjectSettings.Default.AliEnterpolAVLTableName; }
            set
            {
                if (Properties.ProjectSettings.Default.AliEnterpolAVLTableName != value)
                {
                    Properties.ProjectSettings.Default.AliEnterpolAVLTableName = value;
                    OnAliEnterpolAvlTableNameChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliEnterpolAvlTableNameChanged(EventArgs e)
        {
            if (AliEnterpolAvlTableNameChanged != null)
                AliEnterpolAvlTableNameChanged(this, e);
        }

        public event EventHandler AliEnterpolAvlSetMyLocProcChanged;
        public string AliEnterpolAvlSetMyLocProc
        {
            get { return Properties.ProjectSettings.Default.AliEnterpolAVLSetMyLocProc; }
            set
            {
                if (Properties.ProjectSettings.Default.AliEnterpolAVLSetMyLocProc != value)
                {
                    Properties.ProjectSettings.Default.AliEnterpolAVLSetMyLocProc = value;
                    OnAliEnterpolAvlSetMyLocProcChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliEnterpolAvlSetMyLocProcChanged(EventArgs e)
        {
            if (AliEnterpolAvlSetMyLocProcChanged != null)
                AliEnterpolAvlSetMyLocProcChanged(this, e);
        }

        public event EventHandler AliEnterpolAvlWhoAmIProcChanged;
        public string AliEnterpolAvlWhoAmIProc
        {
            get { return Properties.ProjectSettings.Default.AliEnterpolAVLWhoAmIProc; }
            set
            {
                if (Properties.ProjectSettings.Default.AliEnterpolAVLWhoAmIProc != value)
                {
                    Properties.ProjectSettings.Default.AliEnterpolAVLWhoAmIProc = value;
                    OnAliEnterpolAvlWhoAmIProcChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliEnterpolAvlWhoAmIProcChanged(EventArgs e)
        {
            if (AliEnterpolAvlWhoAmIProcChanged != null)
                AliEnterpolAvlWhoAmIProcChanged(this, e);
        }

        public event EventHandler AliEnterpolAvlUpdateFreqChanged;
        public int AliEnterpolAvlUpdateFreq
        {
            get { return Properties.ProjectSettings.Default.AliEnterpolAVLUpdateFreq; }
            set
            {
                if (Properties.ProjectSettings.Default.AliEnterpolAVLUpdateFreq != value)
                {
                    Properties.ProjectSettings.Default.AliEnterpolAVLUpdateFreq = value;
                    OnAliEnterpolAvlUpdateFreqChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliEnterpolAvlUpdateFreqChanged(EventArgs e)
        {
            if (AliEnterpolAvlUpdateFreqChanged != null)
                AliEnterpolAvlUpdateFreqChanged(this, e);
        }

        public event EventHandler AliEnterpolAvlAge1FreqChanged;
        public int AliEnterpolAvlAge1Freq
        {
            get { return Properties.ProjectSettings.Default.AliEnterpolAVLAge1Freq; }
            set
            {
                if (Properties.ProjectSettings.Default.AliEnterpolAVLAge1Freq != value)
                {
                    Properties.ProjectSettings.Default.AliEnterpolAVLAge1Freq = value;
                    OnAliEnterpolAvlAge1FreqChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliEnterpolAvlAge1FreqChanged(EventArgs e)
        {
            if (AliEnterpolAvlAge1FreqChanged != null)
                AliEnterpolAvlAge1FreqChanged(this, e);
        }

        public event EventHandler AliEnterpolAvlAge2FreqChanged;
        public int AliEnterpolAvlAge2Freq
        {
            get { return Properties.ProjectSettings.Default.AliEnterpolAVLAge2Freq; }
            set
            {
                if (Properties.ProjectSettings.Default.AliEnterpolAVLAge2Freq != value)
                {
                    Properties.ProjectSettings.Default.AliEnterpolAVLAge2Freq = value;
                    OnAliEnterpolAvlAge2FreqChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliEnterpolAvlAge2FreqChanged(EventArgs e)
        {
            if (AliEnterpolAvlAge2FreqChanged != null)
                AliEnterpolAvlAge2FreqChanged(this, e);
        }

        public event EventHandler AliEnterpolAvlAge3FreqChanged;
        public int AliEnterpolAvlAge3Freq
        {
            get { return Properties.ProjectSettings.Default.AliEnterpolAVLAge3Freq; }
            set
            {
                if (Properties.ProjectSettings.Default.AliEnterpolAVLAge3Freq != value)
                {
                    Properties.ProjectSettings.Default.AliEnterpolAVLAge3Freq = value;
                    OnAliEnterpolAvlAge3FreqChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliEnterpolAvlAge3FreqChanged(EventArgs e)
        {
            if (AliEnterpolAvlAge3FreqChanged != null)
                AliEnterpolAvlAge3FreqChanged(this, e);
        }

        public event EventHandler AliEnterpolAvlReadFreqChanged;
        public int AliEnterpolAvlReadFreq
        {
            get { return Properties.ProjectSettings.Default.AliEnterpolAVLReadFreq; }
            set
            {
                if (Properties.ProjectSettings.Default.AliEnterpolAVLReadFreq != value)
                {
                    Properties.ProjectSettings.Default.AliEnterpolAVLReadFreq = value;
                    OnAliEnterpolAvlReadFreqChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliEnterpolAvlReadFreqChanged(EventArgs e)
        {
            if (AliEnterpolAvlReadFreqChanged != null)
                AliEnterpolAvlReadFreqChanged(this, e);
        }

        public event EventHandler AliEnterpolAvlFontChanged;
        public Font AliEnterpolAvlFont
        {
            get { return Properties.ProjectSettings.Default.AliEnterpolAVLFont; }
            set
            {
                if (!Equals(Properties.ProjectSettings.Default.AliEnterpolAVLFont, value))
                {
                    Properties.ProjectSettings.Default.AliEnterpolAVLFont = value;
                    OnAliEnterpolAvlFontChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliEnterpolAvlFontChanged(EventArgs e)
        {
            if (AliEnterpolAvlFontChanged != null)
                AliEnterpolAvlFontChanged(this, e);
        }

        public event EventHandler AliAvlAutoHideInactiveUnitsChanged;
        public bool AliAvlAutoHideInactiveUnits
        {
            get { return Properties.ProjectSettings.Default.AliAvlAutoHideInactiveUnits; }
            set
            {
                if (Properties.ProjectSettings.Default.AliAvlAutoHideInactiveUnits != value)
                {
                    Properties.ProjectSettings.Default.AliAvlAutoHideInactiveUnits = value;
                    OnAliEnterpolAvlLeColorChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliAvlAutoHideInactiveUnitsChanged(EventArgs e)
        {
            if (AliAvlAutoHideInactiveUnitsChanged != null)
                AliAvlAutoHideInactiveUnitsChanged(this, e);
        }


        public event EventHandler AliEnterpolAvlLeColorChanged;
        public Color AliEnterpolAvlLeColor
        {
            get { return Properties.ProjectSettings.Default.AliEnterpolAVLLEColor; }
            set
            {
                if (Properties.ProjectSettings.Default.AliEnterpolAVLLEColor != value)
                {
                    Properties.ProjectSettings.Default.AliEnterpolAVLLEColor = value;
                    OnAliEnterpolAvlLeColorChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliEnterpolAvlLeColorChanged(EventArgs e)
        {
            if (AliEnterpolAvlLeColorChanged != null)
                AliEnterpolAvlLeColorChanged(this, e);
        }

        public event EventHandler AliEnterpolAvlEmsColorChanged;
        public Color AliEnterpolAvlEmsColor
        {
            get { return Properties.ProjectSettings.Default.AliEnterpolAVLEMSColor; }
            set
            {
                if (Properties.ProjectSettings.Default.AliEnterpolAVLEMSColor != value)
                {
                    Properties.ProjectSettings.Default.AliEnterpolAVLEMSColor = value;
                    OnAliEnterpolAvlEmsColorChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliEnterpolAvlEmsColorChanged(EventArgs e)
        {
            if (AliEnterpolAvlEmsColorChanged != null)
                AliEnterpolAvlEmsColorChanged(this, e);
        }

        public event EventHandler AliEnterpolAvlFdColorChanged;
        public Color AliEnterpolAvlFdColor
        {
            get { return Properties.ProjectSettings.Default.AliEnterpolAVLFDColor; }
            set
            {
                if (Properties.ProjectSettings.Default.AliEnterpolAVLFDColor != value)
                {
                    Properties.ProjectSettings.Default.AliEnterpolAVLFDColor = value;
                    OnAliEnterpolAvlFdColorChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliEnterpolAvlFdColorChanged(EventArgs e)
        {
            if (AliEnterpolAvlFdColorChanged != null)
                AliEnterpolAvlFdColorChanged(this, e);
        }

        public event EventHandler AliEnterpolAvlLeCharChanged;
        public Char AliEnterpolAvlLeChar
        {
            get { return Properties.ProjectSettings.Default.AliEnterpolAVLLEChar; }
            set
            {
                if (Properties.ProjectSettings.Default.AliEnterpolAVLLEChar != value)
                {
                    Properties.ProjectSettings.Default.AliEnterpolAVLLEChar = value;
                    OnAliEnterpolAvlLeCharChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliEnterpolAvlLeCharChanged(EventArgs e)
        {
            if (AliEnterpolAvlLeCharChanged != null)
                AliEnterpolAvlLeCharChanged(this, e);
        }

        public event EventHandler AliEnterpolAvlEmsCharChanged;
        public Char AliEnterpolAvlEmsChar
        {
            get { return Properties.ProjectSettings.Default.AliEnterpolAVLEMSChar; }
            set
            {
                if (Properties.ProjectSettings.Default.AliEnterpolAVLEMSChar != value)
                {
                    Properties.ProjectSettings.Default.AliEnterpolAVLEMSChar = value;
                    OnAliEnterpolAvlEmsCharChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliEnterpolAvlEmsCharChanged(EventArgs e)
        {
            if (AliEnterpolAvlEmsCharChanged != null)
                AliEnterpolAvlEmsCharChanged(this, e);
        }

        public event EventHandler AliEnterpolAvlFdCharChanged;
        public Char AliEnterpolAvlFdChar
        {
            get { return Properties.ProjectSettings.Default.AliEnterpolAVLFDChar; }
            set
            {
                if (Properties.ProjectSettings.Default.AliEnterpolAVLFDChar != value)
                {
                    Properties.ProjectSettings.Default.AliEnterpolAVLFDChar = value;
                    OnAliEnterpolAvlFdCharChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliEnterpolAvlFdCharChanged(EventArgs e)
        {
            if (AliEnterpolAvlFdCharChanged != null)
                AliEnterpolAvlFdCharChanged(this, e);
        }

        public event EventHandler AliEnterpolAvlMyColorChanged;
        public Color AliEnterpolAvlMyColor
        {
            get { return Properties.ProjectSettings.Default.AliEnterpolAVLMyVehicleColor; }
            set
            {
                if (Properties.ProjectSettings.Default.AliEnterpolAVLMyVehicleColor != value)
                {
                    Properties.ProjectSettings.Default.AliEnterpolAVLMyVehicleColor = value;
                    OnAliEnterpolAvlMyColorChanged(EventArgs.Empty);
                }
            }
        }
        protected virtual void OnAliEnterpolAvlMyColorChanged(EventArgs e)
        {
            if (AliEnterpolAvlMyColorChanged != null)
                AliEnterpolAvlMyColorChanged(this, e);
        }
    }
}
