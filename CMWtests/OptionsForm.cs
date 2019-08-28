using System;
using System.Windows.Forms;

namespace CMWtests
{
    public partial class OptionsForm : Form
    {
        const int STATSRESET = 2;

        public static int StatsCount { get; private set; } = STATSRESET;
        public static bool TempOverride { get; set; }
        public static bool KB036Override { get; set; } = true;

        public OptionsForm()
        {
            InitializeComponent();
#if DEBUG
            checkBoxKB036.Visible = true;
#endif
            numericUpDown_EPS.Value = StatsCount;
            if (TempOverride == true)
                checkBoxTempOverride.CheckState = CheckState.Checked;
            else
                checkBoxTempOverride.CheckState = CheckState.Unchecked;
            if (KB036Override == true)
                checkBoxKB036.CheckState = CheckState.Checked;
            else
                checkBoxKB036.CheckState = CheckState.Unchecked;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            StatsCount = (int)numericUpDown_EPS.Value;
            TempOverride = (checkBoxTempOverride.CheckState == CheckState.Checked);
            KB036Override = (checkBoxKB036.CheckState == CheckState.Checked);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            numericUpDown_EPS.Value = StatsCount = STATSRESET;
            checkBoxTempOverride.CheckState = CheckState.Unchecked;
            KB036Override = (checkBoxKB036.CheckState == CheckState.Checked);
        }
    }
}
