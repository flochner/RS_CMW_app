using System;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace CMWtests
{
    partial class MainForm
    {
        private System.Timers.Timer aTimer = null;
        public bool TimerEnabled
        {
            get
            {
                try { return aTimer.Enabled; }
                catch (Exception) { throw; }
            }
            set
            {
                try { aTimer.Enabled = value; }
                catch (Exception) { throw; }
                BeginInvoke(new MethodInvoker(() =>
                {
                    tempSlider.Visible = value;
                    labelTemp.Visible = value;
                    panel1.Enabled = value;
                }));
            }
        }
        
        private double ReadTemp(VisaIO instr)
        {
            mreMeasure.Reset();
            while (mreMeasure.WaitOne(0) == true)
                Thread.Sleep(100);

            var visaResponse = "25";// instr.QueryString("SENSe:BASE:TEMPerature:OPERating:INTernal?");
            mreMeasure.Set();

            var temp = Convert.ToDouble(visaResponse);
            return temp;
        }

        private void SetTimer()
        {
            aTimer = new System.Timers.Timer(2000);
            
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;

            Invoke(new MethodInvoker(() =>
            {
                panel1.Enabled = true;
                panel1.Invalidate();
            }));
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            double temp = 0.0;

            Invoke(new MethodInvoker(() =>
            {
                try
                {
                    temp = ReadTemp(CMW);
                }
                catch (Exception)
                {
                    aTimer.Dispose();
                    panel1.Enabled = false;
                    tempSlider.Visible = false;
                    labelTemp.Visible = false;
                    return;
                }

                var sliderPos = Convert.ToInt16(((temp - 25) * 4D) + 12);

                tempSlider.Visible = true;
                tempSlider.Left = sliderPos;
                tempSlider.Invalidate();

                labelTemp.Visible = true;
                labelTemp.Text = string.Format("{0:F1}", temp);
                labelTemp.Left = sliderPos - (labelTemp.Size.Width / 2);
                labelTemp.Invalidate();
            }));
        }
    }
}
