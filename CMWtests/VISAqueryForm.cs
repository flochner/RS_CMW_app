﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using RsVisaLoader;

namespace CMWtests
{
    public partial class VISAqueryForm : Form
    {
        private ViSession session = null;
        private ViStatus status = 0;
        private int vi = 0;

        public VISAqueryForm()
        {
            InitializeComponent();
            btnWriteVISA.Enabled = false;
            btnQueryVISA.Enabled = false;
        }

        private void btnQueryVISA_Click(object sender, EventArgs e)
        {
            textBoxResponse.Text = string.Empty;
            Update();

            string sAnswer;
            status = session.Query(vi, textBoxStringToWrite.Text, out sAnswer);
            Thread.Sleep(15000);

            if (status < ViStatus.VI_SUCCESS) ShowErrorText(status);

            status = session.Query(vi, "*OPC?", out sAnswer);
            if (status < ViStatus.VI_SUCCESS) ShowErrorText(status);

            if (status < ViStatus.VI_SUCCESS)
                ShowErrorText(status);
            else
                textBoxResponse.Text = sAnswer;
        }

        private void btnWriteVISA_Click(object sender, EventArgs e)
        {
            status = session.Write(vi, textBoxStringToWrite.Text);
            if (status < ViStatus.VI_SUCCESS) ShowErrorText(status);
        }

        private void btnConnectNew_Click(object sender, EventArgs e)
        {
            ViStatus status;
            string[] modelSer;
            string resource = "";

            labelResource.Text = "No Resource Selected";
            btnWriteVISA.Enabled = false;
            btnQueryVISA.Enabled = false;
            textBoxResponse.Text = string.Empty;

            session = new ViSession();

            var resForm = new VISAresourceForm(session.ResourceMgr);
            resForm.ShowDialog();
            resource = resForm.Resource;
            status = session.OpenSession(resource, out vi);
            resForm.Dispose();

            if (resource != string.Empty)
            {
                status = session.Query(vi, "*IDN?", out string idn);
                if (status == ViStatus.VI_SUCCESS)
                {
                    modelSer = idn.Split(',');
                    if (modelSer.Length >= 3)
                    {
                        if (modelSer[2].Contains("/"))
                            modelSer[2] = modelSer[2].Split('/')[1];
                        labelResource.Text = modelSer[1].Trim() + " - " + modelSer[2].Trim();
                    }
                }
                else
                {
                    ShowErrorText(status);
                }
            }

            if (textBoxStringToWrite.Text != string.Empty && session != null)
            {
                btnWriteVISA.Enabled = true;
                btnQueryVISA.Enabled = true;
            }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            status = session.CloseSession(vi);
            if (status < ViStatus.VI_SUCCESS) ShowErrorText(status);
            session.CloseResMgr();
        }

        private void textBoxStringToWrite_TextChanged(object sender, EventArgs e)
        {
            if (textBoxStringToWrite.Text == string.Empty || session == null)
            {
                btnWriteVISA.Enabled = false;
                btnQueryVISA.Enabled = false;
            }
            else
            {
                btnWriteVISA.Enabled = true;
                btnQueryVISA.Enabled = true;
            }
        }

        private void ShowErrorText(ViStatus status)
        {
            StringBuilder text = new StringBuilder(visa32.VI_FIND_BUFLEN);
            visa32.viStatusDesc(session.ResourceMgr, status, text);
            textBoxResponse.Text += Environment.NewLine + text.ToString();
        }
    }
}
