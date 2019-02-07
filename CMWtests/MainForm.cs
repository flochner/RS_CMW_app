using System;
using System.IO;
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
        private bool IsExitRequested = false;
        private bool PauseTesting = false;

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnBeginTests_Click(object sender, EventArgs e)
        {
            btnBeginTests.Enabled = false;
            textBoxResults.Clear();
            PauseTesting = false;

            cts = null ?? new CancellationTokenSource();

            Task.Run(() => Begin());
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
            }));

        }

        public bool GetBtnCancelEnabled()
        {
            return btnCancelTests.Enabled;
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

        private void communicateWithInstrumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var query = new VISAqueryForm()) { query.ShowDialog(); }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IsExitRequested = true;
            if (status == TestStatus.Complete || status == TestStatus.Abort)
                AppExit();
            else
                btnCancelTests_Click(sender, e);
        }

        public void AppExit()
        {
            Application.Exit();
        }

        private void btnCancelTests_Click(object sender, EventArgs e)
        {
            if (cts.IsCancellationRequested && IsExitRequested)
                AppExit();

            PauseTesting = true;
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
                PauseTesting = false;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.ShowDialog();
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
        }
    }
}
