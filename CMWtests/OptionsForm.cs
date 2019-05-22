using System;
using System.Windows.Forms;

namespace CMWtests
{
    public partial class OptionsForm : Form
    {
        public int StatsCount { get; private set; }

        public OptionsForm(int count)
        {
            InitializeComponent();
            numericUpDown_EPS.Value = count;
            StatsCount = count;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            StatsCount = (int)numericUpDown_EPS.Value;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
        }
    }
}
