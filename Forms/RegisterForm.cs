using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace DailyFinanceApp.Forms
{
    public partial class RegisterForm : Form
    {
        // Kategori bawaan untuk akun baru: { nama, tipe }
        private static readonly string[,] DefaultCategories =
        {
            { "Makanan", "Pengeluaran" },
            { "Transportasi", "Pengeluaran" },
            { "Pendidikan", "Pengeluaran" },
            { "Lainnya", "Pengeluaran" },
            { "Gaji", "Pemasukan" },
            { "Uang Saku", "Pemasukan" },
            { "Lainnya", "Pemasukan" }
        };

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

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();

                    // Cek dulu apakah username sudah dipakai
                    using (var cek = new MySqlCommand(
                        "SELECT COUNT(*) FROM users WHERE username = @username", conn))
                    {
                        cek.Parameters.AddWithValue("@username", txtUsername.Text.Trim());
                        if (Convert.ToInt32(cek.ExecuteScalar()) > 0)
                        {
                            MessageBox.Show("Username sudah dipakai, pilih yang lain.");
                            return;
                        }
                    }

                    // Akun, kategori bawaan, dan dompet bawaan disimpan sekaligus.
                    // Kalau salah satu gagal, semuanya dibatalkan.
                    using (var tx = conn.BeginTransaction())
                    {
                        int userId;

                        using (var cmd = new MySqlCommand(
                            "INSERT INTO users (name, username, password_hash) " +
                            "VALUES (@name, @username, @hash)", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                            cmd.Parameters.AddWithValue("@username", txtUsername.Text.Trim());
                            cmd.Parameters.AddWithValue("@hash", PasswordHelper.Hash(txtPassword.Text));
                            cmd.ExecuteNonQuery();
                            userId = (int)cmd.LastInsertedId;
                        }

                        for (int i = 0; i < DefaultCategories.GetLength(0); i++)
                        {
                            using (var cmd = new MySqlCommand(
                                "INSERT INTO categories (user_id, name, type) " +
                                "VALUES (@uid, @name, @type)", conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@uid", userId);
                                cmd.Parameters.AddWithValue("@name", DefaultCategories[i, 0]);
                                cmd.Parameters.AddWithValue("@type", DefaultCategories[i, 1]);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        using (var cmd = new MySqlCommand(
                            "INSERT INTO wallets (user_id, name, type, initial_balance) " +
                            "VALUES (@uid, 'Dompet Harian', 'Cash', 0)", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@uid", userId);
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                }

                MessageBox.Show("Pendaftaran berhasil! Silakan login.");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mendaftar: " + ex.Message);
            }
        }

        private void lnkBack_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
        }
    }
}