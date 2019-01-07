using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RsVisaLoader;

namespace CMWtests
{
    public partial class VISAqueryForm : Form
    {
        ViSession connection = null;

        public VISAqueryForm()
        {
            InitializeComponent();
            btnWriteVISA.Enabled = false;
            btnQueryVISA.Enabled = false;
        }

         private void btnQueryVISA_Click(object sender, EventArgs e)
        {
            string response;

            int session;
            txtResult.Text = String.Empty;
            Update();
            ViStatus status = visa32.viOpen(m_defRM, item, 0, 0, out session);
            if (status < ViStatus.VI_SUCCESS)
            {
                ShowErrorText(status);
                return;
            }

            string sAnswer;
            status = sesn.Query(session, "*IDN?\n", out sAnswer);
            visa32.viClose(session);

            if (status < ViStatus.VI_SUCCESS)
            {
                ShowErrorText(status);
            }
            else
            {
                textBoxResponse.Text = "Identification: " + sAnswer.ToString();
            }




            //if (_session != null)
            //{
            //    response = _session.Query(TextBoxStringToWrite.Text);
            //    TextBoxResponse.Text = response;
            //}
            //else
            //{
            //    TextBoxResponse.Text = "VISA Resource Not Connected";
            //}
        }

       private void btnWriteVISA_Click(object sender, EventArgs e)
        {
            //if (_session != null)
            //{
            //    TextBoxResponse.Text = "";
            //    _session.Write(TextBoxStringToWrite.Text);
            //}
            //else
            //{
            //    TextBoxResponse.Text = "VISA Resource Not Connected";
            //}
        }

        private void ShowErrorText(ViStatus status)
        {
            StringBuilder text = new StringBuilder(visa32.VI_FIND_BUFLEN);
            ViStatus err = visa32.viStatusDesc(m_defRM, status, text);
            txtResult.Text += Environment.NewLine + text.ToString();
        }

        private void btnConnectNew_Click(object sender, EventArgs e)
        {
            ViStatus status;
            int session;
            string[] modelSer;
            string idn;
            string resource = "";

            labelResource.Text = "No Resource Selected";
            btnWriteVISA.Enabled = false;
            btnQueryVISA.Enabled = false;
            textBoxResponse.Text = "";

            connection = new ViSession();

            var resForm = new VISAresourceForm(connection.ResourceMgr);
            resForm.ShowDialog();

            // get resource string, create session, get session ID
            resource = resForm.Resource;
            status = connection.OpenSession(resource, out session);

            if (status < 0)
                MessageBox.Show("Something went wrong opening session. Try again.");
            resForm.Dispose();

            if (resource != null)
            {
                status = connection.Query("*IDN?", out idn);
                modelSer = idn.Split(',');
                if (modelSer[2].Contains(@"/"))
                    modelSer[2] = modelSer[2].Split('/')[1];
                labelResource.Text = modelSer[1].Trim() + " - " + modelSer[2].Trim();
                btnQueryVISA.Enabled = true;
                btnWriteVISA.Enabled = true;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {

            connection.CloseResMgr();
            //Close();
        }
    }
}
