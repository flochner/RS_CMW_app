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
        public const bool SUCCESS = true;
        public MainForm()
        {
            InitializeComponent();
        }

        private Thread task = null;
        delegate void StringArgReturningVoidDelegate(string text);
        delegate void BoolArgReturningVoidDelegate(bool v);
        private static ManualResetEvent mre = new ManualResetEvent(false);

        private void BtnBeginTests_Click(object sender, EventArgs e)
        {
            BtnBeginEnabled(false);
            TextBoxResults.Clear();

            this.task = new Thread(new ThreadStart(this.SequencerAsync));
            this.task.Start();

            //if (GetBtnBeginEnabled() == false)
            //    SetBtnBeginEnabled(true);

            if (abort)
            {
                AddToResults("\nProcedure Aborted.");
                BtnBeginEnabled(true);
                return;
            }
        }

        private void BtnBeginEnabled(bool v)
        {
            if (this.BtnBeginTests.InvokeRequired)
            {
                BoolArgReturningVoidDelegate d = new BoolArgReturningVoidDelegate(BtnBeginEnabled);
                this.Invoke(d, new object[] { v });
            }
            else
            {
                this.BtnBeginTests.Enabled = v;
            }
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
            string visaResponse = "PA,PC";
            try
            {
                double pmPower = Convert.ToDouble(visaResponse.Split(',')[2]);
            }
            catch (FormatException f)
            {
                MessageBox.Show(f.Message, f.GetType().ToString());
            }
            catch (IndexOutOfRangeException g)
            {
                MessageBox.Show(g.Message, g.GetType().ToString());
            }
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
            MessageBox.Show("canceled!");
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
