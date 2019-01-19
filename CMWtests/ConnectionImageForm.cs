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
                toolTipAbort.InitialDelay = 0;
                toolTipAbort.SetToolTip(button1, "test");
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

        private void button1_Click(object sender, EventArgs e)
        {
            _result = MessageBox.Show("Abort all testing?",
                                      "Warning!",
                                       MessageBoxButtons.YesNo,
                                       MessageBoxIcon.Warning,
                                       MessageBoxDefaultButton.Button2);
            if (_result == DialogResult.Yes)
                this.DialogResult = DialogResult.Abort;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (_buttons == MessageBoxButtons.OKCancel)
                this.DialogResult = DialogResult.OK;
            else
                this.DialogResult = DialogResult.Retry;
        }

        private void button3_Click(object sender, EventArgs e)
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

        private void toolTipAbort_Popup(object sender, PopupEventArgs e)
        {

        }

        private void toolTipRetry_Popup(object sender, PopupEventArgs e)
        {

        }

        private void toolTipIgnore_Popup(object sender, PopupEventArgs e)
        {

        }
    }
}
