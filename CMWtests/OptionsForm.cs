using System;
using System.Windows.Forms;

namespace CMWtests
{
    public partial class Options : Form
    {
        const int STATSRESET = 2;

        public static int StatsCount { get; private set; } = STATSRESET;
        public static bool TempOverride { get; set; } = false;
        public static bool KB036Override { get; set; } = true;
        public static bool RecordTemp { get; set; } = false;
        public static bool RecordTempEnabled { get; set; } = true;

        public Options()
        {
            InitializeComponent();
#if DEBUG
            checkBoxKB036.Visible = true;
#endif
            numericUpDown_EPS.Value = StatsCount;
            checkBoxRecordTemp.Enabled = RecordTempEnabled;

            if (TempOverride == true)
                checkBoxTempOverride.CheckState = CheckState.Checked;
            else
                checkBoxTempOverride.CheckState = CheckState.Unchecked;

            if (KB036Override == true)
                checkBoxKB036.CheckState = CheckState.Checked;
            else
                checkBoxKB036.CheckState = CheckState.Unchecked;

            if (RecordTemp == true)
                checkBoxRecordTemp.CheckState = CheckState.Checked;
            else
                checkBoxRecordTemp.CheckState = CheckState.Unchecked;

            checkBoxTempOverride.Enabled = TempGauge.OptionsOverrideTempEnabled;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            StatsCount = (int)numericUpDown_EPS.Value;
            TempOverride = (checkBoxTempOverride.CheckState == CheckState.Checked);
            RecordTemp = (checkBoxRecordTemp.CheckState == CheckState.Checked);
            KB036Override = (checkBoxKB036.CheckState == CheckState.Checked);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            numericUpDown_EPS.Value = StatsCount = STATSRESET;
            checkBoxTempOverride.CheckState = CheckState.Unchecked;
            checkBoxKB036.CheckState = CheckState.Checked;
            checkBoxRecordTemp.CheckState = CheckState.Unchecked;
        }

        private void buttonCancel_Click(object sender, EventArgs e) { }
    }
}
