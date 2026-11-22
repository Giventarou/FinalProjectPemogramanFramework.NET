using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DailyFinanceApp.Forms;

namespace DailyFinanceApp.Pages
{
    public partial class ucTransaction : UserControl
    {
        // Model data dummy. Nanti diganti model/service dari backend.
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

        private readonly List<TransactionItem> _transactions = new List<TransactionItem>();
        private readonly List<frmTransactionEdit.Option> _wallets = new List<frmTransactionEdit.Option>();
        private readonly List<frmTransactionEdit.Option> _categories = new List<frmTransactionEdit.Option>();
        private readonly CultureInfo _id = new CultureInfo("id-ID");
        private int _nextId = 1;

        public ucTransaction()
        {
            InitializeComponent();

            SetupGrid();
            LoadDummyData();
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

        private void LoadDummyData()
        {
            _wallets.Add(new frmTransactionEdit.Option { Id = 1, Name = "Dompet Harian" });
            _wallets.Add(new frmTransactionEdit.Option { Id = 2, Name = "BCA" });
            _wallets.Add(new frmTransactionEdit.Option { Id = 3, Name = "GoPay" });

            _categories.Add(new frmTransactionEdit.Option { Id = 1, Name = "Makanan", Type = "Pengeluaran" });
            _categories.Add(new frmTransactionEdit.Option { Id = 2, Name = "Transportasi", Type = "Pengeluaran" });
            _categories.Add(new frmTransactionEdit.Option { Id = 3, Name = "Pendidikan", Type = "Pengeluaran" });
            _categories.Add(new frmTransactionEdit.Option { Id = 4, Name = "Gaji", Type = "Pemasukan" });
            _categories.Add(new frmTransactionEdit.Option { Id = 5, Name = "Uang Saku", Type = "Pemasukan" });

            DateTime today = DateTime.Today;
            Add(today, "Pengeluaran", 1, 1, 25000, "Makan siang");
            Add(today.AddDays(-1), "Pengeluaran", 3, 2, 12000, "Ojek online");
            Add(today.AddDays(-2), "Pemasukan", 2, 5, 500000, "Uang saku mingguan");
            Add(today.AddDays(-5), "Pemasukan", 2, 4, 3000000, "Gaji part-time");
        }

        private void Add(DateTime date, string type, int walletId, int categoryId,
                         decimal amount, string note)
        {
            _transactions.Add(new TransactionItem
            {
                Id = _nextId++,
                Date = date,
                Type = type,
                WalletId = walletId,
                CategoryId = categoryId,
                Amount = amount,
                Note = note
            });
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

        // Menampilkan transaksi sesuai filter
        private void ApplyFilter()
        {
            string keyword = txtSearch.Text.Trim().ToLower();
            DateTime from = dtpFrom.Value.Date;
            DateTime to = dtpTo.Value.Date;
            int catId = ((frmTransactionEdit.Option)cmbCategory.SelectedItem).Id;
            int walId = ((frmTransactionEdit.Option)cmbWallet.SelectedItem).Id;
            string type = cmbType.Text;

            var data = _transactions
                .Where(t => t.Date.Date >= from && t.Date.Date <= to)
                .Where(t => catId == 0 || t.CategoryId == catId)
                .Where(t => walId == 0 || t.WalletId == walId)
                .Where(t => type == "Semua" || t.Type == type)
                .Where(t => keyword == "" ||
                            (t.Note ?? "").ToLower().Contains(keyword) ||
                            NameOfCategory(t.CategoryId).ToLower().Contains(keyword))
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.Id);

            dgvTransaction.Rows.Clear();
            foreach (var t in data)
            {
                int row = dgvTransaction.Rows.Add(
                    t.Id,
                    t.Date.ToString("dd/MM/yyyy"),
                    t.Type,
                    NameOfWallet(t.WalletId),
                    NameOfCategory(t.CategoryId),
                    Rp(t.Amount),
                    t.Note);

                // Hijau untuk pemasukan, merah untuk pengeluaran
                dgvTransaction.Rows[row].Cells["Amount"].Style.ForeColor =
                    t.Type == "Pemasukan"
                        ? System.Drawing.Color.FromArgb(39, 174, 96)
                        : System.Drawing.Color.FromArgb(192, 57, 43);
            }
            dgvTransaction.ClearSelection();
        }

        private string NameOfWallet(int id)
        {
            var w = _wallets.FirstOrDefault(x => x.Id == id);
            return w == null ? "-" : w.Name;
        }

        private string NameOfCategory(int id)
        {
            var c = _categories.FirstOrDefault(x => x.Id == id);
            return c == null ? "-" : c.Name;
        }

        private int SelectedId()
        {
            if (dgvTransaction.SelectedRows.Count == 0) return 0;
            return Convert.ToInt32(dgvTransaction.SelectedRows[0].Cells["Id"].Value);
        }

        // ---------- Event ----------

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var dlg = new frmTransactionEdit(_wallets, _categories))
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;

                // TODO: ganti dengan TransactionService.Add(...) dari backend
                // Backend juga yang menolak pengeluaran bila saldo dompet tidak cukup
                // dan mencatat ke transaction_logs.
                Add(dlg.Date, dlg.TypeName, dlg.WalletId, dlg.CategoryId, dlg.Amount, dlg.Note);
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

            var t = _transactions.First(x => x.Id == id);

            using (var dlg = new frmTransactionEdit(_wallets, _categories))
            {
                dlg.LoadData(t.Type, t.WalletId, t.CategoryId, t.Date, t.Amount, t.Note);
                if (dlg.ShowDialog() != DialogResult.OK) return;

                // TODO: ganti dengan TransactionService.Update(...) dari backend
                // (backend menyesuaikan saldo dompet dan mencatat nilai lama/baru ke log)
                t.Type = dlg.TypeName;
                t.WalletId = dlg.WalletId;
                t.CategoryId = dlg.CategoryId;
                t.Date = dlg.Date;
                t.Amount = dlg.Amount;
                t.Note = dlg.Note;
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

            // TODO: ganti dengan TransactionService.Delete(...) dari backend
            // (backend mengembalikan saldo dompet dan mencatat penghapusan ke log)
            _transactions.RemoveAll(x => x.Id == id);

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