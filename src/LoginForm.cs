using System;
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
            {
                FormLogin(this, EventArgs.Empty);
            }
        }

        private void OnFormLogout()
        {
            if (FormLogout != null)
            {
                FormLogout(this, EventArgs.Empty);
            }
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
                Close();
            }
            else
            {
                MessageBox.Show(@"Invalid username or password, please check credentials and try again.", @"Login Failed");
                OnFormLogout();
            }
        }

        private static bool CheckLogin(string username, string password)
        {
            if (String.IsNullOrEmpty(username)) return false;
            if (String.IsNullOrEmpty(password)) return false;

            var conn = SdrConfig.Settings.Instance.ApplicationRepoConnectionString;
            var query = "SELECT username, salt, hash FROM logins WHERE username='" + username + "'";
            var table = SQLiteHelper.GetDataTable(conn, query);

            if (table.Rows.Count <= 0) return false;
            var r = table.Rows[0];
            var salt = r["salt"].ToString();
            var hash = r["hash"].ToString();

            // now verify the password against the dbase salt/hash
            var sh = SaltedHash.Create(salt, hash);
            return sh.Verify(password);
        }
    }
}
