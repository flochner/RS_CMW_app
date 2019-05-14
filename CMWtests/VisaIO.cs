using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RsVisaLoader;

namespace CMWtests
{
    public static class VisaIO
    {
        public void WriteSTB(string command, int timeout)
        {
            try
            {
                session.WriteWithSTBpollSync(command, timeout);
            }
            catch (InstrumentErrorException e)
            {
                MessageBox.Show(e.Message, e.GetType().ToString());
            }
            catch (InstrumentOPCtimeoutException e)
            {
                MessageBox.Show(e.Message, e.GetType().ToString());
            }
            catch (Ivi.Visa.VisaException e)
            {
                MessageBox.Show(e.Message, e.GetType().ToString());
            }
            finally
            {
                string errResp = session.QueryString("SYST:ERR?").TrimEnd();
                if (Convert.ToInt64(errResp.Split(',')[0]) != 0)
                    textBoxResponse.AppendText(errResp + Environment.NewLine);
            }
        }

        public void QuerySTB(string query, int timeout, out string response)
        {
            response = null;
            try
            {
                response = session.QueryWithSTBpollSync(query, timeout);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, e.GetType().ToString());
            }

            //session.RawIO.Write("SYST:ERR?\n");
            //string errResp = session.RawIO.ReadString().TrimEnd();
            //if (Convert.ToInt64(errResp.Split(',')[0]) != 0)
            //    textBoxResponse.AppendText(errResp + Environment.NewLine);
        }

        public static ViStatus Read(int vi, out string response)
        {
            int retCnt = 0;
            StringBuilder viResponse = new StringBuilder(256);
            ViStatus stat = visa32.viRead(vi, viResponse, 256, out retCnt);
            response = viResponse.ToString().Truncate(retCnt > 0 ? retCnt - 1 : 0);

            return stat;
        }

        public static ViStatus Write(int vi, string message)
        {
            int viRetCount = 0;
            ViStatus stat = visa32.viWrite(vi, message, message.Length, out viRetCount);

            return stat;
        }
    }
}
