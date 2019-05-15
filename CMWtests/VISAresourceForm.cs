using System;
using System.Text;
using System.Windows.Forms;
using RsVisaLoader;

namespace CMWtests
{
    public partial class VISAresourceForm : Form
    {
        public string Resource { get; private set; } = "";
        public int ResourcesCount { get; private set; } = 0;
        private int defRM = 0;

        private string[] resources;

        public VISAresourceForm()
        {
            InitializeComponent();
            GetResources();
        }

        public void GetResources()
        {
            int resourceCount = 0;
            int vi = 0;
            int i = 0;
            int findList = 0;
            string response;
            ViStatus stat;

            //if novisa return
            VisaIO.OpenResourceMgr(out defRM);

            listBoxResources.Visible = true;
            BtnSelect.Enabled = false;

            StringBuilder desc = new StringBuilder(256);
            stat = visa32.viFindRsrc(defRM, "[^ASRL]?*", out findList, out resourceCount, desc);
            //MessageBox.Show("count: " + resourceCount.ToString() + "\n" + desc.ToString(), "RS - " + stat.ToString());

            if (resourceCount > 0)
            {
                resources = new string[resourceCount];
                for (int j = 0; j < resourceCount; j++)
                {
                    string s = desc.ToString();
                    if (!(s.Contains("::1::") || s.Contains("inst1") || s.Contains("inst2") || s.Contains("inst3")))
                    {
                        resources[i] = desc.ToString();

                        stat = visa32.viOpen(defRM, resources[i], visa32.VI_NULL, visa32.VI_TMO_IMMEDIATE, out vi);
                        //MessageBox.Show("open " + resources[i], stat.ToString());

                        visa32.viWrite(vi, "*IDN?"  5, 5);
                        //MessageBox.Show("ret " + viRet + "\n" + "*IDN?", stat.ToString());

                        visa32.viRead(vi, response, );
                        //MessageBox.Show("ret " + viRet + "\n" + response + '-', stat.ToString());

                        listBoxResources.Items.Add(i + " - " + resources[i] + "  -  " + response);
                        visa32.viClose(vi);
                        i++;
                    }
                    desc = new StringBuilder(256);
                    visa32.viFindNext(findList, desc);
                    ResourcesCount = 1;
                }
            }
            else
            {
                Resource = null;
                RsVisa.RsViUnloadVisaLibrary();
                return;
            }

            listBoxResources.Height = listBoxResources.Height + 15 * (listBoxResources.Items.Count - 1);
            this.Height = this.Height + 15 * (listBoxResources.Items.Count - 1);
            this.MinimumSize = new System.Drawing.Size(this.Width, this.Height);

            if (listBoxResources.Items.Count == 1)
            {
                ResourcesCount = 1;
                Resource = desc.ToString();
                listBoxResources.SelectedIndex = 0;
                BtnSelect.Enabled = true;
            }
            else
            {
                listBoxResources.SelectedIndex = -1;
            }

            visa32.viClose(defRM);
            RsVisa.RsViUnloadVisaLibrary();
        }

        private void listBoxResources_DoubleClick(object sender, MouseEventArgs e)
        {
            if (listBoxResources.IndexFromPoint(e.Location) != ListBox.NoMatches)
                btnSelect_Click(sender, e);
            this.Close();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (listBoxResources.Visible == true && listBoxResources.SelectedIndex >= 0)
                Resource = resources[listBoxResources.SelectedIndex];
            else
                Resource = null;
        }

        private void listBoxResources_SelectedIndexChanged(object sender, EventArgs e)
        {
            BtnSelect.Enabled = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        { }
    }
}
