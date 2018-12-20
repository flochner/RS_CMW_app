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
    public partial class VISAqueryForm : Form
    {
        private MBSession session;
        private string response;

        public VISAqueryForm()
        {
            InitializeComponent();
            BtnWriteVISA.Enabled = false;
            BtnQueryVISA.Enabled = false;
        }

        private void BtnWriteVISA_Click(object sender, EventArgs e)
        {
            if (session != null)
            {
                TextBoxResponse.Text = null;
                session.MbSession.Write(TextBoxStringToWrite.Text);
            }
            else
            {
                TextBoxResponse.Text = "VISA Resource Not Connected";
            }
        }

        private void BtnQueryVISA_Click(object sender, EventArgs e)
        {
            if (session != null)
            {
                response = session.Query(TextBoxStringToWrite.Text);
                TextBoxResponse.Text = response;
            }
            else
            {
                TextBoxResponse.Text = "VISA Resource Not Connected";
            }
        }

        private void BtnConnectNew_Click(object sender, EventArgs e)
        {
            LabelResource.Text = "No Resource Selected";
            BtnWriteVISA.Enabled = false;
            BtnQueryVISA.Enabled = false;
            TextBoxResponse.Text = null;
            string[] modelSer;
            session = null;

            VISAresourceForm resource = new VISAresourceForm();
            resource.ShowDialog();

            if (!string.Equals(resource.Selection, "No VISA resources found.") && resource.Selection != null)
            {
                session = new MBSession(resource.Selection);
                resource.Dispose();
                modelSer = session.Query("*IDN?").Split(',');
                if (modelSer[2].Contains(@"/"))
                    modelSer[2] = modelSer[2].Split('/')[1];
                LabelResource.Text = modelSer[1].Trim() + " - " + modelSer[2].Trim();
                BtnQueryVISA.Enabled = true;
                BtnWriteVISA.Enabled = true;
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            session = null;
            Close();
        }

        private void VISAquery_Load(object sender, EventArgs e) { }
        private void ContextMenuStrip1_Opening(object sender, CancelEventArgs e) { }
        private void CopyToolStripMenuItem_Click(object sender, EventArgs e) { }
    }
}
