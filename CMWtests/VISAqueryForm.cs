using System;
using System.Windows.Forms;
using System.Threading;

namespace CMWtests
{
    public partial class VISAqueryForm : Form
    {
        private VisaIO instr = null;

        public VISAqueryForm()
        {
            InitializeComponent();
            btnWriteVISA.Enabled = false;
            btnQueryVISA.Enabled = false;
        }

        private void btnConnectNew_Click(object sender, EventArgs e)
        {
            string[] modelSer;
            string resource = string.Empty;
            string idn = string.Empty;

            labelResource.Text = string.Empty;
            btnWriteVISA.Enabled = false;
            btnQueryVISA.Enabled = false;
            textBoxResponse.ResetText();

            if (instr != null)
            {
                instr.Reset();
                instr.CloseInstrument();
                instr = null;
            }

            var resForm = new VISAresourceForm();

            if (resForm.ResourcesCount == 0)
            {
                labelResource.Text = string.Empty;
                Thread.Sleep(250);
                labelResource.Text = "No VISA resources available";
                return;
            }
            else
            {
                resForm.ShowDialog();
            }

            resource = resForm.Resource;
            resForm.Dispose();

            //resource = "TCPIP0::192.168.4.31::inst0::INSTR";

            if (string.IsNullOrEmpty(resource))
            {
                labelResource.Text = "No resource connected";
                return;
            }

            instr = new VisaIO(resource);

            btnClear_Click(sender, e);

            instr.Reset();
#if DEBUG
            instr.Write("ABORt:GPRF:MEAS:EPSensor;:CALibration:GPRF:MEAS:EPSensor:ZERO");
            idn = instr.QueryWithSTB("CALibration:GPRF:MEAS:EPSensor:ZERO?", 20000);
            textBoxResponse.AppendText(idn);
#endif
            idn = instr.QueryString("*IDN?");
            try
            {
                modelSer = idn.Split(',');
                if (modelSer.Length >= 3)
                {
                    if (modelSer[2].Contains("/"))
                        modelSer[2] = modelSer[2].Split('/')[1];
                    labelResource.Text = modelSer[1].Trim() + " - " + modelSer[2].Trim();
                }
            }
            catch (Exception exc)
            {
                textBoxResponse.AppendText(idn + Environment.NewLine);
                MessageBox.Show(exc.Message, exc.GetType().ToString());
                return;
            }

            textBoxStringToWrite_TextChanged(sender, e);
        }

        private void btnQueryVISA_Click(object sender, EventArgs e)
        {
            string response;

            btnQueryVISA.Enabled = false;
            response = instr.QueryWithSTB(textBoxStringToWrite.Text, 20000);
            textBoxResponse.AppendText(response + Environment.NewLine);
            textBoxStringToWrite_TextChanged(sender, e);
        }

        private void btnWriteVISA_Click(object sender, EventArgs e)
        {
            btnWriteVISA.Enabled = false;
            instr.WriteWithSTB(textBoxStringToWrite.Text, 20000);
            textBoxStringToWrite_TextChanged(sender, e);
        }

        private void textBoxStringToWrite_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(textBoxStringToWrite.Text))
            {
                btnWriteVISA.Enabled = false;
                btnQueryVISA.Enabled = false;
                return;
            }

            if (textBoxStringToWrite.Text.Contains("?"))
            {
                btnQueryVISA.Enabled = true;
                btnWriteVISA.Enabled = false;
            }
            else
            {
                btnQueryVISA.Enabled = false;
                btnWriteVISA.Enabled = true;
            }
        }

        private void textBoxResponse_TextChanged(object sender, EventArgs e)
        {
            textBoxResponse.Refresh();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (instr != null)
            {
                instr.Reset();
                instr.CloseInstrument();
                instr = null;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            textBoxResponse.Text = string.Empty;
        }

        private void textBoxResponse_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
        }
    }
}
