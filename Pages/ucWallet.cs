using MySql.Data.MySqlClient;
using System;
using System.Globalization;
using System.Windows.Forms;

namespace DailyFinanceApp.Pages
{
    public partial class ucWallet : UserControl
    {
        private readonly CultureInfo _id = new CultureInfo("id-ID");
        private int _selectedId = 0;

        public ucWallet()
        {
            InitializeComponent();

            SetupGrid();
            cmbType.SelectedIndex = 0;
            LoadWallets();

            // Sambungkan event lewat kode
            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnClear.Click += btnClear_Click;
            dgvWallet.SelectionChanged += dgvWallet_SelectionChanged;
        }

        // Dua method kosong ini masih dirujuk Designer, biarkan saja.
        private void label1_Click(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }

        // ---------- Setup ----------

        private void SetupGrid()
        {
            dgvWallet.Columns.Clear();
            dgvWallet.Columns.Add("Id", "Id");
            dgvWallet.Columns["Id"].Visible = false;
            dgvWallet.Columns.Add("Name", "Nama Dompet");
            dgvWallet.Columns.Add("Type", "Tipe");
            dgvWallet.Columns.Add("Initial", "Saldo Awal");
            dgvWallet.Columns.Add("Current", "Saldo Saat Ini");

            dgvWallet.Columns["Initial"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvWallet.Columns["Current"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        // ---------- Tampilan ----------

        private string Rp(decimal value)
        {
            return "Rp " + value.ToString("N0", _id);
        }

        // Ambil semua dompet milik user yang sedang login.
        // Saldo saat ini = saldo awal + pemasukan - pengeluaran di dompet itu.
        private void LoadWallets()
        {
            dgvWallet.Rows.Clear();

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();

                    string sql =
                        "SELECT w.id, w.name, w.type, w.initial_balance, " +
                        "  w.initial_balance + COALESCE(SUM(CASE " +
                        "    WHEN t.type = 'Pemasukan' THEN t.amount " +
                        "    WHEN t.type = 'Pengeluaran' THEN -t.amount " +
                        "  END), 0) AS current_balance " +
                        "FROM wallets w " +
                        "LEFT JOIN transactions t ON t.wallet_id = w.id " +
                        "WHERE w.user_id = @uid " +
                        "GROUP BY w.id, w.name, w.type, w.initial_balance " +
                        "ORDER BY w.name";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", Session.UserId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dgvWallet.Rows.Add(
                                    reader["id"],
                                    reader["name"].ToString(),
                                    reader["type"].ToString(),
                                    Rp(Convert.ToDecimal(reader["initial_balance"])),
                                    Rp(Convert.ToDecimal(reader["current_balance"])));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat dompet: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            _selectedId = 0;
            dgvWallet.ClearSelection();
        }

        private void ClearForm()
        {
            _selectedId = 0;
            txtWalletName.Clear();
            txtInitialBalance.Clear();
            cmbType.SelectedIndex = 0;
            dgvWallet.ClearSelection();
            txtWalletName.Focus();
        }

        // ---------- Validasi ----------

        private bool ValidateInput(out decimal balance)
        {
            balance = 0;

            if (string.IsNullOrWhiteSpace(txtWalletName.Text))
            {
                Warn("Nama dompet wajib diisi.");
                txtWalletName.Focus();
                return false;
            }

            if (cmbType.SelectedIndex < 0)
            {
                Warn("Pilih tipe dompet.");
                return false;
            }

            if (!decimal.TryParse(txtInitialBalance.Text.Trim(), NumberStyles.Number, _id, out balance))
            {
                Warn("Saldo awal harus berupa angka.");
                txtInitialBalance.Focus();
                return false;
            }

            if (balance < 0)
            {
                Warn("Saldo awal tidak boleh negatif.");
                txtInitialBalance.Focus();
                return false;
            }

            return true;
        }

        private void Warn(string message)
        {
            MessageBox.Show(message, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // ---------- Event ----------

        private void dgvWallet_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvWallet.SelectedRows.Count == 0) return;

            var row = dgvWallet.SelectedRows[0];
            _selectedId = Convert.ToInt32(row.Cells["Id"].Value);
            txtWalletName.Text = row.Cells["Name"].Value.ToString();
            cmbType.SelectedItem = row.Cells["Type"].Value.ToString();

            // Saldo awal di tabel tampil "Rp 250.000", jadi ambil lagi angka aslinya dari database
            txtInitialBalance.Text = GetInitialBalance(_selectedId).ToString("0", _id);
        }

        private decimal GetInitialBalance(int walletId)
        {
            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();

                    using (var cmd = new MySqlCommand(
                        "SELECT initial_balance FROM wallets WHERE id = @id AND user_id = @uid", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", walletId);
                        cmd.Parameters.AddWithValue("@uid", Session.UserId);

                        object result = cmd.ExecuteScalar();
                        return result == null ? 0 : Convert.ToDecimal(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal membaca saldo: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            decimal balance;
            if (!ValidateInput(out balance)) return;

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();

                    using (var cmd = new MySqlCommand(
                        "INSERT INTO wallets (user_id, name, type, initial_balance) " +
                        "VALUES (@uid, @name, @type, @balance)", conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", Session.UserId);
                        cmd.Parameters.AddWithValue("@name", txtWalletName.Text.Trim());
                        cmd.Parameters.AddWithValue("@type", cmbType.Text);
                        cmd.Parameters.AddWithValue("@balance", balance);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menambah dompet: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadWallets();
            ClearForm();

            MessageBox.Show("Dompet berhasil ditambahkan.", "Sukses",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (_selectedId == 0)
            {
                Warn("Pilih dompet di tabel yang ingin diubah.");
                return;
            }

            decimal balance;
            if (!ValidateInput(out balance)) return;

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();

                    using (var cmd = new MySqlCommand(
                        "UPDATE wallets SET name = @name, type = @type, initial_balance = @balance " +
                        "WHERE id = @id AND user_id = @uid", conn))
                    {
                        cmd.Parameters.AddWithValue("@name", txtWalletName.Text.Trim());
                        cmd.Parameters.AddWithValue("@type", cmbType.Text);
                        cmd.Parameters.AddWithValue("@balance", balance);
                        cmd.Parameters.AddWithValue("@id", _selectedId);
                        cmd.Parameters.AddWithValue("@uid", Session.UserId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mengubah dompet: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadWallets();
            ClearForm();

            MessageBox.Show("Dompet berhasil diubah.", "Sukses",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedId == 0)
            {
                Warn("Pilih dompet di tabel yang ingin dihapus.");
                return;
            }

            var hasil = MessageBox.Show("Yakin ingin menghapus dompet ini?", "Konfirmasi",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (hasil != DialogResult.Yes) return;

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();

                    using (var cmd = new MySqlCommand(
                        "DELETE FROM wallets WHERE id = @id AND user_id = @uid", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", _selectedId);
                        cmd.Parameters.AddWithValue("@uid", Session.UserId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException ex)
            {
                // 1451 = dompet masih dipakai oleh transaksi
                if (ex.Number == 1451)
                    Warn("Dompet ini masih dipakai transaksi, jadi tidak bisa dihapus.");
                else
                    MessageBox.Show("Gagal menghapus dompet: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadWallets();
            ClearForm();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
    }
}