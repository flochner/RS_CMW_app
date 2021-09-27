using System;
using System.Drawing;
using System.Windows.Forms;

namespace CMWtests
{
    public partial class ConnectionImageForm : Form
    {
        private MessageBoxButtons _buttons;
        private DialogResult _result;

        public ConnectionImageForm(MessageBoxButtons buttons)
        {
            InitializeComponent();
            _buttons = buttons;
           
            button1.Visible = false;
            button2.Visible = true;
            button3.Visible = true;

            if (_buttons == MessageBoxButtons.AbortRetryIgnore)
            {
                button1.Visible = true;
                button1.Text = "Abort";
                button2.Text = "Retry";
                button3.Text = "Ignore";
            }
            else if (_buttons == MessageBoxButtons.RetryCancel)
            {
                button2.Text = "Retry";
                button3.Text = "Abort";
            }
            else if (_buttons == MessageBoxButtons.OKCancel)
            {
                button2.Text = "OK";
                button3.Text = "Abort";
            }
        }

        public void SetImage(string imageFile)
        {
            var img = (Image)Properties.Resources.ResourceManager.GetObject(imageFile);
            this.Size = new Size(img.Width + 7, img.Height + 78);
            pictureBox1.Size = new Size(img.Width, img.Height);
            pictureBox1.Image = img;
            pictureBox1.Visible = true;
        }

        private void button1_MouseClick(object sender, EventArgs e)
        {
            _result = MessageBox.Show("Abort all testing?",
                                      "Warning!",
                                       MessageBoxButtons.YesNo,
                                       MessageBoxIcon.Warning,
                                       MessageBoxDefaultButton.Button2);
            if (_result == DialogResult.Yes)
                this.DialogResult = DialogResult.Abort;
        }

        private void button2_MouseClick(object sender, EventArgs e)
        {
            if (_buttons == MessageBoxButtons.OKCancel)
                this.DialogResult = DialogResult.OK;
            else
                this.DialogResult = DialogResult.Retry;
        }

        private void button3_MouseClick(object sender, EventArgs e)
        {
            if (_buttons == MessageBoxButtons.AbortRetryIgnore)
            {
                _result = MessageBox.Show("Ignore out of tolerance readings" + Environment.NewLine +
                                          "until next connection message?",
                                          "",
                                           MessageBoxButtons.YesNo,
                                           MessageBoxIcon.Information,
                                           MessageBoxDefaultButton.Button2);
                if (_result == DialogResult.Yes)
                    this.DialogResult = DialogResult.Ignore;
            }
            else
            {
                _result = MessageBox.Show("Abort all testing?",
                                          "Warning!",
                                           MessageBoxButtons.YesNo,
                                           MessageBoxIcon.Warning,
                                           MessageBoxDefaultButton.Button2);
                if (_result == DialogResult.Yes)
                    this.DialogResult = DialogResult.Abort;
            }
        }

        private void button_KeyDown(object sender, KeyEventArgs e)
        {
            //MessageBox.Show("Use a mouse to control this window.", "", 
            //                 MessageBoxButtons.OK,
            //                 MessageBoxIcon.Information);
        }
    }
}