using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RsVisaLoader;
using System.Windows.Forms;

namespace CMWtests
{
    public class ViSession
    {
        public int ResourceMgr { get => _defRM; private set { } }

        private int _defRM = visa32.VI_NULL;

        public ViSession()
        {
            OpenResMgr();
        }

        private int OpenResMgr()
        {
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

                visa32.viOpenDefaultRM(out _defRM);
                return _defRM;
            }
            else
            {
                MessageBox.Show("No VISAs Installed!");
                return -1;
            }
        }

        public void CloseResMgr()
        {
            visa32.viClose(_defRM);
            RsVisa.RsViUnloadVisaLibrary();
        }

        public ViStatus OpenSession(string resource, out int vi)
        {
            return visa32.viOpen(_defRM, resource, 0, 0, out vi);
        }

        public ViStatus CloseSession(int vi)
        {
            return visa32.viClose(vi);
        }

        public ViStatus Query(int vi, string sQuery, out string sAnswer)
        {
            sAnswer = "";
            ViStatus status = Write(vi, sQuery);
            if (status < ViStatus.VI_SUCCESS) return status;
            return Read(vi, out sAnswer);
        }

        public ViStatus Write(int vi, string buffer)
        {
            int retCount;
            return visa32.viWrite(vi, buffer+"\n", buffer.Length, out retCount);
        }

        public ViStatus Read(int vi, out string buffer)
        {
            ViStatus status;
            buffer = string.Empty;
            StringBuilder sTemp = new StringBuilder(1024);
            do
            {
                int retCount;
                status = visa32.viRead(vi, sTemp, sTemp.Capacity, out retCount);
                if (retCount > 0)
                {
                    buffer += sTemp.ToString(0, retCount);
                }
            } while (status == ViStatus.VI_SUCCESS_MAX_CNT);

            return status;
        }

        private static bool IsVisaLibraryInstalled(UInt16 iManfId)
        {
            return RsVisa.RsViIsVisaLibraryInstalled(iManfId) != 0;
        }

        private void ShowErrorText(string source, ViStatus status)
        {
            StringBuilder text = new StringBuilder(visa32.VI_FIND_BUFLEN);
            ViStatus err = visa32.viStatusDesc(_defRM, status, text);
            MessageBox.Show(Environment.NewLine + source + Environment.NewLine + text.ToString());
        }

        private ViStatus EventHandler(int vi, ViEventType inEventType, int inContext, int inUserHandle)
        {
            short stb;
            ViStatus status = visa32.viReadSTB(vi, out stb);
            string text = "EventHandler: Event " + inEventType.ToString() +
                          " occurred with STB = 0x" + stb.ToString("X");
            MessageBox.Show(text);
            return status;
        }

        private void TestSRQ(string item)
        {
        //    visa32.ViEventHandler handler = EventHandler;
        //    ViEventType outEventType;
        //    int outEventContext;
        //    int sesn;
        //    const string sIDN = "*IDN?\n";
        //    string sAnswer;
        //    ViStatus status;

        //    txtResult.Text = String.Empty;
        //    Update();

        //    status = visa32.viOpen(m_defRM, item, 0, 0, out sesn);
        //    if (status < ViStatus.VI_SUCCESS)
        //    {
        //        ShowErrorText(status);
        //        return;
        //    }

        //    // This function is not needed, here:
        //    // But use this function, if you think there are some SRQs left within the internal event queue.
        //    status = visa32.viDiscardEvents(sesn, ViEventType.VI_EVENT_SERVICE_REQ, visa32.VI_ALL_MECH);
        //    if (status < ViStatus.VI_SUCCESS) goto error;

        //    // Enable raising an SRQ when the MAV bit (message available) is set.
        //    status = Write(sesn, "*CLS;*SRE #H10\n");
        //    if (status < ViStatus.VI_SUCCESS) goto error;

        //    //Get SRQ by queuing mechanism:
        //    status = visa32.viEnableEvent(sesn, ViEventType.VI_EVENT_SERVICE_REQ, visa32.VI_QUEUE, visa32.VI_NULL);
        //    if (status < ViStatus.VI_SUCCESS) goto error;

        //    status = Write(sesn, sIDN);
        //    if (status < ViStatus.VI_SUCCESS) goto error;

        //    // Wait for the SRQ ...
        //    status = visa32.viWaitOnEvent(sesn, ViEventType.VI_EVENT_SERVICE_REQ, 2000, out outEventType, out outEventContext);

        //    if (status == ViStatus.VI_SUCCESS)
        //    {
        //        short stb;
        //        status = visa32.viReadSTB(sesn, out stb);
        //        if (status < ViStatus.VI_SUCCESS) goto error;
        //        status = visa32.viClose(outEventContext);
        //        if (status < ViStatus.VI_SUCCESS) goto error;
        //        txtResult.Text += "Event " + outEventType.ToString() + " occurred with STB = 0x" + stb.ToString("X");
        //    }
        //    else
        //    {
        //        txtResult.Text += "viWaitOnEvent() failed with: " + status.ToString();
        //        goto error;
        //    }

        //    status = Read(sesn, out sAnswer);
        //    if (status < ViStatus.VI_SUCCESS) goto error;

        //    status = visa32.viDisableEvent(sesn, ViEventType.VI_EVENT_SERVICE_REQ, visa32.VI_QUEUE);
        //    if (status < ViStatus.VI_SUCCESS) goto error;

        //    // Enable raising an SRQ when the Error bit is set.
        //    status = Write(sesn, "*CLS;*SRE #H04\n");
        //    if (status < ViStatus.VI_SUCCESS) goto error;

        //    int userHandle = 1234;
        //    //Get SRQ by callback:
        //    //First install handlers and then enable events.
        //    status = visa32.viInstallHandler(sesn, ViEventType.VI_EVENT_SERVICE_REQ, handler, userHandle);
        //    if (status < ViStatus.VI_SUCCESS) goto error;

        //    status = visa32.viEnableEvent(sesn, ViEventType.VI_EVENT_SERVICE_REQ, visa32.VI_HNDLR, visa32.VI_NULL);
        //    if (status < ViStatus.VI_SUCCESS) goto error;

        //    // Write nonsense to cause an error.
        //    status = Write(sesn, "bla1\x00bla0\n");
        //    if (status < ViStatus.VI_SUCCESS) goto error;

        //    Thread.Sleep(500); // Need some time to raise the SRQ ...

        //    status = visa32.viDisableEvent(sesn, ViEventType.VI_EVENT_SERVICE_REQ, visa32.VI_ALL_MECH);
        //    if (status < ViStatus.VI_SUCCESS) goto error;

        //    status = visa32.viUninstallHandler(sesn, ViEventType.VI_EVENT_SERVICE_REQ, handler, userHandle);
        //    if (status < ViStatus.VI_SUCCESS) goto error;

        //    status = Query(sesn, "*SRE 0;SYST:ERR?\n", out sAnswer);
        //    if (status < ViStatus.VI_SUCCESS) goto error;

        //    visa32.viClose(sesn);
        //    return;

        //error:
        //    visa32.viClose(sesn);
        //    ShowErrorText(status);
        //    return;
        }
    }
}
