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

        private void BtnBeginTests_Click(object sender, EventArgs e)
        {
            Tests tests;
            SetBtnBeginEnabled(false);
            TextBoxResults.Clear();

            _cts = null ?? new CancellationTokenSource();
            tests = null ?? new Tests(this, _cts);

            var seq = Task.Run(() => tests.Sequencer());
        }

        public void SetBtnBeginEnabled(bool v)
        {
            if (this.BtnBeginTests.InvokeRequired)
            {
                BoolArgReturningVoidDelegate d = new BoolArgReturningVoidDelegate(SetBtnBeginEnabled);
                this.Invoke(d, new object[] { v });
            }
            else
            {
                this.BtnBeginTests.Enabled = v;
                this.Refresh();
            }
        }

        public void SetBtnCancelEnabled(bool v)
        {
            if (this.BtnCancelTests.InvokeRequired)
            {
                BoolArgReturningVoidDelegate d = new BoolArgReturningVoidDelegate(SetBtnCancelEnabled);
                this.Invoke(d, new object[] { v });
            }
            else
            {
                this.BtnCancelTests.Enabled = v;
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
            return this.BtnBeginTests.Enabled;
            //}
        }

        public void AddToResults(string item)
        {
            if (this.TextBoxResults.InvokeRequired)
            {
                StringArgReturningVoidDelegate d = new StringArgReturningVoidDelegate(AddToResults);
                this.Invoke(d, new object[] { item });
            }
            else
            {
                this.TextBoxResults.AppendText(item + Environment.NewLine);
            }
        }

        public void SetHead1Text(string text)
        {
            if (this.LabelHead1.InvokeRequired)
            {
                StringArgReturningVoidDelegate d = new StringArgReturningVoidDelegate(SetHead1Text);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.LabelHead1.Text = text;
            }
        }

        public void SetHead2Text(string text)
        {
            if (this.LabelHead1.InvokeRequired)
            {
                StringArgReturningVoidDelegate d = new StringArgReturningVoidDelegate(SetHead2Text);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.LabelHead2.Text = text;
            }
        }

        private void CommunicateWithInstrumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var commWindow = new VISAqueryForm();
            //commWindow.ShowDialog();
            //commWindow = null;

            new VISAqueryForm().ShowDialog();
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BtnCancelTests_Click(sender, e);
            this.Close();
        }

        private void BtnCancelTests_Click(object sender, EventArgs e)
        {
            try { _cts.Cancel(); }
            catch (NullReferenceException) { TestsDone = true; }
            catch (ObjectDisposedException) { }
            catch
            (NationalInstruments.VisaNS.VisaException exc)
            {
                MessageBox.Show(exc.Message, exc.GetType().ToString());
            }
            while (TestsDone != true)
                Thread.Sleep(500);
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.ShowDialog();
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e) { }
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e) { }
        private void MainForm_Load(object sender, EventArgs e) { }
        private void CopyToolStripMenuItem1_Click(object sender, EventArgs e) { }
        private void MenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) { }
        private void TextBoxResults_TextChanged(object sender, EventArgs e) { this.Refresh(); }
        private void LabelHead1_TextChanged(object sender, EventArgs e) { this.Refresh(); }
        private void LabelHead2_TextChanged(object sender, EventArgs e) { this.Refresh(); }

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
