using System;
using System.Windows.Forms;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using RsVisaLoader;

namespace CMWtests
{
    [Flags]
    public enum StatusByteFlags : short
    {
        None = 0,
        MessageAvailable = 16,
        RequestingService = 32,
        EventStatusRegister = MessageAvailable | RequestingService
    }

    public class VisaIO
    {
        private static int vi = 0;

        public VisaIO(string viDesc)
        {
            visa32.viOpen(MainForm.DefResMgr, viDesc, visa32.VI_NO_LOCK, visa32.VI_TMO_IMMEDIATE, out vi);
        }

        public ViStatus Close()
        {
            ViStatus status = visa32.viClose(MainForm.DefResMgr);
            RsVisa.RsViUnloadVisaLibrary();
            return status;
        }

        public ViStatus Read(out string response, bool readSTB = false)
        {
            StringBuilder viResponse = new StringBuilder(1024);
            ViStatus stat = visa32.viRead(vi, viResponse, 1024, out int retCnt);
            response = viResponse.ToString().Truncate(retCnt > 0 ? retCnt : 0);

            if (stat < ViStatus.VI_SUCCESS)
                ShowErrorText("VisaIO.Read", stat);
            return stat;
        }

        public ViStatus Write(string message, bool waitForOPC = false)
        {
            ViStatus stat = visa32.viWrite(vi, message, message.Length, out int viRetCount);
            if (waitForOPC == true)
                WaitForOPC();

            if (stat < ViStatus.VI_SUCCESS)
                ShowErrorText("VisaIO.Write", stat);
            return stat;
        }

        public int QueryInteger(string query)
        {
            string response = QueryString(query);
            int.TryParse(response, out int result);
            return result;
        }

        public string QueryString(string query)
        {
            Write(query);
            Read(out string response);
            return response.TrimEnd('\n');
        }

        public string ReadString()
        {
            Read(out string response);
            return response.TrimEnd('\n');
        }

        public void WriteWithSTB(string command, int timeout)
        {
            QueryInteger("*ESR?"); //Clear the Event Status Register
            Write(command + ";*OPC");
            string exMessage = String.Format("WriteWithSTB - Timeout occured. Command: \"{0}\", timeout {1} ms", command, timeout);
            _STBpolling(exMessage, timeout);
            QueryInteger("*ESR?"); //Clear the Event Status Register
        }

        public string QueryWithSTB(string query, int timeout)
        {
            QueryInteger("*ESR?"); //Clear the Event Status Register
            Write(query + ";*OPC");
            string exMessage = String.Format("QueryWithSTB - Timeout occured. Query: \"{0}\", timeout {1} ms", query, timeout);
            _STBpolling(exMessage, timeout);
            var response = ReadString();
            response = response.TrimEnd('\n');
            QueryInteger("*ESR?"); //Clear the Event Status Register
            return response;
        }

        public void Reset()
        {
            Write("*RST", true);
            ClearStatus();
            Write("*ESE 1", true);
        }

        public void ClearStatus()
        {
            QueryString("*CLS;*OPC?");
            ErrorChecking();
        }

        public void ErrorChecking()
        {
            var errors = ReadErrorQueue();
            if (errors.Count > 0)
            {
                string combindedString = string.Join("\n", errors);
                throw new InstrumentErrorException(combindedString);
            }
        }

        public List<string> ReadErrorQueue()
        {
            var errors = new List<string>();

            if (((QueryInteger("*STB?") >> 4) & 1) > 0)
            {
                while (true)
                {
                    var response = QueryString("SYST:ERR?");
                    if (response.ToLower().Contains("\"no error\""))
                    {
                        break;
                    }
                    errors.Add(response);

                    // safety stop
                    if (errors.Count > 50)
                    {
                        errors.Add("Cannot clear the error queue");
                        break;
                    }
                }
            }
            return errors;
        }

        private void WaitForOPC()
        {
            QueryString("*OPC?");
        }

        private void ShowErrorText(string source, ViStatus status)
        {
            StringBuilder text = new StringBuilder(visa32.VI_FIND_BUFLEN);
            ViStatus err = visa32.viStatusDesc(vi, status, text);
            MessageBox.Show(status.ToString() + Environment.NewLine + text.ToString(), source);
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

        private void _STBpolling(string exMessage, int timeout)
        {
            var start = DateTime.Now;
            var stop = start.AddMilliseconds(timeout);
            var sleep10ms = start.AddMilliseconds(10);
            var sleep100ms = DateTime.Now.AddMilliseconds(100);
            var sleep1000ms = DateTime.Now.AddMilliseconds(1000);
            var sleep5000ms = DateTime.Now.AddMilliseconds(5000);
            var sleep10000ms = DateTime.Now.AddMilliseconds(10000);
            var sleep30000ms = DateTime.Now.AddMilliseconds(30000);

            // STB polling loop
            while (true)
            {
                var stb = ReadStatusByte();
                if (stb.HasFlag(StatusByteFlags.EventStatusRegister))
                    break;

                if (DateTime.Now > stop)
                    throw new InstrumentOPCtimeoutException(exMessage);

                if (DateTime.Now < sleep10ms) { } //Full speed
                else if (DateTime.Now < sleep100ms) Thread.Sleep(1);
                else if (DateTime.Now < sleep1000ms) Thread.Sleep(10);
                else if (DateTime.Now < sleep5000ms) Thread.Sleep(50);
                else if (DateTime.Now < sleep10000ms) Thread.Sleep(100);
                else if (DateTime.Now < sleep30000ms) Thread.Sleep(500);
                else Thread.Sleep(1000);
            }
        }

        private StatusByteFlags ReadStatusByte()
        {
            visa32.viReadSTB(vi, out short stb);

            StatusByteFlags statusByte = (StatusByteFlags)stb;

            if (statusByte == StatusByteFlags.MessageAvailable || statusByte == StatusByteFlags.RequestingService)
                return StatusByteFlags.EventStatusRegister;
            else
                return statusByte;

            //string bits = "";
            //for (int i = 7; i >= 0; i--)
            //{
            //    bits += ((stb >> i) & 1);
            //    if (bits.Length == 4)
            //        bits += " ";
            //}
            //MessageBox.Show(stb + "\n" + bits, "STB Status");

            //if (((stb >> 4) & 1) > 0)
            //{
            //    var response = QueryString("?");
            //    MessageBox.Show(response, "?");
            //}

            //if (((stb >> 5) & 1) > 0)
        }
    }

    public class InstrumentOPCtimeoutException : Exception
    {
        /// <summary>
        /// Instrument OPC Timeout Exception
        /// </summary>
        public InstrumentOPCtimeoutException(string message) : base(message)
        {
        }
    }

    public class InstrumentErrorException : Exception
    {
        /// <summary>
        /// Instrument Error Exception
        /// </summary>
        public InstrumentErrorException(string errors) : base(errors)
        {
        }
    }
}



