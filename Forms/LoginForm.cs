using MySql.Data.MySqlClient;
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

            int userId = 0;
            string name = null;
            string username = null;
            string role = null;
            bool berhasil = false;

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();

                    using (var cmd = new MySqlCommand(
                        "SELECT id, name, username, password_hash, role " +
                        "FROM users WHERE username = @username", conn))
                    {
                        cmd.Parameters.AddWithValue("@username", txtUsername.Text.Trim());

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string hash = reader["password_hash"].ToString();
                                if (PasswordHelper.Verify(txtPassword.Text, hash))
                                {
                                    userId = Convert.ToInt32(reader["id"]);
                                    name = reader["name"].ToString();
                                    username = reader["username"].ToString();
                                    role = reader["role"].ToString();
                                    berhasil = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tidak bisa terhubung ke database: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!berhasil)
            {
                MessageBox.Show("Username atau password salah.", "Login Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Session.UserId = userId;
            Session.Name = name;
            Session.Username = username;
            Session.Role = role;

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