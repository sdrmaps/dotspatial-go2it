using System;
using System.Data;
using System.Windows.Forms;
using SDR.Authentication;
using SDR.Data.Database;
using SdrConfig = SDR.Configuration;

namespace Go2It
{
    public partial class LoginForm : Form
    {
        public event EventHandler FormLogin;
        public event EventHandler FormLogout;

        private void OnFormLogin()
        {
            if (FormLogin != null)
                FormLogin(this, EventArgs.Empty);
        }

        private void OnFormLogout()
        {
            if (FormLogout != null)
                FormLogout(this, EventArgs.Empty);
        }

        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnCancelLogin_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (CheckLogin(txtUserName.Text, txtPassword.Text))
            {
                OnFormLogin();
            }
            else
            {
                MessageBox.Show(@"Invalid username or password, please check credentials and try again.", @"Failed Login");
                OnFormLogout();
            }
            Close();
        }

        private static bool CheckLogin(string username, string password)
        {
            if (String.IsNullOrEmpty(username)) return false;
            if (String.IsNullOrEmpty(password)) return false;

            string conn = SdrConfig.Settings.Instance.ApplicationRepoConnectionString;
            string query = "SELECT username, salt, hash FROM logins WHERE username='" + username + "'";
            DataTable table = SQLiteHelper.GetDataTable(conn, query);

            if (table.Rows.Count <= 0) return false;
            DataRow r = table.Rows[0];
            string salt = r["salt"].ToString();
            string hash = r["hash"].ToString();
            // now verify the password against the dbase salt/hash
            SaltedHash sh = SaltedHash.Create(salt, hash);
            return sh.Verify(password);
        }
    }
}
