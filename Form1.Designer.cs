namespace frm_HIS_uninstaller_tool
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtunipath = new System.Windows.Forms.TextBox();
            this.btnunistaller = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblbit = new System.Windows.Forms.Label();
            this.lblwin = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblstatus = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.txtprocess = new System.Windows.Forms.RichTextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtunipath);
            this.groupBox1.Controls.Add(this.btnunistaller);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(656, 80);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Huorong Uninstaller";
            // 
            // txtunipath
            // 
            this.txtunipath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtunipath.Location = new System.Drawing.Point(110, 47);
            this.txtunipath.Name = "txtunipath";
            this.txtunipath.ReadOnly = true;
            this.txtunipath.Size = new System.Drawing.Size(436, 25);
            this.txtunipath.TabIndex = 2;
            // 
            // btnunistaller
            // 
            this.btnunistaller.Location = new System.Drawing.Point(9, 44);
            this.btnunistaller.Name = "btnunistaller";
            this.btnunistaller.Size = new System.Drawing.Size(95, 30);
            this.btnunistaller.TabIndex = 1;
            this.btnunistaller.Text = "Gỡ cài đặt";
            this.btnunistaller.UseVisualStyleBackColor = true;
            this.btnunistaller.Click += new System.EventHandler(this.btnunistaller_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.lblbit);
            this.groupBox2.Controls.Add(this.lblwin);
            this.groupBox2.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.groupBox2.Location = new System.Drawing.Point(12, 100);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(656, 48);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Thông tin hệ thống";
            // 
            // lblbit
            // 
            this.lblbit.AutoSize = true;
            this.lblbit.Location = new System.Drawing.Point(550, 22);
            this.lblbit.Name = "lblbit";
            this.lblbit.Size = new System.Drawing.Size(43, 17);
            this.lblbit.TabIndex = 2;
            this.lblbit.Text = "label3";
            // 
            // lblwin
            // 
            this.lblwin.AutoSize = true;
            this.lblwin.Location = new System.Drawing.Point(9, 22);
            this.lblwin.Name = "lblwin";
            this.lblwin.Size = new System.Drawing.Size(43, 17);
            this.lblwin.TabIndex = 1;
            this.lblwin.Text = "label2";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.lblstatus);
            this.groupBox3.Controls.Add(this.progressBar1);
            this.groupBox3.Controls.Add(this.txtprocess);
            this.groupBox3.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.groupBox3.Location = new System.Drawing.Point(12, 156);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(656, 276);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Tiến trình";
            // 
            // lblstatus
            // 
            this.lblstatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblstatus.AutoSize = true;
            this.lblstatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblstatus.Location = new System.Drawing.Point(551, 239);
            this.lblstatus.Name = "lblstatus";
            this.lblstatus.Size = new System.Drawing.Size(38, 15);
            this.lblstatus.TabIndex = 2;
            this.lblstatus.Text = "Chờ...";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(9, 236);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(533, 23);
            this.progressBar1.TabIndex = 1;
            // 
            // txtprocess
            // 
            this.txtprocess.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtprocess.BackColor = System.Drawing.Color.Black;
            this.txtprocess.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtprocess.ForeColor = System.Drawing.Color.Lime;
            this.txtprocess.Location = new System.Drawing.Point(9, 22);
            this.txtprocess.Name = "txtprocess";
            this.txtprocess.ReadOnly = true;
            this.txtprocess.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtprocess.Size = new System.Drawing.Size(638, 206);
            this.txtprocess.TabIndex = 0;
            this.txtprocess.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(680, 447);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(700, 490);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(700, 490);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HIS Uninstaller Tool";
            this.Load += new System.EventHandler(this.Form1_Load_1);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblbit;
        private System.Windows.Forms.Label lblwin;
        private System.Windows.Forms.TextBox txtunipath;
        private System.Windows.Forms.Button btnunistaller;
        private System.Windows.Forms.RichTextBox txtprocess;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblstatus;
    }
}
