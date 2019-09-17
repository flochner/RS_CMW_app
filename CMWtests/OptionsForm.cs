using System;
using System.Windows.Forms;

namespace CMWtests
{
    public partial class OptionsForm : Form
    {
        const int STATSRESET = 2;
        public static int StatsCount = STATSRESET;
        public static bool TempOverride = false;
        public static bool TempOverrideEnabled = true;
        public static bool RecordTemp = false;
        public static bool RecordTempEnabled = true;
        public static bool KB036Override = true;

        public OptionsForm()
        {
            InitializeComponent();
            //ReadOptionsFile();
            numericUpDown_EPS.Value = StatsCount;
            checkBoxTempOverride.Checked = TempOverride;
            checkBoxRecordTemp.Checked = RecordTemp;
            checkBoxKB036.Checked = KB036Override;
#if DEBUG
            checkBoxKB036.Visible = true;
#endif
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            StatsCount = (int)numericUpDown_EPS.Value;
            TempOverride = checkBoxTempOverride.Checked;
            RecordTemp = checkBoxRecordTemp.Checked;
            KB036Override = checkBoxKB036.Checked;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            numericUpDown_EPS.Value = STATSRESET;
            checkBoxTempOverride.Checked = false;
            checkBoxRecordTemp.Checked = false;
            checkBoxKB036.Checked = true;
        }

        private void buttonCancel_Click(object sender, EventArgs e) { }
    }
}
