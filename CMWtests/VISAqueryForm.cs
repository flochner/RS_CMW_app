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
        // private MBSession _session;

        public VISAqueryForm()
        {
            InitializeComponent();
            btnWriteVISA.Enabled = false;
            btnQueryVISA.Enabled = false;
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
            status = Query(session, "*IDN?\n", out sAnswer);
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

        private void ShowErrorText(ViStatus status)
        {
            StringBuilder text = new StringBuilder(visa32.VI_FIND_BUFLEN);
            ViStatus err = visa32.viStatusDesc(m_defRM, status, text);
            txtResult.Text += Environment.NewLine + text.ToString();
        }

        private void btnConnectNew_Click(object sender, EventArgs e)
        {
            string[] modelSer;

            labelResource.Text = "No Resource Selected";
            btnWriteVISA.Enabled = false;
            btnQueryVISA.Enabled = false;
            textBoxResponse.Text = "";
            //_session = null;

            VISAresourceForm resource = new VISAresourceForm();
            resource.ShowDialog();

            if (!string.Equals(resource.Selection, "No VISA resources found.") && resource.Selection != null)
            {
                //_session = new MBSession(resource.Selection);
                //resource.Dispose();
                //modelSer = _session.Query("*IDN?").Split(',');
                //if (modelSer[2].Contains(@"/"))
                //    modelSer[2] = modelSer[2].Split('/')[1];
                //LabelResource.Text = modelSer[1].Trim() + " - " + modelSer[2].Trim();
                //BtnQueryVISA.Enabled = true;
                //BtnWriteVISA.Enabled = true;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            //_session.MbSession.Dispose();
            //Close();
        }
    }
}
