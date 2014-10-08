using System.Windows.Forms;
using DotSpatial.Controls.Docking;

namespace DotSpatial.SDR.Controls
{
    public class DockPanelInfo
    {
        private static int _dockPanelNumber;

        /// <summary>
        /// The TabPage contents of the dockable panel (Dock Content)
        /// </summary>
        public TabPage DockPanelTab { get; private set; }

        /// <summary>
        /// The DotSpatial dockable panel (used by DS plugin interface)
        /// </summary>
        public DockablePanel DotSpatialDockPanel { get; private set; }

        /// <summary>
        /// The sort order of the dockable panel
        /// </summary>
        public int SortOrder { get; private set; }

        /// <summary>
        /// Unique number of dockpanel (each created DockPanel has new number).
        /// </summary>
        public int Number { get; private set; }

        public int Height { get; private set; }

        /// <summary>
        /// Creates a new instance of DockPanelInfo
        /// </summary>
        /// <param name="dotSpatialDockPanel">the DotSpatial DockPanel virtual object</param>
        /// <param name="dockPanelTab">The physical instance of the dock panel (a tabpage)</param>
        /// <param name="sortOrder">the sort order</param>
        /// <param name="height">height of tool panels for panel extension</param>
        public DockPanelInfo(DockablePanel dotSpatialDockPanel, TabPage dockPanelTab, int sortOrder, int height)
        {   
            DotSpatialDockPanel = dotSpatialDockPanel;
            DockPanelTab = dockPanelTab;
            SortOrder = sortOrder;
            Number = _dockPanelNumber++;
            Height = height;
        }
    }
}
