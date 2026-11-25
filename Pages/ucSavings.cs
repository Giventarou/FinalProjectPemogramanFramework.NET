using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace DailyFinanceApp.Pages
{
    public partial class ucSavings : UserControl
    {
        private class Item
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private readonly CultureInfo _id = new CultureInfo("id-ID");
        private int _selectedId = 0;

        public ucSavings()
        {
            InitializeComponent();

            SetupGrid();
            LoadWallets();
            LoadGoals();
            dtpDeadline.Value = DateTime.Today.AddMonths(3);

            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnClear.Click += (s, e) => ClearForm();
            btnDeposit.Click += btnDeposit_Click;
            dgvGoal.SelectionChanged += dgvGoal_SelectionChanged;
        }

        // ---------- Setup ----------

        private void SetupGrid()
        {
            dgvGoal.Columns.Clear();
            dgvGoal.Columns.Add("Id", "Id");
            dgvGoal.Columns["Id"].Visible = false;
            dgvGoal.Columns.Add("TargetRaw", "TargetRaw");
            dgvGoal.Columns["TargetRaw"].Visible = false;
            dgvGoal.Columns.Add("DeadlineRaw", "DeadlineRaw");
            dgvGoal.Columns["DeadlineRaw"].Visible = false;

            dgvGoal.Columns.Add("Name", "Target");
            dgvGoal.Columns.Add("Target", "Nominal Target");
            dgvGoal.Columns.Add("Saved", "Terkumpul");
            dgvGoal.Columns.Add("Percent", "Progres");
            dgvGoal.Columns.Add("Deadline", "Tenggat");
            dgvGoal.Columns.Add("Status", "Status");

            foreach (string c in new[] { "Target", "Saved", "Percent" })
                dgvGoal.Columns[c].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;
        }

        private void LoadWallets()
        {
            var list = new List<Item>();
            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "SELECT id, name FROM wallets WHERE user_id=@uid ORDER BY name", conn))
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
                MessageBox.Show("Gagal memuat dompet: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            cmbWallet.DataSource = list;
            cmbWallet.DisplayMember = "Name";
            cmbWallet.ValueMember = "Id";
        }

        // ---------- Tampilan ----------

        private string Rp(decimal v)
        {
            return "Rp " + v.ToString("N0", _id);
        }

        private void LoadGoals()
        {
            dgvGoal.Rows.Clear();

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "SELECT id, name, target_amount, saved_amount, deadline " +
                        "FROM savings_goals WHERE user_id=@uid ORDER BY deadline, name", conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", Session.UserId);
                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                decimal target = Convert.ToDecimal(r["target_amount"]);
                                decimal saved = Convert.ToDecimal(r["saved_amount"]);
                                decimal percent = target > 0 ? saved / target * 100 : 0;
                                DateTime? deadline = r["deadline"] == DBNull.Value
                                    ? (DateTime?)null : Convert.ToDateTime(r["deadline"]);

                                string status;
                                Color back;
                                if (saved >= target)
                                {
                                    status = "Tercapai";
                                    back = Color.FromArgb(213, 245, 227);
                                }
                                else if (deadline.HasValue && deadline.Value.Date < DateTime.Today)
                                {
                                    status = "Lewat tenggat";
                                    back = Color.FromArgb(250, 219, 216);
                                }
                                else
                                {
                                    status = "Berjalan";
                                    back = Color.White;
                                }

                                int row = dgvGoal.Rows.Add(
                                    r["id"], target,
                                    deadline.HasValue ? deadline.Value.ToString("yyyy-MM-dd") : "",
                                    r["name"].ToString(),
                                    Rp(target), Rp(saved),
                                    percent.ToString("0", _id) + "%",
                                    deadline.HasValue ? deadline.Value.ToString("dd/MM/yyyy") : "-",
                                    status);

                                dgvGoal.Rows[row].DefaultCellStyle.BackColor = back;
                                dgvGoal.Rows[row].DefaultCellStyle.SelectionForeColor = Color.White;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat target: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            _selectedId = 0;
            dgvGoal.ClearSelection();
        }

        private void ClearForm()
        {
            _selectedId = 0;
            txtName.Clear();
            txtTarget.Clear();
            txtDeposit.Clear();
            dtpDeadline.Value = DateTime.Today.AddMonths(3);
            dgvGoal.ClearSelection();
            txtName.Focus();
        }

        private void Warn(string message)
        {
            MessageBox.Show(message, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // ---------- Validasi ----------

        private bool ValidateGoal(out decimal target)
        {
            target = 0;

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                Warn("Nama target wajib diisi.");
                txtName.Focus();
                return false;
            }

            if (!decimal.TryParse(txtTarget.Text.Trim(), NumberStyles.Number, _id, out target))
            {
                Warn("Nominal target harus berupa angka.");
                txtTarget.Focus();
                return false;
            }

            if (target <= 0)
            {
                Warn("Nominal target harus lebih dari 0.");
                txtTarget.Focus();
                return false;
            }

            return true;
        }

        // ---------- Event ----------

        private void dgvGoal_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvGoal.SelectedRows.Count == 0) return;

            var row = dgvGoal.SelectedRows[0];
            _selectedId = Convert.ToInt32(row.Cells["Id"].Value);
            txtName.Text = row.Cells["Name"].Value.ToString();
            txtTarget.Text = Convert.ToDecimal(row.Cells["TargetRaw"].Value).ToString("0", _id);

            DateTime d;
            if (DateTime.TryParse(row.Cells["DeadlineRaw"].Value.ToString(), out d))
                dtpDeadline.Value = d;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            decimal target;
            if (!ValidateGoal(out target)) return;

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "INSERT INTO savings_goals (user_id, name, target_amount, deadline) " +
                        "VALUES (@uid, @name, @target, @deadline)", conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", Session.UserId);
                        cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                        cmd.Parameters.AddWithValue("@target", target);
                        cmd.Parameters.AddWithValue("@deadline", dtpDeadline.Value.Date);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menambah target: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadGoals();
            ClearForm();
            MessageBox.Show("Target tabungan berhasil ditambahkan.", "Sukses",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (_selectedId == 0)
            {
                Warn("Pilih target di tabel yang ingin diubah.");
                return;
            }

            decimal target;
            if (!ValidateGoal(out target)) return;

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "UPDATE savings_goals SET name=@name, target_amount=@target, " +
                        "deadline=@deadline WHERE id=@id AND user_id=@uid", conn))
                    {
                        cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                        cmd.Parameters.AddWithValue("@target", target);
                        cmd.Parameters.AddWithValue("@deadline", dtpDeadline.Value.Date);
                        cmd.Parameters.AddWithValue("@id", _selectedId);
                        cmd.Parameters.AddWithValue("@uid", Session.UserId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mengubah target: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadGoals();
            ClearForm();
            MessageBox.Show("Target tabungan berhasil diubah.", "Sukses",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedId == 0)
            {
                Warn("Pilih target di tabel yang ingin dihapus.");
                return;
            }

            var hasil = MessageBox.Show(
                "Yakin ingin menghapus target ini? Semua setorannya ikut terhapus " +
                "dan uangnya kembali ke saldo dompet.",
                "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (hasil != DialogResult.Yes) return;

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "DELETE FROM savings_goals WHERE id=@id AND user_id=@uid", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", _selectedId);
                        cmd.Parameters.AddWithValue("@uid", Session.UserId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menghapus target: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadGoals();
            ClearForm();
        }

        // Saldo dompet = saldo awal + pemasukan - pengeluaran - setoran tabungan
        private decimal WalletBalance(MySqlConnection conn, MySqlTransaction tx, int walletId)
        {
            using (var cmd = new MySqlCommand(
                "SELECT w.initial_balance " +
                "+ COALESCE((SELECT SUM(amount) FROM transactions " +
                "    WHERE wallet_id=w.id AND type='Pemasukan'),0) " +
                "- COALESCE((SELECT SUM(amount) FROM transactions " +
                "    WHERE wallet_id=w.id AND type='Pengeluaran'),0) " +
                "- COALESCE((SELECT SUM(amount) FROM savings_deposits " +
                "    WHERE wallet_id=w.id),0) " +
                "FROM wallets w WHERE w.id=@w AND w.user_id=@uid", conn, tx))
            {
                cmd.Parameters.AddWithValue("@w", walletId);
                cmd.Parameters.AddWithValue("@uid", Session.UserId);
                object o = cmd.ExecuteScalar();
                return (o == null || o == DBNull.Value) ? 0 : Convert.ToDecimal(o);
            }
        }

        private void btnDeposit_Click(object sender, EventArgs e)
        {
            if (_selectedId == 0)
            {
                Warn("Pilih target di tabel yang ingin disetori.");
                return;
            }

            if (cmbWallet.SelectedValue == null)
            {
                Warn("Pilih dompet sumber setoran.");
                return;
            }

            decimal amount;
            if (!decimal.TryParse(txtDeposit.Text.Trim(), NumberStyles.Number, _id, out amount))
            {
                Warn("Nominal setoran harus berupa angka.");
                txtDeposit.Focus();
                return;
            }

            if (amount <= 0)
            {
                Warn("Nominal setoran harus lebih dari 0.");
                txtDeposit.Focus();
                return;
            }

            int walletId = Convert.ToInt32(cmbWallet.SelectedValue);

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();
                    using (var tx = conn.BeginTransaction())
                    {
                        if (WalletBalance(conn, tx, walletId) < amount)
                        {
                            tx.Rollback();
                            Warn("Saldo dompet tidak cukup untuk setoran ini.");
                            return;
                        }

                        using (var cmd = new MySqlCommand(
                            "INSERT INTO savings_deposits " +
                            "(user_id, goal_id, wallet_id, amount, deposit_date) " +
                            "VALUES (@uid, @goal, @wallet, @amount, @date)", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@uid", Session.UserId);
                            cmd.Parameters.AddWithValue("@goal", _selectedId);
                            cmd.Parameters.AddWithValue("@wallet", walletId);
                            cmd.Parameters.AddWithValue("@amount", amount);
                            cmd.Parameters.AddWithValue("@date", DateTime.Today);
                            cmd.ExecuteNonQuery();
                        }

                        using (var cmd = new MySqlCommand(
                            "UPDATE savings_goals SET saved_amount = saved_amount + @amount " +
                            "WHERE id=@id AND user_id=@uid", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@amount", amount);
                            cmd.Parameters.AddWithValue("@id", _selectedId);
                            cmd.Parameters.AddWithValue("@uid", Session.UserId);
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyetor: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadGoals();
            ClearForm();
            MessageBox.Show("Setoran berhasil. Saldo dompet berkurang.", "Sukses",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}