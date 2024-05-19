using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace WindowsFormsApp1.Login
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }
        
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string UserName = txtUserName.Text;
            string Password = txtPassword.Text;
            string SourceName = "CodeGenerator";

            if (LoginByRegistry.ReadingLoginInfo(UserName, Password, SourceName))
            {
                frmMain frm = new frmMain();

                frm.ShowDialog();

                Loggin.WriteToEventViewer(SourceName, "Login to the system successfully", EventLogEntryType.Information);

                Loggin.WriteToEventViewer(SourceName, $"A user with username '{UserName}' logged into the system , is that you", EventLogEntryType.Warning);
            }
            else
            {
                Loggin.WriteToEventViewer(SourceName, "Login Error", EventLogEntryType.Error);
               
                MessageBox.Show($"Check to your username or password", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
