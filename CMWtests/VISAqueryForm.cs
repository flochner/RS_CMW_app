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

        public VISAqueryForm()
        {
            InitializeComponent();
            btnWriteVISA.Enabled = false;
            btnQueryVISA.Enabled = false;
        }

        private void btnConnectNew_Click(object sender, EventArgs e)
        {
            int resourceMgr = 0;
            int viRetCount = 0;
            int vi = 0;
            string[] modelSer;
            string resource = null;
            ViStatus viStat = visa32.VI_NULL;

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

            viStat = visa32.viOpenDefaultRM(out resourceMgr);
            viStat = visa32.viOpen(resourceMgr, resource, visa32.VI_NULL, 1000, out vi);

            btnClear_Click(sender, e);

            viStat = VisaIO.Write(vi, "*IDN?");

            //
            return;
            //

            session.Clear();
            session.Write("*RST;*CLS");
            session.Write("*ESE 1", true);
            session.ErrorChecking();
            QuerySTB("*IDN?", 2000, out string idn);
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
            btnQueryVISA.Enabled = false;
            QuerySTB(textBoxStringToWrite.Text, 20000, out string response);
            textBoxResponse.AppendText(response + Environment.NewLine);
            textBoxStringToWrite_TextChanged(sender, e);
        }

        private void btnWriteVISA_Click(object sender, EventArgs e)
        {
            btnWriteVISA.Enabled = false;
            WriteSTB(textBoxStringToWrite.Text, 20000);
            textBoxStringToWrite_TextChanged(sender, e);
        }

        private void textBoxStringToWrite_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(textBoxStringToWrite.Text) || session == null)
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
            if (session != null)
            {
                session.Clear();
                session.Write("*RST;*CLS");
                session.Write("*ESE 1");
                session.ErrorChecking();
                session.Dispose();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            textBoxResponse.Text = string.Empty;
        }
    }
}
