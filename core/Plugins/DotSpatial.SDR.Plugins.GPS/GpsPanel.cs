using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotSpatial.Positioning;

namespace DotSpatial.SDR.Plugins.GPS
{
    public sealed partial class GpsPanel : UserControl
    {
        private DeviceDetectionStatus _detectionStatus;

        public GpsPanel()
        {
            InitializeComponent();
        }

        private void gpsDetectCancel_Click(object sender, EventArgs e)
        {
            if (gpsDetectCancel.Text == @"Detect")
            {
                gpsDetectCancel.Text = @"Cancel";
                //Devices.BeginDetection();
            }
            else
            {
                gpsDetectCancel.Text = @"Detect";
                //Devices.CancelDetection(true);
            }
        }
    }
}
