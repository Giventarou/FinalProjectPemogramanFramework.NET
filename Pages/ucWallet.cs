using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

namespace DailyFinanceApp.Pages
{
    public partial class ucWallet : UserControl
    {
        // Model sederhana untuk data dummy.
        // Nanti diganti dengan model/service dari backend.
        private class WalletItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public decimal InitialBalance { get; set; }
            public decimal CurrentBalance { get; set; }
        }

        private readonly List<WalletItem> _wallets = new List<WalletItem>();
        private readonly CultureInfo _id = new CultureInfo("id-ID");
        private int _nextId = 1;
        private int _selectedId = 0;

        public ucWallet()
        {
            InitializeComponent();

            SetupGrid();
            LoadDummyData();
            RefreshGrid();

            // Sambungkan event lewat kode
            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnClear.Click += btnClear_Click;
            dgvWallet.SelectionChanged += dgvWallet_SelectionChanged;

            cmbType.SelectedIndex = 0;
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

        private void LoadDummyData()
        {
            AddWalletToList("Dompet Harian", "Cash", 250000);
            AddWalletToList("BCA", "Bank", 3500000);
            AddWalletToList("GoPay", "E-Wallet", 150000);
        }

        private void AddWalletToList(string name, string type, decimal balance)
        {
            _wallets.Add(new WalletItem
            {
                Id = _nextId++,
                Name = name,
                Type = type,
                InitialBalance = balance,
                CurrentBalance = balance
            });
        }

        // ---------- Tampilan ----------

        private string Rp(decimal value)
        {
            return "Rp " + value.ToString("N0", _id);
        }

        private void RefreshGrid()
        {
            dgvWallet.Rows.Clear();
            foreach (var w in _wallets)
            {
                dgvWallet.Rows.Add(w.Id, w.Name, w.Type, Rp(w.InitialBalance), Rp(w.CurrentBalance));
            }
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

            int id = Convert.ToInt32(dgvWallet.SelectedRows[0].Cells["Id"].Value);
            var w = _wallets.Find(x => x.Id == id);
            if (w == null) return;

            _selectedId = w.Id;
            txtWalletName.Text = w.Name;
            cmbType.SelectedItem = w.Type;
            txtInitialBalance.Text = w.InitialBalance.ToString("0", _id);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            decimal balance;
            if (!ValidateInput(out balance)) return;

            // TODO: ganti dengan WalletService.Add(...) dari backend
            AddWalletToList(txtWalletName.Text.Trim(), cmbType.Text, balance);
            RefreshGrid();
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

            // TODO: ganti dengan WalletService.Update(...) dari backend
            var w = _wallets.Find(x => x.Id == _selectedId);
            if (w == null) return;

            w.CurrentBalance += balance - w.InitialBalance;
            w.InitialBalance = balance;
            w.Name = txtWalletName.Text.Trim();
            w.Type = cmbType.Text;

            RefreshGrid();
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

            // TODO: ganti dengan WalletService.Delete(...) dari backend
            _wallets.RemoveAll(x => x.Id == _selectedId);
            RefreshGrid();
            ClearForm();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
    }
}