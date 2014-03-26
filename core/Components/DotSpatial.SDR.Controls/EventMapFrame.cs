using System;
using DotSpatial.Controls;
using DotSpatial.Symbology;

namespace DotSpatial.SDR.Controls
{
    /// <summary>
    /// Imherited Implementation of the DotSpatial MapFrame for ViewExtent Events
    /// </summary>
    [Serializable]
    public class EventMapFrame : MapFrame
    {
        public void SuspendViewExtentChanged()
        {
            var collection = base.Layers;
            Ignore_Layer_Events(collection);
            base.SuspendExtentChanged();
        }

        public void ResumeViewExtentChanged()
        {
            var collection = base.Layers;
            Handle_Layer_Events(collection);
            base.ResumeExtentChanged();   
        }
    }
}
