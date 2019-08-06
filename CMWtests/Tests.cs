using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using CMWgraph;

namespace CMWtests
{
    public partial class MainForm
    {
        public enum TestStatus : int { Abort = -1, Success, InProgress, Complete };

        private int numOfFrontEnds = 0;
        private int numOfTRX = 0;
        private long minFreq = 0;
        public static long currentFreq = 0;
        private bool hasKB036 = false;
        private bool ignoreAmplError = false;
/// !
#if !DEBUG
        private bool isFirstTest = true;
#else
        private bool isFirstTest = false;
#endif
        private string chartLimits3 = "";
        private string chartLimits6 = "";
        private string cmwID = "";
        private string csvFileName = "";
        private string testHeader = "";
        private StreamWriter csvStream = null;
        private TestStatus _status = TestStatus.Complete;
        private TestStatus Status
        {
            get
            {
                return _status; 
            }
            set
            {
                if (value != _status)
                {
                    _status = value;
#if DEBUG
                    SetStatusLabel(_status.ToString());
#endif
                }
            }
        }
        private VisaIO cmw = null;

        private TestStatus Begin()
        {
            Status = TestStatus.InProgress;
            CancelTesting = false;

            if (ConnectIdentifyDUT() == TestStatus.Abort)
              return GracefulExit(TestStatus.Abort);

            Status = Sequencer();
            
            SetBtnBeginEnabled(true);
            return Status;
        }

        private TestStatus Sequencer()
        {
            int[] amplList = { };
            string testName = "";

            SetBtnCancelEnabled(true);
            ProgressBar1_Init(12 * numOfTRX * (hasKB036 ? 60 : 33));
/// !
#if !DEBUG
            goto gentests;
#endif
            SetHead1Text("GPRF CW Measurement Tests");
            AddToResults(Environment.NewLine + "GPRF CW Measurement Tests");

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
                if (Measure(testName, ampl, "") == TestStatus.Abort)
                   return GracefulExit(TestStatus.Abort);

            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF1C, RX2", true);
                else
                    cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF1C, RX3", true);
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "  Path 2") == TestStatus.Abort)
                       return GracefulExit(TestStatus.Abort);
            }
#if DEBUG
            goto gentests;
#endif
            /// -------------------------------------------------------------
            testName = "RF2COM_RX";

            if (ConnectionMessage(testName) == TestStatus.Abort)
               return GracefulExit(TestStatus.Abort);
            InitMeasureSettings();

            cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF2C, RX1", true);
            foreach (int ampl in amplList)
                if (Measure(testName, ampl, "") == TestStatus.Abort)
                   return GracefulExit(TestStatus.Abort);

            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF2C, RX2", true);
                else
                    cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF2C, RX3", true);
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "  Path 2") == TestStatus.Abort)
                       return GracefulExit(TestStatus.Abort);
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
                    if (Measure(testName, ampl, "  Path 3") == TestStatus.Abort)
                       return GracefulExit(TestStatus.Abort);

                cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF3C, RX4", true);
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "  Path 4") == TestStatus.Abort)
                       return GracefulExit(TestStatus.Abort);

                /// -------------------------------------------------------------
                testName = "RF4COM_RX";

                if (ConnectionMessage(testName) == TestStatus.Abort)
                   return GracefulExit(TestStatus.Abort);
                InitMeasureSettings();

                cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF4C, RX2", true);
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "  Path 3") == TestStatus.Abort)
                       return GracefulExit(TestStatus.Abort);

                cmw.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF4C, RX4", true);
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "  Path 4") == TestStatus.Abort)
                       return GracefulExit(TestStatus.Abort);
            }

        //
        // :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // 

#if DEBUG
        gentests:
#endif
            SetHead1Text("GPRF CW Generator Tests");
            AddToResults(Environment.NewLine + "GPRF CW Generator Tests");

            chartLimits3 = (",-0.8,-0.6,0,0.6,0.8");
            chartLimits6 = (",-1.4,-1.2,0,1.2,1.4");
            amplList = new int[] { -8, -44 };
#if DEBUG
            amplList = new int[] { -8 };
#endif

            testName = "RF1COM_TX";

            if (ConnectionMessage(testName) == TestStatus.Abort)
               return GracefulExit(TestStatus.Abort);

            cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF1C, TX1", true);
            foreach (int ampl in amplList)
                if (Measure(testName, ampl, "") == TestStatus.Abort)
                   return GracefulExit(TestStatus.Abort);

            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF1C, TX2", true);
                else
                    cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF1C, TX3", true);
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "  Path 2") == TestStatus.Abort)
                       return GracefulExit(TestStatus.Abort);
            }

            /// -------------------------------------------------------------
            chartLimits3 = (",-1.0,-0.8,0,0.8,1.0");
            chartLimits6 = (",-1.8,-1.6,0,1.6,1.8");
            amplList = new int[] { 0, -36 };
#if DEBUG
            amplList = new int[] { 0 };
            //return GracefulExit(TestStatus.Complete);
#endif

            testName = "RF1OUT_TX";

            if (ConnectionMessage(testName) == TestStatus.Abort)
               return GracefulExit(TestStatus.Abort);

            cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF1O, TX1", true);
            foreach (int ampl in amplList)
                if (Measure(testName, ampl, "") == TestStatus.Abort)
                   return GracefulExit(TestStatus.Abort);

            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF1O, TX2", true);
                else
                    cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF1O, TX3", true);
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "  Path 2") == TestStatus.Abort)
                       return GracefulExit(TestStatus.Abort);
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
                if (Measure(testName, ampl, "") == TestStatus.Abort)
                   return GracefulExit(TestStatus.Abort);

            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF2C, TX2", true);
                else
                    cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF2C, TX3", true);
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "  Path 2") == TestStatus.Abort)
                       return GracefulExit(TestStatus.Abort);
            }

            /// -------------------------------------------------------------
            if (numOfFrontEnds > 1)
            {
                chartLimits3 = (",-0.8,-0.6,0,0.6,0.8");
                chartLimits6 = (",-1.4,-1.2,0,1.2,1.4");
                amplList = new int[] { -8, -44 };
#if DEBUG
                amplList = new int[] { -44 };
#endif

                testName = "RF3COM_TX";

                if (ConnectionMessage(testName) == TestStatus.Abort)
                   return GracefulExit(TestStatus.Abort);

                cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF3C, TX2", true);
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "  Path 3") == TestStatus.Abort)
                       return GracefulExit(TestStatus.Abort);

                cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF3C, TX4", true);
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "  Path 4") == TestStatus.Abort)
                       return GracefulExit(TestStatus.Abort);

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
                    if (Measure(testName, ampl, "  Path 3") == TestStatus.Abort)
                       return GracefulExit(TestStatus.Abort);

                cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF3O, TX4", true);
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "  Path 4") == TestStatus.Abort)
                       return GracefulExit(TestStatus.Abort);

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
                    if (Measure(testName, ampl, "  Path 3") == TestStatus.Abort)
                       return GracefulExit(TestStatus.Abort);

                cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF4C, TX4", true);
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "  Path 4") == TestStatus.Abort)
                       return GracefulExit(TestStatus.Abort);
            }

            return GracefulExit(TestStatus.Complete);
        }

        private TestStatus Measure(string testName, int testAmpl, string path)
        {
            int pmStatus = -1;
            double amplError = 0.0;
            double cmwMeasPower = 0.0;
            double pmPower = 0.0;
            bool retry = false;
            string chartLimits = "";
            string visaResponse = "";
            string[] pmResponse = { };

            SetDebugText("Waiting at Measure Start");
            mreMeasure.WaitOne();
            if (CancelTesting == true)
                return TestStatus.Abort;

            testHeader = testName.Split('_')[0] + " @ " + testAmpl + " dBm  " + path;
            AddToResults(Environment.NewLine + testHeader);

        start:
            double maxError3 = 0.0;
            double maxError6 = 0.0;

            csvStream = OpenTempFile();
            if (csvStream == null)
                return TestStatus.Abort;

            #region Config RX / TX

            /// setup sensor to read
            cmw.Write("CONFigure:GPRF:MEAS:EPSensor:REPetition SINGleshot; TOUT 15; " +
                      "RESolution PD2; SCOunt " + OptionsForm.StatsCount + "; ATTenuation:STATe OFF");

            ///// setup measurement tests
            if (testName.Contains("RX"))
            {
                csvStream.WriteLine("    GPRF CW Measurement Tests - " + cmwID);
                cmw.Write("INIT:GPRF:MEAS:POWer");
                cmw.Write("CONFigure:GPRF:MEAS:RFSettings:ENPower " + testAmpl);
                if (testName.Contains("1COM") || testName.Contains("2COM"))
                    cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF1O, TX1", true);
                else
                    cmw.Write("ROUTe:GPRF:GEN:SCENario:SALone RF3O, TX2", true);
                cmw.Write("SOURce:GPRF:GEN:RFSettings:LEVel " + (testAmpl + 6.5));
            }
            else if (testName.Contains("TX"))
            {
                csvStream.WriteLine("    GPRF CW Generator Tests - " + cmwID);
                cmw.Write("SOURce:GPRF:GEN:RFSettings:LEVel " + testAmpl);
                minFreq = 70;
            }

            csvStream.WriteLine("0," + chartLimits3);
            cmw.Write("SOURce:GPRF:GEN:STATe ON", true);

            currentFreq = minFreq * (long)1e6;
            var endFreq = hasKB036 ? (long)6000e6 : (long)3300e6;

            string chart;
            if (hasKB036)
                chart = chartLimits6;
            else
                chart = chartLimits3;
            CreateGraph("Graph_" + (Convert.ToDouble(chart.Split(',')[5]) * 10));
            #endregion

            do  ///// Main Loop
            {
                SetDebugText("Waiting at Measure main loop start");
                mreMeasure.WaitOne();
                Status = TestStatus.InProgress;
                if (CancelTesting == true)
                    return TestStatus.Abort;

                #region Set up this loop - set freqs - get GPRF Measure Power
                ProgressBar1_Update(testAmpl);

                SetHead2Text((currentFreq / 1e6).ToString() + " MHz");

                cmw.Write("SOURce:GPRF:GEN:RFSettings:FREQuency " + currentFreq);
                cmw.Write("CONFigure:GPRF:MEAS:EPSensor:FREQuency " + currentFreq);
                if (testName.Contains("RX"))
                {
                    cmw.Write("CONFigure:GPRF:MEAS:RFSettings:FREQuency " + currentFreq);
                    visaResponse = cmw.QueryWithSTB("READ:GPRF:MEAS:POWer:AVERage?", 5000);
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

                #region  Take sensor reading
                do  //while (retry)
                {
                    SetDebugText("Waiting at Take sensor reading");
                    mreMeasure.WaitOne();
                    if (CancelTesting == true)
                        return TestStatus.Abort;

                    retry = false;
                    visaResponse = cmw.QueryWithSTB("READ:GPRF:MEAS:EPSensor?", 20000);
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

                        ModalMessageBox("Recheck connections using the following diagram.", "Test Setup",
                                     MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        var btnCancelEnabled = GetBtnCancelEnabled();
                        SetBtnCancelEnabled(false);

                        var img = new ConnectionImageForm(MessageBoxButtons.RetryCancel);
                        img.SetImage(testName + "_" + numOfFrontEnds);
                        Invoke(new MethodInvoker(() => img.ShowDialog(this)));

                        SetBtnCancelEnabled(btnCancelEnabled);

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
                if ((currentFreq <= 400e6) && (Math.Abs(amplError) > 3) && !ignoreAmplError)
                {
                    cmw.Write("SOURce:GPRF:GEN:STATe OFF", true);
                    cmw.Write("SYSTem:MEASurement:ALL:OFF", true);

                    ModalMessageBox("Recheck connections using the following diagram.", "Test Setup",
                                     MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    var btnCancelEnabled = GetBtnCancelEnabled();
                    SetBtnCancelEnabled(false);
                    var img = new ConnectionImageForm(MessageBoxButtons.AbortRetryIgnore);
                    img.SetImage(testName + "_" + numOfFrontEnds);
                    Invoke(new MethodInvoker(() => img.ShowDialog(this)));
                    SetBtnCancelEnabled(btnCancelEnabled);

                    //DialogResult resp = ModalMessageBox("(Retry) after fixing the connections" + Environment.NewLine +
                    //                                    "(Ignore) further level errors and continue test" + Environment.NewLine +
                    //                                    "(Abort) all testing",
                    //                                    "MEASURING - Check Connections",
                    //                                     MessageBoxButtons.AbortRetryIgnore,
                    //                                     MessageBoxIcon.Question,
                    //                                     MessageBoxDefaultButton.Button3);

                    ignoreAmplError = (img.DialogResult == DialogResult.Ignore);

                    if (img.DialogResult == DialogResult.Abort)
                        return TestStatus.Abort;

                    if (img.DialogResult == DialogResult.Retry)
                    {
                        if (File.Exists(csvFileName))
                            try
                            { csvStream.Dispose();
                                File.Delete(csvFileName); }
                            catch { ModalMessageBox("Temp file delete Exception"); }
                        goto start;
                    }

                    if (ignoreAmplError)
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
                csvStream.WriteLine(currentFreq / 1e6 + "," + amplError + chartLimits);
                PlotPoint(currentFreq, amplError);

                // If current frequency is the minimum measurement frequency of CMW,
                // then the next freq is 200 MHz, otherwise it is increased by 100 MHz
                if (currentFreq == minFreq * 1e6)
                    currentFreq = (long)200e6;
                else
                    currentFreq += (long)100e6;
            #endregion

            } while (currentFreq <= endFreq);

            AddToResults(string.Format("Max error {0} MHz to 3.3 GHz: {1} dB", minFreq, maxError3.ToString("F2")));
            if (hasKB036)
                AddToResults(string.Format("Max error 3.3 GHz to 6 GHz: {0} dB", maxError6.ToString("F2")));

            #region Cleanup - close files - create graph
            /// Set instruments to standby.
            cmw.Write("SOURce:GPRF:GEN:STATe OFF", true);
            cmw.Write("SYSTem:MEASurement:ALL:OFF", true);

            // Push one frequency point beyond test to make whitespace in graph (3400 or 6100 MHz).
            csvStream.WriteLine(currentFreq / 1e6 + "," + chartLimits);
            csvStream.WriteLine(testHeader);
            csvStream.Dispose();

            SetHead2Text("");

            // maxErr tells graph to decide whether to use fixed Y-axis (2 dB),
            //   or dynamic axis if error exceeds 2 dB.
            var maxError = Math.Max(Math.Abs(maxError3), Math.Abs(maxError6));

            // Create Excel graph
            ExcelGraph.Create(cmwID, csvFileName, (hasKB036 ? 60 : 33), maxError, isFirstTest);
            File.Delete(csvFileName);

            isFirstTest = false;

            // Suppress connection error message until the next connection change.
            ignoreAmplError = true;

            return TestStatus.Success;
            #endregion
        }

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

                SetDebugText("Waiting at ConnectionMessage start");
                mreMeasure.WaitOne();
                if (CancelTesting == true)
                    return TestStatus.Abort;

                cmw.Write("*RST");
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
                cmw.Write("ABORt:GPRF:MEAS:EPSensor;:CALibration:GPRF:MEAS:EPSensor:ZERO");
                var visaResponse = cmw.QueryWithSTB("CALibration:GPRF:MEAS:EPSensor:ZERO?", 20000);
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
            int pmStatus = 0;
            bool retrySensor = false;
            string cmwModel = "";
            string cmwSerNum = "";
            string[] identFields = { };

            numOfFrontEnds = 0;
            numOfTRX = 0;

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
            var visaResponse = cmw.QueryString("*IDN?");
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
                ModalMessageBox("DUT not yet covered under this procedure.");
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
            visaResponse = cmw.QueryString("SYSTem:BASE:OPTion:LIST? HWOPtion");
#if DEBUG
            AddToResults(visaResponse);
#endif
            var hwOptions = visaResponse.Split(',');

            for (int i = 0; i < hwOptions.Length; i++)
            {
                hasKB036 = hwOptions[i].Contains("KB036");
#if DEBUG
                //hasKB036 = false;
#endif
                if (hwOptions[i].Contains("H570"))
                    numOfTRX++;
                if (hwOptions[i].Contains("H590"))
                    numOfFrontEnds++;
            }
#if DEBUG
            AddToResults("hasKB036: " + hasKB036.ToString());
            AddToResults("numOfTRX: " + numOfTRX.ToString());
            AddToResults("numOfFrontEnds: " + numOfFrontEnds.ToString());
#endif
            // Check Sensor connection
            SetHead2Text("Please wait...");
            do
            {
/// !
#if !DEBUG
                visaResponse = cmw.QueryWithSTB("READ:GPRF:MEAS:EPSensor?", 15000);
#else
                visaResponse = "0,0";
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
                        return TestStatus.Abort;
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
            cmw.Write("CONFigure:GPRF:MEAS:POWer:MODE POWer; SCOunt 50; SLENgth 1000e-6; MLENgth 950e-6");
            cmw.Write("TRIGger:GPRF:MEAS:POWer:OFFSet 10e-6");
        }

        private TestStatus GracefulExit(TestStatus exitStatus)
        {
            SetBtnCancelEnabled(false);
            SetHead1Text("");
            SetHead2Text("");

            try
            {
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

            SetBtnBeginEnabled(true);

            if (exitStatus == TestStatus.Abort)
            {
                AddToResults(Environment.NewLine + "Tests Aborted.");
                ProgressBar1_Init();
            }
            else if (exitStatus == TestStatus.Complete)
            {
                AddToResults(Environment.NewLine + "Tests Complete.");
            }

            Status = TestStatus.Complete;
            mreExit.Set();
            return Status;
        }

        private DialogResult ModalMessageBox(string message, string title = "",
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
