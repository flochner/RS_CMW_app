using System;
using System.Text;
using System.Windows.Forms;
using RsVisaLoader;

namespace CMWtests
{
    public partial class VISAresourceForm : Form
    {
        public string Resource { get; private set; } = string.Empty;
        public int ResourcesCount { get; private set; } = 0;
        private VisaIO instr = null;

        private string[] resources;

        public VISAresourceForm()
        {
            InitializeComponent();
            GetResources();
        }

        /// <summary>
        /// Commit 
        /// </summary>
        //Fixed resouceForm:separated Count variables.VisaIO: added better Reset(). Implemented Close(). 

        public void GetResources()
        {
            int vi = 0;
            int i = 0;
            int findList = 0;
            string response;
            ViStatus stat;

            listBoxResources.Visible = true;
            BtnSelect.Enabled = false;

            StringBuilder desc = new StringBuilder(1024);
            stat = visa32.viFindRsrc(MainForm.DefResMgr, "[^ASRL]?*", out findList, out int retCount, desc);
            //MessageBox.Show("count: " + retCount.ToString() + "\n" + desc.ToString(), "RS - " + stat.ToString());

            if (retCount > 0)
            {
                resources = new string[retCount];
                for (int j = 0; j < retCount; j++)
                {
                    string s = desc.ToString();
                    if (!(s.Contains("::1::") || s.Contains("inst1") || s.Contains("inst2") || s.Contains("inst3")))
                    {
                        resources[i] = desc.ToString();
                        instr = new VisaIO(resources[i]);
                        response = instr.QueryString("*IDN?");
                        listBoxResources.Items.Add(i + " - " + resources[i] + "  -  " + response);
                        visa32.viClose(vi);
                        i++;
                    }
                    desc = new StringBuilder(1024);
                    visa32.viFindNext(findList, desc);
                }
                ResourcesCount = i;
            }
            else
            {
                Resource = string.Empty;
                RsVisa.RsViUnloadVisaLibrary();
                return;
            }

            listBoxResources.Height = listBoxResources.Height + 15 * (listBoxResources.Items.Count - 1);
            this.Height = this.Height + 15 * (listBoxResources.Items.Count - 1);
            this.MinimumSize = new System.Drawing.Size(this.Width, this.Height);

            if (ResourcesCount == 1)
            {
                Resource = resources[0];
                listBoxResources.SelectedIndex = 0;
                BtnSelect.Enabled = true;
            }
            else
            {
                listBoxResources.SelectedIndex = -1;
            }
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
        {
            Resource = null;
        }
    }
}
