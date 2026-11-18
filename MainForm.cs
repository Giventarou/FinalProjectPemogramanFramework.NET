using DailyFinanceApp.Pages;
using System;
using System.Windows.Forms;

namespace DailyFinanceApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            lblUser.Text = "  " + Session.Name + " (" + Session.Role + ")";
            LoadPage(new ucDashboard());
        }

        private void LoadPage(UserControl page)
        {
            pnlContent.Controls.Clear();
            page.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(page);
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            LoadPage(new ucDashboard());
        }

        private void btnWallet_Click(object sender, EventArgs e)
        {
            LoadPage(new ucWallet());
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            var hasil = MessageBox.Show("Yakin ingin logout?", "Konfirmasi",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (hasil == DialogResult.Yes)
            {
                Session.Clear();
                this.Close();
            }
        }
    }
}