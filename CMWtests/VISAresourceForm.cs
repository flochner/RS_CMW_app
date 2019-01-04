using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using RsVisaLoader;

namespace CMWtests
{
    public partial class VISAresourceForm : Form
    {
        public string Selection { get; private set; }

        public VISAresourceForm()
        {
            InitializeComponent();
            GetResources();
        }

        public void GetResources()
        {
            int m_defRM = visa32.VI_NULL;
            int retCount = 0;
            int vi = 0;

            listBoxResources.Visible = true;
            BtnSelect.Enabled = false;
            Label1.Visible = false;

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

                visa32.viOpenDefaultRM(out m_defRM);
            }
            else
            {
                MessageBox.Show("No VISAs Installed!");
                return;
            }

            StringBuilder desc = new StringBuilder(256);
            visa32.viFindRsrc(m_defRM, "USB?*", out vi, out retCount, desc);

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
        }

        private static bool IsVisaLibraryInstalled(UInt16 iManfId)
        {
            return RsVisa.RsViIsVisaLibraryInstalled(iManfId) != 0;
        }

        private void listBoxResources_DoubleClick(object sender, MouseEventArgs e)
        {
            int index = listBoxResources.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                btnSelect_Click(sender, e);
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (listBoxResources.Visible == true &&
                listBoxResources.SelectedIndex >= 0)
            {
                Selection = listBoxResources.SelectedItem.ToString();
            }
            else
            {
                Selection = "No VISA resources found.";
            }
        }

        private void listBoxResources_SelectedIndexChanged(object sender, EventArgs e)
        {
            BtnSelect.Enabled = true;
        }

        private void btnCancel_Click(object sender, EventArgs e) { }
    }
}
