using System;
using System.Windows.Forms;
using System.Text;
using RsVisaLoader;

namespace CMWtests
{
    public static class VisaIO
    {
        public static void WriteSTB(string command, int timeout)
        {
            //try
            //{
            //    session.WriteWithSTBpollSync(command, timeout);
            //}
            //catch (InstrumentErrorException e)
            //{
            //    MessageBox.Show(e.Message, e.GetType().ToString());
            //}
            //catch (InstrumentOPCtimeoutException e)
            //{
            //    MessageBox.Show(e.Message, e.GetType().ToString());
            //}
            //catch (Ivi.Visa.VisaException e)
            //{
            //    MessageBox.Show(e.Message, e.GetType().ToString());
            //}
            //finally
            //{
            //    string errResp = session.QueryString("SYST:ERR?").TrimEnd();
            //    if (Convert.ToInt64(errResp.Split(',')[0]) != 0)
            //        textBoxResponse.AppendText(errResp + Environment.NewLine);
            //}
        }

        public static void QuerySTB(string query, int timeout, out string response)
        {
            response = null;
            //try
            //{
            //    response = session.QueryWithSTBpollSync(query, timeout);
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message, e.GetType().ToString());
            //}

            //session.RawIO.Write("SYST:ERR?\n");
            //string errResp = session.RawIO.ReadString().TrimEnd();
            //if (Convert.ToInt64(errResp.Split(',')[0]) != 0)
            //    textBoxResponse.AppendText(errResp + Environment.NewLine);
        }

        public static ViStatus Read(int defRM, int vi, out string response, bool readSTB = false)
        {
            StringBuilder viResponse = new StringBuilder(256);
            ViStatus stat = visa32.viRead(vi, viResponse, 256, out int retCnt);
            //response = viResponse.ToString().Truncate(retCnt > 0 ? retCnt - 1 : 0);
            response = viResponse.ToString().TrimEnd('\n');

            if (stat < ViStatus.VI_SUCCESS) ShowErrorText(defRM, "VisaIO.Read", stat);
            return stat;
        }

        public static ViStatus Write(int resourceMgr, int vi, string message)
        {
            ViStatus stat = visa32.viWrite(vi, message, message.Length, out int viRetCount);

            if (stat < ViStatus.VI_SUCCESS) ShowErrorText(resourceMgr, "VisaIO.Write", stat);
            return stat;
        }

        private static void ShowErrorText(int resourceMgr, string source, ViStatus status)
        {
            StringBuilder text = new StringBuilder(visa32.VI_FIND_BUFLEN);
            ViStatus err = visa32.viStatusDesc(resourceMgr, status, text);
            MessageBox.Show(Environment.NewLine + source + Environment.NewLine + text.ToString());
        }
    }
}
