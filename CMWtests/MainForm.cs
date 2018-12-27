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
        public const bool Success = true;
        public const bool Failure = false;
        delegate void StringArgReturningVoidDelegate(string text);
        delegate void BoolArgReturningVoidDelegate(bool v);
        delegate bool VoidArgReturningBoolDelegate();
        CancellationTokenSource cts = new CancellationTokenSource();

        public MainForm()
        {
            InitializeComponent();
        }

        private void BtnBeginTests_Click(object sender, EventArgs e)
        {
            SetBtnBeginEnabled(false);
            TextBoxResults.Clear();

            var t = Task.Run( () => Sequencer(cts), cts.Token);


            //if (t.Result == Failure)
            //{
            //    AddToResults("\nProcedure Aborted.");
            //    // return;
            //}
            //else
            //{
            //    AddToResults("\nProcedure Succeeded.");
            //}
            //    BtnBeginEnabled(true);


            if (GetBtnBeginEnabled() == false)
                SetBtnBeginEnabled(true);

        }

        private void SetBtnBeginEnabled(bool v)
        {
            if (this.BtnBeginTests.InvokeRequired)
            {
                BoolArgReturningVoidDelegate d = new BoolArgReturningVoidDelegate(SetBtnBeginEnabled);
                this.Invoke(d, new object[] { v });
            }
            else
            {
                this.BtnBeginTests.Enabled = v;
            }
        }

        private bool GetBtnBeginEnabled()
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

        private void AddToResults(string item)
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

        private void SetHead1Text(string text)
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

        private void SetHead2Text(string text)
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

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e) => this.Close();

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
        private void BtnCancelTests_Click(object sender, EventArgs e)
        {
            cts.Cancel();
            MessageBox.Show("canceled! (mainform mbox)");
            BtnBeginTests.Enabled = true;
        }

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
