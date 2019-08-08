using System;
using System.Threading;
using System.Windows.Forms;

namespace CMWtests
{
    partial class MainForm
    {
        private AutoResetEvent areTempGauge = null;
        private double cmwTempC = 0.0;

        private void TempGauge()
        {
            areTempGauge = new AutoResetEvent(true);
            
            Invoke(new MethodInvoker(() =>
            {
                tempSlider.Visible = true;
                labelTemp.Visible = true;
                panel1.Enabled = true;
                panel1.Invalidate();
            }));

            while (cmw != null && areTempGauge != null)
            {
                areTempGauge.WaitOne();
                cmwTempC = ReadTemp(cmw);
                var sliderPos = Convert.ToInt16(((cmwTempC - 25) * 4D) + 12);

                Invoke(new MethodInvoker(() =>
                {
                    tempSlider.Left = sliderPos;
                    tempSlider.Invalidate();

                    labelTemp.Text = string.Format("{0:F1}", cmwTempC);
                    labelTemp.Left = sliderPos - (labelTemp.Size.Width / 2);
                    labelTemp.Invalidate();
                }));
            }

            Invoke(new MethodInvoker(() =>
            {
                tempSlider.Visible = false;
                labelTemp.Visible = false;
                panel1.Enabled = false;
                panel1.Invalidate();
            }));
            Status = TestStatus.Abort;
        }

        private double ReadTemp(VisaIO instr)
        {
            var visaResponse = instr.QueryString("SENSe:BASE:TEMPerature:OPERating:INTernal?");
            var temp = Convert.ToDouble(visaResponse);
            return temp;
        }
    }
}
