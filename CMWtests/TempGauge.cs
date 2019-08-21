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
        private bool runComplete = false;
        private double cmwTempC = 0.0;
        private string elapsedTime = "";
        private string csvFileName = "";
        private CancellationTokenSource cts = null;
        private MainForm mainForm = null;
        private Stopwatch stopwatch = null;
        private StreamWriter csvStream = null;
        private TimeSpan ts = TimeSpan.Zero;
        private VisaIO cmw;

        public TempGauge(MainForm obj)
        {
            InitializeComponent();
            mainForm = obj;
        }

        public bool Start(VisaIO instr)
        {
            cmw = instr;
            cts = new CancellationTokenSource();

            csvStream = mainForm.OpenTempFile(out csvFileName);

            stopwatch = Stopwatch.StartNew();

            Task.Factory.StartNew(Run, cts.Token);

            do
            {
                if (MainForm.CancelTesting == true)
                    return false;
                Thread.Sleep(100);
            }
            while (cmwTempC == 0.0);

            int i = 0;
            while (cmwTempC < 45.0 && OptionsForm.TempOverride == false)
            {
                if (MainForm.CancelTesting == true)
                    return false;

                Thread.Sleep(1650);
                //Invoke(new MethodInvoker(() =>
                //{
                //    labelTemp.Text = "";
                //}));
                Thread.Sleep(350);
                i++;
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
                this.Visible = true;
                this.Enabled = true;
                this.Refresh();
            }));

            while (cmw != null)
            {
                labelTemp.ForeColor = System.Drawing.Color.Red;
                Thread.Sleep(100);

                try
                {
                    cmwTempC = ReadTemp(cmw);
#if DEBUG
                    RecordTemp();
#endif
                }
                catch (Exception e)
                {
                    MessageBox.Show("Broken at TempGauge read" +
                                    Environment.NewLine + e.Source + ":" +
                                    Environment.NewLine + e.Message + Environment.NewLine +
                                    Environment.NewLine + e.StackTrace,
                                    e.Source);
                    Stop();
                    return;
                }

                var sliderPos = Convert.ToInt16(((cmwTempC - 25) * 4D) + 12);

                BeginInvoke(new MethodInvoker(() =>
                {
                    pictureBoxSlider.Left = sliderPos;
                    pictureBoxSlider.Visible = true;

                    labelTemp.Text = string.Format("{0:F1}", cmwTempC);
                    labelTemp.Left = sliderPos - (labelTemp.Size.Width / 2);
                    labelTemp.Visible = true;
                    
                    this.Refresh();

                }));

                labelTemp.ForeColor = System.Drawing.Color.Black;
                for (int i = 0; i < 10; i++)
                {
                    if (cts.IsCancellationRequested)
                    {
                        runComplete = true;
                        return;
                    }
                    Thread.Sleep(10);
                }
            }
        }

        public void Stop()
        {
            cts.Cancel();

            while (runComplete == false)
                Thread.Sleep(100);

            Invoke(new MethodInvoker(() =>
            {
                pictureBoxSlider.Visible = false;
                labelTemp.Visible = false;
                this.Enabled = false;
                this.Refresh();
            }));
            
            try
            {
                csvStream.Dispose();
            }
            catch (Exception e)
            {
                mainForm.ModalMessageBox(e.Source + Environment.NewLine + e.Message, 
                                        "Dispose TempGauge csvStream Exception");
            }

            //Process proc = new Process();
            //proc.StartInfo.FileName = "C:\\Program Files\\Sublime Text 3\\subl.exe";
            //proc.StartInfo.Arguments = csvFileName;
            //proc.Start();

            //if (File.Exists(csvFileName))
            //    try
            //    {
            //        File.Delete(csvFileName);
            //    }
            //    catch
            //    {
            //        mainForm.ModalMessageBox("Temp file delete Exception");
            //    }
            }

        private double ReadTemp(VisaIO instr)
        {
            cmw.Lock();
            var visaResponse = instr.QueryString("SENSe:BASE:TEMPerature:OPERating:INTernal?");
            cmw.Unlock();
            return Convert.ToDouble(visaResponse);
        }

        private void RecordTemp()
        {
            ts = stopwatch.Elapsed;
            elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);

            try
            {
                csvStream.WriteLine(elapsedTime + "," + cmwTempC.ToString());
            }
            catch (Exception e)
            {
                mainForm.ModalMessageBox("RecordTemp():\n" + e.Message, e.GetType().ToString());
            }
        }

        private void contextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (OptionsForm.TempOverride == true)
                overrideWarmUpToolStripMenuItem.CheckState = CheckState.Checked;
            else
                overrideWarmUpToolStripMenuItem.CheckState = CheckState.Unchecked;
        }

        private void overrideWarmUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OptionsForm.TempOverride = (overrideWarmUpToolStripMenuItem.CheckState == CheckState.Checked);
        }
    }
}