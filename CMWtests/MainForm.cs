using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CMWtests
{
    public partial class MainForm : Form
    {
        //delegate void StringDelegate(string text);
        //delegate void BoolDelegate(bool v);
        //delegate void IntDelegate(int n);
        //delegate void VoidDelegate();
        private CancellationTokenSource cts = null;
        private bool isExitRequested = false;
        private bool isExitOK = true;
        private bool pauseTesting = false;
        private static int _defResMgr;

        public static int DefResMgr { get => _defResMgr; private set => _defResMgr = value; }

        public MainForm()
        {
            InitializeComponent();
            VisaIO.OpenResourceMgr(out _defResMgr);
        }

        private void btnBeginTests_Click(object sender, EventArgs e)
        {
            btnBeginTests.Enabled = false;
            newToolStripMenuItem.Enabled = false;
            textBoxResults.Clear();
            pauseTesting = false;

            cts = null ?? new CancellationTokenSource();

            Task.Factory.StartNew(Begin, CancellationToken.None, TaskCreationOptions.AttachedToParent, TaskScheduler.Default);
            //Task.Factory.StartNewOnDefaultScheduler(() => Begin());
            //Task.Run(() => Begin());
        }

        public void SetBtnBeginEnabled(bool v)
        {
            //if (this.btnBeginTests.InvokeRequired)
            //{
            //    BoolDelegate d = new BoolDelegate(SetBtnBeginEnabled);
            //    this.BeginInvoke(d, new object[] { v });
            //}
            //else
            //{
            //    this.btnBeginTests.Enabled = v;
            //    this.Refresh();
            //}

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
            //if (this.btnCancelTests.InvokeRequired)
            //{
            //    BoolDelegate d = new BoolDelegate(SetBtnCancelEnabled);
            //    this.BeginInvoke(d, new object[] { v });
            //}
            //else
            //{
            //    this.btnCancelTests.Enabled = v;
            //    this.Refresh();
            //}

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
            using (var query = new VISAqueryForm()) { query.ShowDialog(); }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isExitRequested = true;
            if (status == TestStatus.Complete || status == TestStatus.Abort)
                AppExit();
            else
                btnCancelTests_Click(sender, e);
        }

        public void AppExit()
        {
            while (!isExitOK)
                Thread.Sleep(100);
            VisaIO.CloseDefMgr();
            Application.Exit();
        }

        private void btnCancelTests_Click(object sender, EventArgs e)
        {
            if (cts.IsCancellationRequested && isExitRequested)
                AppExit();

            pauseTesting = true;
            var abort = MessageBox.Show("Really abort testing?",
                                        "Warning",
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Warning,
                                         MessageBoxDefaultButton.Button2);
            if (abort == DialogResult.Yes)
                try
                {
                    cts.Cancel();
                }
                catch (NullReferenceException exc) { MessageBox.Show("btnCancelTests_Click\n" + exc.Message, exc.GetType().ToString()); }
                catch (ObjectDisposedException exc) { MessageBox.Show("btnCancelTests_Click\n" + exc.Message, exc.GetType().ToString()); }
                catch (Exception exc) { MessageBox.Show("btnCancelTests_Click\n" + exc.Message, exc.GetType().ToString()); }
            else
                pauseTesting = false;
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

        private void SetHead2Text(string text)
        {
            Invoke((MethodInvoker)(() =>
            {
                labelHead2.Text = text;
                Refresh();
            }));
        }

        private void AddToResults(string item)
        {
            Invoke((MethodInvoker)(() =>
            {
                textBoxResults.AppendText(item + Environment.NewLine);
            }));
        }

        private void MainForm_Load(object sender, EventArgs e) { }
        private void copyToolStripMenuItem1_Click(object sender, EventArgs e) { }
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) { }
        private void textBoxResults_TextChanged(object sender, EventArgs e) { }//this.Refresh(); }
        private void labelHead1_TextChanged(object sender, EventArgs e) { }//this.Refresh(); }
        private void labelHead2_TextChanged(object sender, EventArgs e) { }//this.Refresh(); }

        #region Code for future use
        //string[] args = new string[]
        //{
        //        "ll",
        //        "ww",
        //};
        //CMWgraph.Graph graph = new CMWgraph.Graph(args);    }
        #endregion
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

            pb.CreateGraphics().DrawString(((int)((double)pb.Value / (double)pb.Maximum * 100)).ToString() + "%",
                new Font("Arial", (float)8.25, FontStyle.Regular),
                Brushes.Black,
                new PointF(pb.Width / 2 - 10, pb.Height / 2 - 7));
            pb.Refresh();

        }
    }
}
