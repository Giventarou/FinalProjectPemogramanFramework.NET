namespace DailyFinanceApp.Pages
{
    partial class ucLog
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlFilter = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.cmbAction = new System.Windows.Forms.ComboBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.dgvLog = new System.Windows.Forms.DataGridView();
            this.pnlFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLog)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(216, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Riwayat Transaksi";
            // 
            // pnlFilter
            // 
            this.pnlFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.pnlFilter.Controls.Add(this.btnReset);
            this.pnlFilter.Controls.Add(this.btnSearch);
            this.pnlFilter.Controls.Add(this.cmbAction);
            this.pnlFilter.Controls.Add(this.dtpTo);
            this.pnlFilter.Controls.Add(this.dtpFrom);
            this.pnlFilter.Controls.Add(this.label3);
            this.pnlFilter.Controls.Add(this.label2);
            this.pnlFilter.Controls.Add(this.label1);
            this.pnlFilter.Location = new System.Drawing.Point(20, 65);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Size = new System.Drawing.Size(860, 80);
            this.pnlFilter.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "Dari Tanggal";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(175, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 19);
            this.label2.TabIndex = 1;
            this.label2.Text = "Sampai Tanggal";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(335, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 19);
            this.label3.TabIndex = 2;
            this.label3.Text = "Aksi";
            // 
            // dtpFrom
            // 
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFrom.Location = new System.Drawing.Point(15, 33);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(140, 25);
            this.dtpFrom.TabIndex = 3;
            // 
            // dtpTo
            // 
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTo.Location = new System.Drawing.Point(175, 33);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(140, 25);
            this.dtpTo.TabIndex = 4;
            // 
            // cmbAction
            // 
            this.cmbAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAction.FormattingEnabled = true;
            this.cmbAction.Items.AddRange(new object[] {
            "Semua",
            "Tambah",
            "Ubah",
            "Hapus"});
            this.cmbAction.Location = new System.Drawing.Point(335, 33);
            this.cmbAction.Name = "cmbAction";
            this.cmbAction.Size = new System.Drawing.Size(140, 25);
            this.cmbAction.TabIndex = 5;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.btnSearch.FlatAppearance.BorderSize = 0;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.Location = new System.Drawing.Point(500, 31);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(100, 32);
            this.btnSearch.TabIndex = 6;
            this.btnSearch.Text = "Tampilkan";
            this.btnSearch.UseVisualStyleBackColor = false;
            // 
            // btnReset
            // 
            this.btnReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(165)))), ((int)(((byte)(166)))));
            this.btnReset.FlatAppearance.BorderSize = 0;
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.ForeColor = System.Drawing.Color.White;
            this.btnReset.Location = new System.Drawing.Point(610, 31);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(100, 32);
            this.btnReset.TabIndex = 7;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = false;
            // 
            // dgvLog
            // 
            this.dgvLog.AllowUserToAddRows = false;
            this.dgvLog.AllowUserToDeleteRows = false;
            this.dgvLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLog.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLog.BackgroundColor = System.Drawing.Color.White;
            this.dgvLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(58)))), ((int)(((byte)(95)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 10F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLog.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLog.EnableHeadersVisualStyles = false;
            this.dgvLog.Location = new System.Drawing.Point(20, 160);
            this.dgvLog.MultiSelect = false;
            this.dgvLog.Name = "dgvLog";
            this.dgvLog.ReadOnly = true;
            this.dgvLog.RowHeadersVisible = false;
            this.dgvLog.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLog.Size = new System.Drawing.Size(860, 420);
            this.dgvLog.TabIndex = 2;
            // 
            // ucLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.dgvLog);
            this.Controls.Add(this.pnlFilter);
            this.Controls.Add(this.lblTitle);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "ucLog";
            this.Size = new System.Drawing.Size(900, 600);
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLog)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlFilter;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbAction;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.DataGridView dgvLog;
    }
}
