using System;
using System.Windows.Forms;

namespace DotSpatial.SDR.Controls
{
    public class TablessControl : TabControl
    {
        private const int TcmAdjustrect = 0x1328;

        protected override void WndProc(ref Message m)
        {
            // Hide the tab headers at run-time
            if (m.Msg == TcmAdjustrect && !DesignMode)
            {
                m.Result = (IntPtr)1;
                return;
            }
            // call the base class implementation
            base.WndProc(ref m);
        }
    }
}
