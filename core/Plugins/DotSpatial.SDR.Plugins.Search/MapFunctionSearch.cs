using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Controls;

namespace DotSpatial.SDR.Plugins.Search
{
    /// <summary>
    /// A MapFunction that allows searching of various configured layers
    /// </summary>
    public class MapFunctionSearch : MapFunction
    {
        private SearchPanel _searchPanel;

        #region Constructors
        
                /// <summary>
        /// Creates a new instance of MapMeasureFunction, with panel
        /// </summary>
        /// <param name="sp"></param>
        public MapFunctionSearch(SearchPanel sp)
        {
            _searchPanel = sp;
            Configure();
        }

        private void Configure()
        {
            YieldStyle = YieldStyles.AlwaysOn;
            HandleSearchPanelEvents();
            Name = "MapFunctionSearch";
        }

        private void HandleSearchPanelEvents()
        {
            _searchPanel.SearchModeChanged += SearchPanelOnSearchModeChanged;
            _searchPanel.SearchesCleared += SearchPanelOnSearchesCleared; 
        }

        private void SearchPanelOnSearchModeChanged(object sender, EventArgs eventArgs)
        {
            /*_previousParts.Clear();

            _areaMode = (_measurePanel.MeasureMode == MeasureMode.Area);
            if (_coordinates != null)
            {
                _coordinates = new List<Coordinate>();
            }
            Map.Invalidate();*/
        }

        private void SearchPanelOnSearchesCleared(object sender, EventArgs eventArgs)
        {
            /*_previousParts.Clear();
            if (_coordinates != null)
                _coordinates.Clear();
            _previousDistance = 0;
            _currentDistance = 0;
            _currentArea = 0;
            Map.MapFrame.Invalidate();
            Map.Invalidate();
            _measurePanel.Distance = 0;
            _measurePanel.TotalDistance = 0;
            _measurePanel.TotalArea = 0;*/
        }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            if (_searchPanel == null || _searchPanel.IsDisposed)
            {
                _searchPanel = new SearchPanel();
                HandleSearchPanelEvents();
            }
            _searchPanel.Show();
            base.OnActivate();
        }

        #endregion
    }
}
