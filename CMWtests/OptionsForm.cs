using System;
using System.Windows.Forms;

namespace CMWtests
{
    public partial class OptionsForm : Form
    {
        public static int StatsCount { get; private set; } = 2;

        public OptionsForm()
        {
            InitializeComponent();
            numericUpDown_EPS.Value = StatsCount;
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
            numericUpDown_EPS.Value = StatsCount = 2;
        }
    }
}
