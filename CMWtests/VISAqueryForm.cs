using System;
using System.Windows.Forms;
using System.Threading;
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

        private void btnConnectNew_Click(object sender, EventArgs e)
        {
            string[] modelSer;
            string resource = null;

            session = null;
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

//            resource = "USB0::0x0AAD::0x0057::0116578::INSTR";
//            resource = "TCPIP0::cmw50050-116578::inst0::INSTR";

            if (string.IsNullOrEmpty(resource))
            {
                labelResource.Text = "No resource connected";
                return;
            }

            try
            {
                session = GlobalResourceManager.Open(resource) as IMessageBasedSession;
            }
            catch (Ivi.Visa.NativeVisaException exc)
            {
                MessageBox.Show(exc.Message, exc.GetType().ToString());
                return;
            }
            catch (Ivi.Visa.VisaException exc)
            {
                MessageBox.Show(exc.Message, exc.GetType().ToString());
                return;
            }

            btnClear_Click(sender, e);

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
            finally
            {
                string errResp = session.QueryString("SYST:ERR?").TrimEnd();
                if (Convert.ToInt64(errResp.Split(',')[0]) != 0)
                    textBoxResponse.AppendText(errResp + Environment.NewLine);
            }
        }

        private void QuerySTB(string query, int timeout, out string response)
        {
            response = null;
            try
            {
                response = session.QueryWithSTBpollSync(query, timeout);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, e.GetType().ToString());
            }

            //session.RawIO.Write("SYST:ERR?\n");
            //string errResp = session.RawIO.ReadString().TrimEnd();
            //if (Convert.ToInt64(errResp.Split(',')[0]) != 0)
            //    textBoxResponse.AppendText(errResp + Environment.NewLine);
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
