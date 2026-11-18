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

            // Tampilkan Dashboard saat pertama dibuka
            LoadPage(new ucDashboard());
        }

        // Mengganti halaman yang tampil di area putih
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
    }
}