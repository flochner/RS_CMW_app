﻿using NationalInstruments.VisaNS;
using System;
using System.Windows.Forms;

namespace CMWtests
{
    class MBSession
    {
        public MessageBasedSession MbSession { get => _mbSession; private set { } }
        private MessageBasedSession _mbSession = null;

        public MBSession(string resourceString)
        {
            Create(resourceString);
        }

        private void Create(string resourceString)
        {
            if (resourceString != null)
            {
                try
                {
                    _mbSession = (MessageBasedSession)ResourceManager.GetLocalManager().Open(resourceString);
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.Message, exp.GetType().ToString());
                }
            }
            else
            {
                MessageBox.Show("Empty resource string");
            }
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
                return _mbSession.Query(visaQuery);
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
                _mbSession.Write(visaStmt);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, exp.GetType().ToString());
            }
        }
    }
}
