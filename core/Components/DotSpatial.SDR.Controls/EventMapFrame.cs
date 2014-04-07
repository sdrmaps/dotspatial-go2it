using System;
using DotSpatial.Controls;

namespace DotSpatial.SDR.Controls
{
    /// <summary>
    /// Inherited Implementation of the DotSpatial MapFrame for ViewExtent Events
    /// </summary>
    [Serializable]
    public class EventMapFrame : MapFrame
    {
        public bool ViewExtentChangedSuspended { get; private set; }

        public void SuspendViewExtentChanged()
        {
            ViewExtentChangedSuspended = true;
            SuspendExtentChanged();
        }

        public void ResumeViewExtentChanged()
        {
            ViewExtentChangedSuspended = false;
            ResumeExtentChanged();   
        }
    }
}
