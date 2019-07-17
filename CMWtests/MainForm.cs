using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CMWtests
{
    public partial class MainForm : Form
    {
        public static int DefResMgr { get; private set; } = -1;
        private bool CancelTesting { get; set; }
        private bool PauseTesting { get; set; }

        public MainForm()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);

            DefResMgr = VisaIO.OpenResourceMgr();
            if (DefResMgr == 0)
            {
                btnBeginTests.Enabled = false;
                newToolStripMenuItem.Enabled = false;
                optionsToolStripMenuItem.Enabled = false;
                communicateWithInstrumentToolStripMenuItem.Enabled = false;
            }
        }

        private void btnBeginTests_Click(object sender, EventArgs e)
        {
            ControlAero(true);
            textBoxResults.Clear();
            btnBeginTests.Enabled = false;
            newToolStripMenuItem.Enabled = false;
            communicateWithInstrumentToolStripMenuItem.Enabled = false;

            Task.Factory.StartNew(Begin, CancellationToken.None, TaskCreationOptions.AttachedToParent, TaskScheduler.Default);

            // dotNet >= 4.5
            //
            //Task.Factory.StartNewOnDefaultScheduler(() => Begin());
            //Task.Run(() => Begin());
        }

        private void SetMenuStripEnabled(bool v)
        {
            Invoke(new MethodInvoker(() =>
            {
                menuStrip1.Enabled = v;
            }));
        }

        private bool GetBtnBeginEnabled()
        {
            return btnBeginTests.Enabled;
        }

        private void SetBtnBeginEnabled(bool v)
        {
            Invoke(new MethodInvoker(() =>
            {
                btnBeginTests.Enabled = v;
                newToolStripMenuItem.Enabled = v;
                //communicateWithInstrumentToolStripMenuItem.Enabled = v;
            }));
        }

        private bool GetBtnCancelEnabled()
        {
            return btnCancelTests.Enabled;
        }

        private void SetBtnCancelEnabled(bool v)
        {
            Invoke(new MethodInvoker(() =>
            {
                btnCancelTests.Enabled = v;
            }));
        }

        private void communicateWithInstrumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var query = new VISAqueryForm())
            {
                query.ShowDialog();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void Exit()
        {
            if (CancelTests() == true)
            {
                VisaIO.CloseDefMgr();
                Application.Exit();
            }
        }

        private void btnCancelTests_Click(object sender, EventArgs e)
        {
            CancelTests();
        }

        private bool CancelTests()
        {
            PauseTesting = true;
            if (Status != TestStatus.Complete)
                pauseEvent.WaitOne();

            if (Status == TestStatus.Complete ||
                MessageBox.Show("Really abort testing?",
                                "Warning",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Warning,
                                    MessageBoxDefaultButton.Button2)
                == DialogResult.Yes)
            {
                CancelTesting = true;
                return true;
            }
            else
            {
                PauseTesting = false;
                return false;
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnBeginTests_Click(sender, e);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    string bookName = Environment.GetEnvironmentVariable("USERPROFILE") + @"\Desktop\" + tests?.cmwID + ".xlsx";
            //    FileInfo book = new FileInfo(bookName);
            //}
            //catch { }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.ShowDialog();
        }

        private void ProgressBar1_Update(int value)
        {
            Invoke(new MethodInvoker(() =>
            {
                progressBar1.SetProgressNoAnimation(value);
                //progressBar1.Value = value;
                //progressBar1.Refresh();
            }));
        }

        private void ProgressBar2_Update(int value)
        {
            Invoke(new MethodInvoker(() =>
            {
                progressBar2.SetProgressNoAnimation(value);
                //progressBar2.Value = value;
                //progressBar2.Refresh();
            }));
        }

        private void ProgressBar1_Settings(int maxValue)
        {
            Invoke(new MethodInvoker(() =>
            {
                progressBar1.Maximum = maxValue;
                progressBar1.Value = 0;
            }));
        }

        private void ProgressBar2_Settings(int maxValue)
        {
            Invoke(new MethodInvoker(() =>
            {
                progressBar2.Maximum = maxValue;
                progressBar2.Value = 0;
            }));
        }

        private void ProgressBar1_Reset()
        {
            Invoke(new MethodInvoker(() =>
            {
                progressBar1.Value = 0;
            }));
        }

        private void ProgressBar2_Reset()
        {
            Invoke(new MethodInvoker(() =>
            {
                progressBar2.Value = 0;
            }));
        }

        private void SetHead1Text(string text)
        {
            Invoke(new MethodInvoker(() =>
            {
                labelHead1.Text = text;
            }));
        }

        private void SetDebugText(string text)
        {
            Invoke(new MethodInvoker(() =>
            {
                labelDebug.Text = text;
                labelDebug.Refresh();
                Thread.Sleep(500);
                labelDebug.Text = string.Empty;
                labelDebug.Refresh();
            }));
        }

        private void SetHead2Text(string text)
        {
            Invoke(new MethodInvoker(() =>
            {
                labelHead2.Text = text;
                Refresh();
            }));
        }

        private void SetStatusLabel(string text)
        {
            Invoke(new MethodInvoker(() =>
            {
                labelStatus.Text = text;
            }));
        }

        private void AddToResults(string item)
        {
            Invoke(new MethodInvoker(() =>
            {
                textBoxResults.AppendText(item + Environment.NewLine);
            }));
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PauseTesting = true;
            if (Status != TestStatus.Complete)
                pauseEvent.WaitOne();

            var options = new OptionsForm();
            options.ShowDialog(this);
            options.Dispose();

            if (cmw != null)
                cmw.Write("CONFigure:GPRF:MEAS:EPSensor:SCOunt " + OptionsForm.StatsCount);

            PauseTesting = false;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs f)
        {
            switch (f.CloseReason)
            {
                case CloseReason.ApplicationExitCall:
                    break;
                default:
                    Exit();
                    break;
            }
        }

        private void copyToolStripMenuItem1_Click(object sender, EventArgs e) { }
        private void textBoxResults_TextChanged(object sender, EventArgs e) { }
        private void labelHead1_TextChanged(object sender, EventArgs e) { }
        private void labelHead2_TextChanged(object sender, EventArgs e) { }

        public readonly uint DWM_EC_DISABLECOMPOSITION = 0;
        public readonly uint DWM_EC_ENABLECOMPOSITION = 1;

        [DllImport("dwmapi.dll", EntryPoint = "DwmEnableComposition")]
        protected extern static uint Win32DwmEnableComposition(uint uCompositionAction);
        public bool ControlAero(bool enable)
        {
            //return true;
            try
            {
                if (enable)
                    Win32DwmEnableComposition(DWM_EC_ENABLECOMPOSITION);
                else
                    Win32DwmEnableComposition(DWM_EC_DISABLECOMPOSITION);
                return true;
            }
            catch
            {
                MessageBox.Show("ControlAero failed");
                return false;
            }
        }
    }

    public static class ExtensionMethods
    {
        /// <summary>
        /// Sets the progress bar value, without using 'Windows Aero' animation.
        /// This is to work around a known WinForms issue where the progress bar 
        /// is slow to update. 
        /// </summary>
        public static void SetProgressNoAnimation(this ProgressBar pb, int value)
        {
            //if (value != pb.Maximum)
            //pb.Value = value;
            //return;

            // To get around the progressive animation, we need to move the 
            // progress bar backwards.
            if (value == pb.Maximum)
            {
                // Special case as value can't be set greater than Maximum.
                pb.Maximum = value + 1;     // Temporarily Increase Maximum
                pb.Value = value + 1;       // Move past
                pb.Maximum = value;         // Reset maximum
            }
            else
            {
                pb.Value = value + 1;       // Move past
                pb.Value = value;           // Move to correct value
            }
        }
    }
}
