using System;
using System.Drawing;
using System.Windows.Forms;

namespace frm_HIS_uninstaller_tool
{
    public partial class Form4 : Form
    {
        public Form4() => InitializeComponent();

        private void Form4_Load(object sender, EventArgs e)
        {
            ApplyLanguage();
            LoadQrImages();
        }

        internal void ApplyLanguage()
        {
            var L = LangManager.L;
            this.Text        = L.Get("form4.title");
            lblTitle4.Text   = L.Get("form4.maintitle");
            lblSub4.Text     = L.Get("form4.subtitle");
            lblTcbName.Text  = L.Get("form4.label.tcb");
            lblMomoName.Text = L.Get("form4.label.momo");
            btnClose4.Text   = L.Get("form4.btn.close");
        }

        private void LoadQrImages()
        {
            Image tcb = ResourceHelper.LoadImage("tcb.jpg");
            if (tcb != null) { pbTCB.Image = tcb; pbTCB.SizeMode = PictureBoxSizeMode.Zoom; }

            Image momo = ResourceHelper.LoadImage("momo.jpg");
            if (momo != null) { pbMomo.Image = momo; pbMomo.SizeMode = PictureBoxSizeMode.Zoom; }
        }

        private void btnClose4_Click(object sender, EventArgs e) => this.Close();
    }
}
