using System;
using DotSpatial.Controls;

namespace DotSpatial.SDR.Controls
{
    /// <summary>
    /// Imherited Implementation of the DotSpatial MapFrame for ViewExtent Events
    /// </summary>
    [Serializable]
    public class EventMapFrame : MapFrame
    {
        private bool _viewExtentSuspended;

        public EventMapFrame()
        {
            _viewExtentSuspended = false;
        }

        public void SuspendViewExtentChanged()
        {
            _viewExtentSuspended = true;
            // var collection = base.Layers;
            // Ignore_Layer_Events(collection);
            base.SuspendExtentChanged();
        }

        public void ResumeViewExtentChanged()
        {
            _viewExtentSuspended = false;
            // var collection = base.Layers;
            // Handle_Layer_Events(collection);
            base.ResumeExtentChanged();   
        }

        public bool ViewExtentChangesSuspended
        {
            get { return _viewExtentSuspended;  }
        }

    }
}
