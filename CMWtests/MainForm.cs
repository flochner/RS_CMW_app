using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CMWtests
{
    public partial class MainForm : Form
    {
        delegate void StringDelegate(string text);
        delegate void BoolDelegate(bool v);
        private CancellationTokenSource _cts = null;
        public bool IsExitRequested { get; private set; } = false;
        public bool PauseTesting { get; private set; } = false;

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnBeginTests_Click(object sender, EventArgs e)
        {
            SetBtnBeginEnabled(false);
            textBoxResults.Clear();
            PauseTesting = false;

            _cts = null ?? new CancellationTokenSource();
            Tests tests = null ?? new Tests(this, _cts);

            var seq = Task.Run(() => tests.Begin());
        }

        public void SetBtnBeginEnabled(bool v)
        {
            if (this.btnBeginTests.InvokeRequired)
            {
                BoolDelegate d = new BoolDelegate(SetBtnBeginEnabled);
                this.BeginInvoke(d, new object[] { v });
            }
            else
            {
                this.btnBeginTests.Enabled = v;
                this.Refresh();
            }
        }

        public bool GetBtnCancelEnabled()
        {
            return btnCancelTests.Enabled;
        }

        public void SetBtnCancelEnabled(bool v)
        {
            if (this.btnCancelTests.InvokeRequired)
            {
                BoolDelegate d = new BoolDelegate(SetBtnCancelEnabled);
                this.BeginInvoke(d, new object[] { v });
            }
            else
            {
                this.btnCancelTests.Enabled = v;
                this.Refresh();
            }
        }

        public void AddToResults(string item)
        {
            if (this.textBoxResults.InvokeRequired)
            {
                StringDelegate d = new StringDelegate(AddToResults);
                this.BeginInvoke(d, new object[] { item });
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
                StringDelegate d = new StringDelegate(SetHead1Text);
                this.BeginInvoke(d, new object[] { text });
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
                StringDelegate d = new StringDelegate(SetHead2Text);
                this.BeginInvoke(d, new object[] { text });
            }
            else
            {
                this.labelHead2.Text = text;
            }
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
            btnCancelTests_Click(sender, e);
        }

        public void AppExit()
        {
            Application.Exit();
        }

        private void btnCancelTests_Click(object sender, EventArgs e)
        {
            if (_cts == null && IsExitRequested == true)
                AppExit();

            PauseTesting = true;
            var abort = MessageBox.Show("Really abort testing?",
                                    "Warning",
                                     MessageBoxButtons.YesNo,
                                     MessageBoxIcon.Warning,
                                     MessageBoxDefaultButton.Button2);
            if (abort == DialogResult.Yes)
                try { _cts.Cancel(); _cts = null; }
                catch (NullReferenceException) { }
                catch (ObjectDisposedException) { }
                catch (Exception exc) { MessageBox.Show(exc.Message, exc.GetType().ToString()); }
            else
                PauseTesting = false;
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
