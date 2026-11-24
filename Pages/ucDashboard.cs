using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace DailyFinanceApp.Pages
{
    public partial class ucDashboard : UserControl
    {
        private readonly CultureInfo _id = new CultureInfo("id-ID");

        public ucDashboard()
        {
            InitializeComponent();
            SetupGrids();
            LoadData();
        }

        private void SetupGrids()
        {
            dgvWarning.Columns.Clear();
            dgvWarning.Columns.Add("Category", "Kategori");
            dgvWarning.Columns.Add("Usage", "Pemakaian");
            dgvWarning.Columns.Add("Status", "Status");

            dgvRecent.Columns.Clear();
            dgvRecent.Columns.Add("Date", "Tanggal");
            dgvRecent.Columns.Add("Type", "Tipe");
            dgvRecent.Columns.Add("Category", "Kategori");
            dgvRecent.Columns.Add("Amount", "Nominal");
            dgvRecent.Columns.Add("Note", "Catatan");
            dgvRecent.Columns["Amount"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;
        }

        private string Rp(decimal v)
        {
            return "Rp " + v.ToString("N0", _id);
        }

        private void LoadData()
        {
            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();
                    LoadSummary(conn);
                    LoadChart(conn);
                    LoadWarnings(conn);
                    LoadRecent(conn);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat dashboard: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private decimal Scalar(MySqlConnection conn, string sql)
        {
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@uid", Session.UserId);
                cmd.Parameters.AddWithValue("@m", DateTime.Today.Month);
                cmd.Parameters.AddWithValue("@y", DateTime.Today.Year);
                object o = cmd.ExecuteScalar();
                return (o == null || o == DBNull.Value) ? 0 : Convert.ToDecimal(o);
            }
        }

        // Total saldo = saldo awal semua dompet + pemasukan - pengeluaran
        private void LoadSummary(MySqlConnection conn)
        {
            decimal initial = Scalar(conn,
                "SELECT COALESCE(SUM(initial_balance),0) FROM wallets WHERE user_id=@uid");
            decimal income = Scalar(conn,
                "SELECT COALESCE(SUM(amount),0) FROM transactions " +
                "WHERE user_id=@uid AND type='Pemasukan'");
            decimal expense = Scalar(conn,
                "SELECT COALESCE(SUM(amount),0) FROM transactions " +
                "WHERE user_id=@uid AND type='Pengeluaran'");
            lblBalance.Text = Rp(initial + income - expense);

            lblIncome.Text = Rp(Scalar(conn,
                "SELECT COALESCE(SUM(amount),0) FROM transactions " +
                "WHERE user_id=@uid AND type='Pemasukan' " +
                "AND MONTH(trx_date)=@m AND YEAR(trx_date)=@y"));

            lblExpense.Text = Rp(Scalar(conn,
                "SELECT COALESCE(SUM(amount),0) FROM transactions " +
                "WHERE user_id=@uid AND type='Pengeluaran' " +
                "AND MONTH(trx_date)=@m AND YEAR(trx_date)=@y"));
        }

        private void LoadChart(MySqlConnection conn)
        {
            chartExpense.Series.Clear();
            chartExpense.Titles.Clear();
            chartExpense.Legends.Clear();
            chartExpense.Legends.Add(new Legend("legend") { Docking = Docking.Right });

            var series = new Series("Pengeluaran") { ChartType = SeriesChartType.Pie };
            series.Legend = "legend";

            using (var cmd = new MySqlCommand(
                "SELECT c.name, SUM(t.amount) AS total FROM transactions t " +
                "JOIN categories c ON c.id = t.category_id " +
                "WHERE t.user_id=@uid AND t.type='Pengeluaran' " +
                "AND MONTH(t.trx_date)=@m AND YEAR(t.trx_date)=@y " +
                "GROUP BY c.name ORDER BY total DESC", conn))
            {
                cmd.Parameters.AddWithValue("@uid", Session.UserId);
                cmd.Parameters.AddWithValue("@m", DateTime.Today.Month);
                cmd.Parameters.AddWithValue("@y", DateTime.Today.Year);

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        int i = series.Points.AddXY(
                            r["name"].ToString(), Convert.ToDouble(r["total"]));
                        series.Points[i].ToolTip = Rp(Convert.ToDecimal(r["total"]));
                    }
                }
            }

            chartExpense.Titles.Add(series.Points.Count == 0
                ? "Belum ada pengeluaran bulan ini"
                : "Pengeluaran per Kategori (bulan ini)");
            chartExpense.Series.Add(series);
        }

        // Anggaran bulan ini yang sudah mencapai 80% atau lebih
        private void LoadWarnings(MySqlConnection conn)
        {
            dgvWarning.Rows.Clear();

            using (var cmd = new MySqlCommand(
                "SELECT c.name, b.amount, " +
                "COALESCE((SELECT SUM(t.amount) FROM transactions t " +
                "  WHERE t.user_id=b.user_id AND t.category_id=b.category_id " +
                "  AND t.type='Pengeluaran' " +
                "  AND MONTH(t.trx_date)=b.month AND YEAR(t.trx_date)=b.year),0) AS spent " +
                "FROM budgets b JOIN categories c ON c.id=b.category_id " +
                "WHERE b.user_id=@uid AND b.month=@m AND b.year=@y", conn))
            {
                cmd.Parameters.AddWithValue("@uid", Session.UserId);
                cmd.Parameters.AddWithValue("@m", DateTime.Today.Month);
                cmd.Parameters.AddWithValue("@y", DateTime.Today.Year);

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        decimal limit = Convert.ToDecimal(r["amount"]);
                        decimal spent = Convert.ToDecimal(r["spent"]);
                        decimal percent = limit > 0 ? spent / limit * 100 : 0;
                        if (percent < 80) continue;

                        bool over = percent > 100;
                        int row = dgvWarning.Rows.Add(
                            r["name"].ToString(),
                            percent.ToString("0", _id) + "%",
                            over ? "Melebihi batas" : "Hampir habis");

                        dgvWarning.Rows[row].DefaultCellStyle.BackColor = over
                            ? Color.FromArgb(250, 219, 216)
                            : Color.FromArgb(253, 243, 205);
                        dgvWarning.Rows[row].DefaultCellStyle.SelectionForeColor = Color.White;
                    }
                }
            }

            if (dgvWarning.Rows.Count == 0)
                dgvWarning.Rows.Add("Tidak ada peringatan", "-", "Aman");

            dgvWarning.ClearSelection();
        }

        private void LoadRecent(MySqlConnection conn)
        {
            dgvRecent.Rows.Clear();

            using (var cmd = new MySqlCommand(
                "SELECT t.trx_date, t.type, c.name, t.amount, t.note " +
                "FROM transactions t JOIN categories c ON c.id=t.category_id " +
                "WHERE t.user_id=@uid " +
                "ORDER BY t.trx_date DESC, t.id DESC LIMIT 5", conn))
            {
                cmd.Parameters.AddWithValue("@uid", Session.UserId);

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        string type = r["type"].ToString();
                        int row = dgvRecent.Rows.Add(
                            Convert.ToDateTime(r["trx_date"]).ToString("dd/MM/yyyy"),
                            type,
                            r["name"].ToString(),
                            Rp(Convert.ToDecimal(r["amount"])),
                            r["note"] == DBNull.Value ? "" : r["note"].ToString());

                        dgvRecent.Rows[row].Cells["Amount"].Style.ForeColor =
                            type == "Pemasukan"
                                ? Color.FromArgb(39, 174, 96)
                                : Color.FromArgb(192, 57, 43);
                    }
                }
            }

            dgvRecent.ClearSelection();
        }
    }
}