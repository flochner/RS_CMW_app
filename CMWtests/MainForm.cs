using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CMWtests
{
    public partial class MainForm : Form
    {
        delegate void StringArgReturningVoidDelegate(string text);
        delegate void BoolArgReturningVoidDelegate(bool v);
        delegate bool VoidArgReturningBoolDelegate();
        private CancellationTokenSource _cts = null;

        public bool TestsDone { get; set; } = false;

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnBeginTests_Click(object sender, EventArgs e)
        {
            Tests tests;
            SetBtnBeginEnabled(false);
            textBoxResults.Clear();

            _cts = null ?? new CancellationTokenSource();
            tests = null ?? new Tests(this, _cts);

            var seq = Task.Run(() => tests.Sequencer());
        }

        public void SetBtnBeginEnabled(bool v)
        {
            if (this.btnBeginTests.InvokeRequired)
            {
                BoolArgReturningVoidDelegate d = new BoolArgReturningVoidDelegate(SetBtnBeginEnabled);
                this.Invoke(d, new object[] { v });
            }
            else
            {
                this.btnBeginTests.Enabled = v;
                this.Refresh();
            }
        }

        public void SetBtnCancelEnabled(bool v)
        {
            if (this.btnCancelTests.InvokeRequired)
            {
                BoolArgReturningVoidDelegate d = new BoolArgReturningVoidDelegate(SetBtnCancelEnabled);
                this.Invoke(d, new object[] { v });
            }
            else
            {
                this.btnCancelTests.Enabled = v;
                this.Refresh();
            }
        }

        public bool GetBtnBeginEnabled()
        {
            //if (this.BtnBeginTests.InvokeRequired)
            //{
            //    VoidArgReturningBoolDelegate d = new VoidArgReturningBoolDelegate(GetBtnBeginEnabled);
            //    this.Invoke(d, new object[] { });
            //    return false;
            //}
            //else
            //{
            return this.btnBeginTests.Enabled;
            //}
        }

        public void AddToResults(string item)
        {
            if (this.textBoxResults.InvokeRequired)
            {
                StringArgReturningVoidDelegate d = new StringArgReturningVoidDelegate(AddToResults);
                this.Invoke(d, new object[] { item });
            }
            else
            {
                this.textBoxResults.AppendText(item + Environment.NewLine);
            }
        }

        public void SetHead1Text(string text)
        {
            if (this.labelHead1.InvokeRequired)
            {
                StringArgReturningVoidDelegate d = new StringArgReturningVoidDelegate(SetHead1Text);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.labelHead1.Text = text;
            }
        }

        public void SetHead2Text(string text)
        {
            if (this.labelHead1.InvokeRequired)
            {
                StringArgReturningVoidDelegate d = new StringArgReturningVoidDelegate(SetHead2Text);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.labelHead2.Text = text;
            }
        }

        private void communicateWithInstrumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var commWindow = new VISAqueryForm();
            //commWindow.ShowDialog();
            //commWindow = null;

            new VISAqueryForm().ShowDialog();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VISAresourceForm resource = new VISAresourceForm();
            resource.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnCancelTests_Click(sender, e);
            Application.Exit();
        }

        private void btnCancelTests_Click(object sender, EventArgs e)
        {
            try { _cts.Cancel(); }
            catch (NullReferenceException) { TestsDone = true; }
            catch (ObjectDisposedException) { }
            //catch
            //(NationalInstruments.VisaNS.VisaException exc)
            //{
            //    MessageBox.Show(exc.Message, exc.GetType().ToString());
            //}

            int i = 0;
            while (TestsDone != true)
            {
                i++;
                Console.WriteLine("sleeping {0}", i);
                Thread.Sleep(500);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.ShowDialog();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e) { }
        private void openToolStripMenuItem_Click(object sender, EventArgs e) { }
        private void MainForm_Load(object sender, EventArgs e) { }
        private void copyToolStripMenuItem1_Click(object sender, EventArgs e) { }
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) { }
        private void textBoxResults_TextChanged(object sender, EventArgs e) { this.Refresh(); }
        private void labelHead1_TextChanged(object sender, EventArgs e) { this.Refresh(); }
        private void labelHead2_TextChanged(object sender, EventArgs e) { this.Refresh(); }

        #region Code for future use
        //string[] args = new string[]
        //{
        //        "ll",
        //        "ww",
        //};
        //CMWgraph.Graph graph = new CMWgraph.Graph(args);    }
        #endregion
    }
}
