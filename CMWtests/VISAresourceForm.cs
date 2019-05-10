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
        public MainForm.TestStatus Status { get; private set; } = MainForm.TestStatus.InProgress;
        private IMessageBasedSession session = null;
        private string[] resources;

        public VISAresourceForm()
        {
            InitializeComponent();
            GetResources();
        }

        public void GetResources()
        {
            int resourceMgr = 0;
            int retCount = 0;
            int vi = 0;
            int i = 0;

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

            StringBuilder desc = new StringBuilder(256);
            visa32.viFindRsrc(resourceMgr, "[^ASRL]?*", out vi, out retCount, desc);

            resources = new string[retCount];

            if (retCount > 0)
            {
                for (int j = 0; j < retCount; j++)
                {
                    if (!desc.ToString().Contains("::1::"))
                    {
                        resources[i] = desc.ToString();
                        session = GlobalResourceManager.Open(resources[i]) as IMessageBasedSession;
                        session.Clear();
                        session.RawIO.Write("*IDN?\n");
                        listBoxResources.Items.Add(session.RawIO.ReadString().TrimEnd());
                        session.Dispose();
                        i++;
                    }
                    visa32.viFindNext(vi, desc);
                }
            }
            else
            {
                Resource = null;
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
