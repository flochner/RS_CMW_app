using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CMWtests
{
    public partial class TempGauge : UserControl
    {
        private bool stopRecording;
        private double cmwTempC = 0.0;
        private string elapsedTime = "";
        private string csvFileName = "";
        private CancellationTokenSource cts = null;
        private MainForm mainForm = null;
        private Stopwatch stopwatch = null;
        private StreamWriter csvStream = null;
        private TimeSpan ts = TimeSpan.Zero;
        private VisaIO cmw;
        private Task task;


        public TempGauge(MainForm obj)
        {
            InitializeComponent();
            mainForm = obj;
        }

        public bool Start(VisaIO instr)
        {
            cmw = instr;
            cts = new CancellationTokenSource();

            Options.RecordTempEnabled = false;
            if (Options.RecordTemp == true)
            {
                csvStream = mainForm.OpenTempFile(out csvFileName);
                csvStream.AutoFlush = true;
            }

            stopwatch = Stopwatch.StartNew();

            task = Task.Factory.StartNew(Run, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            do
            {
                if (MainForm.CancelTesting == true)
                    return false;
                Thread.Sleep(100);
            }
            while (cmwTempC == 0.0);

            while (cmwTempC < 45.0 && Options.TempOverride == false)
            {
                if (MainForm.CancelTesting == true)
                    return false;

                Thread.Sleep(500);
            }

            stopwatch.Stop();
            ts = stopwatch.Elapsed;
            elapsedTime = string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
            mainForm.AddToResults("Warmup Time: " + elapsedTime);
            mainForm.AddToResults(csvFileName);

            return true;
        }

        private void Run()
        {
            Thread.CurrentThread.Name = "TempGauge";

            Invoke(new MethodInvoker(() =>
            {
                stopRecordingToolStripMenuItem.Enabled = true;
                this.Visible = true;
                this.Enabled = true;
                this.Refresh();
            }));

            while (cmw != null)
            {
                try
                {
                    cmwTempC = ReadTemp(cmw);
                    if (stopwatch.IsRunning && Options.RecordTemp == true)
                        RecordTemp();
                }
                catch (Exception e)
                {
                    mainForm.ModalMessageBox(
                        "Broken at TempGauge read" +
                         Environment.NewLine + e.Source + ":" +
                         Environment.NewLine + e.Message + Environment.NewLine +
                         Environment.NewLine + e.StackTrace,
                         e.Source);
                    Stop();
                    return;
                }

                var sliderPos = Convert.ToInt16(((cmwTempC - 25.0) * 4.0) + 12.0);

                BeginInvoke(new MethodInvoker(() =>
                {
                    pictureBoxSlider.Left = sliderPos;
                    pictureBoxSlider.Visible = true;

                    labelTemp.ForeColor = System.Drawing.Color.Red;
                    labelTemp.Visible = true;
                    labelTemp.Text = string.Format("{0:F1}", cmwTempC);
                    labelTemp.Left = sliderPos - (labelTemp.Size.Width / 2);
                    
                    this.Refresh();
                }));

                Thread.Sleep(100);  //1000
                labelTemp.ForeColor = System.Drawing.Color.Black;

                for (int i = 0; i < 9; i++)  //29
                {
                    if (cts.IsCancellationRequested)
                        return;
                    
                    Thread.Sleep(100);  //1000
                }
            }
        }

        public bool Stop()
        {
            if (cts == null || cts.IsCancellationRequested == true)
                return true;

            cts.Cancel();

            while (task.Status == TaskStatus.Running)
                Thread.Sleep(10);

            Invoke(new MethodInvoker(() =>
            {
                pictureBoxSlider.Visible = false;
                labelTemp.Visible = false;
                this.Enabled = false;
                this.Refresh();
            }));

            if (Options.RecordTemp == true)
                StopRecording();

            Options.RecordTempEnabled = true;
            cts.Dispose();

            //Process proc = new Process();
            //proc.StartInfo.FileName = "C:\\Program Files\\Sublime Text 3\\subl.exe";
            //proc.StartInfo.Arguments = newFullName;
            //proc.Start();

            return true; 
        }

        public bool StopRecording()
        {
            stopRecording = true;

            try
            {
                csvStream.Close();
                csvStream.Dispose();
            }
            catch (ObjectDisposedException) { }
            catch (Exception e)
            {
                mainForm.ModalMessageBox(
                    "Close TempGauge csvStream Exception" +
                     Environment.NewLine + e.Source + ":" +
                     Environment.NewLine + e.Message + Environment.NewLine +
                     Environment.NewLine + e.StackTrace,
                     e.Source);
                return false;
            }

            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var dataDir = Path.Combine(desktop, "Data");
            var newFullName = Path.Combine(dataDir, Path.GetFileName(csvFileName));

            if (Directory.Exists(dataDir) == false)
                Directory.CreateDirectory(dataDir);

            try
            {
                File.Move(csvFileName, newFullName);
            }
            catch (Exception e)
            {
                mainForm.ModalMessageBox(
                    "File move Exception" +
                     Environment.NewLine + e.Source + ":" +
                     Environment.NewLine + e.Message + Environment.NewLine +
                     Environment.NewLine + e.StackTrace,
                     e.Source);
                return false;
            }
            return true;
        }

        private double ReadTemp(VisaIO instr)
        {
            cmw.IoLock();
            var visaResponse = instr.QueryWithSTB("SENSe:BASE:TEMPerature:OPERating:INTernal?", 2000);
            cmw.IoUnlock();
            return Convert.ToDouble(visaResponse);
        }

        private void RecordTemp()
        {
            ts = stopwatch.Elapsed;
            elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);

            if (csvStream != null && stopRecording == false)
                try
                {
                    csvStream.WriteLine(elapsedTime + "," + cmwTempC.ToString());
                }
                catch (ObjectDisposedException) { }
                catch (Exception e)
                {
                    mainForm.ModalMessageBox("RecordTemp():\n" + e.Message, e.GetType().ToString());
                }
        }

        public int KillTask()
        {
            cts.Dispose();
            task.Dispose();
            return 0;
        }

        private void contextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Options.TempOverride == true)
                overrideWarmUpToolStripMenuItem.CheckState = CheckState.Checked;
            else
                overrideWarmUpToolStripMenuItem.CheckState = CheckState.Unchecked;

            if (Options.RecordTemp == true)
                stopRecordingToolStripMenuItem.Enabled = true;
            else
                stopRecordingToolStripMenuItem.Enabled = false;
        }

        private void overrideWarmUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Options.TempOverride = (overrideWarmUpToolStripMenuItem.CheckState == CheckState.Checked);
        }

        private void stopRecordingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Options.RecordTemp = false;
            stopRecordingToolStripMenuItem.Enabled = false;
            StopRecording();
        }
    }
}