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
            Properties.ProjectSettings.Default.DrivewaysLayer = string.Empty;
            Properties.ProjectSettings.Default.ActiveMapViewKey = string.Empty;
            Properties.ProjectSettings.Default.ActiveMapViewCaption = string.Empty;
            Properties.ProjectSettings.Default.AddressesProjectType = "POINT";
            Properties.ProjectSettings.Default.KeyLocationsProjectType = "POINT";
            Properties.ProjectSettings.Default.MapBgColor = Color.Black;
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
            set { Properties.ProjectSettings.Default.ActiveMapViewCaption = value;  }
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

        public Color MapBgColor
        {
            set { Properties.ProjectSettings.Default.MapBgColor = value; }
            get { return Properties.ProjectSettings.Default.MapBgColor; }
        }

        /// <summary>
        /// Active Driveway Layer
        /// </summary>
        public string DrivewaysLayer
        {
            set { Properties.ProjectSettings.Default.DrivewaysLayer = value; }
            get { return Properties.ProjectSettings.Default.DrivewaysLayer; }
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
    }
}
