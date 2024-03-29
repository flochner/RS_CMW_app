﻿using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CMWtests
{
    public partial class MainForm : Form
    {
        public static int DefResMgr { get; private set; } = -1;
        public static bool CancelTesting { get; set; }
        private AutoResetEvent areExit;
        private ManualResetEvent mreMeasure;
        private TempGauge tempGauge = null;

        public MainForm()
        {
            InitializeComponent();
            Thread.CurrentThread.Name = "MainForm";

            tempGauge = new TempGauge(this)
            { Location = new System.Drawing.Point(539, 29) };
            this.Controls.Add(tempGauge);

            mreMeasure = new ManualResetEvent(true);
            areExit = new AutoResetEvent(false);

            DefResMgr = VisaIO.OpenResourceMgr();
            if (DefResMgr == 0)
            {
                btnBeginTests.Enabled = false;
                newToolStripMenuItem.Enabled = false;
                optionsToolStripMenuItem.Enabled = false;
                communicateWithInstrumentToolStripMenuItem.Enabled = false;
                MessageBox.Show("No VISA Resource Manager installed.");
            }
        }

        private void btnBeginTests_Click(object sender, EventArgs e)
        {
            pictureBoxGraph.Image = null;
            textBoxResults.Clear();
            btnBeginTests.Enabled = false;
            newToolStripMenuItem.Enabled = false;
            communicateWithInstrumentToolStripMenuItem.Enabled = false;

            Task.Factory.StartNew(Begin, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void btnCancelTests_Click(object sender, EventArgs e)
        {
            CancelTests();
        }

        public void SetHead1Text(string text)
        {
            BeginInvoke(new MethodInvoker(() =>
            {
                labelHead1.Text = text;
                labelHead1.Refresh();
            }));
        }

        public void SetHead2Text(string text)
        {
            BeginInvoke(new MethodInvoker(() =>
            {
                labelHead2.Text = text;
                labelHead2.Refresh();
            }));
        }

        private void SetStatusText(string text)
        {
#if DEBUG
            BeginInvoke(new MethodInvoker(() =>
            {
                labelStatus.Text = text;
                labelStatus.Refresh();
            }));
#endif
        }

        public void SetDebugText(string text)
        {
#if DEBUG
            BeginInvoke(new MethodInvoker(() =>
            {
                labelDebug.Text = text;
                labelDebug.Refresh();
                //Thread.Sleep(500);
                //labelDebug.Text = "";
                //labelDebug.Refresh();
            }));
#endif
        }

        public void AddToResults(string item)
        {
            BeginInvoke(new MethodInvoker(() =>
            {
                textBoxResults.AppendText(item + Environment.NewLine);
                textBoxResults.Refresh();
            }));
        }

        private void ProgressBar1_Init(int maxValue = 0)
        {
            BeginInvoke(new MethodInvoker(() =>
            {
                if (maxValue > 0)
                    progressBar1.Maximum = maxValue;
                progressBar1.Value = 0;
                progressBar1.Refresh();
            }));
        }

        private void ProgressBar1_Update(int ampl)
        {
            BeginInvoke(new MethodInvoker(() =>
            {
                progressBar1.PerformStep();
                progressBar1.Refresh();
#if DEBUG
                //labelDebug.Text = progressBar1.Value.ToString();
                //labelDebug.Refresh();
#endif
            }));
        }

        private bool CancelTests()
        {
            mreMeasure.Reset();
            while ((Status != TestStatus.Complete) && (mreMeasure.WaitOne(0) == true))
                Thread.Sleep(100);

            if (Status == TestStatus.Complete || MessageBox.Show("Really abort testing?", "Warning",
                                                                  MessageBoxButtons.YesNo,
                                                                  MessageBoxIcon.Warning,
                                                                  MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                CancelTesting = true;
                mreMeasure.Set();
                return true;
            }
            else
            {
                CancelTesting = false;
                mreMeasure.Set();
                return false;
            }
        }

        private void Exit()
        {
            if (CancelTests() == true)
                Task.Factory.StartNew(() =>
                {
                    if (Status != TestStatus.Complete)
                        areExit.WaitOne();
                    VisaIO.CloseDefMgr();
                    Application.Exit();
                });
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

        private void SetBtnBeginEnabled(bool v)
        {
            BeginInvoke(new MethodInvoker(() =>
            {
                btnBeginTests.Enabled = v;
                newToolStripMenuItem.Enabled = v;
                communicateWithInstrumentToolStripMenuItem.Enabled = v;
            }));
        }

        private bool GetBtnCancelEnabled()
        {
            return btnCancelTests.Enabled;
        }

        private void SetBtnCancelEnabled(bool v)
        {
            BeginInvoke(new MethodInvoker(() =>
            {
                btnCancelTests.Enabled = v;
            }));
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnBeginTests_Click(sender, e);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string bookName = Environment.GetEnvironmentVariable("USERPROFILE") + @"\Desktop\" + cmwID + ".xlsx";
                FileInfo book = new FileInfo(bookName);
            }
            catch { }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mreMeasure.Reset();  // sends blocking signal for WaitOne() to catch
            while (Status != TestStatus.Complete &&
                   mreMeasure.WaitOne(0) == true)  // WaitOne(0) :: true = thread is not blocking :: WaitOne() has not received Reset() signal
                Thread.Sleep(100);

            using (var options = new OptionsForm())
                options.ShowDialog(this);

            if (cmw != null)
                Write(cmw, "CONFigure:GPRF:MEAS:EPSensor:SCOunt " + OptionsForm.StatsCount);

            mreMeasure.Set();  // sends unblocking signal for WaitOne() to catch
        }

        private void communicateWithInstrumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var query = new VISAqueryForm())
                query.ShowDialog();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var about = new AboutBox())
                about.ShowDialog();
        }

        private void pictureBoxGraph_Paint(object sender, PaintEventArgs e) { }

        private void pictureBoxGraph_Click(object sender, MouseEventArgs e) { }

        private void textBoxResults_Click(object sender, EventArgs e){ }

        private void contextMenu_Opening(object sender, CancelEventArgs e)
        {
            if (this.contextMenu.SourceControl.Name == "pictureBoxGraph")
            {
                selectAllContextMenuItem.Enabled = false;
                if (pictureBoxGraph.Image == null)
                    copyContextMenuItem.Enabled = false;
                else
                {
                    pictureBoxGraph.Select();
                    copyContextMenuItem.Enabled = true;
                }
            }
            else if (this.contextMenu.SourceControl.Name == "textBoxResults")
            {
                if (string.IsNullOrEmpty(textBoxResults.Text))
                {
                    selectAllContextMenuItem.Enabled = false;
                    copyContextMenuItem.Enabled = false;
                }
                else
                {
                    selectAllContextMenuItem.Enabled = true;
                    if (string.IsNullOrEmpty(textBoxResults.SelectedText))
                        copyContextMenuItem.Enabled = false;
                    else
                        copyContextMenuItem.Enabled = true;
                }
            }
        }

        private void copyContextMenuItem_Click(object sender, EventArgs e)
        {
            if (this.contextMenu.SourceControl.Name == "pictureBoxGraph" && pictureBoxGraph.Image != null)
                Clipboard.SetImage(pictureBoxGraph.Image);
            else if (this.contextMenu.SourceControl.Name == "textBoxResults")
                Clipboard.SetText(textBoxResults.SelectedText);
        }

        private void selectAllContextMenuItem_Click(object sender, EventArgs e)
        {
            textBoxResults.SelectAll();
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
