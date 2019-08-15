using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CMWtests
{
    public partial class TempGauge : UserControl
    {
        private CancellationTokenSource cts = null;
        private double cmwTempC = 0.0;
        private VisaIO cmw;

        public TempGauge()
        {
            InitializeComponent();
        }

        public bool Start(VisaIO instr)
        {
            cmw = instr;
            cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            Task.Factory.StartNew(Run, token);

            while (cmwTempC == 0.0)
            {
                if (MainForm.CancelTesting == true)
                    return false;
                Thread.Sleep(100);
            }

            while (cmwTempC < 40.0)
            {
                if (MainForm.CancelTesting == true)
                    return false;

                Thread.Sleep(1750);
                Invoke(new MethodInvoker(() =>
                {
                    labelTemp.Text = "";
                }));
                Thread.Sleep(250);
            }
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
#if !DEBUG
                Thread.Sleep(5000);
#else
                Thread.Sleep(900);
                labelTemp.ForeColor = System.Drawing.Color.Red;
                Thread.Sleep(100);
                labelTemp.ForeColor = System.Drawing.Color.Black;
#endif
                if (cts.IsCancellationRequested)
                    return;

                try
                {
                    cmwTempC = ReadTemp(cmw);
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

                Invoke(new MethodInvoker(() =>
                {
                    pictureBoxSlider.Left = sliderPos;
                    pictureBoxSlider.Visible = true;

                    labelTemp.Text = string.Format("{0:F1}", cmwTempC);
                    labelTemp.Left = sliderPos - (labelTemp.Size.Width / 2);
                    labelTemp.Visible = true;

                    this.Refresh();
                }));
            }
        }

        public void Stop()
        {
            Invoke(new MethodInvoker(() =>
            {
                pictureBoxSlider.Visible = false;
                labelTemp.Visible = false;
                this.Enabled = false;
                this.Refresh();
            }));

            cts.Cancel();
        }

        private double ReadTemp(VisaIO instr)
        {
            cmw.Lock();
            var visaResponse = instr.QueryString("SENSe:BASE:TEMPerature:OPERating:INTernal?");
            cmw.Unlock();
            var temp = Convert.ToDouble(visaResponse);
            return temp;
        }
    }
}