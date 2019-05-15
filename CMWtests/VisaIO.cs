using System;
using System.Windows.Forms;
using System.Text;
using RsVisaLoader;

namespace CMWtests
{
    public class VisaIO
    {
        private static int vi = 0;
        private static int defRM = 0;

        public VisaIO(int resMgr, string viDesc)
        {
            defRM = resMgr;
            visa32.viOpen(defRM, viDesc, visa32.VI_NO_LOCK, visa32.VI_TMO_IMMEDIATE, out vi);
        }

        public ViStatus Read(out string response, bool readSTB = false)
        {
            StringBuilder viResponse = new StringBuilder(256);
            ViStatus stat = visa32.viRead(vi, viResponse, 256, out int retCnt);
            //response = viResponse.ToString().Truncate(retCnt > 0 ? retCnt - 1 : 0);
            response = viResponse.ToString().TrimEnd('\n');

            if (stat < ViStatus.VI_SUCCESS) ShowErrorText("VisaIO.Read", stat);
            return stat;
        }

        public ViStatus Write(string message)
        {
            ViStatus stat = visa32.viWrite(vi, message, message.Length, out int viRetCount);

            if (stat < ViStatus.VI_SUCCESS) ShowErrorText("VisaIO.Write", stat);
            return stat;
        }

        private void ShowErrorText(string source, ViStatus status)
        {
            StringBuilder text = new StringBuilder(visa32.VI_FIND_BUFLEN);
            ViStatus err = visa32.viStatusDesc(vi, status, text);
            MessageBox.Show(Environment.NewLine + source + Environment.NewLine + text.ToString());
        }

        public void WriteSTB(string command, int timeout)
        {
            QueryInteger("*ESR?"); //Clear the Event Status Register
            Write(command + ";*OPC");
            //string exMessage = String.Format("WriteWithSTBpollSync - Timeout occured. Command: \"{0}\", timeout {1} ms", command, timeout);
            //_STBpolling(exMessage, timeout);
            QueryInteger("*ESR?"); //Clear the Event Status Register

            //    string errResp = session.QueryString("SYST:ERR?").TrimEnd();
            //    if (Convert.ToInt64(errResp.Split(',')[0]) != 0)
            //        textBoxResponse.AppendText(errResp + Environment.NewLine);
        }

        public string QuerySTB(string query, int timeout)
        {
            QueryInteger("*ESR?"); //Clear the Event Status Register
            Write(query + ";*OPC");
            //string exMessage = String.Format("QueryWithSTBpollSync - Timeout occured. Query: \"{0}\", timeout {1} ms", query, timeout);
            //_STBpolling(exMessage, timeout);
            var response = ReadString();
            response = response.TrimEnd('\n');
            QueryInteger("*ESR?"); //Clear the Event Status Register
            return response;

            //session.RawIO.Write("SYST:ERR?\n");
            //string errResp = session.RawIO.ReadString().TrimEnd();
            //if (Convert.ToInt64(errResp.Split(',')[0]) != 0)
            //    textBoxResponse.AppendText(errResp + Environment.NewLine);

        }

        public int QueryInteger(string query)
        {
            return Int32.Parse(QueryString(query));
        }

        public string QueryString(string query)
        {
            Write(query);
            Read(out string response);
            response = response.TrimEnd('\n');
            return response;
        }

        public string ReadString()
        {
            Read(out string response);
            response = response.TrimEnd('\n');
            return response;
        }

        public static void OpenResourceMgr(out int defRM)
        {
            defRM = 0;

            if (IsVisaLibraryInstalled(RsVisa.RSVISA_MANFID_DEFAULT))
            {
                if (IsVisaLibraryInstalled(RsVisa.RSVISA_MANFID_RS))
                    RsVisa.RsViSetDefaultLibrary(RsVisa.RSVISA_MANFID_RS);
                else if (IsVisaLibraryInstalled(RsVisa.RSVISA_MANFID_NI))
                    RsVisa.RsViSetDefaultLibrary(RsVisa.RSVISA_MANFID_NI);
                else if (IsVisaLibraryInstalled(RsVisa.RSVISA_MANFID_AG))
                    RsVisa.RsViSetDefaultLibrary(RsVisa.RSVISA_MANFID_AG);
                else
                    RsVisa.RsViSetDefaultLibrary(RsVisa.RSVISA_MANFID_DEFAULT);

                visa32.viOpenDefaultRM(out defRM);
                visa32.viSetAttribute(defRM, ViAttr.VI_RS_ATTR_TCPIP_FIND_RSRC_TMO, 0x3E8);
                visa32.viSetAttribute(defRM, ViAttr.VI_RS_ATTR_TCPIP_FIND_RSRC_MODE, 0x3);
            }
            else
            {
                MessageBox.Show("No VISAs Installed!");
            }
        }

        private static bool IsVisaLibraryInstalled(UInt16 iManfId)
        {
            return RsVisa.RsViIsVisaLibraryInstalled(iManfId) != 0;
        }
    }
}
