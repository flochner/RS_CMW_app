using System;
using System.Text;
using System.Windows.Forms;
using RsVisaLoader;

namespace CMWtests
{
    public partial class VISAresourceForm : Form
    {
        public string Resource { get; private set; }
        public Tests.TestStatus Status { get; private set; }

        public VISAresourceForm()
        {
            InitializeComponent();
            GetResources();
            Resource = null;
        }

        public void GetResources()
        {
            int resourceMgr = 0;
            int retCount = 0;
            int vi = 0;

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
            }
            else
            {
                MessageBox.Show("No VISAs Installed!");
                return;
            }

            listBoxResources.Visible = true;
            BtnSelect.Enabled = false;
            Label1.Visible = false;

            StringBuilder desc = new StringBuilder(256);
            visa32.viFindRsrc(resourceMgr, "USB?*", out vi, out retCount, desc);

            if (retCount > 0)
            {
                for (int i = 0; i < retCount; ++i)
                {
                    if (!desc.ToString().Contains("::1::"))
                        listBoxResources.Items.Add(desc.ToString());
                    visa32.viFindNext(vi, desc);
                }
            }
            else
            {
                listBoxResources.Visible = false;
                Label1.Visible = true;
                BtnSelect.Visible = false;
                this.Height = 120;
            }

            if (listBoxResources.Items.Count > 0)
            {
                listBoxResources.Height = 15 * (listBoxResources.Items.Count + 1);
                this.Height = this.Height + listBoxResources.Height;
            }

            if (listBoxResources.Items.Count == 1)
            {
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
                Resource = listBoxResources.SelectedItem.ToString();
            else
                Resource = null;
        }

        private void listBoxResources_SelectedIndexChanged(object sender, EventArgs e)
        {
            BtnSelect.Enabled = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Status = Tests.TestStatus.Abort;
        }

        private static bool IsVisaLibraryInstalled(UInt16 iManfId)
        {
            return RsVisa.RsViIsVisaLibraryInstalled(iManfId) != 0;
        }
    }
}
