using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CMWtests
{
    public partial class ConnectionImageForm : Form
    {
        public ConnectionImageForm()
        {
            InitializeComponent();
        }

        public void SetImage(string img)
        {
            if (img.Contains("RX"))
            {
                pictureBox1.Size = new Size(900, 377);
                this.Size = new Size(918, 468);
            }
            else
            {
                pictureBox1.Size = new Size(702, 377);
                this.Size = new Size(720, 468);
            }
            pictureBox1.Image = (Image)Properties.Resources.ResourceManager.GetObject(img);
            pictureBox1.Visible = true;
        }

        private void BtnAdvance_Click(object sender, EventArgs e) => this.Close();
        private void PictureBox1_Click(object sender, EventArgs e) { }
    }
}
