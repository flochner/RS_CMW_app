using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using CMWgraph;
using Ivi.Visa; //This .NET assembly is installed with your NI VISA installation
using IviVisaExtended; //Custom extention functions for Ivi.Visa

namespace CMWtests
{
    public class Tests
    {
        private IMessageBasedSession cmw = null;
        public enum TestStatus : int { Abort = -1, Success, InProgress, Complete };
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
        private string csvFileName = "";

        public TestStatus Status { get; private set; } // = TestStatus.Complete;
        public string cmwID { get; private set; } = "";

        public Tests(MainForm parent, CancellationTokenSource cts)
        {
            _parent = parent;
            _cts = cts;
        }

        public void Begin()
        {
            Status = TestStatus.InProgress;
            Status = Sequencer();

            if (Status == TestStatus.Abort)
                AddToResults(Environment.NewLine + "Tests Aborted.");
            else if (Status == TestStatus.Complete)
                AddToResults(Environment.NewLine + "Tests Complete.");

            //if (_parent.IsExitRequested)
            //    _parent.AppExit();

            _parent.SetBtnBeginEnabled(true);
        }

        private TestStatus Sequencer()
        {
            int[] amplList = { };
            string testName = "";

            if (ConnectIdentifyDUT() == TestStatus.Abort)
                return GracefulExit(TestStatus.Abort);

            _parent.SetBtnCancelEnabled(true);
            ProgressBar2_Settings(12 * numOfTRX);

            /// fml
            goto gentests;
            
            SetHead1Text("GPRF CW Measurement Tests");
            AddToResults(Environment.NewLine + Environment.NewLine + "GPRF CW Measurement Tests");

            /// -------------------------------------------------------------
            chartLimits3 = ",-0.7,-0.5,0,0.5,0.7";
            chartLimits6 = ",-1.2,-1.0,0,1.0,1.2";
            amplList = new int[] { 0, -8, -20 };
#if DEBUG
            amplList = new int[] { 0 };
#endif

            testName = "RF1COM_RX";

            if (ConnectionMessage(testName) == TestStatus.Abort)
                return GracefulExit(TestStatus.Abort);
            InitMeasureSettings();

            cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF1C, RX1", true);
            foreach (int ampl in amplList)
            {
                if (Measure(testName, ampl, "") == TestStatus.Abort)
                    return GracefulExit(TestStatus.Abort);
                ProgressBar2_Update();
            }

            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF1C, RX2", true);
                else
                    cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF1C, RX3", true);
                foreach (int ampl in amplList)
                {
                    if (Measure(testName, ampl, "  Path 2") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);
                    ProgressBar2_Update();
                }
            }

            /// -------------------------------------------------------------
            testName = "RF2COM_RX";

            if (ConnectionMessage(testName) == TestStatus.Abort)
                return GracefulExit(TestStatus.Abort);
            InitMeasureSettings();

            cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF2C, RX1", true);
            foreach (int ampl in amplList)
            {
                if (Measure(testName, ampl, "") == TestStatus.Abort)
                    return GracefulExit(TestStatus.Abort);
                ProgressBar2_Update();
            }

            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF2C, RX2", true);
                else
                    cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF2C, RX3", true);
                foreach (int ampl in amplList)
                {
                    if (Measure(testName, ampl, "  Path 2") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);
                    ProgressBar2_Update();
                }
            }

            /// -------------------------------------------------------------
            if (numOfFrontEnds > 1)
            {
                testName = "RF3COM_RX";

                if (ConnectionMessage(testName) == TestStatus.Abort)
                    return GracefulExit(TestStatus.Abort);
                InitMeasureSettings();

                cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF3C, RX2", true);
                foreach (int ampl in amplList)
                {
                    if (Measure(testName, ampl, "  Path 3") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);
                    ProgressBar2_Update();
                }

                cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF3C, RX4", true);
                foreach (int ampl in amplList)
                {
                    if (Measure(testName, ampl, "  Path 4") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);
                    ProgressBar2_Update();
                }

                /// -------------------------------------------------------------
                testName = "RF4COM_RX";

                if (ConnectionMessage(testName) == TestStatus.Abort)
                    return GracefulExit(TestStatus.Abort);
                InitMeasureSettings();

                cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF4C, RX2", true);
                foreach (int ampl in amplList)
                {
                    if (Measure(testName, ampl, "  Path 3") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);
                    ProgressBar2_Update();
                }

                cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF4C, RX4", true);
                foreach (int ampl in amplList)
                {
                    if (Measure(testName, ampl, "  Path 4") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);
                    ProgressBar2_Update();
                }
            }

        ///
        /// :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        /// 
       
        //fml
        gentests:

            SetHead1Text("GPRF CW Generator Tests");
            AddToResults(Environment.NewLine + Environment.NewLine + "GPRF CW Generator Tests");

            chartLimits3 = (",-0.8,-0.6,0,0.6,0.8");
            chartLimits6 = (",-1.4,-1.2,0,1.2,1.4");
            amplList = new int[] { -8, -44 };
#if DEBUG
            amplList = new int[] { -36 };
#endif

            testName = "RF1COM_TX";

            if (ConnectionMessage(testName) == TestStatus.Abort)
                return GracefulExit(TestStatus.Abort);

            cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF1C, TX1", true);
            foreach (int ampl in amplList)
            {
                if (Measure(testName, ampl, "") == TestStatus.Abort)
                    return GracefulExit(TestStatus.Abort);
                ProgressBar2_Update();
            }

            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF1C, TX2", true);
                else
                    cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF1C, TX3", true);
                foreach (int ampl in amplList)
                {
                    if (Measure(testName, ampl, "  Path 2") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);
                    ProgressBar2_Update();
                }
            }

            /// -------------------------------------------------------------
            chartLimits3 = (",-1.0,-0.8,0,0.8,1.0");
            chartLimits6 = (",-1.8,-1.6,0,1.6,1.8");
            amplList = new int[] { 0, -36 };
#if DEBUG
            return GracefulExit(TestStatus.Complete);
            amplList = new int[] { -36 };
#endif

            testName = "RF1OUT_TX";

            if (ConnectionMessage(testName) == TestStatus.Abort)
                return GracefulExit(TestStatus.Abort);

            cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF1O, TX1", true);
            foreach (int ampl in amplList)
            {
                if (Measure(testName, ampl, "") == TestStatus.Abort)
                    return GracefulExit(TestStatus.Abort);
                ProgressBar2_Update();
            }

            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF1O, TX2", true);
                else
                    cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF1O, TX3", true);
                foreach (int ampl in amplList)
                {
                    if (Measure(testName, ampl, "  Path 2") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);
                    ProgressBar2_Update();
                }
            }

            /// -------------------------------------------------------------
            chartLimits3 = (",-0.8,-0.6,0,0.6,0.8");
            chartLimits6 = (",-1.4,-1.2,0,1.2,1.4");
            amplList = new int[] { -8, -44 };
#if DEBUG
            amplList = new int[] { -44 };
#endif

            testName = "RF2COM_TX";

            if (ConnectionMessage(testName) == TestStatus.Abort)
                return GracefulExit(TestStatus.Abort);

            cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF2C, TX1", true);
            foreach (int ampl in amplList)
            {
                if (Measure(testName, ampl, "") == TestStatus.Abort)
                    return GracefulExit(TestStatus.Abort);
                ProgressBar2_Update();
            }

            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF2C, TX2", true);
                else
                    cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF2C, TX3", true);
                foreach (int ampl in amplList)
                {
                    if (Measure(testName, ampl, "  Path 2") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);
                    ProgressBar2_Update();
                }
            }

            /// -------------------------------------------------------------
            if (numOfFrontEnds > 1)
            {
                chartLimits3 = (",-0.8,-0.6,0,0.6,0.8");
                chartLimits6 = (",-1.4,-1.2,0,1.2,1.4");
                amplList = new int[] { -8, -44 };
//#if DEBUG
//                amplList = new int[] { -44 };
//#endif

                testName = "RF3COM_TX";

                if (ConnectionMessage(testName) == TestStatus.Abort)
                    return GracefulExit(TestStatus.Abort);

                cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF3C, TX2", true);
                foreach (int ampl in amplList)
                {
                    if (Measure(testName, ampl, "  Path 3") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);
                    ProgressBar2_Update();
                }

                cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF3C, TX4", true);
                foreach (int ampl in amplList)
                {
                    if (Measure(testName, ampl, "  Path 4") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);
                    ProgressBar2_Update();
                }

                /// -------------------------------------------------------------
                chartLimits3 = (",-1.0,-0.8,0,0.8,1.0");
                chartLimits6 = (",-1.8,-1.6,0,1.6,1.8");
                amplList = new int[] { 0, -36 };
#if DEBUG
                amplList = new int[] { -36 };
#endif

                testName = "RF3OUT_TX";

                if (ConnectionMessage(testName) == TestStatus.Abort)
                    return GracefulExit(TestStatus.Abort);

                cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF3O, TX2", true);
                foreach (int ampl in amplList)
                {
                    if (Measure(testName, ampl, "  Path 3") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);
                    ProgressBar2_Update();
                }

                cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF3O, TX4", true);
                foreach (int ampl in amplList)
                {
                    if (Measure(testName, ampl, "  Path 4") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);
                    ProgressBar2_Update();
                }

            /// -------------------------------------------------------------
                chartLimits3 = (",-0.8,-0.6,0,0.6,0.8");
                chartLimits6 = (",-1.4,-1.2,0,1.2,1.4");
                amplList = new int[] { -8, -44 };
#if DEBUG
                amplList = new int[] { -44 };
#endif

                testName = "RF4COM_TX";

                if (ConnectionMessage(testName) == TestStatus.Abort)
                    return GracefulExit(TestStatus.Abort);

                cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF4C, TX2", true);
                foreach (int ampl in amplList)
                {
                    if (Measure(testName, ampl, "  Path 3") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);
                    ProgressBar2_Update();
                }

                cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF4C, TX4", true);
                foreach (int ampl in amplList)
                {
                    if (Measure(testName, ampl, "  Path 4") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);
                    ProgressBar2_Update();
                }
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
            AddToResults(Environment.NewLine + testHeader);

            ProgressBar1_Settings(hasKB036 ? 59 : 32);

        start:

            int pointsCount = 0;
            double maxError3 = 0.0;
            double maxError6 = 0.0;

            _csvStream = OpenTempFile();
            if (_csvStream == null)
                return TestStatus.Abort;

            #region Config RX / TX
            ///// setup sensor to read
            cmw.Write("CONFigure:GPRF:MEAS:EPSensor:REPetition SINGleshot; " +
                      "TOUT 15; ATTenuation:STATe OFF; RESolution PD2", true);

            ///// setup measurement tests
            if (testName.Contains("RX"))
            {
                _csvStream.WriteLine("    GPRF CW Measurement Tests - " + cmwID);
                cmw.Write("INIT:GPRF:MEAS:POWer", true);
                cmw.Write("CONFigure:GPRF:MEAS:RFSettings:ENPower " + testAmpl, true);
                if (testName.Contains("1COM") || testName.Contains("2COM"))
                    cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF1O, TX1", true);
                else
                    cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF3O, TX2", true);
                cmw.Write("SOURce:GPRF:GEN:RFSettings:LEVel " + (testAmpl + 6.5), true);
            }
            else if (testName.Contains("TX"))
            {
                _csvStream.WriteLine("    GPRF CW Generator Tests - " + cmwID);

             //   int statsCount = (testAmpl == -44) ? 2 : 1;
                int statsCount = 1; //// fml
                AddToResults("" + testAmpl + ", " + statsCount);
                cmw.Write("CONFigure:GPRF:MEAS:EPSensor:SCOunt " + statsCount, true);
                cmw.Write("SOURce:GPRF:GEN:RFSettings:LEVel " + testAmpl, true);
                minFreq = 70;
            }

            _csvStream.WriteLine("0," + chartLimits3);
            cmw.Write("SOURce:GPRF:GEN:STATe ON", true);

            currentFreq = minFreq * (long)1e6;
            endFreq = hasKB036 ? (long)6000e6 : (long)3300e6;
            #endregion

            do  ///// Main Loop
            {
                while (_parent.PauseTesting == true && _cts.IsCancellationRequested == false)
                    Thread.Sleep(500);

                if (_cts.IsCancellationRequested)
                    return TestStatus.Abort;

                #region Set up this loop - set freqs - get GPRF Measure Power
                pointsCount += 1;
                SetHead2Text((currentFreq / 1e6).ToString() + " MHz");

                cmw.Write("SOURce:GPRF:GEN:RFSettings:FREQuency " + currentFreq, true);
                cmw.Write("CONFigure:GPRF:MEAS:EPSensor:FREQuency " + currentFreq, true);
                if (testName.Contains("RX"))
                {
                    cmw.Write("CONFigure:GPRF:MEAS:RFSettings:FREQuency " + currentFreq, true);
                    QuerySTB("READ:GPRF:MEAS:POWer:AVERage?", 5000, out visaResponse);
                    try
                    {
                        cmwMeasPower = Convert.ToDouble(visaResponse.Split(',')[1]);
                    }
                    catch (Exception e)
                    {
                        ModalMessageBox(e.Message, e.GetType().ToString());
                    }
                }
                #endregion

                #region Take sensor reading
                do  //while (retry)
                {
                    retry = false;
                    QuerySTB("READ:GPRF:MEAS:EPSensor?", 10000, out visaResponse);
                    try
                    {
                        pmResponse = visaResponse.Split(',');
                        int.TryParse(pmResponse[0], out pmStatus);
                        double.TryParse(pmResponse[2], out pmPower);
                    }
                    catch (Exception e)
                    {
                        ModalMessageBox(e.Message, e.GetType().ToString());
                    }

                    //if (pmStatus == 1)
                    //{
                    //    retry = true;
                    //    ModalMessageBox("Measurement Timeout");
                    //}
                    //else 
                    if (pmStatus != 0)
                    {
                        cmw.Write("SOURce:GPRF:GEN:STATe OFF", true);

                        while (_parent.PauseTesting)
                        {
                            Thread.Sleep(100);
                            if (_cts.IsCancellationRequested)
                                return TestStatus.Abort;
                        }
                        ModalMessageBox("Re-check connections using the following diagram.", "Test Setup",
                                     MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                        var btnCancelEnabled = _parent.GetBtnCancelEnabled();
                        _parent.SetBtnCancelEnabled(false);
                        var img = new ConnectionImageForm(MessageBoxButtons.RetryCancel);
                        img.SetImage(testName + "-" + numOfFrontEnds);
                        while (_parent.PauseTesting)
                        {
                            Thread.Sleep(100);
                            if (_cts.IsCancellationRequested)
                                return TestStatus.Abort;
                        }
                        img.ShowDialog();
                        _parent.SetBtnCancelEnabled(btnCancelEnabled);

                        if (img.DialogResult == DialogResult.Abort)
                            return TestStatus.Abort;

                        retry = (img.DialogResult == DialogResult.Retry);
                        cmw.Write("SOURce:GPRF:GEN:STATe ON", true);
                    }
                } while (retry);

                if (testName.Contains("RX"))
                    amplError = cmwMeasPower - pmPower;
                else
                    amplError = pmPower - testAmpl;
                #endregion

                #region Handle excessive error
                // If error is excessive, assume improper connections and prompt to fix.
                if ((currentFreq <= 200e6) && (Math.Abs(amplError) > 3) && !_ignoreAmplError)
                {
                    cmw.Write("SOURce:GPRF:GEN:STATe OFF", true);
                    cmw.Write("SYSTem:MEASurement:ALL:OFF", true);

                    ModalMessageBox("Re-check connections using the following diagram.", "Test Setup",
                                     MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                    var btnCancelEnabled = _parent.GetBtnCancelEnabled();
                    _parent.SetBtnCancelEnabled(false);
                    var img = new ConnectionImageForm(MessageBoxButtons.AbortRetryIgnore);
                    img.SetImage(testName + "-" + numOfFrontEnds);
                    img.ShowDialog();
                    _parent.SetBtnCancelEnabled(btnCancelEnabled);

                    //DialogResult resp = ModalMessageBox("(Retry) after fixing the connections" + Environment.NewLine +
                    //                                    "(Ignore) further level errors and continue test" + Environment.NewLine +
                    //                                    "(Abort) all testing",
                    //                                    "MEASURING - Check Connections",
                    //                                     MessageBoxButtons.AbortRetryIgnore,
                    //                                     MessageBoxIcon.Question,
                    //                                     MessageBoxDefaultButton.Button3);

                    _ignoreAmplError = (img.DialogResult == DialogResult.Ignore);

                    if (img.DialogResult == DialogResult.Abort)
                        return TestStatus.Abort;

                    if (img.DialogResult == DialogResult.Retry)
                    {
                        if (File.Exists(csvFileName))
                            try
                            { _csvStream.Dispose();
                              File.Delete(csvFileName); }
                            catch { ModalMessageBox("Temp file delete Exception"); }
                        goto start;
                    }

                    if (_ignoreAmplError)
                        cmw.Write("SOURce:GPRF:GEN:STATe ON", true);
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

                ProgressBar1_Update();
                #endregion

            } while (currentFreq <= endFreq);

            AddToResults(string.Format("Max error {0} MHz to 3.3 GHz: {1} dB", minFreq, maxError3.ToString("F2")));
            if (hasKB036)
                AddToResults(string.Format("Max error 3.3 GHz to 6 GHz: {0} dB", maxError6.ToString("F2")));

            #region Cleanup - close files - create graph
            ///// Set instruments to standby.
            cmw.Write("SOURce:GPRF:GEN:STATe OFF", true);
            cmw.Write("SYSTem:MEASurement:ALL:OFF", true);

            // Push one frequency point beyond test to make whitespace in graph (3400 or 6100 MHz).
            _csvStream.WriteLine(currentFreq / 1e6 + "," + chartLimits);
            _csvStream.WriteLine(testHeader);
            _csvStream.Dispose();

            SetHead2Text("");

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
                cmw.Write("*RST;*CLS", true);
                cmw.Write("*ESE 1", true);
                cmw.ErrorChecking();

                var btnCancelEnabled = _parent.GetBtnCancelEnabled();
                _parent.SetBtnCancelEnabled(false);
                var img = new ConnectionImageForm(MessageBoxButtons.OKCancel);
                img.SetImage(connection + "-" + numOfFrontEnds);
                img.ShowDialog();
                _parent.SetBtnCancelEnabled(btnCancelEnabled);
                if (img.DialogResult == DialogResult.Abort)
                    return TestStatus.Abort;

                QuerySTB("READ:GPRF:MEAS:EPSensor?", 1000000000, out visaResponse);
                try
                {
                    pmResponse = visaResponse.Split(',');
                    int.TryParse(pmResponse[0], out pmStatus);
                }
                catch (Exception e)
                {
                    ModalMessageBox(e.Message, e.GetType().ToString());
                }

                if (pmStatus == 0 || pmStatus == 4)
                {
                    SetHead2Text("Zeroing Sensor...");
#if !DEBUG
                    cmw.Write("ABORt:GPRF:MEAS:EPSensor", true);
                    WriteSTB("CALibration:GPRF:MEAS:EPSensor:ZERO", 20000);
                    QuerySTB("CALibration:GPRF:MEAS:EPSensor:ZERO?", 20000, out visaResponse);
#endif
#if DEBUG
                    visaResponse = "PASS";
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
                            return TestStatus.Abort;
                    }
                }
                else if (pmStatus == 27)
                {
                    var verifyConnection = ModalMessageBox("Ensure an NRP sensor is connected to the SENSOR port." + Environment.NewLine + Environment.NewLine +
                                                 "(Retry) after verifying the connection." + Environment.NewLine +
                                                 "(Cancel) all testing.",
                                                 "Sensor Status Error",
                                                  MessageBoxButtons.RetryCancel,
                                                  MessageBoxIcon.Exclamation,
                                                  MessageBoxDefaultButton.Button1);

                    if (verifyConnection == DialogResult.Retry)
                        retryZero = true;
                    else
                        return TestStatus.Abort;
                }
            } while (retryZero);

            _ignoreAmplError = false;

           SetHead2Text("");
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
                    cmw.Write("*RST;*CLS", true);
                    cmw.Write("*ESE 1", true);
                    cmw.ErrorChecking();
                }
                catch (Exception e)
                {
                    ModalMessageBox(String.Format("Error initializing the session:\n{0}", e.Message), e.GetType().ToString());
                    return TestStatus.Abort;
                }
            }
            else
            {
                AddToResults("No resource selected.");
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
                ModalMessageBox(String.Format("Error identifying instrument:\n{0}", e.Message), e.GetType().ToString());
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
                ModalMessageBox("DUT Not Yet Covered under this procedure.");
                return TestStatus.Abort;
            }
            else
            {
                ModalMessageBox("This is not a CMW.");
                return TestStatus.Abort;
            }

            cmwID = cmwModel + " " + cmwSerNum;
            AddToResults(cmwID);

            // CMW Options
            visaResponse = cmw.QueryString("SYSTem:BASE:OPTion:LIST? HWOPtion");
            AddToResults(visaResponse);
            hwOptions = visaResponse.Split(',');

            for (int i = 0; i < hwOptions.Length; i++)
            {
                hasKB036 = hwOptions[i].Contains("KB036");
                if (hwOptions[i].Contains("H570"))
                    numOfTRX++;
                if (hwOptions[i].Contains("H590"))
                    numOfFrontEnds++;
            }
            AddToResults("hasKB036: " + hasKB036.ToString());
            AddToResults("numOfTRX: " + numOfTRX.ToString());
            AddToResults("numOfFrontEnds: " + numOfFrontEnds.ToString());

#if DEBUG
            hasKB036 = false;
            //numOfFrontEnds = 1;
#endif

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
                ModalMessageBox(e.Message, e.GetType().ToString());
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
            cmw.Write("CONFigure:GPRF:MEAS:POWer:MODE POWer; SCOunt 50; SLENgth 1000e-6; MLENgth 950e-6", true);
            cmw.Write("TRIGger:GPRF:MEAS:POWer:OFFSet 10e-6", true);
        }

        private TestStatus GracefulExit(TestStatus exitStatus)
        {
            _parent.SetBtnCancelEnabled(false);
            SetHead1Text("");
            SetHead2Text("");

            try
            {
                cmw.Clear();
                cmw.Write("*RST;*CLS", true);
                cmw.Write("*ESE 1", true);
                cmw.ErrorChecking();
                cmw.Dispose();
            }
            catch (NullReferenceException) { }

            if (_csvStream != null)
                try
                {
                    _csvStream.Dispose();
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

            try
            {
                _cts.Dispose();
                _parent.SetBtnBeginEnabled(true);
            }
            catch (Exception exc)
            {
                ModalMessageBox(exc.Message, exc.GetType().ToString());
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
                ModalMessageBox(e.Message, e.GetType().ToString());
            }
            catch (InstrumentOPCtimeoutException e)
            {
                ModalMessageBox(e.Message, e.GetType().ToString());
            }
            catch (Ivi.Visa.VisaException e)
            {
                ModalMessageBox(e.Message, e.GetType().ToString());
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
                ModalMessageBox(e.Message, e.GetType().ToString());
            }
            catch (InstrumentOPCtimeoutException e)
            {
                ModalMessageBox(e.Message, e.GetType().ToString());
            }
            catch (Ivi.Visa.VisaException e)
            {
                ModalMessageBox(e.Message, e.GetType().ToString());
            }
        }

        private void ProgressBar1_Update()
        {
            _parent.Invoke((MethodInvoker)(() =>
            {
                _parent.progressBar1.Increment(1);
            }));
        }

        private void ProgressBar2_Update()
        {
            _parent.Invoke((MethodInvoker)(() =>
            {
                _parent.progressBar2.Increment(1);
            }));
        }

        private void ProgressBar1_Settings(int maxValue)
        {
            _parent.Invoke((MethodInvoker)(() =>
            {
                _parent.progressBar1.Maximum = maxValue;
                _parent.progressBar1.Value = 0;
            }));
        }

        private void ProgressBar2_Settings(int maxValue)
        {
            _parent.Invoke((MethodInvoker)(() =>
            {
                _parent.progressBar2.Maximum = maxValue;
                _parent.progressBar2.Value = 0;
            }));
        }

        private void SetHead1Text(string text)
        {
            _parent.Invoke((MethodInvoker)(() =>
            {
                _parent.labelHead1.Text = text;
            }));
        }

        private void SetHead2Text(string text)
        {
            _parent.Invoke((MethodInvoker)(() =>
            {
                _parent.labelHead2.Text = text;
            }));
        }

        private void AddToResults(string item)
        {
            _parent.Invoke((MethodInvoker)(() =>
            {
                _parent.textBoxResults.AppendText(item + Environment.NewLine);
            }));
        }

        private DialogResult ModalMessageBox(string message, string title = "",
                                     MessageBoxButtons buttons = MessageBoxButtons.OK,
                                     MessageBoxIcon icon = MessageBoxIcon.None,
                                     MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
        {
            DialogResult result = DialogResult.OK;
            _parent.Invoke((MethodInvoker)(() =>
            {
                var btnCancelEnabled = _parent.GetBtnCancelEnabled();
                _parent.SetBtnCancelEnabled(false);
                result = MessageBox.Show(message, title, buttons, icon, defaultButton);
                _parent.SetBtnCancelEnabled(btnCancelEnabled);
            }));


            //if (_parent.InvokeRequired)
            //{
            //    _parent.Invoke((Action)delegate {
            //        var btnCancelEnabled = _parent.GetBtnCancelEnabled();
            //        _parent.SetBtnCancelEnabled(false);
            //        result = MessageBox.Show(message, title, buttons, icon, defaultButton);
            //        _parent.SetBtnCancelEnabled(btnCancelEnabled);
            //    });
            //}
            //else
            //{
            //    var btnCancelEnabled = _parent.GetBtnCancelEnabled();
            //    _parent.SetBtnCancelEnabled(false);
            //    result = MessageBox.Show(message, title, buttons, icon, defaultButton);
            //    _parent.SetBtnCancelEnabled(btnCancelEnabled);
            //}
            return result;
        }
    }
}
