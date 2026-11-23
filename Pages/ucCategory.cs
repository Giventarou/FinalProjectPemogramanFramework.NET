using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace DailyFinanceApp.Pages
{
    public partial class ucCategory : UserControl
    {
        private int _selectedId = 0;

        public ucCategory()
        {
            InitializeComponent();

            SetupGrid();
            cmbType.SelectedIndex = 0;
            LoadCategories();

            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnClear.Click += btnClear_Click;
            dgvCategory.SelectionChanged += dgvCategory_SelectionChanged;
        }

        // ---------- Setup ----------

        private void SetupGrid()
        {
            dgvCategory.Columns.Clear();
            dgvCategory.Columns.Add("Id", "Id");
            dgvCategory.Columns["Id"].Visible = false;
            dgvCategory.Columns.Add("Name", "Nama Kategori");
            dgvCategory.Columns.Add("Type", "Tipe");
        }

        // ---------- Tampilan ----------

        // Ambil semua kategori milik user yang sedang login dari database
        private void LoadCategories()
        {
            dgvCategory.Rows.Clear();

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();

                    using (var cmd = new MySqlCommand(
                        "SELECT id, name, type FROM categories " +
                        "WHERE user_id = @uid ORDER BY type, name", conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", Session.UserId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dgvCategory.Rows.Add(
                                    reader["id"],
                                    reader["name"].ToString(),
                                    reader["type"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat kategori: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            _selectedId = 0;
            dgvCategory.ClearSelection();
        }

        private void ClearForm()
        {
            _selectedId = 0;
            txtCategoryName.Clear();
            cmbType.SelectedIndex = 0;
            dgvCategory.ClearSelection();
            txtCategoryName.Focus();
        }

        private void Warn(string message)
        {
            MessageBox.Show(message, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // ---------- Validasi ----------

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                Warn("Nama kategori wajib diisi.");
                txtCategoryName.Focus();
                return false;
            }

            if (cmbType.SelectedIndex < 0)
            {
                Warn("Pilih tipe kategori.");
                return false;
            }

            return true;
        }

        // ---------- Event ----------

        private void dgvCategory_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCategory.SelectedRows.Count == 0) return;

            var row = dgvCategory.SelectedRows[0];
            _selectedId = Convert.ToInt32(row.Cells["Id"].Value);
            txtCategoryName.Text = row.Cells["Name"].Value.ToString();
            cmbType.SelectedItem = row.Cells["Type"].Value.ToString();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();

                    using (var cmd = new MySqlCommand(
                        "INSERT INTO categories (user_id, name, type) " +
                        "VALUES (@uid, @name, @type)", conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", Session.UserId);
                        cmd.Parameters.AddWithValue("@name", txtCategoryName.Text.Trim());
                        cmd.Parameters.AddWithValue("@type", cmbType.Text);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException ex)
            {
                // 1062 = data kembar (nama dan tipe yang sama sudah ada)
                if (ex.Number == 1062)
                    Warn("Kategori dengan nama dan tipe yang sama sudah ada.");
                else
                    MessageBox.Show("Gagal menambah kategori: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadCategories();
            ClearForm();

            MessageBox.Show("Kategori berhasil ditambahkan.", "Sukses",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (_selectedId == 0)
            {
                Warn("Pilih kategori di tabel yang ingin diubah.");
                return;
            }

            if (!ValidateInput()) return;

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();

                    using (var cmd = new MySqlCommand(
                        "UPDATE categories SET name = @name, type = @type " +
                        "WHERE id = @id AND user_id = @uid", conn))
                    {
                        cmd.Parameters.AddWithValue("@name", txtCategoryName.Text.Trim());
                        cmd.Parameters.AddWithValue("@type", cmbType.Text);
                        cmd.Parameters.AddWithValue("@id", _selectedId);
                        cmd.Parameters.AddWithValue("@uid", Session.UserId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062)
                    Warn("Kategori dengan nama dan tipe yang sama sudah ada.");
                else
                    MessageBox.Show("Gagal mengubah kategori: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadCategories();
            ClearForm();

            MessageBox.Show("Kategori berhasil diubah.", "Sukses",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedId == 0)
            {
                Warn("Pilih kategori di tabel yang ingin dihapus.");
                return;
            }

            var hasil = MessageBox.Show("Yakin ingin menghapus kategori ini?", "Konfirmasi",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (hasil != DialogResult.Yes) return;

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();

                    using (var cmd = new MySqlCommand(
                        "DELETE FROM categories WHERE id = @id AND user_id = @uid", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", _selectedId);
                        cmd.Parameters.AddWithValue("@uid", Session.UserId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException ex)
            {
                // 1451 = kategori masih dipakai oleh transaksi
                if (ex.Number == 1451)
                    Warn("Kategori ini masih dipakai transaksi, jadi tidak bisa dihapus.");
                else
                    MessageBox.Show("Gagal menghapus kategori: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadCategories();
            ClearForm();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
    }
}