using System;
using System.Windows.Forms;

namespace CMWtests
{
    public partial class OptionsForm : Form
    {
        public static int StatsCount { get; private set; } = 2;

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

        private void btnReset_Click(object sender, EventArgs e)
        {
            StatsCount = 2;
        }
    }
}
