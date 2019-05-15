using System;
using System.Windows.Forms;
using System.Text;
using System.Threading;
//using Ivi.Visa;
//using IviVisaExtended;
using RsVisaLoader;


namespace CMWtests
{
    public partial class VISAqueryForm : Form
    {
        private int vi = 0;
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
            string resource = null;
            ViStatus viStat = visa32.VI_NULL;
            string idn = "";

            labelResource.Text = "";
            btnWriteVISA.Enabled = false;
            btnQueryVISA.Enabled = false;

            var resForm = new VISAresourceForm();

            if (resForm.Resource == null)
            {
                labelResource.Text = "";
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

            if (string.IsNullOrEmpty(resource))
            {
                labelResource.Text = "No resource connected";
                return;
            }

            VisaIO.OpenResourceMgr(out int defRM);
            instr = new VisaIO(defRM, resource);

            btnClear_Click(sender, e);

            //session.Clear();
            instr.Write("*RST;*CLS");
            instr.Write("*ESE 1", true);
            instr.ErrorChecking();
            idn = instr.QuerySTB("*IDN?", 2000);
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
                MessageBox.Show(exc.Message, exc.GetType().ToString());
                return;
            }

            textBoxStringToWrite_TextChanged(sender, e);
        }

        private void btnQueryVISA_Click(object sender, EventArgs e)
        {
            string response;

            btnQueryVISA.Enabled = false;
            response = instr.QuerySTB(textBoxStringToWrite.Text, 20000);
            textBoxResponse.AppendText(response + Environment.NewLine);
            textBoxStringToWrite_TextChanged(sender, e);
        }

        private void btnWriteVISA_Click(object sender, EventArgs e)
        {
            btnWriteVISA.Enabled = false;
            instr.WriteSTB(textBoxStringToWrite.Text, 20000);
            textBoxStringToWrite_TextChanged(sender, e);
        }

        private void textBoxStringToWrite_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(textBoxStringToWrite.Text))// || session == null)
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            //if (session != null)
            //{
            //    session.Clear();
            //    session.Write("*RST;*CLS");
            //    session.Write("*ESE 1");
            //    session.ErrorChecking();
            //    session.Dispose();
            //}
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            textBoxResponse.Text = string.Empty;
        }
    }
}
