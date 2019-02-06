using System;
using System.Windows.Forms;
using Ivi.Visa;
using IviVisaExtended;

namespace CMWtests
{
    public partial class VISAqueryForm : Form
    {
        private IMessageBasedSession session = null;

        public VISAqueryForm()
        {
            InitializeComponent();
            btnWriteVISA.Enabled = false;
            btnQueryVISA.Enabled = false;
        }

        private void btnQueryVISA_Click(object sender, EventArgs e)
        {
            textBoxResponse.Text = string.Empty;
            Update();

            QuerySTB(textBoxStringToWrite.Text, 20000, out string response);
            textBoxResponse.Text = response;
        }

        private void btnWriteVISA_Click(object sender, EventArgs e)
        {
            WriteSTB(textBoxStringToWrite.Text, 20000);
        }

        private void btnConnectNew_Click(object sender, EventArgs e)
        {
            string[] modelSer;
            string resource = null;

            session = null;
            labelResource.Text = "No Resource Selected";
            btnWriteVISA.Enabled = false;
            btnQueryVISA.Enabled = false;
            textBoxResponse.Text = string.Empty;

            var resForm = new VISAresourceForm();
            resForm.ShowDialog();
            resource = resForm.Resource;
            var status = resForm.Status;
            resForm.Dispose();

            if (status == MainForm.TestStatus.Abort || string.IsNullOrEmpty(resource))
                return;

            try
            {
                session = GlobalResourceManager.Open(resource) as IMessageBasedSession;
            }
            catch (Ivi.Visa.NativeVisaException exc)
            {
                MessageBox.Show(exc.Message, exc.GetType().ToString());
                return;
            }

            session.Clear();
            session.Write("*RST;*CLS");
            session.Write("*ESE 1");
            session.ErrorChecking();

            QuerySTB("*IDN?", 20000, out string idn);
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

            if (!string.IsNullOrWhiteSpace(textBoxStringToWrite.Text) && session != null)
            {
                btnWriteVISA.Enabled = true;
                btnQueryVISA.Enabled = true;
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

        private void textBoxStringToWrite_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxStringToWrite.Text) || session == null)
            {
                btnWriteVISA.Enabled = false;
                btnQueryVISA.Enabled = false;
            }
            else
            {
                btnWriteVISA.Enabled = true;
                btnQueryVISA.Enabled = true;
            }
        }

        private void WriteSTB(string command, int timeout)
        {
            try
            {
                session.WriteWithSTBpollSync(command, timeout);
            }
            catch (InstrumentErrorException e)
            {
                MessageBox.Show(e.Message, e.GetType().ToString());
            }
            catch (InstrumentOPCtimeoutException e)
            {
                MessageBox.Show(e.Message, e.GetType().ToString());
            }
            catch (Ivi.Visa.VisaException e)
            {
                MessageBox.Show(e.Message, e.GetType().ToString());
            }
        }

        private void QuerySTB(string query, int timeout, out string response)
        {
            response = null;
            try
            {
                response = session.QueryWithSTBpollSync(query, timeout);
            }
            catch (InstrumentErrorException e)
            {
                MessageBox.Show(e.Message, e.GetType().ToString());
            }
            catch (InstrumentOPCtimeoutException e)
            {
                MessageBox.Show(e.Message, e.GetType().ToString());
            }
            catch (Ivi.Visa.VisaException e)
            {
                MessageBox.Show(e.Message, e.GetType().ToString());
            }
        }
    }
}
