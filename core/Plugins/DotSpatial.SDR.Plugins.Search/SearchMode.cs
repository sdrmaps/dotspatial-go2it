namespace DotSpatial.SDR.Plugins.Search
{
    public enum SearchMode
    {
        /// <summary>
        /// Search addresses attributes
        /// </summary>
        Address,
        /// <summary>
        /// Search People's Names
        /// </summary>
        Name,
        /// <summary>
        /// Search Phone Numbers
        /// </summary>
        Phone,
        /// <summary>
        /// Search Roads Attributes
        /// </summary>
        Road,
        /// <summary>
        /// Search Spatial Intersections
        /// </summary>
        Intersection,
        /// <summary>
        /// Search Key Locations Attributes
        /// </summary>
        KeyLocation,
        /// <summary>
        /// Search All Attributes
        /// </summary>
        All,
        /// <summary>
        /// Search City Names
        /// </summary>
        City,
        /// <summary>
        /// Search ESN Names
        /// </summary>
        Esn,
        /// <summary>
        /// Search Cell Sector Coverages
        /// </summary>
        CellSector,
        /// <summary>
        /// Search Parcel Attributes
        /// </summary>
        Parcel,
        /// <summary>
        /// Locate Hydrants near by active Selections
        /// </summary>
        Hydrant,
        /// <summary>
        /// Zoom to a Coordinate
        /// </summary>
        Coordinate,
    }
}
