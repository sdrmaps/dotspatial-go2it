using System.Threading;

namespace DotSpatial.SDR.Controls
{
    // TODO: we really need a better way to properly parent the form to the calling form
    public class ProgressPanel
    {
        private static Thread _t = new Thread(ShowProgressForm);
        private static string ProgressPanelLabel { get; set; }

        public void StartProgress()
        {
            _t = new Thread(ShowProgressForm) { Name = "progress" };
            _t.Start();
        }

        public void StartProgress(string label)
        {
            ProgressPanelLabel = label;
            _t = new Thread(ShowProgressForm) { Name = "progress" };
            _t.Start();
        }

        private static void ShowProgressForm()
        {
            ProgressPanelForm loadForm;
            if (ProgressPanelLabel.Length > 0)
            {
                loadForm = new ProgressPanelForm(ProgressPanelLabel);
            }
            else
            {
                loadForm = new ProgressPanelForm();
            }
            loadForm.ShowDialog();
        }

        public void StopProgress()
        {
            _t.Abort();
            _t = null;
        }
    }
}
