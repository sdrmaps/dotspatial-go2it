using System;
using System.Collections;
using System.Drawing;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using DotSpatial.Topology;
using PointShape = DotSpatial.Symbology.PointShape;

namespace DotSpatial.SDR.Plugins.GPS
{
    // delegate type for hooking up change notifications for arraylist
    public delegate void UpdateEventHandler(object sender, EventArgs e);

    public class GpsDisplayQueue : Queue
    {
        public event UpdateEventHandler ListUpdated;

        // invoke the changed event; called whenever list changes
        protected virtual void OnUpdate(EventArgs e)
        {
            if (ListUpdated != null)
                ListUpdated(this, e);
        }

        public MapPointLayer GpsGraphicsLayer { get; private set; }
        public PointShape GpsPointShape { get; set; }
        public int GpsPointSize { get; set; }
        public Color GpsNewPointColor { get; set; }

        public int GpsPointDisplayCount
        {
            set
            {
                if (_fsArray == null)
                {
                    _fsArray = new FeatureSymbolizer[value];
                }
                else
                {
                    Array.Resize(ref _fsArray, value);
                }
            }
            get { return _fsArray.Length; }
        }

        private FeatureSymbolizer[] _fsArray;
        private readonly FeatureSet _pointGraphics;

        public GpsDisplayQueue(PointShape pShape, int pSize, Color pColor, int pCount)
        {
            _pointGraphics = new FeatureSet(FeatureType.Point);
            GpsGraphicsLayer = new MapPointLayer(_pointGraphics);
            GpsPointShape = pShape;
            GpsPointSize = pSize;
            GpsNewPointColor = pColor;
            GpsPointDisplayCount = pCount;
            SetColorOpacityLevels();
        }

        private void SetColorOpacityLevels()
        {
            // GetOpacity() returns a value float between 0 (transparent) and 1 (opaque)
            var maxFltOpacity = GpsNewPointColor.GetOpacity();
            // to set the opacity we need an int between 0 and 255
            var maxIntOpacity = maxFltOpacity * 255;
            var alphaIncr = maxIntOpacity/GpsPointDisplayCount + 1;  // adding 1 gets us a slightly better opacity setting (higher values)
            var curOpacity = maxIntOpacity;

            FeatureSymbolizer fs = new PointSymbolizer(GpsNewPointColor, GpsPointShape, GpsPointSize);
            _fsArray[0] = fs;

            for (int i = 1; i <= GpsPointDisplayCount - 1; i++)
            {
                curOpacity = curOpacity - alphaIncr;
                var c = Color.FromArgb(Convert.ToInt32(curOpacity), GpsNewPointColor);
                fs = new PointSymbolizer(c, GpsPointShape, GpsPointSize);
                _fsArray[i] = fs;  // newest values on top of array
            }
        }

        public override void Enqueue(object obj)
        {
            if (base.Count == GpsPointDisplayCount)
            {
                base.Dequeue();
            }
            var p = obj as Topology.Point;
            base.Enqueue(p);
            var ftArray = base.ToArray();
            Array.Reverse(ftArray);  // reverse to move newest values to front of array
            _pointGraphics.Features.Clear();
            for (int i = 0; i <= ftArray.Length - 1; i++)
            {
                var point = ftArray[i] as Topology.Point;
                _pointGraphics.AddFeature(point);
                GpsGraphicsLayer.SetShapeSymbolizer(i, _fsArray[i]);
            }
            OnUpdate(EventArgs.Empty);
        }
    }
}
