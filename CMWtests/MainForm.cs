using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CMWtests
{
    public partial class MainForm : Form
    {
        public static int DefResMgr { get; private set; } = -1;
        private bool CancelTesting { get; set; }
        private ManualResetEvent mreMeasure;
        private ManualResetEvent mreExit;

        public MainForm()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);
            mreMeasure = new ManualResetEvent(true);
            mreExit = new ManualResetEvent(false);


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
            textBoxResults.Clear();
            btnBeginTests.Enabled = false;
            newToolStripMenuItem.Enabled = false;
            communicateWithInstrumentToolStripMenuItem.Enabled = false;

            Task.Factory.StartNew(Begin, TaskCreationOptions.LongRunning);
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
                Task.Factory.StartNew(() =>
                {
                    if (Status != TestStatus.Complete)
                        mreExit.WaitOne();
                    VisaIO.CloseDefMgr();
                    Application.Exit();
                });
            mreExit.Reset();
        }

        private void btnCancelTests_Click(object sender, EventArgs e)
        {
            CancelTests();
        }

        private bool CancelTests()
        {
            mreMeasure.Reset();
            while (Status != TestStatus.Complete &&
                   mreMeasure.WaitOne(0) == true)
                Thread.Sleep(100);

            if (Status == TestStatus.Complete ||
                MessageBox.Show("Really abort testing?",
                                "Warning",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Warning,
                                    MessageBoxDefaultButton.Button2)
                == DialogResult.Yes)
            {
                CancelTesting = true;
                mreMeasure.Set();
                return true;
            }
            else
            {
                mreMeasure.Set();
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
                for (int i = 0; i < 10; i++)
                {
                    progressBar1.Maximum++;
                    progressBar1.Value++;
                    progressBar1.Maximum--;
                    progressBar1.Refresh();

                    progressBar2.Maximum++;
                    progressBar2.Value++;
                    progressBar2.Maximum--;
                    progressBar2.Refresh();

                    
                    
                    //progressBar1.Maximum++;
                    //progressBar1.Value++;
                    //progressBar1.Maximum--;
                    //progressBar1.Refresh();

                    //progressBar2.Maximum++;
                    //progressBar2.Value++;
                    //progressBar2.Maximum--;
                    //progressBar2.Refresh();
                }
#if DEBUG
                labelDebug.Text = progressBar1.Value.ToString();
                //     Thread.Sleep(500);
#endif
            }));
        }

        private void ProgressBar2_Update(int value)
        {
            Invoke(new MethodInvoker(() =>
            {
                progressBar2.SetProgressNoAnimation(value);
#if DEBUG
                labelDebug.Text = progressBar2.Value.ToString();
             //   Thread.Sleep(500);
#endif
            }));
        }

        private void ProgressBar1_Settings(int maxValue)
        {
            Invoke(new MethodInvoker(() =>
            {
                if (maxValue > 0)
                    progressBar1.Maximum = maxValue;
                progressBar1.Value = 0;
                progressBar1.Refresh();
            }));
        }

        private void ProgressBar2_Settings(int maxValue)
        {
            Invoke(new MethodInvoker(() =>
            {
                if (maxValue > 0)
                    progressBar2.Maximum = maxValue;
                progressBar2.Value = 0;
                progressBar2.Refresh();
            }));
        }

        private void SetHead1Text(string text)
        {
            Invoke(new MethodInvoker(() =>
            {
                labelHead1.Text = text;
                labelHead1.Refresh();
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
                labelHead2.Refresh();
            }));
        }

        private void SetStatusLabel(string text)
        {
            Invoke(new MethodInvoker(() =>
            {
                labelStatus.Text = text;
                labelStatus.Refresh();
            }));
        }

        private void AddToResults(string item)
        {
            Invoke(new MethodInvoker(() =>
            {
                textBoxResults.AppendText(item + Environment.NewLine);
                textBoxResults.Refresh();
            }));
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mreMeasure.Reset();
            while (Status != TestStatus.Complete &&
                   mreMeasure.WaitOne(0) == true)
                Thread.Sleep(100);

            var options = new OptionsForm();
            options.ShowDialog(this);
            options.Dispose();

            if (cmw != null)
                cmw.Write("CONFigure:GPRF:MEAS:EPSensor:SCOunt " + OptionsForm.StatsCount);

            mreMeasure.Set();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs f)
        {
            switch (f.CloseReason)
            {
                case CloseReason.ApplicationExitCall:
                    break;
                default:
                    f.Cancel = true;
                    Exit();
                    break;
            }
        }

        private void copyToolStripMenuItem1_Click(object sender, EventArgs e) { }
        private void textBoxResults_TextChanged(object sender, EventArgs e) { }
        private void labelHead1_TextChanged(object sender, EventArgs e) { }
        private void labelHead2_TextChanged(object sender, EventArgs e) { }
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
            pb.Maximum++;
            pb.Value++; pb.Refresh();
            pb.Value++; pb.Refresh();
            pb.Value++; pb.Refresh();
            pb.Value++; pb.Refresh();
            pb.Value++; pb.Refresh();
            pb.Value++; pb.Refresh();
            pb.Value++; pb.Refresh();
            pb.Value++; pb.Refresh();
            pb.Value++; pb.Refresh();
            pb.Value++; pb.Refresh();
            pb.Maximum--;
            return;


            if (pb.Value != 0)
                pb.Value--;
            pb.Refresh();
            //for (int i = 0; i < 11; i++)
            //{
                if (pb.Value != pb.Maximum)
                    pb.Value++;
                Thread.Sleep(50);
                pb.Refresh();
            //}
            

            return;

            int curValue = pb.Value;
            for (int i = -1; i < 10; i++)
            {
                if (curValue == 0)
                    i++;
                if (pb.Value != pb.Maximum)
                    pb.Value += i;
            }
            return;

            // To get around the progressive animation, we need to move the 
            // progress bar backwards.
            if (pb.Value == pb.Minimum)
            {
                // Special case as value can't be set greater than Maximum.
                //pb.Value = pb.Value + 2;       // Move past
                pb.Value = pb.Value + 1;           // Move to correct value
            }
            else
            {
                //pb.Value = pb.Value - 1;       // Move past
                pb.Value = pb.Value + 1;           // Move to correct value
            }
            pb.Refresh();
        }
    }
}
