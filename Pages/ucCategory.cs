using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DailyFinanceApp.Pages
{
    public partial class ucCategory : UserControl
    {
        // Model sederhana untuk data dummy.
        // Nanti diganti dengan model/service dari backend.
        private class CategoryItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
        }

        private readonly List<CategoryItem> _categories = new List<CategoryItem>();
        private int _nextId = 1;
        private int _selectedId = 0;

        public ucCategory()
        {
            InitializeComponent();

            SetupGrid();
            LoadDummyData();
            RefreshGrid();

            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnClear.Click += btnClear_Click;
            dgvCategory.SelectionChanged += dgvCategory_SelectionChanged;

            cmbType.SelectedIndex = 0;
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

        private void LoadDummyData()
        {
            AddToList("Makanan", "Pengeluaran");
            AddToList("Transportasi", "Pengeluaran");
            AddToList("Pendidikan", "Pengeluaran");
            AddToList("Gaji", "Pemasukan");
            AddToList("Uang Saku", "Pemasukan");
        }

        private void AddToList(string name, string type)
        {
            _categories.Add(new CategoryItem { Id = _nextId++, Name = name, Type = type });
        }

        // ---------- Tampilan ----------

        private void RefreshGrid()
        {
            dgvCategory.Rows.Clear();
            foreach (var c in _categories)
            {
                dgvCategory.Rows.Add(c.Id, c.Name, c.Type);
            }
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

            // Cegah nama kembar pada tipe yang sama
            string name = txtCategoryName.Text.Trim();
            bool duplicate = _categories.Exists(c =>
                c.Id != _selectedId &&
                c.Type == cmbType.Text &&
                string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));

            if (duplicate)
            {
                Warn("Kategori dengan nama dan tipe yang sama sudah ada.");
                txtCategoryName.Focus();
                return false;
            }

            return true;
        }

        // ---------- Event ----------

        private void dgvCategory_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCategory.SelectedRows.Count == 0) return;

            int id = Convert.ToInt32(dgvCategory.SelectedRows[0].Cells["Id"].Value);
            var c = _categories.Find(x => x.Id == id);
            if (c == null) return;

            _selectedId = c.Id;
            txtCategoryName.Text = c.Name;
            cmbType.SelectedItem = c.Type;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            // TODO: ganti dengan CategoryService.Add(...) dari backend
            AddToList(txtCategoryName.Text.Trim(), cmbType.Text);
            RefreshGrid();
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

            // TODO: ganti dengan CategoryService.Update(...) dari backend
            var c = _categories.Find(x => x.Id == _selectedId);
            if (c == null) return;

            c.Name = txtCategoryName.Text.Trim();
            c.Type = cmbType.Text;

            RefreshGrid();
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

            // TODO: ganti dengan CategoryService.Delete(...) dari backend
            // Catatan: backend perlu menolak hapus kalau kategori masih dipakai transaksi.
            _categories.RemoveAll(x => x.Id == _selectedId);
            RefreshGrid();
            ClearForm();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
    }
}