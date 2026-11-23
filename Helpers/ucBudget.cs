using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace DailyFinanceApp.Pages
{
    public partial class ucBudget : UserControl
    {
        private class Item
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private static readonly string[] MonthNames =
        {
            "Januari", "Februari", "Maret", "April", "Mei", "Juni",
            "Juli", "Agustus", "September", "Oktober", "November", "Desember"
        };

        private readonly CultureInfo _id = new CultureInfo("id-ID");
        private int _selectedId = 0;

        public ucBudget()
        {
            InitializeComponent();

            SetupGrid();
            SetupForm();
            LoadCategories();
            LoadBudgets();

            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnClear.Click += (s, e) => ClearForm();
            dgvBudget.SelectionChanged += dgvBudget_SelectionChanged;
        }

        // ---------- Setup ----------

        private void SetupGrid()
        {
            dgvBudget.Columns.Clear();
            dgvBudget.Columns.Add("Id", "Id");
            dgvBudget.Columns["Id"].Visible = false;
            dgvBudget.Columns.Add("CategoryId", "CategoryId");
            dgvBudget.Columns["CategoryId"].Visible = false;
            dgvBudget.Columns.Add("Month", "Month");
            dgvBudget.Columns["Month"].Visible = false;
            dgvBudget.Columns.Add("Year", "Year");
            dgvBudget.Columns["Year"].Visible = false;
            dgvBudget.Columns.Add("LimitRaw", "LimitRaw");
            dgvBudget.Columns["LimitRaw"].Visible = false;

            dgvBudget.Columns.Add("Category", "Kategori");
            dgvBudget.Columns.Add("Period", "Periode");
            dgvBudget.Columns.Add("Limit", "Batas");
            dgvBudget.Columns.Add("Spent", "Terpakai");
            dgvBudget.Columns.Add("Remaining", "Sisa");
            dgvBudget.Columns.Add("Percent", "Pemakaian");
            dgvBudget.Columns.Add("Status", "Status");

            foreach (string c in new[] { "Limit", "Spent", "Remaining", "Percent" })
                dgvBudget.Columns[c].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;
        }

        private void SetupForm()
        {
            cmbMonth.Items.Clear();
            foreach (var m in MonthNames) cmbMonth.Items.Add(m);
            cmbMonth.SelectedIndex = DateTime.Today.Month - 1;
            nudYear.Value = DateTime.Today.Year;
        }

        // Hanya kategori pengeluaran yang bisa dianggarkan
        private void LoadCategories()
        {
            var list = new List<Item>();
            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "SELECT id, name FROM categories " +
                        "WHERE user_id = @uid AND type = 'Pengeluaran' ORDER BY name", conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", Session.UserId);
                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                                list.Add(new Item
                                {
                                    Id = Convert.ToInt32(r["id"]),
                                    Name = r["name"].ToString()
                                });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat kategori: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            cmbCategory.DataSource = list;
            cmbCategory.DisplayMember = "Name";
            cmbCategory.ValueMember = "Id";
        }

        // ---------- Tampilan ----------

        private string Rp(decimal v)
        {
            return "Rp " + v.ToString("N0", _id);
        }

        // Terpakai dihitung dari transaksi pengeluaran pada kategori, bulan, dan tahun yang sama
        private void LoadBudgets()
        {
            dgvBudget.Rows.Clear();

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "SELECT b.id, b.category_id, c.name, b.month, b.year, b.amount, " +
                        "COALESCE((SELECT SUM(t.amount) FROM transactions t " +
                        "  WHERE t.user_id = b.user_id AND t.category_id = b.category_id " +
                        "  AND t.type = 'Pengeluaran' " +
                        "  AND MONTH(t.trx_date) = b.month AND YEAR(t.trx_date) = b.year), 0) AS spent " +
                        "FROM budgets b JOIN categories c ON c.id = b.category_id " +
                        "WHERE b.user_id = @uid " +
                        "ORDER BY b.year DESC, b.month DESC, c.name", conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", Session.UserId);
                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                int month = Convert.ToInt32(r["month"]);
                                int year = Convert.ToInt32(r["year"]);
                                decimal limit = Convert.ToDecimal(r["amount"]);
                                decimal spent = Convert.ToDecimal(r["spent"]);
                                decimal remaining = limit - spent;
                                decimal percent = limit > 0 ? spent / limit * 100 : 0;

                                string status;
                                Color back;
                                if (percent > 100)
                                {
                                    status = "Melebihi batas";
                                    back = Color.FromArgb(250, 219, 216);
                                }
                                else if (percent >= 80)
                                {
                                    status = "Hampir habis";
                                    back = Color.FromArgb(253, 243, 205);
                                }
                                else
                                {
                                    status = "Aman";
                                    back = Color.FromArgb(213, 245, 227);
                                }

                                int row = dgvBudget.Rows.Add(
                                    r["id"], r["category_id"], month, year, limit,
                                    r["name"].ToString(),
                                    MonthNames[month - 1] + " " + year,
                                    Rp(limit), Rp(spent), Rp(remaining),
                                    percent.ToString("0", _id) + "%",
                                    status);

                                dgvBudget.Rows[row].DefaultCellStyle.BackColor = back;
                                dgvBudget.Rows[row].DefaultCellStyle.SelectionForeColor = Color.White;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat anggaran: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            _selectedId = 0;
            dgvBudget.ClearSelection();
        }

        private void ClearForm()
        {
            _selectedId = 0;
            if (cmbCategory.Items.Count > 0) cmbCategory.SelectedIndex = 0;
            cmbMonth.SelectedIndex = DateTime.Today.Month - 1;
            nudYear.Value = DateTime.Today.Year;
            txtAmount.Clear();
            dgvBudget.ClearSelection();
            cmbCategory.Focus();
        }

        private void Warn(string message)
        {
            MessageBox.Show(message, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // ---------- Validasi ----------

        private bool ValidateInput(out decimal amount)
        {
            amount = 0;

            if (cmbCategory.SelectedValue == null)
            {
                Warn("Belum ada kategori pengeluaran. Tambahkan dulu di menu Kategori.");
                return false;
            }

            if (cmbMonth.SelectedIndex < 0)
            {
                Warn("Pilih bulan.");
                return false;
            }

            if (!decimal.TryParse(txtAmount.Text.Trim(), NumberStyles.Number, _id, out amount))
            {
                Warn("Batas anggaran harus berupa angka.");
                txtAmount.Focus();
                return false;
            }

            if (amount <= 0)
            {
                Warn("Batas anggaran harus lebih dari 0.");
                txtAmount.Focus();
                return false;
            }

            return true;
        }

        // ---------- Event ----------

        private void dgvBudget_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvBudget.SelectedRows.Count == 0) return;

            var row = dgvBudget.SelectedRows[0];
            _selectedId = Convert.ToInt32(row.Cells["Id"].Value);
            cmbCategory.SelectedValue = Convert.ToInt32(row.Cells["CategoryId"].Value);
            cmbMonth.SelectedIndex = Convert.ToInt32(row.Cells["Month"].Value) - 1;
            nudYear.Value = Convert.ToInt32(row.Cells["Year"].Value);
            txtAmount.Text = Convert.ToDecimal(row.Cells["LimitRaw"].Value).ToString("0", _id);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            decimal amount;
            if (!ValidateInput(out amount)) return;

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "INSERT INTO budgets (user_id, category_id, month, year, amount) " +
                        "VALUES (@uid, @cat, @month, @year, @amount)", conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", Session.UserId);
                        cmd.Parameters.AddWithValue("@cat", cmbCategory.SelectedValue);
                        cmd.Parameters.AddWithValue("@month", cmbMonth.SelectedIndex + 1);
                        cmd.Parameters.AddWithValue("@year", (int)nudYear.Value);
                        cmd.Parameters.AddWithValue("@amount", amount);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException ex)
            {
                // 1062 = anggaran untuk kategori dan bulan itu sudah ada
                if (ex.Number == 1062)
                    Warn("Anggaran untuk kategori dan bulan ini sudah ada. Pilih barisnya lalu klik Ubah.");
                else
                    MessageBox.Show("Gagal menambah anggaran: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadBudgets();
            ClearForm();
            MessageBox.Show("Anggaran berhasil ditambahkan.", "Sukses",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (_selectedId == 0)
            {
                Warn("Pilih anggaran di tabel yang ingin diubah.");
                return;
            }

            decimal amount;
            if (!ValidateInput(out amount)) return;

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "UPDATE budgets SET category_id = @cat, month = @month, " +
                        "year = @year, amount = @amount " +
                        "WHERE id = @id AND user_id = @uid", conn))
                    {
                        cmd.Parameters.AddWithValue("@cat", cmbCategory.SelectedValue);
                        cmd.Parameters.AddWithValue("@month", cmbMonth.SelectedIndex + 1);
                        cmd.Parameters.AddWithValue("@year", (int)nudYear.Value);
                        cmd.Parameters.AddWithValue("@amount", amount);
                        cmd.Parameters.AddWithValue("@id", _selectedId);
                        cmd.Parameters.AddWithValue("@uid", Session.UserId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062)
                    Warn("Anggaran untuk kategori dan bulan ini sudah ada.");
                else
                    MessageBox.Show("Gagal mengubah anggaran: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadBudgets();
            ClearForm();
            MessageBox.Show("Anggaran berhasil diubah.", "Sukses",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedId == 0)
            {
                Warn("Pilih anggaran di tabel yang ingin dihapus.");
                return;
            }

            var hasil = MessageBox.Show("Yakin ingin menghapus anggaran ini?", "Konfirmasi",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (hasil != DialogResult.Yes) return;

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "DELETE FROM budgets WHERE id = @id AND user_id = @uid", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", _selectedId);
                        cmd.Parameters.AddWithValue("@uid", Session.UserId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menghapus anggaran: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadBudgets();
            ClearForm();
        }
    }
}