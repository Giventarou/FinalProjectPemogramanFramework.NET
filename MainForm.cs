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

            // Menu disambungkan lewat kode
            btnCategory.Click += (s, e) => LoadPage(new ucCategory());
            btnTransaction.Click += (s, e) => LoadPage(new ucTransaction());
            btnBudget.Click += (s, e) => LoadPage(new ucBudget());
            btnSavings.Click += (s, e) => LoadPage(new ucSavings());
            btnLog.Click += (s, e) => LoadPage(new ucLog());
            btnReport.Click += (s, e) => LoadPage(new ucReport());

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