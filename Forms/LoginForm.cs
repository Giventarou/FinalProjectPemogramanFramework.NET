using System;
using System.Windows.Forms;
using DailyFinanceApp.Forms;

namespace DailyFinanceApp
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

            // Sambungkan event link Daftar lewat kode
            // (-= dulu supaya tidak terpanggil dua kali kalau di Designer sudah tersambung)
            lnkRegister.LinkClicked -= lnkRegister_LinkClicked;
            lnkRegister.LinkClicked += lnkRegister_LinkClicked;

            // Tekan Enter = klik tombol Login
            this.AcceptButton = btnLogin;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Username dan password wajib diisi.", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Sementara menggunakan data login dummy
            Session.UserId = 1;
            Session.Name = txtUsername.Text;
            Session.Username = txtUsername.Text;
            Session.Role = txtUsername.Text.ToLower() == "admin" ? "Admin" : "User";

            this.Hide();
            new MainForm().ShowDialog();

            txtPassword.Clear();
            this.Show();
        }

        // Membuka halaman pendaftaran
        private void lnkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new RegisterForm().ShowDialog();
        }
    }
}