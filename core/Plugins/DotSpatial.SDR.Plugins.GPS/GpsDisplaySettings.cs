using System;
using System.Drawing;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Symbology;
using DotSpatial.Data;
using GeoAPI.Geometries;
using Point = System.Drawing.Point;
using PointShape = DotSpatial.Symbology.PointShape;
using SdrConfig = SDR.Configuration;

namespace DotSpatial.SDR.Plugins.GPS
{
    public partial class GpsDisplaySettings : Form
    {
        public GpsDisplaySettings()
        {
            InitializeComponent();
            PopulateSettingsToForm();
            InitializeEventHandlers();
        }

        private void InitializeEventHandlers()
        {
            numGpsDisplayCount.ValueChanged += delegate {
                PluginSettings.Instance.GpsDisplayCount = Convert.ToInt32(numGpsDisplayCount.Text);
            };
            gpsIntervalModeCount.CheckedChanged += delegate
            {
                if (gpsIntervalModeCount.Checked)
                {
                    PluginSettings.Instance.GpsIntervalType = "Count";
                }
            };
            gpsIntervalModeTime.CheckedChanged += delegate
            {
                if (gpsIntervalModeTime.Checked)
                {
                    PluginSettings.Instance.GpsIntervalType = "Time";
                }
            };
            numGpsIntervalValue.ValueChanged += delegate(object sender, EventArgs args)
            {
                var n = (NumericUpDown) sender;
                PluginSettings.Instance.GpsIntervalValue = Convert.ToInt32(n.Value);
            };
            gpsSymbolColor.Click += delegate
            {
                var dlg = new ColorDialog();
                if (dlg.ShowDialog() != DialogResult.OK) return;

                // update the slider max color value for display
                gpsSymbolColorSlider.MaximumColor = Color.FromArgb(255, dlg.Color.R, dlg.Color.G, dlg.Color.B);
                // update the color and map display with new color accounting for alpha
                int alpha = Convert.ToInt32(gpsSymbolColorSlider.Value * 255);
                gpsSymbolColor.BackColor = Color.FromArgb(alpha, dlg.Color.R, dlg.Color.G, dlg.Color.B);
                PluginSettings.Instance.GpsPointColor = gpsSymbolColor.BackColor;
                DrawGpsPointGraphics();
            };
            gpsSymbolColorSlider.ValueChanged += delegate
            {
                int alpha = Convert.ToInt32(gpsSymbolColorSlider.Value * 255);
                gpsSymbolColor.BackColor = Color.FromArgb(alpha, gpsSymbolColor.BackColor.R, gpsSymbolColor.BackColor.G, gpsSymbolColor.BackColor.B);
                PluginSettings.Instance.GpsPointColor = gpsSymbolColor.BackColor;
                DrawGpsPointGraphics();
            };
            gpsSymbolStyle.SelectedIndexChanged += delegate
            {
                PluginSettings.Instance.GpsPointStyle = ApplyComboBoxSetting(gpsSymbolStyle);
                DrawGpsPointGraphics();
            };
            gpsSymbolSize.ValueChanged += delegate
            {
                PluginSettings.Instance.GpsPointSize = Convert.ToInt32(gpsSymbolSize.Text);
                DrawGpsPointGraphics();
            };
        }

        public void PopulateSettingsToForm()
        {
            var gpsIntType = PluginSettings.Instance.GpsIntervalType;
            if (gpsIntType == "Time")
            {
                gpsIntervalModeTime.Checked = true;
            }
            else
            {
                gpsIntervalModeCount.Checked = true;
            }
            numGpsIntervalValue.Value = PluginSettings.Instance.GpsIntervalValue;
            numGpsDisplayCount.Value = PluginSettings.Instance.GpsDisplayCount;

            Color gpsColor = PluginSettings.Instance.GpsPointColor;
            gpsSymbolColorSlider.Value = gpsColor.GetOpacity();
            gpsSymbolColorSlider.MaximumColor = Color.FromArgb(255, gpsColor.R, gpsColor.G, gpsColor.B);
            gpsSymbolColor.BackColor = gpsColor;
            gpsSymbolSize.Value = PluginSettings.Instance.GpsPointSize;

            foreach (PointShape ptShape in Enum.GetValues(typeof(PointShape)))
            {
                if (ptShape.ToString().ToUpper() != "UNDEFINED")
                {
                    gpsSymbolStyle.Items.Add(ptShape.ToString());
                }
            }
            var idx = gpsSymbolStyle.Items.IndexOf(PluginSettings.Instance.GpsPointStyle);
            gpsSymbolStyle.SelectedIndex = idx;

            DrawGpsPointGraphics();
        }

        private void DrawGpsPointGraphics()
        {
            Map gpsMap;
            if (gpsSymbolGraphic.Controls.Count != 0)
            {
                gpsMap = gpsSymbolGraphic.Controls[0] as Map;
            }
            else
            {
                gpsMap = new Map
                {
                    ViewExtents = new Envelope(-130, -60, 10, 55).ToExtent(),
                    FunctionMode = FunctionMode.None,
                };
                gpsMap.MapFunctions.Clear(); // clear all built in map functions (nav/zoom/etc)
                gpsSymbolGraphic.Controls.Add(gpsMap);

                var ftSet = new FeatureSet(FeatureType.Point);
                var ftLyr = new MapPointLayer(ftSet);
                gpsMap.MapFrame.DrawingLayers.Add(ftLyr);

                // get the center of the control panel (location to render point)
                var y = ((gpsSymbolGraphic.Bottom - gpsSymbolGraphic.Top) / 2) - 1;
                var x = ((gpsSymbolGraphic.Right - gpsSymbolGraphic.Left) / 2) - 1;
                var c = gpsMap.PixelToProj(new Point(x, y));
                ftSet.AddFeature(new NetTopologySuite.Geometries.Point(c));
            }
            UpdateGpsGraphics(gpsMap);
        }

        private void UpdateGpsGraphics(Map map)
        {
            PointShape ptShape;  // parse out point shape style
            Enum.TryParse(PluginSettings.Instance.GpsPointStyle, true, out ptShape);
            var pLyr = map.MapFrame.DrawingLayers[0] as MapPointLayer;
            if (pLyr != null)
            {
                pLyr.Symbolizer = new PointSymbolizer(PluginSettings.Instance.GpsPointColor, ptShape, PluginSettings.Instance.GpsPointSize);
            }
            Color mapColor = SdrConfig.Project.Go2ItProjectSettings.Instance.MapBgColor;
            map.BackColor = mapColor;
            map.MapFrame.Invalidate();
        }

        private static string ApplyComboBoxSetting(ComboBox cmb)
        {
            if (cmb.SelectedItem == null) return string.Empty;
            return cmb.SelectedItem.ToString().Length > 0 ? cmb.SelectedItem.ToString() : string.Empty;
        }
    }
}
