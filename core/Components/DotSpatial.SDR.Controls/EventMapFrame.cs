using System;
using System.Diagnostics;
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

        public EventMapFrame()
        {
            ViewExtentChangedSuspended = true;
            SuspendExtentChanged();
            base.Initialize();
        }

        public void DisableViewChanged(bool flag)
        {
            base.DisableIsViewChanged(flag);
        }

        //public void DisplayReprojectionDialog()
        //{

        //    //get { _projectionModeReproject; }
        //    //set { _projectionModeReproject = value; }
        //}

        ///// <summary>
        ///// Gets or sets the PromptMode that determines how to warn users when attempting to add a layer without
        ///// a projection to a map that has a projection.
        ///// </summary>
        //public void AutoReprojection()
        //{
        //    //get { return _projectionModeDefine; }
        //    //set
        //    //{
        //    //    _projectionModeDefine = value;
        //    //}
        //}

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
