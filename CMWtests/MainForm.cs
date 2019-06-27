using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CMWtests
{
    public partial class MainForm : Form
    {
        public static int DefResMgr { get; private set; } = -1;

        private CancellationTokenSource cts = null;
        private bool isExitRequested = false;
        private bool isExitOK = true;
        //private bool paused = true;
        private bool pauseTesting = false;

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
            textBoxResults.Clear();
            btnBeginTests.Enabled = false;
            newToolStripMenuItem.Enabled = false;
            communicateWithInstrumentToolStripMenuItem.Enabled = false;
            pauseTesting = false;
            Status = TestStatus.InProgress;

            //return;

            cts = null ?? new CancellationTokenSource();

            Task.Factory.StartNew(Begin, CancellationToken.None, TaskCreationOptions.AttachedToParent, TaskScheduler.Default);

            // dotNet >= 4.5
            //
            //Task.Factory.StartNewOnDefaultScheduler(() => Begin());
            //Task.Run(() => Begin());
        }

        public void SetBtnBeginEnabled(bool v)
        {
            Invoke((MethodInvoker)(() =>
            {
                btnBeginTests.Enabled = v;
                newToolStripMenuItem.Enabled = v;
            }));
        }

        public bool GetBtnBeginEnabled()
        {
            return btnBeginTests.Enabled;
        }

        public void SetBtnCancelEnabled(bool v)
        {
            Invoke((MethodInvoker)(() =>
            {
                btnCancelTests.Enabled = v;
            }));
        }

        public bool GetBtnCancelEnabled()
        {
            return btnCancelTests.Enabled;
        }

        private void communicateWithInstrumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var query = new VISAqueryForm())
            {
                query.ShowDialog();
                communicateWithInstrumentToolStripMenuItem.Enabled = true;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isExitRequested = true;
            if (Status == TestStatus.Complete || Status == TestStatus.Abort)
                AppExit();
            else
                btnCancelTests_Click(sender, e);
        }

        private void AppExit()
        {
            while (!isExitOK)
            {
                Thread.Sleep(100);
#if DEBUG
                DebugText = "Locked at AppExit";
#endif
            }
            VisaIO.CloseDefMgr();
            Application.Exit();
        }

        private void btnCancelTests_Click(object sender, EventArgs e)
        {
            if (cts.IsCancellationRequested && isExitRequested)
                AppExit();

            pauseTesting = true;
            while (Status == TestStatus.InProgress)
            {
                Thread.Sleep(100);
#if DEBUG
                DebugText = "Locked at abort button click";
#endif
            }

            var abort = MessageBox.Show("Really abort testing?",
                                        "Warning",
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Warning,
                                         MessageBoxDefaultButton.Button2);
            if (abort == DialogResult.Yes)
            {
                try { cts.Cancel(); }
                catch (NullReferenceException exc) { MessageBox.Show("btnCancelTests_Click\n" + exc.Message, exc.GetType().ToString()); }
                catch (ObjectDisposedException exc) { MessageBox.Show("btnCancelTests_Click\n" + exc.Message, exc.GetType().ToString()); }
                catch (Exception exc) { MessageBox.Show("btnCancelTests_Click\n" + exc.Message, exc.GetType().ToString()); }
            }
            else
            {
                pauseTesting = false;
                Status = TestStatus.InProgress;
               // SetStatusLabel(Status.ToString());
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnBeginTests_Click(sender, e);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (tests == null)
            //{
            //    MessageBox.Show("no excel sheet");
            //    return;
            //}

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

        private void ProgressBar1_Update()
        {
            Invoke((MethodInvoker)(() =>
            {
                progressBar1.SetProgressNoAnimation(pointsCount);
            }));
        }

        private void ProgressBar2_Update(int value)
        {
            Invoke((MethodInvoker)(() =>
            {
                progressBar2.SetProgressNoAnimation(value);
            }));
        }

        private void ProgressBar1_Settings(int maxValue)
        {
            Invoke((MethodInvoker)(() =>
            {
                progressBar1.Maximum = maxValue;
                progressBar1.Value = 0;
                Refresh();
            }));
        }

        private void ProgressBar2_Settings(int maxValue)
        {
            Invoke((MethodInvoker)(() =>
            {
                progressBar2.Maximum = maxValue;
                progressBar2.Value = 0;
            }));
        }

        private void ProgressBar1_Reset()
        {
            Invoke((MethodInvoker)(() =>
            {
                progressBar1.Value = 0;
            }));
        }

        private void ProgressBar2_Reset()
        {
            Invoke((MethodInvoker)(() =>
            {
                progressBar2.Value = 0;
            }));
        }

        private void SetHead1Text(string text)
        {
            Invoke((MethodInvoker)(() =>
            {
                labelHead1.Text = text;
            }));
        }

        private void SetDebugText(string text)
        {
            Invoke((MethodInvoker)(() =>
            {
                labelDebug.Text = text;
            }));
        }

        private void SetHead2Text(string text)
        {
            Invoke((MethodInvoker)(() =>
            {
                labelHead2.Text = text;
                Refresh();
            }));
        }

        private void SetStatusLabel(string text)
        {
            Invoke((MethodInvoker)(() =>
            {
                labelStatus.Text = text;
            }));
        }

        private void AddToResults(string item)
        {
            Invoke((MethodInvoker)(() =>
            {
                textBoxResults.AppendText(item + Environment.NewLine);
            }));
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pauseTesting = true;
            while (Status == TestStatus.InProgress)
            {
                Thread.Sleep(100);
#if DEBUG
                DebugText = "Locked at options menu item click";
#endif
            }
            var options = new OptionsForm(statsCount);
            options.ShowDialog(this);
            statsCount = options.StatsCount;
            options.Dispose();

            if (cmw != null)
                cmw.Write("CONFigure:GPRF:MEAS:EPSensor:SCOunt " + statsCount);

            pauseTesting = false;
        }

        private void ResetOptions()
        {
            statsCount = 2;
        }

        void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (e.CloseReason)
            {
                case CloseReason.ApplicationExitCall:
                    break;
                case CloseReason.FormOwnerClosing:
                    break;
                case CloseReason.MdiFormClosing:
                    break;
                case CloseReason.None:
                    break;
                case CloseReason.TaskManagerClosing:
                    break;
                case CloseReason.UserClosing:
                    exitToolStripMenuItem_Click(sender, e);
                    break;
                case CloseReason.WindowsShutDown:
                    break;
                default:
                    break;
            }
        }

        private void MainForm_Load(object sender, EventArgs e) { }
        private void copyToolStripMenuItem1_Click(object sender, EventArgs e) { }
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) { }
        private void textBoxResults_TextChanged(object sender, EventArgs e) { }//this.Refresh(); }
        private void labelHead1_TextChanged(object sender, EventArgs e) { }//this.Refresh(); }
        private void labelHead2_TextChanged(object sender, EventArgs e) { }//this.Refresh(); }
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

            //pb.CreateGraphics().DrawString(((int)((double)pb.Value / (double)pb.Maximum * 100)).ToString() + "%",
            //    new Font("Arial", (float)8.25, FontStyle.Regular),
            //    Brushes.Black,
            //    new PointF(pb.Width / 2 - 10, pb.Height / 2 - 7));
            //pb.Refresh();
        }
    }
}
