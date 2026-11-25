using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace DailyFinanceApp.Pages
{
    public partial class ucReport : UserControl
    {
        private class ReportRow
        {
            public string Label;
            public decimal Income;
            public decimal Expense;
        }

        private static readonly string[] MonthNames =
        {
            "Januari", "Februari", "Maret", "April", "Mei", "Juni",
            "Juli", "Agustus", "September", "Oktober", "November", "Desember"
        };

        private readonly CultureInfo _id = new CultureInfo("id-ID");
        private readonly List<ReportRow> _rows = new List<ReportRow>();
        private string _title = "";
        private int _printIndex = 0;

        public ucReport()
        {
            InitializeComponent();

            SetupGrid();
            dtpDate.Value = DateTime.Today;
            cmbPeriod.SelectedIndexChanged += (s, e) => UpdateDateFormat();
            cmbPeriod.SelectedIndex = 1; // default: Bulanan

            btnShow.Click += (s, e) => LoadReport();
            btnExport.Click += btnExport_Click;
            btnPrint.Click += btnPrint_Click;

            LoadReport();
        }

        // ---------- Setup ----------

        private void SetupGrid()
        {
            dgvReport.Columns.Clear();
            dgvReport.Columns.Add("Label", "Periode");
            dgvReport.Columns.Add("Income", "Pemasukan");
            dgvReport.Columns.Add("Expense", "Pengeluaran");
            dgvReport.Columns.Add("Net", "Selisih");

            foreach (string c in new[] { "Income", "Expense", "Net" })
                dgvReport.Columns[c].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;
        }

        private void UpdateDateFormat()
        {
            switch (cmbPeriod.Text)
            {
                case "Harian":
                    dtpDate.Format = DateTimePickerFormat.Short;
                    break;
                case "Bulanan":
                    dtpDate.Format = DateTimePickerFormat.Custom;
                    dtpDate.CustomFormat = "MMMM yyyy";
                    break;
                default:
                    dtpDate.Format = DateTimePickerFormat.Custom;
                    dtpDate.CustomFormat = "yyyy";
                    break;
            }
        }

        private string Rp(decimal v)
        {
            return "Rp " + v.ToString("N0", _id);
        }

        // ---------- Ambil data ----------

        private void LoadReport()
        {
            _rows.Clear();
            DateTime d = dtpDate.Value.Date;

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();

                    switch (cmbPeriod.Text)
                    {
                        case "Harian":
                            _title = "Laporan Harian " + d.ToString("dd MMMM yyyy", _id);
                            LoadDaily(conn, d);
                            break;
                        case "Tahunan":
                            _title = "Laporan Tahunan " + d.Year;
                            LoadYearly(conn, d.Year);
                            break;
                        default:
                            _title = "Laporan Bulanan " + MonthNames[d.Month - 1] + " " + d.Year;
                            LoadMonthly(conn, d.Year, d.Month);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat laporan: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ShowRows();
        }

        private const string SumCols =
            "COALESCE(SUM(CASE WHEN t.type='Pemasukan' THEN t.amount ELSE 0 END),0) AS inc, " +
            "COALESCE(SUM(CASE WHEN t.type='Pengeluaran' THEN t.amount ELSE 0 END),0) AS exp ";

        // Harian: per kategori
        private void LoadDaily(MySqlConnection conn, DateTime d)
        {
            using (var cmd = new MySqlCommand(
                "SELECT c.name AS label, " + SumCols +
                "FROM transactions t JOIN categories c ON c.id=t.category_id " +
                "WHERE t.user_id=@uid AND t.trx_date=@d " +
                "GROUP BY c.name ORDER BY c.name", conn))
            {
                cmd.Parameters.AddWithValue("@uid", Session.UserId);
                cmd.Parameters.AddWithValue("@d", d);
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        _rows.Add(new ReportRow
                        {
                            Label = r["label"].ToString(),
                            Income = Convert.ToDecimal(r["inc"]),
                            Expense = Convert.ToDecimal(r["exp"])
                        });
                }
            }
        }

        // Bulanan: per hari, semua hari dalam bulan ditampilkan
        private void LoadMonthly(MySqlConnection conn, int year, int month)
        {
            var inc = new Dictionary<int, decimal>();
            var exp = new Dictionary<int, decimal>();

            using (var cmd = new MySqlCommand(
                "SELECT DAY(t.trx_date) AS k, " + SumCols +
                "FROM transactions t " +
                "WHERE t.user_id=@uid AND YEAR(t.trx_date)=@y AND MONTH(t.trx_date)=@m " +
                "GROUP BY DAY(t.trx_date)", conn))
            {
                cmd.Parameters.AddWithValue("@uid", Session.UserId);
                cmd.Parameters.AddWithValue("@y", year);
                cmd.Parameters.AddWithValue("@m", month);
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        int k = Convert.ToInt32(r["k"]);
                        inc[k] = Convert.ToDecimal(r["inc"]);
                        exp[k] = Convert.ToDecimal(r["exp"]);
                    }
                }
            }

            int days = DateTime.DaysInMonth(year, month);
            for (int day = 1; day <= days; day++)
                _rows.Add(new ReportRow
                {
                    Label = day.ToString("00") + "/" + month.ToString("00") + "/" + year,
                    Income = inc.ContainsKey(day) ? inc[day] : 0,
                    Expense = exp.ContainsKey(day) ? exp[day] : 0
                });
        }

        // Tahunan: per bulan, 12 bulan ditampilkan
        private void LoadYearly(MySqlConnection conn, int year)
        {
            var inc = new Dictionary<int, decimal>();
            var exp = new Dictionary<int, decimal>();

            using (var cmd = new MySqlCommand(
                "SELECT MONTH(t.trx_date) AS k, " + SumCols +
                "FROM transactions t " +
                "WHERE t.user_id=@uid AND YEAR(t.trx_date)=@y " +
                "GROUP BY MONTH(t.trx_date)", conn))
            {
                cmd.Parameters.AddWithValue("@uid", Session.UserId);
                cmd.Parameters.AddWithValue("@y", year);
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        int k = Convert.ToInt32(r["k"]);
                        inc[k] = Convert.ToDecimal(r["inc"]);
                        exp[k] = Convert.ToDecimal(r["exp"]);
                    }
                }
            }

            for (int m = 1; m <= 12; m++)
                _rows.Add(new ReportRow
                {
                    Label = MonthNames[m - 1],
                    Income = inc.ContainsKey(m) ? inc[m] : 0,
                    Expense = exp.ContainsKey(m) ? exp[m] : 0
                });
        }

        // ---------- Tampilkan ----------

        private void ShowRows()
        {
            dgvReport.Rows.Clear();

            decimal totalInc = 0, totalExp = 0;
            foreach (var r in _rows)
            {
                decimal net = r.Income - r.Expense;
                int i = dgvReport.Rows.Add(r.Label, Rp(r.Income), Rp(r.Expense), Rp(net));
                dgvReport.Rows[i].Cells["Net"].Style.ForeColor = net >= 0
                    ? Color.FromArgb(39, 174, 96) : Color.FromArgb(192, 57, 43);
                totalInc += r.Income;
                totalExp += r.Expense;
            }

            // Baris total
            int t = dgvReport.Rows.Add("TOTAL", Rp(totalInc), Rp(totalExp), Rp(totalInc - totalExp));
            dgvReport.Rows[t].DefaultCellStyle.Font = new Font(dgvReport.Font, FontStyle.Bold);
            dgvReport.Rows[t].DefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            dgvReport.ClearSelection();

            // Chart kolom: pemasukan vs pengeluaran
            chartReport.Series.Clear();
            chartReport.Titles.Clear();
            chartReport.ChartAreas.Clear();
            chartReport.Legends.Clear();
            chartReport.ChartAreas.Add(new ChartArea("main"));
            chartReport.Legends.Add(new Legend("legend"));
            chartReport.Titles.Add(_title);

            var sInc = new Series("Pemasukan")
            { ChartType = SeriesChartType.Column, Color = Color.FromArgb(39, 174, 96), Legend = "legend" };
            var sExp = new Series("Pengeluaran")
            { ChartType = SeriesChartType.Column, Color = Color.FromArgb(192, 57, 43), Legend = "legend" };

            foreach (var r in _rows)
            {
                sInc.Points.AddXY(r.Label, (double)r.Income);
                sExp.Points.AddXY(r.Label, (double)r.Expense);
            }

            chartReport.Series.Add(sInc);
            chartReport.Series.Add(sExp);
            chartReport.ChartAreas[0].AxisX.Interval = 1;
            chartReport.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
        }

        // ---------- Export CSV ----------

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (_rows.Count == 0)
            {
                MessageBox.Show("Tidak ada data untuk diekspor.", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "File CSV (*.csv)|*.csv";
                dlg.FileName = _title.Replace(" ", "_") + ".csv";
                if (dlg.ShowDialog() != DialogResult.OK) return;

                try
                {
                    var sb = new StringBuilder();
                    sb.AppendLine(_title);
                    sb.AppendLine("Periode;Pemasukan;Pengeluaran;Selisih");

                    decimal ti = 0, te = 0;
                    foreach (var r in _rows)
                    {
                        sb.AppendLine(Esc(r.Label) + ";" + (long)r.Income + ";" +
                                      (long)r.Expense + ";" + (long)(r.Income - r.Expense));
                        ti += r.Income;
                        te += r.Expense;
                    }
                    sb.AppendLine("TOTAL;" + (long)ti + ";" + (long)te + ";" + (long)(ti - te));

                    // UTF-8 dengan BOM supaya terbaca benar di Excel
                    File.WriteAllText(dlg.FileName, sb.ToString(), new UTF8Encoding(true));

                    MessageBox.Show("Laporan berhasil diekspor.", "Sukses",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal mengekspor: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string Esc(string s)
        {
            return s.Contains(";") ? "\"" + s.Replace("\"", "\"\"") + "\"" : s;
        }

        // ---------- Cetak ----------

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (_rows.Count == 0)
            {
                MessageBox.Show("Tidak ada data untuk dicetak.", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _printIndex = 0;
            using (var doc = new PrintDocument())
            using (var preview = new PrintPreviewDialog())
            {
                doc.PrintPage += PrintPage;
                preview.Document = doc;
                preview.Width = 900;
                preview.Height = 700;
                preview.ShowDialog();
            }
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            var titleFont = new Font("Segoe UI", 14F, FontStyle.Bold);
            var headFont = new Font("Segoe UI", 10F, FontStyle.Bold);
            var bodyFont = new Font("Segoe UI", 10F);

            float left = e.MarginBounds.Left;
            float y = e.MarginBounds.Top;
            float w = e.MarginBounds.Width;
            float[] cols = { 0, w * 0.34f, w * 0.56f, w * 0.78f };

            if (_printIndex == 0)
            {
                e.Graphics.DrawString(_title, titleFont, Brushes.Black, left, y);
                y += 40;
            }

            e.Graphics.DrawString("Periode", headFont, Brushes.Black, left + cols[0], y);
            e.Graphics.DrawString("Pemasukan", headFont, Brushes.Black, left + cols[1], y);
            e.Graphics.DrawString("Pengeluaran", headFont, Brushes.Black, left + cols[2], y);
            e.Graphics.DrawString("Selisih", headFont, Brushes.Black, left + cols[3], y);
            y += 24;
            e.Graphics.DrawLine(Pens.Black, left, y, left + w, y);
            y += 6;

            while (_printIndex < _rows.Count)
            {
                if (y + 22 > e.MarginBounds.Bottom)
                {
                    e.HasMorePages = true;
                    return;
                }

                var r = _rows[_printIndex];
                e.Graphics.DrawString(r.Label, bodyFont, Brushes.Black, left + cols[0], y);
                e.Graphics.DrawString(Rp(r.Income), bodyFont, Brushes.Black, left + cols[1], y);
                e.Graphics.DrawString(Rp(r.Expense), bodyFont, Brushes.Black, left + cols[2], y);
                e.Graphics.DrawString(Rp(r.Income - r.Expense), bodyFont, Brushes.Black, left + cols[3], y);
                y += 22;
                _printIndex++;
            }

            decimal ti = 0, te = 0;
            foreach (var r in _rows) { ti += r.Income; te += r.Expense; }

            e.Graphics.DrawLine(Pens.Black, left, y, left + w, y);
            y += 6;
            e.Graphics.DrawString("TOTAL", headFont, Brushes.Black, left + cols[0], y);
            e.Graphics.DrawString(Rp(ti), headFont, Brushes.Black, left + cols[1], y);
            e.Graphics.DrawString(Rp(te), headFont, Brushes.Black, left + cols[2], y);
            e.Graphics.DrawString(Rp(ti - te), headFont, Brushes.Black, left + cols[3], y);

            e.HasMorePages = false;
        }
    }
}