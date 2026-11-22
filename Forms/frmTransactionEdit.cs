using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace DailyFinanceApp.Forms
{
    public partial class frmTransactionEdit : Form
    {
        // Pasangan id + nama untuk combobox
        public class Option
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }   // dipakai kategori (Pemasukan/Pengeluaran)
            public override string ToString() { return Name; }
        }

        private readonly List<Option> _allCategories;
        private readonly CultureInfo _id = new CultureInfo("id-ID");

        // Hasil yang dibaca oleh halaman Transaksi setelah dialog ditutup
        public int WalletId { get; private set; }
        public int CategoryId { get; private set; }
        public string TypeName { get; private set; }
        public DateTime Date { get; private set; }
        public decimal Amount { get; private set; }
        public string Note { get; private set; }

        public frmTransactionEdit(List<Option> wallets, List<Option> categories)
        {
            InitializeComponent();

            _allCategories = categories;

            cmbWallet.DataSource = wallets.ToList();
            cmbType.SelectedIndex = 1; // default Pengeluaran

            cmbType.SelectedIndexChanged += (s, e) => FillCategories();
            btnSave.Click += btnSave_Click;
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            FillCategories();
        }

        // Dipanggil saat mode Ubah untuk mengisi form dengan data lama
        public void LoadData(string type, int walletId, int categoryId,
                             DateTime date, decimal amount, string note)
        {
            Text = "Ubah Transaksi";
            cmbType.SelectedItem = type;
            FillCategories();

            cmbWallet.SelectedItem = ((List<Option>)cmbWallet.DataSource)
                .FirstOrDefault(w => w.Id == walletId);
            cmbCategory.SelectedItem = ((List<Option>)cmbCategory.DataSource)
                .FirstOrDefault(c => c.Id == categoryId);

            dtpDate.Value = date;
            txtAmount.Text = amount.ToString("0", _id);
            txtNote.Text = note;
        }

        // Kategori difilter sesuai tipe yang dipilih
        private void FillCategories()
        {
            string type = cmbType.Text;
            cmbCategory.DataSource = _allCategories
                .Where(c => c.Type == type).ToList();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbWallet.SelectedItem == null)
            {
                Warn("Pilih dompet.");
                return;
            }

            if (cmbCategory.SelectedItem == null)
            {
                Warn("Pilih kategori.");
                return;
            }

            decimal amount;
            if (!decimal.TryParse(txtAmount.Text.Trim(), NumberStyles.Number, _id, out amount))
            {
                Warn("Nominal harus berupa angka.");
                txtAmount.Focus();
                return;
            }

            if (amount <= 0)
            {
                Warn("Nominal harus lebih dari 0.");
                txtAmount.Focus();
                return;
            }

            WalletId = ((Option)cmbWallet.SelectedItem).Id;
            CategoryId = ((Option)cmbCategory.SelectedItem).Id;
            TypeName = cmbType.Text;
            Date = dtpDate.Value.Date;
            Amount = amount;
            Note = txtNote.Text.Trim();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void Warn(string message)
        {
            MessageBox.Show(message, "Peringatan",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}