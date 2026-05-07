namespace frm_HIS_uninstaller_tool
{
    partial class Form2
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.tblMain = new System.Windows.Forms.TableLayoutPanel();
            this.pnlFiles = new System.Windows.Forms.Panel();
            this.lstFiles = new System.Windows.Forms.ListView();
            this.colFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFilePath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFileSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pnlFileBtns = new System.Windows.Forms.Panel();
            this.btnDeleteFiles = new System.Windows.Forms.Button();
            this.pnlFileTop = new System.Windows.Forms.Panel();
            this.lblFilesCount = new System.Windows.Forms.Label();
            this.lblFileHeader = new System.Windows.Forms.Label();
            this.pnlReg = new System.Windows.Forms.Panel();
            this.lstRegistry = new System.Windows.Forms.ListView();
            this.colRegKey = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colRegValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblRegWarning = new System.Windows.Forms.Label();
            this.pnlRegBtns = new System.Windows.Forms.Panel();
            this.btnDeepScan = new System.Windows.Forms.Button();
            this.btnDeleteReg = new System.Windows.Forms.Button();
            this.pnlRegTop = new System.Windows.Forms.Panel();
            this.lblRegCount = new System.Windows.Forms.Label();
            this.lblRegHeader = new System.Windows.Forms.Label();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            this.tblMain.SuspendLayout();
            this.pnlFiles.SuspendLayout();
            this.pnlFileBtns.SuspendLayout();
            this.pnlFileTop.SuspendLayout();
            this.pnlReg.SuspendLayout();
            this.pnlRegBtns.SuspendLayout();
            this.pnlRegTop.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(90)))), ((int)(((byte)(160)))));
            this.pnlHeader.Controls.Add(this.lblSubtitle);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Padding = new System.Windows.Forms.Padding(16, 8, 16, 8);
            this.pnlHeader.Size = new System.Drawing.Size(740, 64);
            this.pnlHeader.TabIndex = 2;
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(220)))), ((int)(((byte)(255)))));
            this.lblSubtitle.Location = new System.Drawing.Point(18, 38);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(308, 15);
            this.lblSubtitle.TabIndex = 0;
            this.lblSubtitle.Text = "Quét và dọn dẹp file/registry còn sót sau khi gỡ Huorong";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(16, 8);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(165, 25);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "HIS Cleanup Tool";
            // 
            // tblMain
            // 
            this.tblMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.tblMain.ColumnCount = 2;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblMain.Controls.Add(this.pnlFiles, 0, 0);
            this.tblMain.Controls.Add(this.pnlReg, 1, 0);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(0, 64);
            this.tblMain.Name = "tblMain";
            this.tblMain.Padding = new System.Windows.Forms.Padding(6);
            this.tblMain.RowCount = 1;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.Size = new System.Drawing.Size(740, 273);
            this.tblMain.TabIndex = 0;
            // 
            // pnlFiles
            // 
            this.pnlFiles.BackColor = System.Drawing.Color.White;
            this.pnlFiles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFiles.Controls.Add(this.lstFiles);
            this.pnlFiles.Controls.Add(this.pnlFileBtns);
            this.pnlFiles.Controls.Add(this.pnlFileTop);
            this.pnlFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFiles.Location = new System.Drawing.Point(6, 6);
            this.pnlFiles.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.pnlFiles.Name = "pnlFiles";
            this.pnlFiles.Size = new System.Drawing.Size(361, 261);
            this.pnlFiles.TabIndex = 0;
            // 
            // lstFiles
            // 
            this.lstFiles.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstFiles.CheckBoxes = true;
            this.lstFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colFileName,
            this.colFilePath,
            this.colFileSize});
            this.lstFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstFiles.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lstFiles.FullRowSelect = true;
            this.lstFiles.GridLines = true;
            this.lstFiles.HideSelection = false;
            this.lstFiles.Location = new System.Drawing.Point(0, 38);
            this.lstFiles.Name = "lstFiles";
            this.lstFiles.Size = new System.Drawing.Size(359, 177);
            this.lstFiles.TabIndex = 0;
            this.lstFiles.UseCompatibleStateImageBehavior = false;
            this.lstFiles.View = System.Windows.Forms.View.Details;
            // 
            // colFileName
            // 
            this.colFileName.Text = "Tên";
            this.colFileName.Width = 120;
            // 
            // colFilePath
            // 
            this.colFilePath.Text = "Đường dẫn";
            this.colFilePath.Width = 220;
            // 
            // colFileSize
            // 
            this.colFileSize.Text = "Kích thước";
            this.colFileSize.Width = 80;
            // 
            // pnlFileBtns
            // 
            this.pnlFileBtns.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            this.pnlFileBtns.Controls.Add(this.btnDeleteFiles);
            this.pnlFileBtns.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFileBtns.Location = new System.Drawing.Point(0, 215);
            this.pnlFileBtns.Name = "pnlFileBtns";
            this.pnlFileBtns.Size = new System.Drawing.Size(359, 44);
            this.pnlFileBtns.TabIndex = 1;
            // 
            // btnDeleteFiles
            // 
            this.btnDeleteFiles.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(57)))), ((int)(((byte)(43)))));
            this.btnDeleteFiles.Enabled = false;
            this.btnDeleteFiles.FlatAppearance.BorderSize = 0;
            this.btnDeleteFiles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteFiles.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnDeleteFiles.ForeColor = System.Drawing.Color.White;
            this.btnDeleteFiles.Location = new System.Drawing.Point(10, 8);
            this.btnDeleteFiles.Name = "btnDeleteFiles";
            this.btnDeleteFiles.Size = new System.Drawing.Size(160, 28);
            this.btnDeleteFiles.TabIndex = 0;
            this.btnDeleteFiles.Text = "🗑  Xóa mục đã chọn";
            this.btnDeleteFiles.UseVisualStyleBackColor = false;
            this.btnDeleteFiles.Click += new System.EventHandler(this.btnDeleteFiles_Click);
            // 
            // pnlFileTop
            // 
            this.pnlFileTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(250)))));
            this.pnlFileTop.Controls.Add(this.lblFilesCount);
            this.pnlFileTop.Controls.Add(this.lblFileHeader);
            this.pnlFileTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFileTop.Location = new System.Drawing.Point(0, 0);
            this.pnlFileTop.Name = "pnlFileTop";
            this.pnlFileTop.Size = new System.Drawing.Size(359, 38);
            this.pnlFileTop.TabIndex = 2;
            // 
            // lblFilesCount
            // 
            this.lblFilesCount.AutoSize = true;
            this.lblFilesCount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblFilesCount.ForeColor = System.Drawing.Color.Gray;
            this.lblFilesCount.Location = new System.Drawing.Point(250, 12);
            this.lblFilesCount.Name = "lblFilesCount";
            this.lblFilesCount.Size = new System.Drawing.Size(62, 15);
            this.lblFilesCount.TabIndex = 0;
            this.lblFilesCount.Text = "Chưa quét";
            // 
            // lblFileHeader
            // 
            this.lblFileHeader.AutoSize = true;
            this.lblFileHeader.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblFileHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(90)))), ((int)(((byte)(160)))));
            this.lblFileHeader.Location = new System.Drawing.Point(10, 10);
            this.lblFileHeader.Name = "lblFileHeader";
            this.lblFileHeader.Size = new System.Drawing.Size(183, 19);
            this.lblFileHeader.TabIndex = 1;
            this.lblFileHeader.Text = "📁  File / Thư mục còn sót";
            // 
            // pnlReg
            // 
            this.pnlReg.BackColor = System.Drawing.Color.White;
            this.pnlReg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlReg.Controls.Add(this.lstRegistry);
            this.pnlReg.Controls.Add(this.lblRegWarning);
            this.pnlReg.Controls.Add(this.pnlRegBtns);
            this.pnlReg.Controls.Add(this.pnlRegTop);
            this.pnlReg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlReg.Location = new System.Drawing.Point(373, 6);
            this.pnlReg.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.pnlReg.Name = "pnlReg";
            this.pnlReg.Size = new System.Drawing.Size(361, 261);
            this.pnlReg.TabIndex = 1;
            // 
            // lstRegistry
            // 
            this.lstRegistry.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstRegistry.CheckBoxes = true;
            this.lstRegistry.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colRegKey,
            this.colRegValue});
            this.lstRegistry.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstRegistry.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lstRegistry.FullRowSelect = true;
            this.lstRegistry.GridLines = true;
            this.lstRegistry.HideSelection = false;
            this.lstRegistry.Location = new System.Drawing.Point(0, 38);
            this.lstRegistry.Name = "lstRegistry";
            this.lstRegistry.Size = new System.Drawing.Size(359, 155);
            this.lstRegistry.TabIndex = 0;
            this.lstRegistry.UseCompatibleStateImageBehavior = false;
            this.lstRegistry.View = System.Windows.Forms.View.Details;
            // 
            // colRegKey
            // 
            this.colRegKey.Text = "Registry Key";
            this.colRegKey.Width = 290;
            // 
            // colRegValue
            // 
            this.colRegValue.Text = "Giá trị";
            this.colRegValue.Width = 150;
            // 
            // lblRegWarning
            // 
            this.lblRegWarning.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblRegWarning.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            this.lblRegWarning.ForeColor = System.Drawing.Color.DarkOrange;
            this.lblRegWarning.Location = new System.Drawing.Point(0, 193);
            this.lblRegWarning.Name = "lblRegWarning";
            this.lblRegWarning.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.lblRegWarning.Size = new System.Drawing.Size(359, 22);
            this.lblRegWarning.TabIndex = 1;
            this.lblRegWarning.Text = "⚠  Registry hệ thống sẽ được backup vào Desktop trước khi xóa";
            this.lblRegWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlRegBtns
            // 
            this.pnlRegBtns.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            this.pnlRegBtns.Controls.Add(this.btnDeepScan);
            this.pnlRegBtns.Controls.Add(this.btnDeleteReg);
            this.pnlRegBtns.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlRegBtns.Location = new System.Drawing.Point(0, 215);
            this.pnlRegBtns.Name = "pnlRegBtns";
            this.pnlRegBtns.Size = new System.Drawing.Size(359, 44);
            this.pnlRegBtns.TabIndex = 2;
            // 
            // btnDeepScan
            // 
            this.btnDeepScan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(100)))), ((int)(((byte)(50)))));
            this.btnDeepScan.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnDeepScan.FlatAppearance.BorderSize = 0;
            this.btnDeepScan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeepScan.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnDeepScan.ForeColor = System.Drawing.Color.White;
            this.btnDeepScan.Location = new System.Drawing.Point(239, 0);
            this.btnDeepScan.Name = "btnDeepScan";
            this.btnDeepScan.Size = new System.Drawing.Size(120, 44);
            this.btnDeepScan.TabIndex = 0;
            this.btnDeepScan.Text = "🔎  Deep Scan...";
            this.btnDeepScan.UseVisualStyleBackColor = false;
            this.btnDeepScan.Click += new System.EventHandler(this.btnDeepScan_Click);
            // 
            // btnDeleteReg
            // 
            this.btnDeleteReg.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(36)))), ((int)(((byte)(27)))));
            this.btnDeleteReg.Enabled = false;
            this.btnDeleteReg.FlatAppearance.BorderSize = 0;
            this.btnDeleteReg.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteReg.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnDeleteReg.ForeColor = System.Drawing.Color.White;
            this.btnDeleteReg.Location = new System.Drawing.Point(10, 8);
            this.btnDeleteReg.Name = "btnDeleteReg";
            this.btnDeleteReg.Size = new System.Drawing.Size(180, 28);
            this.btnDeleteReg.TabIndex = 1;
            this.btnDeleteReg.Text = "💾  Backup & Xóa đã chọn";
            this.btnDeleteReg.UseVisualStyleBackColor = false;
            this.btnDeleteReg.Click += new System.EventHandler(this.btnDeleteReg_Click);
            // 
            // pnlRegTop
            // 
            this.pnlRegTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(250)))));
            this.pnlRegTop.Controls.Add(this.lblRegCount);
            this.pnlRegTop.Controls.Add(this.lblRegHeader);
            this.pnlRegTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlRegTop.Location = new System.Drawing.Point(0, 0);
            this.pnlRegTop.Name = "pnlRegTop";
            this.pnlRegTop.Size = new System.Drawing.Size(359, 38);
            this.pnlRegTop.TabIndex = 3;
            // 
            // lblRegCount
            // 
            this.lblRegCount.AutoSize = true;
            this.lblRegCount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblRegCount.ForeColor = System.Drawing.Color.Gray;
            this.lblRegCount.Location = new System.Drawing.Point(200, 12);
            this.lblRegCount.Name = "lblRegCount";
            this.lblRegCount.Size = new System.Drawing.Size(62, 15);
            this.lblRegCount.TabIndex = 0;
            this.lblRegCount.Text = "Chưa quét";
            // 
            // lblRegHeader
            // 
            this.lblRegHeader.AutoSize = true;
            this.lblRegHeader.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblRegHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(90)))), ((int)(((byte)(160)))));
            this.lblRegHeader.Location = new System.Drawing.Point(10, 10);
            this.lblRegHeader.Name = "lblRegHeader";
            this.lblRegHeader.Size = new System.Drawing.Size(145, 19);
            this.lblRegHeader.TabIndex = 1;
            this.lblRegHeader.Text = "🔑  Registry còn sót";
            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.pnlBottom.Controls.Add(this.txtLog);
            this.pnlBottom.Controls.Add(this.btnClose);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 337);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Padding = new System.Windows.Forms.Padding(6);
            this.pnlBottom.Size = new System.Drawing.Size(740, 120);
            this.pnlBottom.TabIndex = 1;
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 8.5F);
            this.txtLog.ForeColor = System.Drawing.Color.Lime;
            this.txtLog.Location = new System.Drawing.Point(6, 6);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(628, 108);
            this.txtLog.TabIndex = 0;
            this.txtLog.Text = "";
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnClose.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(634, 6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 108);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Đóng";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(240)))), ((int)(((byte)(245)))));
            this.ClientSize = new System.Drawing.Size(744, 461);
            this.Controls.Add(this.tblMain);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(760, 500);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(760, 500);
            this.Name = "Form2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HIS Cleanup Tool";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.tblMain.ResumeLayout(false);
            this.pnlFiles.ResumeLayout(false);
            this.pnlFileBtns.ResumeLayout(false);
            this.pnlFileTop.ResumeLayout(false);
            this.pnlFileTop.PerformLayout();
            this.pnlReg.ResumeLayout(false);
            this.pnlRegBtns.ResumeLayout(false);
            this.pnlRegTop.ResumeLayout(false);
            this.pnlRegTop.PerformLayout();
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel            pnlHeader;
        private System.Windows.Forms.Label            lblTitle, lblSubtitle;
        private System.Windows.Forms.TableLayoutPanel tblMain;
        private System.Windows.Forms.Panel            pnlFiles, pnlFileTop, pnlFileBtns;
        private System.Windows.Forms.Label            lblFileHeader, lblFilesCount;
        private System.Windows.Forms.Button           btnDeleteFiles;
        private System.Windows.Forms.ListView         lstFiles;
        private System.Windows.Forms.ColumnHeader     colFileName, colFilePath, colFileSize;
        private System.Windows.Forms.Panel            pnlReg, pnlRegTop, pnlRegBtns;
        private System.Windows.Forms.Label            lblRegHeader, lblRegCount, lblRegWarning;
        private System.Windows.Forms.Button           btnDeleteReg, btnDeepScan;
        private System.Windows.Forms.ListView         lstRegistry;
        private System.Windows.Forms.ColumnHeader     colRegKey, colRegValue;
        private System.Windows.Forms.Panel            pnlBottom;
        private System.Windows.Forms.RichTextBox      txtLog;
        private System.Windows.Forms.Button           btnClose;
    }
}
