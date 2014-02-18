using System;
using System.Windows.Forms;
using reggen.Properties;
using SDR.Desktop.Authorize;

namespace reggen
{
    public partial class Reggen : Form
    {
        public Reggen()
        {
            Icon = Resources.globe;
            InitializeComponent();
        }

        private void btnGen_Click(object sender, EventArgs e)
        {
            if (txtIdent.Text.Length <= 0) return;
            txtReg.Text = Create(txtIdent.Text, Convert.ToInt32(txtDays.Text));
        }

        public string Create(string fp, int d)
        {
            try
            {
                string xfp = Crypto.StringIn(fp.Substring(0, 1), fp.Remove(0, 1));
                Random rndNum = new Random();
                int randNum = rndNum.Next(5);
                string nd = String.Format("{0:000.#}", d);
                return Verify.SetEx(randNum, nd, xfp);
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid Identification Value");
                txtIdent.Text = string.Empty;
                return string.Empty;
            }
        }
    }
}
