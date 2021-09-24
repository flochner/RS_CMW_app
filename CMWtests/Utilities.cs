using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace CMWtests
{
    public partial class MainForm
    {
        private TestStatus ConnectionMessage(string connection)
        {
            bool retryZero = false;
            string[] pmResponse = { };

            if (CancelTesting == true)
                return TestStatus.Abort;

            SetBtnCancelEnabled(false);
            SetMenuStripEnabled(false);

            do //while retryZero
            {
                retryZero = false;

                //SetDebugText("Waiting at ConnectionMessage start");
                mreMeasure.WaitOne();
                if (CancelTesting == true)
                    return TestStatus.Abort;

                Write(cmw, "*RST");
                cmw.ClearStatus();
                cmw.ErrorChecking();

                var btnCancelEnabled = GetBtnCancelEnabled();
                SetBtnCancelEnabled(false);
                var img = new ConnectionImageForm(MessageBoxButtons.OKCancel);
                img.SetImage(connection + "_" + numOfFrontEnds);
                Invoke(new MethodInvoker(() => img.ShowDialog(this)));
                SetBtnCancelEnabled(btnCancelEnabled);
                if (img.DialogResult == DialogResult.Abort || CancelTesting == true)
                {
                    SetMenuStripEnabled(true);
                    return TestStatus.Abort;
                }

                SetHead2Text("Zeroing Sensor...");
                /// !
#if !DEBUG
                Write(cmw, "ABORt:GPRF:MEAS:EPSensor;:CALibration:GPRF:MEAS:EPSensor:ZERO");
                var visaResponse = Query(cmw, "CALibration:GPRF:MEAS:EPSensor:ZERO?", 20000);
#else
                var visaResponse = "PASS";
#endif
                if (!visaResponse.Contains("PASS"))
                {
                    var verifyConnection = ModalMessageBox("Ensure sensor is not connected to an active source." + Environment.NewLine + Environment.NewLine +
                                                           "(Retry) after verifying all outputs are off." + Environment.NewLine +
                                                           "(Cancel) all testing.",
                                                           "Sensor Zero Failure",
                                                            MessageBoxButtons.RetryCancel,
                                                            MessageBoxIcon.Exclamation,
                                                            MessageBoxDefaultButton.Button1);

                    retryZero = (verifyConnection == DialogResult.Retry);
                    if (verifyConnection == DialogResult.Cancel)
                    {
                        SetMenuStripEnabled(true);
                        return TestStatus.Abort;
                    }
                }
            } while (retryZero);

            ignoreAmplError = false;

            SetHead2Text("");
            SetBtnCancelEnabled(true);
            SetMenuStripEnabled(true);

            return TestStatus.Success;
        }

        private TestStatus ConnectIdentifyDUT()
        {
            string cmwModel = "";
            string cmwSerNum = "";
            string[] identFields = { };

            numOfFrontEnds = 0;
            numOfTRX = 0;
            isFirstTest = true;

            var btnCancelEnabled = GetBtnCancelEnabled();
            SetBtnCancelEnabled(false);
            var resourceForm = new VISAresourceForm();

            if (resourceForm.ResourcesCount == 0)
            {
                AddToResults("No resources available.");
                return TestStatus.Abort;
            }
            if (resourceForm.ResourcesCount > 1)
                Invoke(new MethodInvoker(() => resourceForm.ShowDialog(this)));

            SetBtnCancelEnabled(btnCancelEnabled);
            var resource = resourceForm.Resource;
            resourceForm.Dispose();

            if (!string.IsNullOrWhiteSpace(resource))
            {
                cmw = new VisaIO(resource);
                cmw.Reset();
            }
            else
            {
                AddToResults("No resource selected.");
                return TestStatus.Abort;
            }

            // CMW Identification
            var visaResponse = Query(cmw, "*IDN?");
            try
            {
                identFields = visaResponse.Split(',');
            }
            catch (ArgumentOutOfRangeException e)
            {
                ModalMessageBox(String.Format("Error identifying instrument:\n{0}", e.Message), e.GetType().ToString());
                return TestStatus.Abort;
            }

            if (identFields.Length >= 3)
                if (identFields[2].Contains("/"))
                    cmwSerNum = identFields[2].Split('/')[1];
                else
                    cmwSerNum = identFields[2];

            if (visaResponse.ToLower().Contains("1201.0002k50"))
            {
                cmwModel = "CMW500";
            }
            else if (visaResponse.ToLower().Contains("1201.0002k29"))
            {
                cmwModel = "CMW290";
            }
            else if (visaResponse.ToLower().Contains("1201.0002k75"))
            {
                cmwModel = "CMW270";
            }
            else if (visaResponse.ToLower().Contains("1201.0002k"))
            {
                ModalMessageBox("This CMW is not yet covered under this procedure.");
                return TestStatus.Abort;
            }
            else
            {
                ModalMessageBox("This is not a CMW.");
                return TestStatus.Abort;
            }

            cmwID = cmwModel + " " + cmwSerNum;
            AddToResults(cmwID);

            var cmwFW = identFields[3];
            AddToResults("FW " + cmwFW);

            // CMW Options
            visaResponse = Query(cmw, "SYSTem:BASE:OPTion:LIST? HWOPtion");
#if DEBUG
            AddToResults(visaResponse);
#endif
            var hwOptions = visaResponse.Split(',');

            foreach (string option in hwOptions)
            {
                hasKB036 = option.Contains("KB036") && OptionsForm.KB036Override;
                if (option.Contains("H570"))
                    numOfTRX++;
                if (option.Contains("H590"))
                    numOfFrontEnds++;
                if (option.Contains("H570H"))
                    minRecvFreq = 150;
            }
#if DEBUG
            AddToResults("hasKB036: " + hasKB036.ToString());
            AddToResults("numOfTRX: " + numOfTRX.ToString());
            AddToResults("numOfFrontEnds: " + numOfFrontEnds.ToString());
#endif
            return TestStatus.Success;
        }

        private TestStatus CheckSensor()
        {
            int pmStatus = 0;
            bool retrySensor = false;

            SetHead2Text("Please wait...");
            do
            {
                mreMeasure.WaitOne();
                Status = TestStatus.InProgress;
                if (CancelTesting == true)
                    return TestStatus.Abort;
#if !DEBUG
                var visaResponse = Query(cmw, "READ:GPRF:MEAS:EPSensor?", 15000);
#else
                var visaResponse = "0,0";
#endif
                try
                {
                    int.TryParse(visaResponse.Split(',')[0], out pmStatus);
                }
                catch (Exception e)
                {
                    pmStatus = -1;
                    ModalMessageBox(e.Message, e.GetType().ToString());
                }

                if (pmStatus == 27)
                {
                    var verifyConnection = ModalMessageBox("Ensure an NRP sensor is connected to the SENSOR port." + Environment.NewLine + Environment.NewLine +
                                                           "(Retry) after verifying the connection." + Environment.NewLine +
                                                           "(Cancel) all testing.",
                                                           "Sensor Status Error",
                                                            MessageBoxButtons.RetryCancel,
                                                            MessageBoxIcon.Exclamation,
                                                            MessageBoxDefaultButton.Button1);

                    if (verifyConnection == DialogResult.Retry)
                    {
                        retrySensor = true;
                        Thread.Sleep(5000);
                    }
                    else
                    {
                        return TestStatus.Abort;
                    }
                }
                else if (pmStatus == 0 || pmStatus == 4)
                {
                    retrySensor = false;
                }
                else
                {
                    ModalMessageBox(string.Format("pmStatus: {0}", pmStatus.ToString()), "Sensor Error");
                }
            } while (retrySensor == true);

            SetHead2Text("");
            return TestStatus.Success;
        }

        public StreamWriter OpenTempFile(out string tempFile)
        {
            try
            {
                tempFile = GetTempFileName();
                return new StreamWriter(tempFile);
            }
            catch (IOException e)
            {
                ModalMessageBox(e.Message, e.GetType().ToString());
                tempFile = null;
                return null;
            }
        }

        private string GetTempFileName()
        {
            int attempt = 0;
            string fileName = "";

            while (true)
            {
                fileName = Path.Combine(Path.GetTempPath(), Path.ChangeExtension(Path.GetRandomFileName(), "csv"));
                try
                {
                    using (new FileStream(fileName, FileMode.CreateNew)) { }
                    return fileName;
                }
                catch (IOException ex)
                {
                    if (++attempt == 10)
                        throw new IOException("No unique temporary file name is available.", ex);
                    return null;
                }
            }
        }

        private void InitMeasureSettings()
        {
            Write(cmw, "CONFigure:GPRF:MEAS:POWer:MODE POWer; SCOunt 50; SLENgth 1000e-6; MLENgth 950e-6");
            Write(cmw, "TRIGger:GPRF:MEAS:POWer:OFFSet 10e-6");
        }

        private void Write(VisaIO instr, string message, int timeout = 2000)
        {
            instr.IoLock();
            instr.WriteWithSTB(message, timeout);
            instr.IoUnlock();
        }

        private string Query(VisaIO instr, string message, int timeout = 2000)
        {
            instr.IoLock();
            var response = instr.QueryWithSTB(message, timeout);
            instr.IoUnlock();
            return response;
        }

        private TestStatus GracefulExit(TestStatus exitStatus)
        {
            SetBtnCancelEnabled(false);
            SetHead1Text("");
            SetHead2Text("");

            try
            {
                tempGauge.Stop();
                cmw.Reset();
                cmw.CloseInstrument();
                cmw = null;
            }
            catch (NullReferenceException) { }

            if (csvStream != null)
                try
                {
                    csvStream.Dispose();
                }
                catch
                {
                    ModalMessageBox("Dispose csvStream Exception");
                }

            if (File.Exists(csvFileName))
                try
                {
                    File.Delete(csvFileName);
                }
                catch
                {
                    ModalMessageBox("Temp file delete Exception");
                }

            if (exitStatus == TestStatus.Abort)
            {
                AddToResults(Environment.NewLine + "Tests Aborted.");
                ProgressBar1_Init();
            }
            else if (exitStatus == TestStatus.Complete)
            {
                AddToResults(Environment.NewLine + "Tests Complete.");
            }

            OptionsForm.TempOverride = false;
            SetBtnBeginEnabled(true);
            Status = TestStatus.Complete;
            CancelTesting = false;
            areExit.Set();
            return Status;
        }

        public DialogResult ModalMessageBox(
            string message, string title = "",
            MessageBoxButtons buttons = MessageBoxButtons.OK,
            MessageBoxIcon icon = MessageBoxIcon.None,
            MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
        {
            DialogResult result = DialogResult.OK;
            Invoke(new MethodInvoker(() =>
            {
                var btnCancelEnabled = GetBtnCancelEnabled();
                SetBtnCancelEnabled(false);
                result = MessageBox.Show(this, message, title, buttons, icon, defaultButton);
                SetBtnCancelEnabled(btnCancelEnabled);
            }));
            return result;
        }
    }
}
