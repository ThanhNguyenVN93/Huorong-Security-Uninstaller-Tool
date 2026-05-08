namespace frm_HIS_uninstaller_tool
{
    partial class Form3
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form3));
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.pbDonate  = new System.Windows.Forms.PictureBox();
            this.lblSubtitle3 = new System.Windows.Forms.Label();
            this.lblTitle3 = new System.Windows.Forms.Label();
            this.pnlToolbar = new System.Windows.Forms.Panel();
            this.lblDeepCount = new System.Windows.Forms.Label();
            this.progressDeep = new System.Windows.Forms.ProgressBar();
            this.btnUncheckAll = new System.Windows.Forms.Button();
            this.btnCheckAll = new System.Windows.Forms.Button();
            this.btnStopScan = new System.Windows.Forms.Button();
            this.btnDeepScan = new System.Windows.Forms.Button();
            this.lstDeepScan = new System.Windows.Forms.ListView();
            this.colDRisk = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDKey = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDData = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.txtDeepLog = new System.Windows.Forms.RichTextBox();
            this.pnlBtnBar = new System.Windows.Forms.Panel();
            this.btnCloseDeep = new System.Windows.Forms.Button();
            this.btnDeleteDeep = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbDonate)).BeginInit();
            this.pnlHeader.SuspendLayout();
            this.pnlToolbar.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.pnlBtnBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(70)))), ((int)(((byte)(140)))));
            this.pnlHeader.Controls.Add(this.pbDonate);
            this.pnlHeader.Controls.Add(this.lblSubtitle3);
            this.pnlHeader.Controls.Add(this.lblTitle3);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Padding = new System.Windows.Forms.Padding(16, 8, 16, 8);
            this.pnlHeader.Size = new System.Drawing.Size(880, 60);
            this.pnlHeader.TabIndex = 3;
            // 
            // lblSubtitle3
            // 
            this.lblSubtitle3.AutoSize = true;
            this.lblSubtitle3.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblSubtitle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(210)))), ((int)(((byte)(255)))));
            this.lblSubtitle3.Location = new System.Drawing.Point(16, 37);
            this.lblSubtitle3.Name = "lblSubtitle3";
            this.lblSubtitle3.Size = new System.Drawing.Size(564, 15);
            this.lblSubtitle3.TabIndex = 0;
            this.lblSubtitle3.Text = "Quét \"Huorong\" / \"火绒\" trong data của mọi value thuộc HKLM\\SOFTWARE, HKLM\\SYSTEM, " +
    "HKCU, HKU";
            // 
            // lblTitle3
            //
            this.lblTitle3.AutoSize = true;
            this.lblTitle3.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblTitle3.ForeColor = System.Drawing.Color.White;
            this.lblTitle3.Location = new System.Drawing.Point(14, 7);
            this.lblTitle3.Name = "lblTitle3";
            this.lblTitle3.Size = new System.Drawing.Size(343, 25);
            this.lblTitle3.TabIndex = 1;
            this.lblTitle3.Text = "🔎  Deep Scan — Tìm trong data value";
            //
            // pbDonate
            //
            this.pbDonate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pbDonate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbDonate.Location = new System.Drawing.Point(840, 14);
            this.pbDonate.Name = "pbDonate";
            this.pbDonate.Size = new System.Drawing.Size(30, 30);
            this.pbDonate.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbDonate.TabIndex = 2;
            this.pbDonate.TabStop = false;
            this.pbDonate.Click += new System.EventHandler(this.pbDonate_Click);
            //
            // pnlToolbar
            // 
            this.pnlToolbar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.pnlToolbar.Controls.Add(this.lblDeepCount);
            this.pnlToolbar.Controls.Add(this.progressDeep);
            this.pnlToolbar.Controls.Add(this.btnUncheckAll);
            this.pnlToolbar.Controls.Add(this.btnCheckAll);
            this.pnlToolbar.Controls.Add(this.btnStopScan);
            this.pnlToolbar.Controls.Add(this.btnDeepScan);
            this.pnlToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlToolbar.Location = new System.Drawing.Point(0, 60);
            this.pnlToolbar.Name = "pnlToolbar";
            this.pnlToolbar.Padding = new System.Windows.Forms.Padding(8);
            this.pnlToolbar.Size = new System.Drawing.Size(880, 46);
            this.pnlToolbar.TabIndex = 2;
            // 
            // lblDeepCount
            //
            this.lblDeepCount.AutoSize = true;
            this.lblDeepCount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDeepCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.lblDeepCount.Location = new System.Drawing.Point(658, 14);
            this.lblDeepCount.Name = "lblDeepCount";
            this.lblDeepCount.Size = new System.Drawing.Size(62, 15);
            this.lblDeepCount.TabIndex = 0;
            this.lblDeepCount.Text = "Chưa quét";
            // 
            // progressDeep
            //
            this.progressDeep.Location = new System.Drawing.Point(450, 12);
            this.progressDeep.MarqueeAnimationSpeed = 20;
            this.progressDeep.Name = "progressDeep";
            this.progressDeep.Size = new System.Drawing.Size(200, 22);
            this.progressDeep.TabIndex = 1;
            // 
            // btnUncheckAll
            //
            this.btnUncheckAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.btnUncheckAll.Enabled = false;
            this.btnUncheckAll.FlatAppearance.BorderSize = 0;
            this.btnUncheckAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUncheckAll.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.btnUncheckAll.ForeColor = System.Drawing.Color.White;
            this.btnUncheckAll.Location = new System.Drawing.Point(342, 8);
            this.btnUncheckAll.Name = "btnUncheckAll";
            this.btnUncheckAll.Size = new System.Drawing.Size(100, 30);
            this.btnUncheckAll.TabIndex = 2;
            this.btnUncheckAll.Text = "✘ Bỏ chọn tất";
            this.btnUncheckAll.UseVisualStyleBackColor = false;
            this.btnUncheckAll.Click += new System.EventHandler(this.btnUncheckAll_Click);
            //
            // btnCheckAll
            //
            this.btnCheckAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(120)))), ((int)(((byte)(80)))));
            this.btnCheckAll.Enabled = false;
            this.btnCheckAll.FlatAppearance.BorderSize = 0;
            this.btnCheckAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCheckAll.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.btnCheckAll.ForeColor = System.Drawing.Color.White;
            this.btnCheckAll.Location = new System.Drawing.Point(244, 8);
            this.btnCheckAll.Name = "btnCheckAll";
            this.btnCheckAll.Size = new System.Drawing.Size(90, 30);
            this.btnCheckAll.TabIndex = 3;
            this.btnCheckAll.Text = "✔ Chọn tất cả";
            this.btnCheckAll.UseVisualStyleBackColor = false;
            this.btnCheckAll.Click += new System.EventHandler(this.btnCheckAll_Click);
            //
            // btnStopScan
            //
            this.btnStopScan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(50)))), ((int)(((byte)(30)))));
            this.btnStopScan.Enabled = false;
            this.btnStopScan.FlatAppearance.BorderSize = 0;
            this.btnStopScan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStopScan.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.btnStopScan.ForeColor = System.Drawing.Color.White;
            this.btnStopScan.Location = new System.Drawing.Point(146, 8);
            this.btnStopScan.Name = "btnStopScan";
            this.btnStopScan.Size = new System.Drawing.Size(90, 30);
            this.btnStopScan.TabIndex = 5;
            this.btnStopScan.Text = "■ Dừng";
            this.btnStopScan.UseVisualStyleBackColor = false;
            this.btnStopScan.Click += new System.EventHandler(this.btnStopScan_Click);
            // 
            // btnDeepScan
            // 
            this.btnDeepScan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(90)))), ((int)(((byte)(160)))));
            this.btnDeepScan.FlatAppearance.BorderSize = 0;
            this.btnDeepScan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeepScan.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnDeepScan.ForeColor = System.Drawing.Color.White;
            this.btnDeepScan.Location = new System.Drawing.Point(8, 8);
            this.btnDeepScan.Name = "btnDeepScan";
            this.btnDeepScan.Size = new System.Drawing.Size(130, 30);
            this.btnDeepScan.TabIndex = 4;
            this.btnDeepScan.Text = "🔍  Bắt đầu quét";
            this.btnDeepScan.UseVisualStyleBackColor = false;
            this.btnDeepScan.Click += new System.EventHandler(this.btnDeepScan_Click);
            // 
            // lstDeepScan
            // 
            this.lstDeepScan.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstDeepScan.CheckBoxes = true;
            this.lstDeepScan.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colDRisk,
            this.colDKey,
            this.colDName,
            this.colDType,
            this.colDData});
            this.lstDeepScan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstDeepScan.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lstDeepScan.FullRowSelect = true;
            this.lstDeepScan.GridLines = true;
            this.lstDeepScan.HideSelection = false;
            this.lstDeepScan.Location = new System.Drawing.Point(0, 106);
            this.lstDeepScan.Name = "lstDeepScan";
            this.lstDeepScan.Size = new System.Drawing.Size(880, 251);
            this.lstDeepScan.TabIndex = 0;
            this.lstDeepScan.UseCompatibleStateImageBehavior = false;
            this.lstDeepScan.View = System.Windows.Forms.View.Details;
            // 
            //
            // colDRisk
            //
            this.colDRisk.Text = "Mức độ";
            this.colDRisk.Width = 80;
            //
            // colDKey
            //
            this.colDKey.Text = "Registry Key";
            this.colDKey.Width = 260;
            // 
            //
            // colDName
            //
            this.colDName.Text = "Value Name";
            this.colDName.Width = 140;
            // 
            //
            // colDType
            //
            this.colDType.Text = "Type";
            this.colDType.Width = 80;
            // 
            //
            // colDData
            //
            this.colDData.Text = "Data (chứa Huorong)";
            this.colDData.Width = 290;
            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.pnlBottom.Controls.Add(this.txtDeepLog);
            this.pnlBottom.Controls.Add(this.pnlBtnBar);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 357);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Padding = new System.Windows.Forms.Padding(6, 6, 6, 0);
            this.pnlBottom.Size = new System.Drawing.Size(880, 160);
            this.pnlBottom.TabIndex = 1;
            // 
            // txtDeepLog
            // 
            this.txtDeepLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.txtDeepLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtDeepLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDeepLog.Font = new System.Drawing.Font("Consolas", 8.5F);
            this.txtDeepLog.ForeColor = System.Drawing.Color.Lime;
            this.txtDeepLog.Location = new System.Drawing.Point(6, 6);
            this.txtDeepLog.Name = "txtDeepLog";
            this.txtDeepLog.ReadOnly = true;
            this.txtDeepLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtDeepLog.Size = new System.Drawing.Size(868, 108);
            this.txtDeepLog.TabIndex = 0;
            this.txtDeepLog.Text = "";
            // 
            // pnlBtnBar
            // 
            this.pnlBtnBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.pnlBtnBar.Controls.Add(this.btnCloseDeep);
            this.pnlBtnBar.Controls.Add(this.btnDeleteDeep);
            this.pnlBtnBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBtnBar.Location = new System.Drawing.Point(6, 114);
            this.pnlBtnBar.Name = "pnlBtnBar";
            this.pnlBtnBar.Size = new System.Drawing.Size(868, 46);
            this.pnlBtnBar.TabIndex = 1;
            // 
            // btnCloseDeep
            // 
            this.btnCloseDeep.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnCloseDeep.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCloseDeep.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btnCloseDeep.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCloseDeep.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCloseDeep.ForeColor = System.Drawing.Color.White;
            this.btnCloseDeep.Location = new System.Drawing.Point(768, 0);
            this.btnCloseDeep.Name = "btnCloseDeep";
            this.btnCloseDeep.Size = new System.Drawing.Size(100, 46);
            this.btnCloseDeep.TabIndex = 0;
            this.btnCloseDeep.Text = "Đóng";
            this.btnCloseDeep.UseVisualStyleBackColor = false;
            this.btnCloseDeep.Click += new System.EventHandler(this.btnCloseDeep_Click);
            // 
            // btnDeleteDeep
            // 
            this.btnDeleteDeep.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(30)))), ((int)(((byte)(20)))));
            this.btnDeleteDeep.Enabled = false;
            this.btnDeleteDeep.FlatAppearance.BorderSize = 0;
            this.btnDeleteDeep.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteDeep.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnDeleteDeep.ForeColor = System.Drawing.Color.White;
            this.btnDeleteDeep.Location = new System.Drawing.Point(8, 8);
            this.btnDeleteDeep.Name = "btnDeleteDeep";
            this.btnDeleteDeep.Size = new System.Drawing.Size(200, 30);
            this.btnDeleteDeep.TabIndex = 1;
            this.btnDeleteDeep.Text = "💾  Backup & Xóa đã chọn";
            this.btnDeleteDeep.UseVisualStyleBackColor = false;
            this.btnDeleteDeep.Click += new System.EventHandler(this.btnDeleteDeep_Click);
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(880, 517);
            this.Controls.Add(this.lstDeepScan);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlToolbar);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(900, 560);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(900, 560);
            this.Name = "Form3";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Deep Scan — Tìm data value chứa Huorong";
            this.Load += new System.EventHandler(this.Form3_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbDonate)).EndInit();
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlToolbar.ResumeLayout(false);
            this.pnlToolbar.PerformLayout();
            this.pnlBottom.ResumeLayout(false);
            this.pnlBtnBar.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel       pnlHeader;
        private System.Windows.Forms.PictureBox  pbDonate;
        private System.Windows.Forms.Label       lblTitle3, lblSubtitle3;
        private System.Windows.Forms.Panel       pnlToolbar;
        private System.Windows.Forms.Button      btnDeepScan, btnStopScan, btnCheckAll, btnUncheckAll;
        private System.Windows.Forms.ProgressBar progressDeep;
        private System.Windows.Forms.Label       lblDeepCount;
        private System.Windows.Forms.ListView    lstDeepScan;
        private System.Windows.Forms.ColumnHeader colDRisk, colDKey, colDName, colDType, colDData;
        private System.Windows.Forms.Panel       pnlBottom, pnlBtnBar;
        private System.Windows.Forms.RichTextBox txtDeepLog;
        private System.Windows.Forms.Button      btnDeleteDeep, btnCloseDeep;
    }
}
