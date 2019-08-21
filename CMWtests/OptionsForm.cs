using System;
using System.Windows.Forms;

namespace CMWtests
{
    public partial class OptionsForm : Form
    {
        public static int StatsCount { get; private set; } = 2;
        public static bool TempOverride;

        public OptionsForm()
        {
            InitializeComponent();

            numericUpDown_EPS.Value = StatsCount;
            if (TempOverride == true)
                checkBoxTempOverride.CheckState = CheckState.Checked;
            else
                checkBoxTempOverride.CheckState = CheckState.Unchecked;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            StatsCount = (int)numericUpDown_EPS.Value;
            TempOverride = (checkBoxTempOverride.CheckState == CheckState.Checked);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            numericUpDown_EPS.Value = StatsCount = 2;
            checkBoxTempOverride.CheckState = CheckState.Unchecked;
        }
    }
}
