using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DailyFinanceApp.Pages
{
    public partial class ucLog : UserControl
    {
        public ucLog()
        {
            InitializeComponent();

            SetupGrid();
            ResetFilterValues();
            LoadLogs();

            btnSearch.Click += (s, e) => LoadLogs();
            btnReset.Click += (s, e) => { ResetFilterValues(); LoadLogs(); };
        }

        private void SetupGrid()
        {
            dgvLog.Columns.Clear();
            dgvLog.Columns.Add("Time", "Waktu");
            dgvLog.Columns.Add("Action", "Aksi");
            dgvLog.Columns.Add("TrxId", "ID Transaksi");
            dgvLog.Columns.Add("Old", "Nilai Lama");
            dgvLog.Columns.Add("New", "Nilai Baru");

            dgvLog.Columns["Time"].FillWeight = 18;
            dgvLog.Columns["Action"].FillWeight = 10;
            dgvLog.Columns["TrxId"].FillWeight = 10;
            dgvLog.Columns["Old"].FillWeight = 31;
            dgvLog.Columns["New"].FillWeight = 31;
        }

        private void ResetFilterValues()
        {
            dtpFrom.Value = DateTime.Today.AddDays(-30);
            dtpTo.Value = DateTime.Today;
            cmbAction.SelectedIndex = 0;
        }

        private void LoadLogs()
        {
            dgvLog.Rows.Clear();

            string sql =
                "SELECT logged_at, action, transaction_id, old_value, new_value " +
                "FROM transaction_logs " +
                "WHERE user_id = @uid " +
                "AND logged_at >= @from AND logged_at < @to ";

            string action = cmbAction.Text;
            if (!string.IsNullOrEmpty(action) && action != "Semua")
                sql += "AND action = @action ";

            sql += "ORDER BY logged_at DESC, id DESC";

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", Session.UserId);
                        cmd.Parameters.AddWithValue("@from", dtpFrom.Value.Date);
                        cmd.Parameters.AddWithValue("@to", dtpTo.Value.Date.AddDays(1));
                        if (!string.IsNullOrEmpty(action) && action != "Semua")
                            cmd.Parameters.AddWithValue("@action", action);

                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                string act = r["action"].ToString();
                                int row = dgvLog.Rows.Add(
                                    Convert.ToDateTime(r["logged_at"]).ToString("dd/MM/yyyy HH:mm:ss"),
                                    act,
                                    r["transaction_id"] == DBNull.Value ? "-" : r["transaction_id"].ToString(),
                                    r["old_value"] == DBNull.Value ? "-" : r["old_value"].ToString(),
                                    r["new_value"] == DBNull.Value ? "-" : r["new_value"].ToString());

                                // Hijau = tambah, biru = ubah, merah = hapus
                                Color c = act == "Tambah" ? Color.FromArgb(39, 174, 96)
                                        : act == "Ubah" ? Color.FromArgb(0, 102, 204)
                                        : Color.FromArgb(192, 57, 43);
                                dgvLog.Rows[row].Cells["Action"].Style.ForeColor = c;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat riwayat: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            dgvLog.ClearSelection();
        }
    }
}