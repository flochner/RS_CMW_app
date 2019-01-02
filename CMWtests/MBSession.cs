using NationalInstruments.VisaNS;
using System;
using System.Windows.Forms;

namespace CMWtests
{
    class MBSession
    {
        public MessageBasedSession MbSession { get; private set; } = null;

        public MBSession(string resourceString)
        {
            if (resourceString != null)
                try
                { MbSession = (MessageBasedSession)ResourceManager.GetLocalManager().Open(resourceString); }
                catch (Exception exc)
                { MessageBox.Show(exc.Message, exc.GetType().ToString()); }
            else
                MessageBox.Show("Empty resource string");
        }

        public string Query(string visaQuery)
        {
            return Query(visaQuery, 2000);
        }

        public string Query(string visaQuery, int timeout)
        {
            MbSession.Timeout = timeout;
            try
            {
                return MbSession.Query(visaQuery);
            }
            catch (VisaException exp)
            {
                MessageBox.Show(exp.Message, exp.GetType().ToString());
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, exp.GetType().ToString());
            }
            return null;
        }

        public void Write(string visaStmt)
        {
            Write(visaStmt, 2000);
        }

        public void Write(string visaStmt, int timeout)
        {
            MbSession.Timeout = timeout;
            try
            {
                MbSession.Write(visaStmt);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, exp.GetType().ToString());
            }
        }
    }
}
