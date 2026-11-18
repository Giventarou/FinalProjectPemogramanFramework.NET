namespace DailyFinanceApp
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlContent = new System.Windows.Forms.Panel();
            this.pnlSidebar = new System.Windows.Forms.Panel();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnLog = new System.Windows.Forms.Button();
            this.btnReport = new System.Windows.Forms.Button();
            this.btnSavings = new System.Windows.Forms.Button();
            this.btnBudget = new System.Windows.Forms.Button();
            this.btnTransaction = new System.Windows.Forms.Button();
            this.btnCategory = new System.Windows.Forms.Button();
            this.btnWallet = new System.Windows.Forms.Button();
            this.btnDashboard = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.lblUser = new System.Windows.Forms.Label();
            this.pnlSidebar.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlContent
            // 
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(220, 0);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(864, 611);
            this.pnlContent.TabIndex = 1;
            // 
            // pnlSidebar
            // 
            this.pnlSidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(58)))), ((int)(((byte)(95)))));
            this.pnlSidebar.Controls.Add(this.lblUser);
            this.pnlSidebar.Controls.Add(this.btnSettings);
            this.pnlSidebar.Controls.Add(this.btnLog);
            this.pnlSidebar.Controls.Add(this.btnReport);
            this.pnlSidebar.Controls.Add(this.btnSavings);
            this.pnlSidebar.Controls.Add(this.btnBudget);
            this.pnlSidebar.Controls.Add(this.btnTransaction);
            this.pnlSidebar.Controls.Add(this.btnCategory);
            this.pnlSidebar.Controls.Add(this.btnWallet);
            this.pnlSidebar.Controls.Add(this.btnDashboard);
            this.pnlSidebar.Controls.Add(this.btnLogout);
            this.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSidebar.Location = new System.Drawing.Point(0, 0);
            this.pnlSidebar.Name = "pnlSidebar";
            this.pnlSidebar.Size = new System.Drawing.Size(220, 611);
            this.pnlSidebar.TabIndex = 0;
            // 
            // btnSettings
            // 
            this.btnSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSettings.FlatAppearance.BorderSize = 0;
            this.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSettings.ForeColor = System.Drawing.Color.White;
            this.btnSettings.Location = new System.Drawing.Point(0, 360);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnSettings.Size = new System.Drawing.Size(220, 45);
            this.btnSettings.TabIndex = 8;
            this.btnSettings.TabStop = false;
            this.btnSettings.Text = "Pengaturan";
            this.btnSettings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSettings.UseVisualStyleBackColor = true;
            // 
            // btnLog
            // 
            this.btnLog.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnLog.FlatAppearance.BorderSize = 0;
            this.btnLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLog.ForeColor = System.Drawing.Color.White;
            this.btnLog.Location = new System.Drawing.Point(0, 315);
            this.btnLog.Name = "btnLog";
            this.btnLog.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnLog.Size = new System.Drawing.Size(220, 45);
            this.btnLog.TabIndex = 7;
            this.btnLog.TabStop = false;
            this.btnLog.Text = "Riwayat Transaksi";
            this.btnLog.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLog.UseVisualStyleBackColor = true;
            // 
            // btnReport
            // 
            this.btnReport.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnReport.FlatAppearance.BorderSize = 0;
            this.btnReport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReport.ForeColor = System.Drawing.Color.White;
            this.btnReport.Location = new System.Drawing.Point(0, 270);
            this.btnReport.Name = "btnReport";
            this.btnReport.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnReport.Size = new System.Drawing.Size(220, 45);
            this.btnReport.TabIndex = 6;
            this.btnReport.TabStop = false;
            this.btnReport.Text = "Laporan";
            this.btnReport.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReport.UseVisualStyleBackColor = true;
            // 
            // btnSavings
            // 
            this.btnSavings.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSavings.FlatAppearance.BorderSize = 0;
            this.btnSavings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSavings.ForeColor = System.Drawing.Color.White;
            this.btnSavings.Location = new System.Drawing.Point(0, 225);
            this.btnSavings.Name = "btnSavings";
            this.btnSavings.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnSavings.Size = new System.Drawing.Size(220, 45);
            this.btnSavings.TabIndex = 5;
            this.btnSavings.TabStop = false;
            this.btnSavings.Text = "Tabungan";
            this.btnSavings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSavings.UseVisualStyleBackColor = true;
            // 
            // btnBudget
            // 
            this.btnBudget.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnBudget.FlatAppearance.BorderSize = 0;
            this.btnBudget.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBudget.ForeColor = System.Drawing.Color.White;
            this.btnBudget.Location = new System.Drawing.Point(0, 180);
            this.btnBudget.Name = "btnBudget";
            this.btnBudget.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnBudget.Size = new System.Drawing.Size(220, 45);
            this.btnBudget.TabIndex = 4;
            this.btnBudget.TabStop = false;
            this.btnBudget.Text = "Anggaran";
            this.btnBudget.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBudget.UseVisualStyleBackColor = true;
            // 
            // btnTransaction
            // 
            this.btnTransaction.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnTransaction.FlatAppearance.BorderSize = 0;
            this.btnTransaction.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTransaction.ForeColor = System.Drawing.Color.White;
            this.btnTransaction.Location = new System.Drawing.Point(0, 135);
            this.btnTransaction.Name = "btnTransaction";
            this.btnTransaction.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnTransaction.Size = new System.Drawing.Size(220, 45);
            this.btnTransaction.TabIndex = 3;
            this.btnTransaction.TabStop = false;
            this.btnTransaction.Text = "Transaksi";
            this.btnTransaction.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTransaction.UseVisualStyleBackColor = true;
            // 
            // btnCategory
            // 
            this.btnCategory.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCategory.FlatAppearance.BorderSize = 0;
            this.btnCategory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCategory.ForeColor = System.Drawing.Color.White;
            this.btnCategory.Location = new System.Drawing.Point(0, 90);
            this.btnCategory.Name = "btnCategory";
            this.btnCategory.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnCategory.Size = new System.Drawing.Size(220, 45);
            this.btnCategory.TabIndex = 2;
            this.btnCategory.TabStop = false;
            this.btnCategory.Text = "Kategori";
            this.btnCategory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCategory.UseVisualStyleBackColor = true;
            // 
            // btnWallet
            // 
            this.btnWallet.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnWallet.FlatAppearance.BorderSize = 0;
            this.btnWallet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnWallet.ForeColor = System.Drawing.Color.White;
            this.btnWallet.Location = new System.Drawing.Point(0, 45);
            this.btnWallet.Name = "btnWallet";
            this.btnWallet.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnWallet.Size = new System.Drawing.Size(220, 45);
            this.btnWallet.TabIndex = 1;
            this.btnWallet.TabStop = false;
            this.btnWallet.Text = "Dompet";
            this.btnWallet.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnWallet.UseVisualStyleBackColor = true;
            this.btnWallet.Click += new System.EventHandler(this.btnWallet_Click);
            // 
            // btnDashboard
            // 
            this.btnDashboard.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnDashboard.FlatAppearance.BorderSize = 0;
            this.btnDashboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDashboard.ForeColor = System.Drawing.Color.White;
            this.btnDashboard.Location = new System.Drawing.Point(0, 0);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnDashboard.Size = new System.Drawing.Size(220, 45);
            this.btnDashboard.TabIndex = 0;
            this.btnDashboard.TabStop = false;
            this.btnDashboard.Text = "Dashboard";
            this.btnDashboard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDashboard.UseVisualStyleBackColor = true;
            this.btnDashboard.Click += new System.EventHandler(this.btnDashboard_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(57)))), ((int)(((byte)(43)))));
            this.btnLogout.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.ForeColor = System.Drawing.Color.White;
            this.btnLogout.Location = new System.Drawing.Point(0, 566);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnLogout.Size = new System.Drawing.Size(220, 45);
            this.btnLogout.TabIndex = 9;
            this.btnLogout.TabStop = false;
            this.btnLogout.Text = "Logout";
            this.btnLogout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLogout.UseVisualStyleBackColor = false;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // lblUser
            // 
            this.lblUser.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblUser.ForeColor = System.Drawing.Color.White;
            this.lblUser.Location = new System.Drawing.Point(0, 531);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(220, 35);
            this.lblUser.TabIndex = 10;
            this.lblUser.Text = "User";
            this.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1084, 611);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlSidebar);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Aplikasi Manajemen Keuangan Harian";
            this.pnlSidebar.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Panel pnlSidebar;
        private System.Windows.Forms.Button btnDashboard;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnLog;
        private System.Windows.Forms.Button btnReport;
        private System.Windows.Forms.Button btnSavings;
        private System.Windows.Forms.Button btnBudget;
        private System.Windows.Forms.Button btnTransaction;
        private System.Windows.Forms.Button btnCategory;
        private System.Windows.Forms.Button btnWallet;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Label lblUser;
    }
}