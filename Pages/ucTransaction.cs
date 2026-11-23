using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using DailyFinanceApp.Forms;

namespace DailyFinanceApp.Pages
{
    public partial class ucTransaction : UserControl
    {
        private class TransactionItem
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public string Type { get; set; }       // Pemasukan / Pengeluaran
            public int WalletId { get; set; }
            public int CategoryId { get; set; }
            public decimal Amount { get; set; }
            public string Note { get; set; }
        }

        private readonly List<frmTransactionEdit.Option> _wallets = new List<frmTransactionEdit.Option>();
        private readonly List<frmTransactionEdit.Option> _categories = new List<frmTransactionEdit.Option>();
        private readonly CultureInfo _id = new CultureInfo("id-ID");

        public ucTransaction()
        {
            InitializeComponent();

            SetupGrid();
            LoadLookups();
            SetupFilter();
            ApplyFilter();

            btnAdd.Click += btnAdd_Click;
            btnEdit.Click += btnEdit_Click;
            btnDelete.Click += btnDelete_Click;
            btnSearch.Click += (s, e) => ApplyFilter();
            btnReset.Click += btnReset_Click;
        }

        // ---------- Setup ----------

        private void SetupGrid()
        {
            dgvTransaction.Columns.Clear();
            dgvTransaction.Columns.Add("Id", "Id");
            dgvTransaction.Columns["Id"].Visible = false;
            dgvTransaction.Columns.Add("Date", "Tanggal");
            dgvTransaction.Columns.Add("Type", "Tipe");
            dgvTransaction.Columns.Add("Wallet", "Dompet");
            dgvTransaction.Columns.Add("Category", "Kategori");
            dgvTransaction.Columns.Add("Amount", "Nominal");
            dgvTransaction.Columns.Add("Note", "Catatan");
            dgvTransaction.Columns["Amount"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;
        }

        // Ambil daftar dompet dan kategori milik user dari database
        private void LoadLookups()
        {
            _wallets.Clear();
            _categories.Clear();

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();

                    using (var cmd = new MySqlCommand(
                        "SELECT id, name FROM wallets WHERE user_id = @uid ORDER BY name", conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", Session.UserId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _wallets.Add(new frmTransactionEdit.Option
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    Name = reader["name"].ToString()
                                });
                            }
                        }
                    }

                    using (var cmd = new MySqlCommand(
                        "SELECT id, name, type FROM categories WHERE user_id = @uid ORDER BY name", conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", Session.UserId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _categories.Add(new frmTransactionEdit.Option
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    Name = reader["name"].ToString(),
                                    Type = reader["type"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat dompet dan kategori: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupFilter()
        {
            // Kategori dan dompet diawali pilihan "Semua"
            var cats = new List<frmTransactionEdit.Option>
                { new frmTransactionEdit.Option { Id = 0, Name = "Semua" } };
            cats.AddRange(_categories);
            cmbCategory.DataSource = cats;

            var wals = new List<frmTransactionEdit.Option>
                { new frmTransactionEdit.Option { Id = 0, Name = "Semua" } };
            wals.AddRange(_wallets);
            cmbWallet.DataSource = wals;

            ResetFilterValues();
        }

        // ---------- Tampilan ----------

        private string Rp(decimal v)
        {
            return "Rp " + v.ToString("N0", _id);
        }

        // Mengembalikan filter ke kondisi awal tanpa menjalankan pencarian
        private void ResetFilterValues()
        {
            txtSearch.Clear();
            cmbCategory.SelectedIndex = 0;
            cmbWallet.SelectedIndex = 0;
            cmbType.SelectedIndex = 0;
            dtpFrom.Value = DateTime.Today.AddDays(-30);
            dtpTo.Value = DateTime.Today;
        }

        // Menampilkan transaksi sesuai filter (disaring langsung oleh database)
        private void ApplyFilter()
        {
            string keyword = txtSearch.Text.Trim();
            DateTime from = dtpFrom.Value.Date;
            DateTime to = dtpTo.Value.Date;
            int catId = ((frmTransactionEdit.Option)cmbCategory.SelectedItem).Id;
            int walId = ((frmTransactionEdit.Option)cmbWallet.SelectedItem).Id;
            string type = cmbType.Text;

            dgvTransaction.Rows.Clear();

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();

                    string sql =
                        "SELECT t.id, t.trx_date, t.type, w.name AS wallet_name, " +
                        "       c.name AS category_name, t.amount, t.note " +
                        "FROM transactions t " +
                        "JOIN wallets w ON w.id = t.wallet_id " +
                        "JOIN categories c ON c.id = t.category_id " +
                        "WHERE t.user_id = @uid " +
                        "  AND t.trx_date BETWEEN @from AND @to " +
                        "  AND (@cat = 0 OR t.category_id = @cat) " +
                        "  AND (@wal = 0 OR t.wallet_id = @wal) " +
                        "  AND (@type = 'Semua' OR t.type = @type) " +
                        "  AND (@kw = '' OR t.note LIKE @like OR c.name LIKE @like) " +
                        "ORDER BY t.trx_date DESC, t.id DESC";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", Session.UserId);
                        cmd.Parameters.AddWithValue("@from", from);
                        cmd.Parameters.AddWithValue("@to", to);
                        cmd.Parameters.AddWithValue("@cat", catId);
                        cmd.Parameters.AddWithValue("@wal", walId);
                        cmd.Parameters.AddWithValue("@type", type);
                        cmd.Parameters.AddWithValue("@kw", keyword);
                        cmd.Parameters.AddWithValue("@like", "%" + keyword + "%");

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string trxType = reader["type"].ToString();

                                int row = dgvTransaction.Rows.Add(
                                    reader["id"],
                                    Convert.ToDateTime(reader["trx_date"]).ToString("dd/MM/yyyy"),
                                    trxType,
                                    reader["wallet_name"].ToString(),
                                    reader["category_name"].ToString(),
                                    Rp(Convert.ToDecimal(reader["amount"])),
                                    reader["note"].ToString());

                                // Hijau untuk pemasukan, merah untuk pengeluaran
                                dgvTransaction.Rows[row].Cells["Amount"].Style.ForeColor =
                                    trxType == "Pemasukan"
                                        ? System.Drawing.Color.FromArgb(39, 174, 96)
                                        : System.Drawing.Color.FromArgb(192, 57, 43);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat transaksi: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            dgvTransaction.ClearSelection();
        }

        private int SelectedId()
        {
            if (dgvTransaction.SelectedRows.Count == 0) return 0;
            return Convert.ToInt32(dgvTransaction.SelectedRows[0].Cells["Id"].Value);
        }

        // ---------- Akses database ----------

        private string NameOf(List<frmTransactionEdit.Option> list, int id)
        {
            var o = list.Find(x => x.Id == id);
            return o == null ? "-" : o.Name;
        }

        // Teks ringkas satu transaksi, untuk disimpan di transaction_logs
        private string Describe(TransactionItem t)
        {
            return t.Type + " | " + NameOf(_wallets, t.WalletId) + " | " +
                   NameOf(_categories, t.CategoryId) + " | " +
                   t.Date.ToString("yyyy-MM-dd") + " | Rp " +
                   t.Amount.ToString("N0", _id) + " | " + t.Note;
        }

        // Saldo dompet = saldo awal + pemasukan - pengeluaran.
        // excludeId dipakai saat mengubah transaksi, supaya transaksi itu tidak dihitung dua kali.
        private decimal GetBalance(MySqlConnection conn, MySqlTransaction tx, int walletId, int excludeId)
        {
            string sql =
                "SELECT w.initial_balance + COALESCE(SUM(CASE " +
                "  WHEN t.type = 'Pemasukan' THEN t.amount " +
                "  WHEN t.type = 'Pengeluaran' THEN -t.amount END), 0) " +
                "FROM wallets w " +
                "LEFT JOIN transactions t ON t.wallet_id = w.id AND t.id <> @ex " +
                "WHERE w.id = @wid AND w.user_id = @uid " +
                "GROUP BY w.id, w.initial_balance";

            using (var cmd = new MySqlCommand(sql, conn, tx))
            {
                cmd.Parameters.AddWithValue("@ex", excludeId);
                cmd.Parameters.AddWithValue("@wid", walletId);
                cmd.Parameters.AddWithValue("@uid", Session.UserId);
                object result = cmd.ExecuteScalar();
                return result == null ? 0 : Convert.ToDecimal(result);
            }
        }

        private TransactionItem GetTransaction(MySqlConnection conn, MySqlTransaction tx, int id)
        {
            using (var cmd = new MySqlCommand(
                "SELECT id, trx_date, type, wallet_id, category_id, amount, note " +
                "FROM transactions WHERE id = @id AND user_id = @uid", conn, tx))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@uid", Session.UserId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;

                    return new TransactionItem
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Date = Convert.ToDateTime(reader["trx_date"]),
                        Type = reader["type"].ToString(),
                        WalletId = Convert.ToInt32(reader["wallet_id"]),
                        CategoryId = Convert.ToInt32(reader["category_id"]),
                        Amount = Convert.ToDecimal(reader["amount"]),
                        Note = reader["note"].ToString()
                    };
                }
            }
        }

        private void SaveLog(MySqlConnection conn, MySqlTransaction tx, string action,
                             int trxId, string oldValue, string newValue)
        {
            using (var cmd = new MySqlCommand(
                "INSERT INTO transaction_logs (user_id, transaction_id, action, old_value, new_value) " +
                "VALUES (@uid, @trx, @action, @old, @new)", conn, tx))
            {
                cmd.Parameters.AddWithValue("@uid", Session.UserId);
                cmd.Parameters.AddWithValue("@trx", trxId);
                cmd.Parameters.AddWithValue("@action", action);
                cmd.Parameters.AddWithValue("@old", (object)oldValue ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@new", (object)newValue ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        // ---------- Event ----------

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (_wallets.Count == 0 || _categories.Count == 0)
            {
                Warn("Buat minimal satu dompet dan satu kategori dulu sebelum menambah transaksi.");
                return;
            }

            TransactionItem item;

            using (var dlg = new frmTransactionEdit(_wallets, _categories))
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;

                item = new TransactionItem
                {
                    Date = dlg.Date,
                    Type = dlg.TypeName,
                    WalletId = dlg.WalletId,
                    CategoryId = dlg.CategoryId,
                    Amount = dlg.Amount,
                    Note = dlg.Note
                };
            }

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();

                    using (var tx = conn.BeginTransaction())
                    {
                        // Tolak pengeluaran kalau saldo dompet tidak cukup
                        if (item.Type == "Pengeluaran" &&
                            item.Amount > GetBalance(conn, tx, item.WalletId, 0))
                        {
                            Warn("Saldo dompet tidak cukup untuk pengeluaran ini.");
                            return;
                        }

                        int newId;
                        using (var cmd = new MySqlCommand(
                            "INSERT INTO transactions (user_id, wallet_id, category_id, type, amount, note, trx_date) " +
                            "VALUES (@uid, @wid, @cid, @type, @amount, @note, @date)", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@uid", Session.UserId);
                            cmd.Parameters.AddWithValue("@wid", item.WalletId);
                            cmd.Parameters.AddWithValue("@cid", item.CategoryId);
                            cmd.Parameters.AddWithValue("@type", item.Type);
                            cmd.Parameters.AddWithValue("@amount", item.Amount);
                            cmd.Parameters.AddWithValue("@note", item.Note);
                            cmd.Parameters.AddWithValue("@date", item.Date);
                            cmd.ExecuteNonQuery();
                            newId = (int)cmd.LastInsertedId;
                        }

                        SaveLog(conn, tx, "Tambah", newId, null, Describe(item));
                        tx.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menambah transaksi: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ResetFilterValues();
            ApplyFilter();
            MessageBox.Show("Transaksi berhasil ditambahkan.", "Sukses",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            int id = SelectedId();
            if (id == 0)
            {
                Warn("Pilih transaksi di tabel yang ingin diubah.");
                return;
            }

            TransactionItem old;
            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();
                    old = GetTransaction(conn, null, id);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal membaca transaksi: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (old == null)
            {
                Warn("Transaksi tidak ditemukan.");
                return;
            }

            TransactionItem baru;

            using (var dlg = new frmTransactionEdit(_wallets, _categories))
            {
                dlg.LoadData(old.Type, old.WalletId, old.CategoryId, old.Date, old.Amount, old.Note);
                if (dlg.ShowDialog() != DialogResult.OK) return;

                baru = new TransactionItem
                {
                    Id = id,
                    Date = dlg.Date,
                    Type = dlg.TypeName,
                    WalletId = dlg.WalletId,
                    CategoryId = dlg.CategoryId,
                    Amount = dlg.Amount,
                    Note = dlg.Note
                };
            }

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();

                    using (var tx = conn.BeginTransaction())
                    {
                        // Saldo dihitung tanpa transaksi ini, lalu dicek dengan nilai barunya
                        if (baru.Type == "Pengeluaran" &&
                            baru.Amount > GetBalance(conn, tx, baru.WalletId, id))
                        {
                            Warn("Saldo dompet tidak cukup untuk pengeluaran ini.");
                            return;
                        }

                        using (var cmd = new MySqlCommand(
                            "UPDATE transactions SET wallet_id = @wid, category_id = @cid, type = @type, " +
                            "amount = @amount, note = @note, trx_date = @date " +
                            "WHERE id = @id AND user_id = @uid", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@wid", baru.WalletId);
                            cmd.Parameters.AddWithValue("@cid", baru.CategoryId);
                            cmd.Parameters.AddWithValue("@type", baru.Type);
                            cmd.Parameters.AddWithValue("@amount", baru.Amount);
                            cmd.Parameters.AddWithValue("@note", baru.Note);
                            cmd.Parameters.AddWithValue("@date", baru.Date);
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.Parameters.AddWithValue("@uid", Session.UserId);
                            cmd.ExecuteNonQuery();
                        }

                        SaveLog(conn, tx, "Ubah", id, Describe(old), Describe(baru));
                        tx.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mengubah transaksi: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ResetFilterValues();
            ApplyFilter();
            MessageBox.Show("Transaksi berhasil diubah.", "Sukses",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int id = SelectedId();
            if (id == 0)
            {
                Warn("Pilih transaksi di tabel yang ingin dihapus.");
                return;
            }

            var hasil = MessageBox.Show("Yakin ingin menghapus transaksi ini?", "Konfirmasi",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (hasil != DialogResult.Yes) return;

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();

                    using (var tx = conn.BeginTransaction())
                    {
                        var old = GetTransaction(conn, tx, id);
                        if (old == null)
                        {
                            Warn("Transaksi tidak ditemukan.");
                            return;
                        }

                        using (var cmd = new MySqlCommand(
                            "DELETE FROM transactions WHERE id = @id AND user_id = @uid", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.Parameters.AddWithValue("@uid", Session.UserId);
                            cmd.ExecuteNonQuery();
                        }

                        SaveLog(conn, tx, "Hapus", id, Describe(old), null);
                        tx.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menghapus transaksi: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ResetFilterValues();
            ApplyFilter();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetFilterValues();
            ApplyFilter();
        }

        private void Warn(string message)
        {
            MessageBox.Show(message, "Peringatan",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}