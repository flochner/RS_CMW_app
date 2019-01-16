using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CMWgraph;
using Ivi.Visa; //This .NET assembly is installed with your NI VISA installation
using IviVisaExtended; //Custom extention functions for Ivi.Visa

namespace CMWtests
{
    public class Tests
    {
        private IMessageBasedSession cmw = null;
        public enum TestStatus : int { Abort = -1, Success, Complete };
        private CancellationTokenSource _cts;
        private MainForm _parent = null;
        private StreamWriter _csvStream = null;
        private int numOfFrontEnds = 0;
        private int numOfTRX = 0;
        private long minFreq = 0;
        private bool hasKB036 = false;
        private bool _ignoreAmplError = false;
        private bool isFirstTest = true;
        private string chartLimits3 = "";
        private string chartLimits6 = "";
        private string cmwID = "";
        private string csvFileName = "";

        public Tests(MainForm parent, CancellationTokenSource cts)
        {
            _parent = parent;
            _cts = cts;
        }

        public void Begin()
        {
            var status = Sequencer();

            if (status == TestStatus.Abort)
                _parent.AddToResults(Environment.NewLine + "Tests Aborted.");
            else if (status == TestStatus.Complete)
                _parent.AddToResults(Environment.NewLine + "Tests Complete.");
        }

        private TestStatus Sequencer()
        {
            int[] amplList = { };
            string testName = "";

            if (ConnectIdentifyDUT() == TestStatus.Abort)
            {
                _parent.SetBtnBeginEnabled(true);
                return GracefulExit(TestStatus.Abort);
            }

            _parent.SetBtnCancelEnabled(true);
            _parent.SetHead1Text("GPRF CW Measurement Tests");

            /// -------------------------------------------------------------
            chartLimits3 = ",-0.7,-0.5,0,0.5,0.7";
            chartLimits6 = ",-1.2,-1.0,0,1.0,1.2";
            amplList = new int[] { 0 };//, -8 };//, -20 };

            testName = "RF1COM_RX";
            if (ConnectionMessage(testName) == TestStatus.Abort)
                return GracefulExit(TestStatus.Abort);
            InitMeasureSettings();

            cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF1C, RX1");
            foreach (int ampl in amplList)
                if (Measure(testName, ampl, "") == TestStatus.Abort)
                    return GracefulExit(TestStatus.Abort);


            ///// fml
            return GracefulExit(TestStatus.Complete);

            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF1C, RX2");
                else
                    cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF1C, RX3");
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);
            }

            /// -------------------------------------------------------------
            testName = "RF2COM_RX";
            if (ConnectionMessage(testName) == TestStatus.Abort)
                return GracefulExit(TestStatus.Abort);
            InitMeasureSettings();

            cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF2C, RX1");
            foreach (int ampl in amplList)
                if (Measure(testName, ampl, "") == TestStatus.Abort)
                    return GracefulExit(TestStatus.Abort);

            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF2C, RX2");
                else
                    cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF2C, RX3");
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);
            }

            /// -------------------------------------------------------------
            if (numOfFrontEnds > 1)
            {
                testName = "RF3COM_RX";
                if (ConnectionMessage(testName) == TestStatus.Abort)
                    return GracefulExit(TestStatus.Abort);
                InitMeasureSettings();

                cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF3C, RX2");
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);

                cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF3C, RX4");
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);

                /// -------------------------------------------------------------
                testName = "RF4COM_RX";
                if (ConnectionMessage(testName) == TestStatus.Abort)
                    return GracefulExit(TestStatus.Abort);
                InitMeasureSettings();

                cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF4C, RX2");
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);

                cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF4C, RX4");
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);
            }

            ///
            /// :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            /// 

            _parent.SetHead1Text("GPRF CW Generator Tests");

            chartLimits3 = (",-0.8,-0.6,0,0.6,0.8");
            chartLimits6 = (",-1.4,-1.2,0,1.2,1.4");
            amplList = new int[] { -8, -44 };

            testName = "RF1COM_TX";
            if (ConnectionMessage(testName) == TestStatus.Abort)
                return GracefulExit(TestStatus.Abort);

            cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF1C, TX1");
            foreach (int ampl in amplList)
                if (Measure(testName, ampl, "") == TestStatus.Abort)
                    return GracefulExit(TestStatus.Abort);

            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF1C, TX2");
                else
                    cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF1C, TX3");
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);
            }

            /// -------------------------------------------------------------
            chartLimits3 = (",-1.0,-0.8,0,0.8,1.0");
            chartLimits6 = (",-1.8,-1.6,0,1.6,1.8");
            amplList = new int[] { -0, -36 };

            testName = "RF1OUT_TX";
            if (ConnectionMessage(testName) == TestStatus.Abort)
                return GracefulExit(TestStatus.Abort);

            cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF1O, TX1");
            foreach (int ampl in amplList)
                if (Measure(testName, ampl, "") == TestStatus.Abort)
                    return GracefulExit(TestStatus.Abort);

            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF1O, TX2");
                else
                    cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF1O, TX3");
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);
            }

            /// -------------------------------------------------------------
            chartLimits3 = (",-0.8,-0.6,0,0.6,0.8");
            chartLimits6 = (",-1.4,-1.2,0,1.2,1.4");
            amplList = new int[] { -8, -44 };

            testName = "RF2COM_TX";
            if (ConnectionMessage(testName) == TestStatus.Abort)
                return GracefulExit(TestStatus.Abort);

            cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF2C, TX1");
            foreach (int ampl in amplList)
                if (Measure(testName, ampl, "") == TestStatus.Abort)
                    return GracefulExit(TestStatus.Abort);

            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF2C, TX2");
                else
                    cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF2C, TX3");
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);
            }

            /// -------------------------------------------------------------
            if (numOfFrontEnds > 1)
            {
                chartLimits3 = (",-0.8,-0.6,0,0.6,0.8");
                chartLimits6 = (",-1.4,-1.2,0,1.2,1.4");
                amplList = new int[] { -8, -44 };

                testName = "RF3COM_TX";
                if (ConnectionMessage(testName) == TestStatus.Abort)
                    return GracefulExit(TestStatus.Abort);

                cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF3C, TX2");
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);

                cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF3C, TX4");
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);

                /// -------------------------------------------------------------
                chartLimits3 = (",-1.0,-0.8,0,0.8,1.0");
                chartLimits6 = (",-1.8,-1.6,0,1.6,1.8");
                amplList = new int[] { -0, -36 };

                testName = "RF3OUT_TX";
                if (ConnectionMessage(testName) == TestStatus.Abort)
                    return GracefulExit(TestStatus.Abort);

                cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF3O, TX2");
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);

                cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF3O, TX4");
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);

                /// -------------------------------------------------------------
                chartLimits3 = (",-0.8,-0.6,0,0.6,0.8");
                chartLimits6 = (",-1.4,-1.2,0,1.2,1.4");
                amplList = new int[] { -8, -44 };

                testName = "RF4COM_TX";
                if (ConnectionMessage(testName) == TestStatus.Abort)
                    return GracefulExit(TestStatus.Abort);

                cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF4C, TX2");
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);

                cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF4C, TX4");
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);
            }

            return GracefulExit(TestStatus.Complete);
        }

        private TestStatus Measure(string testName, int testAmpl, string path)
        {
            int pmStatus = 0;
            bool retry = false;
            double amplError = 0.0;
            double cmwMeasPower = 0.0;
            double maxError = 0.0;
            double pmPower = 0.0;
            long currentFreq = 0;
            long endFreq = 0;
            string chartLimits = "";
            string testHeader = "";
            string visaResponse = "";
            string[] pmResponse = { };

            testHeader = testName.Split('_')[0] + " @ " + testAmpl + " dBm  " + path;
            _parent.AddToResults(Environment.NewLine + testHeader);

        start:

            int pointsCount = 0;
            double maxError3 = 0.0;
            double maxError6 = 0.0;

            _csvStream = OpenTempFile();
            if (_csvStream == null)
                return TestStatus.Abort;

            #region Config RX / TX
            ///// setup sensor to read
            cmw.Write("CONFigure:GPRF:MEAS:EPSensor:REPetition SINGleshot; TOUT 3; SCOunt 1; ATTenuation:STATe OFF; RESolution PD2");

            ///// setup measurement tests
            if (testName.Contains("RX"))
            {
                _csvStream.WriteLine("    GPRF CW Measurement Tests - " + cmwID);
                cmw.Write("INIT:GPRF:MEAS:POWer");
                cmw.Write("CONFigure:GPRF:MEAS:RFSettings:ENPower " + testAmpl);
                if (testName.Contains("1") || testName.Contains("2"))
                    cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF1O, TX1");
                else
                    cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF3O, TX1");
                cmw.Write("SOURce:GPRF:GEN:RFSettings:LEVel " + (testAmpl + 6.5));
            }
            else if (testName.Contains("TX"))
            {
                _csvStream.WriteLine("    GPRF CW Generator Tests - " + cmwID);
                cmw.Write("SOURce:GPRF:GEN:RFSettings:LEVel " + testAmpl);
                minFreq = 70;
            }

            _csvStream.WriteLine("0," + chartLimits3);
            cmw.Write("SOURce:GPRF:GEN:STATe ON");

            currentFreq = minFreq * (long)1e6;
            if (hasKB036)
                endFreq = (long)6000e6;
            else
                endFreq = (long)3300e6;
            #endregion

            var btnCancelEnabled = _parent.GetBtnCancelEnabled();
            _parent.SetBtnCancelEnabled(false);
            MessageBox.Show(_parent, "tol set for 0.003");
            _parent.SetBtnCancelEnabled(btnCancelEnabled);

            do  ///// Main Loop
            {
                if (_cts.IsCancellationRequested)
                    return GracefulExit(TestStatus.Abort);

                #region Set up this loop - set freqs - get GPRF Measure Power
                pointsCount += 1;
                _parent.SetHead2Text((currentFreq / 1e6).ToString() + " MHz");

                cmw.Write("SOURce:GPRF:GEN:RFSettings:FREQuency " + currentFreq);
                cmw.Write("CONFigure:GPRF:MEAS:EPSensor:FREQuency " + currentFreq);
                if (testName.Contains("RX"))
                {
                    cmw.Write("CONFigure:GPRF:MEAS:RFSettings:FREQuency " + currentFreq);
                    visaResponse = cmw.QueryString("READ:GPRF:MEAS:POWer:AVERage?");
                    try
                    {
                        cmwMeasPower = Convert.ToDouble(visaResponse.Split(',')[1]);
                    }
                    catch (Exception e)
                    {
                        btnCancelEnabled = _parent.GetBtnCancelEnabled();
                        _parent.SetBtnCancelEnabled(false);
                        MessageBox.Show(_parent, e.Message, e.GetType().ToString());
                        _parent.SetBtnCancelEnabled(btnCancelEnabled);
                    }
                }
                #endregion

                #region Take sensor reading
                do  //while (retry)
                {
                    retry = false;

                    visaResponse = cmw.QueryString("READ:GPRF:MEAS:EPSensor?");
                    try
                    {
                        pmResponse = visaResponse.Split(',');
                        int.TryParse(pmResponse[0], out pmStatus);
                        double.TryParse(pmResponse[1], out pmPower);
                    }
                    catch (Exception e)
                    {
                        btnCancelEnabled = _parent.GetBtnCancelEnabled();
                        _parent.SetBtnCancelEnabled(false);
                        MessageBox.Show(_parent, e.Message, e.Source);
                        _parent.SetBtnCancelEnabled(btnCancelEnabled);
                    }
                    
                    if ( pmStatus != 0 )
                    {
                        cmw.Write("SOURce:GPRF:GEN:STATe OFF");

                        btnCancelEnabled = _parent.GetBtnCancelEnabled();
                        _parent.SetBtnCancelEnabled(false);
                        MessageBox.Show(_parent, "Re-check connections using the following diagram.",
                                        "Test Setup",
                                         MessageBoxButtons.OK,
                                         MessageBoxIcon.Asterisk);
                        _parent.SetBtnCancelEnabled(btnCancelEnabled);

                        btnCancelEnabled = _parent.GetBtnCancelEnabled();
                        _parent.SetBtnCancelEnabled(false);
                        var img = new ConnectionImageForm(MessageBoxButtons.RetryCancel);
                        img.SetImage(testName + "-" + numOfFrontEnds);
                        img.ShowDialog();
                        _parent.SetBtnCancelEnabled(btnCancelEnabled);

                        if (img.DialogResult == DialogResult.Abort)
                            return TestStatus.Abort;

                        retry = (img.DialogResult == DialogResult.Retry);
                        cmw.Write("SOURce:GPRF:GEN:STATe ON");
                    }
                } while (retry);

                if (testName.Contains("RX"))
                    amplError = cmwMeasPower - pmPower;
                else
                    amplError = pmPower - testAmpl;
                #endregion

                ///fml
                amplError = 4;


                #region Handle excessive error
                // If error is excessive, assume improper connections and prompt to fix.
                if ((currentFreq <= 200e6) && (Math.Abs(amplError) > 3) && !_ignoreAmplError)
                {
                    cmw.Write("SOURce:GPRF:GEN:STATe OFF");
                    cmw.Write("SYSTem:MEASurement:ALL:OFF");

                    btnCancelEnabled = _parent.GetBtnCancelEnabled();
                    _parent.SetBtnCancelEnabled(false);
                    MessageBox.Show(_parent, "Re-check connections using the following diagram.",
                                    "Test Setup",
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Asterisk);
                    _parent.SetBtnCancelEnabled(btnCancelEnabled);

                    btnCancelEnabled = _parent.GetBtnCancelEnabled();
                    _parent.SetBtnCancelEnabled(false);
                    var img = new ConnectionImageForm(MessageBoxButtons.AbortRetryIgnore);
                    img.SetImage(testName + "-" + numOfFrontEnds);
                    img.ShowDialog();
                    _parent.SetBtnCancelEnabled(btnCancelEnabled);

                    //btnCancelEnabled = _parent.GetBtnCancelEnabled();
                    //_parent.SetBtnCancelEnabled(false);
                    //DialogResult resp = MessageBox.Show(_parent, "(Retry) after fixing the connections" + Environment.NewLine +
                    //                                    "(Ignore) further level errors and continue test" + Environment.NewLine +
                    //                                    "(Abort) all testing",
                    //                                    "MEASURING - Check Connections",
                    //                                     MessageBoxButtons.AbortRetryIgnore,
                    //                                     MessageBoxIcon.Question,
                    //                                     MessageBoxDefaultButton.Button3);
                    //_parent.SetBtnCancelEnabled(btnCancelEnabled);

                    _ignoreAmplError = (img.DialogResult == DialogResult.Ignore);

                    if (img.DialogResult == DialogResult.Abort)
                        return TestStatus.Abort;

                    if (img.DialogResult == DialogResult.Retry)
                        goto start;

                    if (_ignoreAmplError)
                        cmw.Write("SOURce:GPRF:GEN:STATe ON");
                }
                #endregion

                #region Record results - setup next loop
                // Determine active band to record error for,
                //   and store only if it is greater than the current maximum error.
                if (currentFreq <= 3300e6)
                {
                    chartLimits = chartLimits3;
                    if (Math.Abs(amplError) > Math.Abs(maxError3))
                        maxError3 = amplError;
                }
                else
                {
                    chartLimits = chartLimits6;
                    if (Math.Abs(amplError) > Math.Abs(maxError6))
                        maxError6 = amplError;
                }

                // Push frequency, point-error and limit-line values.
                _csvStream.WriteLine(currentFreq / 1e6 + "," + amplError + chartLimits);

                // If current frequency is the minimum measurement frequency of CMW,
                // then the next freq is 200 MHz, otherwise it is increased by 100 MHz
                if (currentFreq == minFreq * 1e6)
                    currentFreq = (long)200e6;
                else
                    currentFreq += (long)100e6;
                #endregion

            } while (currentFreq <= endFreq);

            #region Cleanup - close files - create graph
            ///// Set instruments to standby.
            cmw.Write("SOURce:GPRF:GEN:STATe OFF");
            cmw.Write("SYSTem:MEASurement:ALL:OFF");

            // Push one frequency point beyond test to make whitespace in graph (3400 or 6100 MHz).
            _csvStream.WriteLine(currentFreq / 1e6 + "," + chartLimits);
            _csvStream.WriteLine(testHeader);
            _csvStream.Dispose();

            _parent.SetHead2Text("");

            // maxErr tells graph to decide whether to use fixed Y-axis (2 dB),
            //   or dynamic axis if error exceeds 2 dB.
            maxError = Math.Max(Math.Abs(maxError3), Math.Abs(maxError6));

            // Create graph
            Graph.Create(cmwID, csvFileName, pointsCount, maxError, isFirstTest);
            File.Delete(csvFileName);

            isFirstTest = false;

            // Suppress connection error message until the next connection change.
            _ignoreAmplError = true;

            return TestStatus.Success;
            #endregion
        }

        private TestStatus ConnectionMessage(string connection)
        {
            int pmStatus = -1;
            bool retryZero = false;
            string visaResponse = "";
            string[] pmResponse = { };

            _parent.SetBtnCancelEnabled(false);

            do //while retryZero
            {
                retryZero = false;

                cmw.Clear();
                cmw.Write("*RST;*CLS");
                cmw.Write("*ESE 1");
                cmw.ErrorChecking();

                var btnCancelEnabled = _parent.GetBtnCancelEnabled();
                _parent.SetBtnCancelEnabled(false);
                var img = new ConnectionImageForm(MessageBoxButtons.OKCancel);
                img.SetImage(connection + "-" + numOfFrontEnds);
                img.ShowDialog();
                _parent.SetBtnCancelEnabled(btnCancelEnabled);
                if (img.DialogResult == DialogResult.Abort)
                    return TestStatus.Abort;

                visaResponse = cmw.QueryString("READ:GPRF:MEAS:EPSensor?");
                try
                {
                    pmResponse = visaResponse.Split(',');
                    int.TryParse(pmResponse[0], out pmStatus);
                }
                catch (Exception e)
                {
                    btnCancelEnabled = _parent.GetBtnCancelEnabled();
                    _parent.SetBtnCancelEnabled(false);
                    MessageBox.Show(_parent, e.Message, e.Source);
                    _parent.SetBtnCancelEnabled(btnCancelEnabled);
                }

                if (pmStatus == 0 || pmStatus == 4)
                {
                    _parent.SetHead2Text("Zeroing Sensor...");
                //    WriteSTB("CALibration:GPRF:MEAS:EPSensor:ZERO", 0);
                //    QuerySTB("CALibration:GPRF:MEAS:EPSensor:ZERO?", 20000, out visaResponse);

                    //fml
                    visaResponse = "PASS";

                    if (!visaResponse.Contains("PASS"))
                    {
                        btnCancelEnabled = _parent.GetBtnCancelEnabled();
                        _parent.SetBtnCancelEnabled(false);
                        var verifyConnection = MessageBox.Show(_parent, "Ensure sensor is not connected to an active source." + Environment.NewLine + Environment.NewLine +
                                                     "(Retry) after verifying all outputs are off." + Environment.NewLine +
                                                     "(Cancel) all testing.",
                                                     "Sensor Zero Failure",
                                                      MessageBoxButtons.RetryCancel,
                                                      MessageBoxIcon.Exclamation,
                                                      MessageBoxDefaultButton.Button1);
                        _parent.SetBtnCancelEnabled(btnCancelEnabled);

                        retryZero = (verifyConnection == DialogResult.Retry);
                        if (verifyConnection == DialogResult.Cancel)
                            return TestStatus.Abort;
                    }
                }
                else if (pmStatus == 27)
                {
                    btnCancelEnabled = _parent.GetBtnCancelEnabled();
                    _parent.SetBtnCancelEnabled(false);
                    var verifyConnection = MessageBox.Show(_parent, "Ensure an NRP sensor is connected to the SENSOR port." + Environment.NewLine + Environment.NewLine +
                                                 "(Retry) after verifying the connection." + Environment.NewLine +
                                                 "(Cancel) all testing.",
                                                 "Sensor Status Error",
                                                  MessageBoxButtons.RetryCancel,
                                                  MessageBoxIcon.Exclamation,
                                                  MessageBoxDefaultButton.Button1);
                    _parent.SetBtnCancelEnabled(btnCancelEnabled);

                    if (verifyConnection == DialogResult.Retry)
                        retryZero = true;
                    else
                        return TestStatus.Abort;
                }
            } while (retryZero);

            _ignoreAmplError = false;

            _parent.SetHead2Text("");
            _parent.SetBtnCancelEnabled(true);

            return TestStatus.Success;
        }

        private TestStatus ConnectIdentifyDUT()
        {
            string cmwModel = "";
            string cmwSerNum = "";
            string resource = "";
            string visaResponse = "";
            string[] hwOptions = { };
            string[] identFields = { };

            var btnCancelEnabled = _parent.GetBtnCancelEnabled();
            _parent.SetBtnCancelEnabled(false);
            var resForm = new VISAresourceForm();
            resForm.ShowDialog();
            _parent.SetBtnCancelEnabled(btnCancelEnabled);
            resource = resForm.Resource;
            resForm.Dispose();

            if (!string.IsNullOrWhiteSpace(resource))
            {
                try
                {
                    cmw = GlobalResourceManager.Open(resource) as IMessageBasedSession;
                    cmw.Clear();
                    cmw.Write("*RST;*CLS");
                    cmw.Write("*ESE 1");
                    cmw.ErrorChecking();
                }
                catch (Exception e)
                {
                    btnCancelEnabled = _parent.GetBtnCancelEnabled();
                    _parent.SetBtnCancelEnabled(false);
                    MessageBox.Show(_parent, String.Format("Error initializing the session:\n{0}", e.Message), e.GetType().ToString());
                    _parent.SetBtnCancelEnabled(btnCancelEnabled);
                    return TestStatus.Abort;
                }
            }
            else
            {
                _parent.AddToResults("No resource selected.");
                return TestStatus.Abort;
            }

            // CMW Identification

            visaResponse = cmw.QueryString("*IDN?");
            try
            {
                identFields = visaResponse.Split(',');
            }
            catch (ArgumentOutOfRangeException e)
            {
                btnCancelEnabled = _parent.GetBtnCancelEnabled();
                _parent.SetBtnCancelEnabled(false);
                MessageBox.Show(_parent, String.Format("Error identifying instrument:\n{0}", e.Message), e.GetType().ToString());
                _parent.SetBtnCancelEnabled(btnCancelEnabled);
                return TestStatus.Abort;
            }

            if (identFields.Length >= 3)
                if (identFields[2].Contains("/"))
                    cmwSerNum = identFields[2].Split('/')[1];
                else
                    cmwSerNum = identFields[2];

            if (visaResponse.Contains("1201.0002k50") ||
                visaResponse.Contains("1201.0002K50"))
            {
                cmwModel = "CMW500";
                minFreq = 70;
            }
            else if (visaResponse.Contains("1201.0002k29") ||
                     visaResponse.Contains("1201.0002K29"))
            {
                cmwModel = "CMW290";
                minFreq = 70;
            }
            else if (visaResponse.Contains("1201.0002k75") ||
                     visaResponse.Contains("1201.0002K75"))
            {
                cmwModel = "CMW270";
                minFreq = 150;
            }
            else if (visaResponse.Contains("1201.0002k") ||
                     visaResponse.Contains("1201.0002K"))
            {
                btnCancelEnabled = _parent.GetBtnCancelEnabled();
                _parent.SetBtnCancelEnabled(false);
                MessageBox.Show(_parent, "DUT Not Yet Covered under this procedure.");
                _parent.SetBtnCancelEnabled(btnCancelEnabled);
                return TestStatus.Abort;
            }
            else
            {
                btnCancelEnabled = _parent.GetBtnCancelEnabled();
                _parent.SetBtnCancelEnabled(false);
                MessageBox.Show(_parent, "This is not a CMW.");
                _parent.SetBtnCancelEnabled(btnCancelEnabled);
                return TestStatus.Abort;
            }

            cmwID = cmwModel + " " + cmwSerNum;
            _parent.AddToResults(cmwID);

            // CMW Options
            visaResponse = cmw.QueryString("SYSTem:BASE:OPTion:LIST? HWOPtion");
            _parent.AddToResults(visaResponse);
            hwOptions = visaResponse.Split(',');

            for (int i = 0; i < hwOptions.Length; i++)
            {
                hasKB036 = hwOptions[i].Contains("KB036") || hasKB036;
                if (hwOptions[i].Contains("H570"))
                    numOfTRX++;
                if (hwOptions[i].Contains("H590"))
                    numOfFrontEnds++;
            }
            _parent.AddToResults("hasKB036: " + hasKB036.ToString());
            _parent.AddToResults("numOfTRX: " + numOfTRX.ToString());
            _parent.AddToResults("numOfFrontEnds: " + numOfFrontEnds.ToString());
            return TestStatus.Success;
        }

        private StreamWriter OpenTempFile()
        {
            try
            {
                return new StreamWriter(GetTempFileName());
            }
            catch (IOException e)
            {
                var btnCancelEnabled = _parent.GetBtnCancelEnabled();
                _parent.SetBtnCancelEnabled(false);
                MessageBox.Show(_parent, e.Message, "IO Exception");
                _parent.SetBtnCancelEnabled(btnCancelEnabled);
                return null;
            }
        }

        private string GetTempFileName()
        {
            int attempt = 0;
            while (true)
            {
                csvFileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

                try
                {
                    using (new FileStream(csvFileName, FileMode.CreateNew)) { }
                    return csvFileName;
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
            cmw.Write("CONFigure:GPRF:MEAS:POWer:MODE POWer; SCOunt 50; SLENgth 1000e-6; MLENgth 950e-6");
            cmw.Write("TRIGger:GPRF:MEAS:POWer:OFFSet 10e-6");
        }

        private TestStatus GracefulExit(TestStatus exitStatus)
        {
            _parent.SetBtnCancelEnabled(false);
            _parent.SetHead1Text("");
            _parent.SetHead2Text("");

            //_parent.AddToResults("In graceful");
            try
            {
                cmw.Clear();
                cmw.Write("*RST;*CLS");
                cmw.Write("*ESE 1");
                cmw.ErrorChecking();
                cmw.Dispose();
            }
            catch { }

            if (_csvStream != null)
                try { _csvStream.Dispose(); }
                catch
                {
                    var btnCancelEnabled = _parent.GetBtnCancelEnabled();
                    _parent.SetBtnCancelEnabled(false);
                    MessageBox.Show(_parent, "Dispose csvStream Exception");
                    _parent.SetBtnCancelEnabled(btnCancelEnabled);
                }

            if (File.Exists(csvFileName))
                try { File.Delete(csvFileName); }
                catch
                {
                    var btnCancelEnabled = _parent.GetBtnCancelEnabled();
                    _parent.SetBtnCancelEnabled(false);
                    MessageBox.Show(_parent, "Temp file delete Exception");
                    _parent.SetBtnCancelEnabled(btnCancelEnabled);
                }

            try
            {
                _cts.Dispose();
                _parent.SetBtnBeginEnabled(true);
            }
            catch (Exception exc)
            {
                var btnCancelEnabled = _parent.GetBtnCancelEnabled();
                _parent.SetBtnCancelEnabled(false);
                MessageBox.Show(_parent, exc.Message, exc.GetType().ToString());
                _parent.SetBtnCancelEnabled(btnCancelEnabled);
            }

            return exitStatus;
        }

        private void WriteSTB(string command, int timeout)
        {
            try // try block to catch any InstrumentErrorException() or InstrumentOPCtimeoutException()
            {
                cmw.WriteWithSTBpollSync(command, timeout);
            }
            catch (InstrumentErrorException e)
            {
                var btnCancelEnabled = _parent.GetBtnCancelEnabled();
                _parent.SetBtnCancelEnabled(false);
                MessageBox.Show(_parent, e.Message, e.GetType().ToString());
                _parent.SetBtnCancelEnabled(btnCancelEnabled);
            }
            catch (InstrumentOPCtimeoutException e)
            {
                var btnCancelEnabled = _parent.GetBtnCancelEnabled();
                _parent.SetBtnCancelEnabled(false);
                MessageBox.Show(_parent, e.Message, e.GetType().ToString());
                _parent.SetBtnCancelEnabled(btnCancelEnabled);
            }
            catch (Ivi.Visa.VisaException e)
            {
                var btnCancelEnabled = _parent.GetBtnCancelEnabled();
                _parent.SetBtnCancelEnabled(false);
                MessageBox.Show(_parent, e.Message, e.GetType().ToString());
                _parent.SetBtnCancelEnabled(btnCancelEnabled);
            }
        }

        private void QuerySTB(string query, int timeout, out string response)
        {
            response = null;
            try
            {
                response = cmw.QueryWithSTBpollSync(query, timeout);
            }
            catch (InstrumentErrorException e)
            {
                var btnCancelEnabled = _parent.GetBtnCancelEnabled();
                _parent.SetBtnCancelEnabled(false);
                MessageBox.Show(_parent, e.Message, e.GetType().ToString());
                _parent.SetBtnCancelEnabled(btnCancelEnabled);
            }
            catch (InstrumentOPCtimeoutException e)
            {
                var btnCancelEnabled = _parent.GetBtnCancelEnabled();
                _parent.SetBtnCancelEnabled(false);
                MessageBox.Show(_parent, e.Message, e.GetType().ToString());
                _parent.SetBtnCancelEnabled(btnCancelEnabled);
            }
            catch (Ivi.Visa.VisaException e)
            {
                var btnCancelEnabled = _parent.GetBtnCancelEnabled();
                _parent.SetBtnCancelEnabled(false);
                MessageBox.Show(_parent, e.Message, e.GetType().ToString());
                _parent.SetBtnCancelEnabled(btnCancelEnabled);
            }
        }

        private void ModalMessageBox(string message)
        {
            if (_parent.InvokeRequired)
            {
                _parent.Invoke((Action)delegate { MessageBox.Show(_parent, message); });
            }
        }
    }
}
