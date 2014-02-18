using System.Windows.Forms;
using DotSpatial.Controls;

namespace DotSpatial.SDR.Controls
{
    public class RestrictedLegend : Legend
    {
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            // remove any double click functionality from legend
            // base.OnMouseDoubleClick(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            // remove any right click functionality from legend
            if (e.Button != MouseButtons.Right)
            {
                base.OnMouseUp(e);
            }
        }
    }
}
