using System;
using System.Text;
using System.Windows.Forms;
using RsVisaLoader;
using Ivi.Visa;

namespace CMWtests
{
    public partial class VISAresourceForm : Form
    {
        public string Resource { get; private set; } = "";
        public int ResourcesCount { get; private set; } = 0;
        private string[] resources;

        public VISAresourceForm()
        {
            InitializeComponent();
            GetResources();
        }

        public void GetResources()
        {
            int resourceMgr = 0;
            int resCount = 0;
            int vi = 0;
            int i = 0;
            int findList = 0;
            string response;
            ViStatus stat;

            if (IsVisaLibraryInstalled(RsVisa.RSVISA_MANFID_DEFAULT))
            {
                if (IsVisaLibraryInstalled(RsVisa.RSVISA_MANFID_RS))
                    RsVisa.RsViSetDefaultLibrary(RsVisa.RSVISA_MANFID_RS);
                else if (IsVisaLibraryInstalled(RsVisa.RSVISA_MANFID_NI))
                    RsVisa.RsViSetDefaultLibrary(RsVisa.RSVISA_MANFID_NI);
                else if (IsVisaLibraryInstalled(RsVisa.RSVISA_MANFID_AG))
                    RsVisa.RsViSetDefaultLibrary(RsVisa.RSVISA_MANFID_AG);
                else
                    RsVisa.RsViSetDefaultLibrary(RsVisa.RSVISA_MANFID_DEFAULT);

                visa32.viOpenDefaultRM(out resourceMgr);
                visa32.viSetAttribute(resourceMgr, ViAttr.VI_RS_ATTR_TCPIP_FIND_RSRC_TMO, 0x3E8);
                visa32.viSetAttribute(resourceMgr, ViAttr.VI_RS_ATTR_TCPIP_FIND_RSRC_MODE, 0x7);
           }
            else
            {
                MessageBox.Show("No VISAs Installed!");
                return;
            }

            listBoxResources.Visible = true;
            BtnSelect.Enabled = false;

            StringBuilder desc = new StringBuilder(256);
            stat = visa32.viFindRsrc(resourceMgr, "[^ASRL]?*", out findList, out resCount, desc);
            //MessageBox.Show("count: " + resCount.ToString() + "\n" + desc.ToString(), "RS - " + stat.ToString());

            int viRetCount = 0;

            if (resCount > 0)
            {
                resources = new string[resCount];
                for (int j = 0; j < resCount; j++)
                {
                    if (true)//(!desc.ToString().Contains("::1::"))
                    {
                        resources[i] = desc.ToString();

                        stat = visa32.viOpen(resourceMgr, resources[i], visa32.VI_NULL, 1000, out vi);
                        //MessageBox.Show("open " + resources[i], stat.ToString());

                        stat = VisaIO.Write(vi, "*IDN?");
                        //MessageBox.Show("ret " + viRet + "\n" + "*IDN?", stat.ToString());

                        stat = VisaIO.Read(vi, out response);
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

            visa32.viClose(resourceMgr);
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

        private static bool IsVisaLibraryInstalled(UInt16 iManfId)
        {
            return RsVisa.RsViIsVisaLibraryInstalled(iManfId) != 0;
        }
    }
}
