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
        private int m_defRM = visa32.VI_NULL;

        public ViSession()
        {
        }

        private void OpenSession ()
        {
            bool bNetworkSearch = false;
            if (IsVisaLibraryInstalled(RsVisa.RSVISA_MANFID_DEFAULT))
            {
                if (IsVisaLibraryInstalled(RsVisa.RSVISA_MANFID_RS))
                {
                    RsVisa.RsViSetDefaultLibrary(RsVisa.RSVISA_MANFID_RS);
                    bNetworkSearch = true;
                }
                else if (IsVisaLibraryInstalled(RsVisa.RSVISA_MANFID_NI))
                {
                    RsVisa.RsViSetDefaultLibrary(RsVisa.RSVISA_MANFID_NI);
                }
                else if (IsVisaLibraryInstalled(RsVisa.RSVISA_MANFID_AG))
                {
                    RsVisa.RsViSetDefaultLibrary(RsVisa.RSVISA_MANFID_AG);
                }
                else
                {
                    RsVisa.RsViSetDefaultLibrary(RsVisa.RSVISA_MANFID_DEFAULT);
                }
                visa32.viOpenDefaultRM(out m_defRM);
            }
            else
            {
                MessageBox.Show("No VISAs Installed!");
                return;
            }

            // visa32.viClose(m_defRM);
            // RsVisa.RsViUnloadVisaLibrary();


        }


        private static bool IsVisaLibraryInstalled(UInt16 iManfId)
        {
            return RsVisa.RsViIsVisaLibraryInstalled(iManfId) != 0;
        }

        private delegate void DelegateMethod();
        private delegate void DelegateMethodString(string text);
        private delegate void DelegateMethodAddDevice(int iHostIndex, string text);



        private void FindLxiDevices()
        {
            //if (mDNScheckBox.Enabled)
            //{
            //    RsVisaLoader.RsAttr attrValue = RsAttr.VI_RS_FIND_MODE_CONFIG;

            //    if (mDNScheckBox.Checked)
            //    {
            //        attrValue |= RsAttr.VI_RS_FIND_MODE_MDNS;
            //    }

            //    if (VXI11checkBox.Checked)
            //    {
            //        attrValue |= RsAttr.VI_RS_FIND_MODE_VXI11;
            //    }

            //    visa32.viSetAttribute(m_defRM, ViAttr.VI_RS_ATTR_TCPIP_FIND_RSRC_MODE, (int)attrValue);
            //}

            //int vi = 0;
            //int retCount = 0;
            //StringBuilder desc = new StringBuilder(256);
            //visa32.viFindRsrc(m_defRM, "USB?*", out vi, out retCount, desc);
            //if (retCount > 0)
            //{
            //    listDevices.Items.Add(desc.ToString());
            //    for (int i = 0; i < retCount - 1; ++i)
            //    {
            //        visa32.viFindNext(vi, desc);
            //        listDevices.Items.Add(desc.ToString());
            //    }
            //}
            //listDevices.SelectedIndex = 0;
            //btnIDN.Enabled = btnSRQ.Enabled = (listDevices.Items.Count > 0);
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            //listDevices.Items.Clear();
            //btnFind.Enabled = false;
            //UseWaitCursor = true;

            //FindLxiDevices();

            //UseWaitCursor = false;
            //btnFind.Enabled = true;
        }

        private void ShowErrorText(ViStatus status)
        {
            //StringBuilder text = new StringBuilder(visa32.VI_FIND_BUFLEN);
            //ViStatus err = visa32.viStatusDesc(m_defRM, status, text);
            //txtResult.Text += Environment.NewLine + text.ToString();
        }

        private void ShowIDN(string item)
        {
            //int session;
            //txtResult.Text = String.Empty;
            //Update();
            //ViStatus status = visa32.viOpen(m_defRM, item, 0, 0, out session);
            //if (status < ViStatus.VI_SUCCESS)
            //{
            //    ShowErrorText(status);
            //    return;
            //}

            //string sAnswer;
            //status = Query(session, "*IDN?\n", out sAnswer);
            //visa32.viClose(session);

            //if (status < ViStatus.VI_SUCCESS)
            //{
            //    ShowErrorText(status);
            //}
            //else
            //{
            //    txtResult.Text = "Identification: " + sAnswer.ToString();
            //}
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnIDN_Click(object sender, EventArgs e)
        {
            //string curItem = listDevices.SelectedItem.ToString();
            //if (curItem != null)
            //{
            //    ShowIDN(curItem);
            //}
        }

        private void btnSRQ_Click(object sender, EventArgs e)
        {
            //string curItem = listDevices.SelectedItem.ToString();
            //if (curItem != null)
            //{
            //    TestSRQ(curItem);
            //}
        }

        private void cmbVISA_SelectedIndexChanged(object sender, EventArgs e)
        {
            //visa32.viClose(m_defRM);
            //RsVisa.RsViUnloadVisaLibrary();
            //string sVisa = cmbVISA.SelectedItem.ToString();
            //bool bNetworkSearch = false;
            //if (sVisa == "Keysight")
            //{
            //    RsVisa.RsViSetDefaultLibrary(RsVisa.RSVISA_MANFID_AG);

            //}
            //else if (sVisa == "R&S")
            //{
            //    RsVisa.RsViSetDefaultLibrary(RsVisa.RSVISA_MANFID_RS);
            //    bNetworkSearch = true;
            //}
            //else if (sVisa == "National")
            //{
            //    RsVisa.RsViSetDefaultLibrary(RsVisa.RSVISA_MANFID_NI);
            //}
            //else
            //{
            //    RsVisa.RsViSetDefaultLibrary(RsVisa.RSVISA_MANFID_DEFAULT);
            //}
            //mDNScheckBox.Enabled = bNetworkSearch;
            //VXI11checkBox.Enabled = bNetworkSearch;
            //visa32.viOpenDefaultRM(out m_defRM);
            //StringBuilder sManufacturer = new StringBuilder(visa32.VI_FIND_BUFLEN);

            //visa32.viGetAttribute(m_defRM, ViAttr.VI_ATTR_RSRC_MANF_NAME, sManufacturer);
            //txtResult.Text = "VISA loaded of: " + sManufacturer.ToString();

        }

        private ViStatus EventHandler(int vi, ViEventType inEventType, int inContext, int inUserHandle)
        {
            short stb;
            ViStatus status = visa32.viReadSTB(vi, out stb);
            string text = Environment.NewLine + "EventHandler: Event " + inEventType.ToString() +
                          " occurred with STB = 0x" + stb.ToString("X");

      //      txtResult.BeginInvoke(new DelegateMethodString(AddStatusText), text);
            //Note! following call produces a deadlock
            //AddStatusText(text);
            return status;
        }

        private static ViStatus Write(int vi, string buffer)
        {
            int retCount;
            return visa32.viWrite(vi, buffer, buffer.Length, out retCount);
        }

        private static ViStatus Read(int vi, out string buffer)
        {
            ViStatus status;
            buffer = "";
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

        private ViStatus Query(int vi, string sQuery, out string sAnswer)
        {
            sAnswer = "";
            ViStatus status = Write(vi, sQuery);
            if (status < 0) return status;
            return Read(vi, out sAnswer);
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

        //private void AddStatusText(string text)
        //{
        //    if (txtResult.InvokeRequired)
        //    {
        //        txtResult.Invoke(new DelegateMethodString(AddStatusText), text);
        //    }
        //    else
        //    {
        //        txtResult.Text += text;
        //    }
        //}
    }
}
