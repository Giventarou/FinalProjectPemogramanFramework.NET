
using System;
using System.Windows.Forms;

namespace DailyFinanceApp.Forms
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(txtConfirm.Text))
            {
                MessageBox.Show("Semua kolom harus diisi.");
                return;
            }

            if (txtPassword.Text.Length < 6)
            {
                MessageBox.Show("Password minimal 6 karakter.");
                return;
            }

            if (txtPassword.Text != txtConfirm.Text)
            {
                MessageBox.Show("Konfirmasi password tidak sama.");
                return;
            }

            MessageBox.Show("Pendaftaran berhasil! Silakan login.");
            this.Close();
        }

        private void lnkBack_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
        }
    }
}
