using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using NationalInstruments.VisaNS;

namespace CMWtests
{
    public partial class VISAresourceForm : Form
    {
        public string Selection { get => _selection; private set { } }
        private string _selection;

        public VISAresourceForm()
        {
            InitializeComponent();
            GetResources();
        }

        public void GetResources()
        {
            string[] resources = { };
            string[] tempResources = { };
            listBoxResources.Visible = true;
            Label1.Visible = false;

            try
            {
                tempResources = ResourceManager.GetLocalManager().FindResources(@"(GPIB|USB)?*INSTR");
            }
            catch
            {
                listBoxResources.Visible = false;
                Label1.Visible = true;

                resources = new string[] { "No VISA resources available" };
                BtnSelect.Enabled = false;
                return;
            }

    //        tempResources = null;// new string[] { "asdfasdfasdf", "waerwqerqwer", "cvbnvcmnvcm",
   //                              //        "sdf::1::poiu", "fjofh34tgjs", "areg::1::hsercvbc", "xcvbxcsdfsdfw443" };

            // Remove any resource with "::1::"
            if (tempResources != null)
            {
                int tempLen = tempResources.Length;

                for (int i = 0; i < tempLen; i++)
                {
                    if (tempResources[i].Contains("::1::"))
                        tempLen--;
                }
                resources = new string[tempLen];
                for (int i = 0, j = 0; i < tempLen; i++)
                {
                    if (tempResources[j].Contains("::1::"))
                        j++;
                    resources[i] = tempResources[j];
                    j++;
                }
            }

            listBoxResources.Height = listBoxResources.ItemHeight * (resources.Length + 1);
            Height = Height + ((resources.Length - 1) * 15);
            listBoxResources.DataSource = null;
            listBoxResources.DataSource = resources;
            listBoxResources.SelectedIndex = -1;
        }

        private void ListBoxResources_DoubleClick(object sender, MouseEventArgs e)
        {
            int index = listBoxResources.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                BtnSelect_Click(sender, e);
            }
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            if (listBoxResources.Visible == true &&
                listBoxResources.SelectedIndex >= 0)
            {
                _selection = listBoxResources.SelectedItem.ToString();
                this.Dispose();
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
