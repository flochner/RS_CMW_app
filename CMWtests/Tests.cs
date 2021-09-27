using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using CMWgraph;

namespace CMWtests
{
    public partial class MainForm
    {
        private enum TestStatus : int { Abort = -1, Success, InProgress, Complete };

        private int numOfFrontEnds = 0;
        private int numOfTRX = 0;
        private int minRecvFreq = 70;
        private bool hasKB036 = false;
        private bool ignoreAmplError = false;
        private bool isFirstTest = true;
        private string chartLimits3 = "";
        private string chartLimits6 = "";
        private string cmwID = "";
        private string csvFileName = "";
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
                    SetStatusText(_status.ToString());
                }
            }
        }
        private VisaIO cmw = null;

        private TestStatus Begin()
        {
            Thread.CurrentThread.Name = "Tests";
            Status = TestStatus.InProgress;
            CancelTesting = false;

            if (ConnectIdentifyDUT() == TestStatus.Abort)
                return GracefulExit(TestStatus.Abort);

            if (CheckSensor() == TestStatus.Abort)
                return GracefulExit(TestStatus.Abort);

            if (tempGauge.Start(cmw) == false)
                return GracefulExit(TestStatus.Abort);
            
            Status = Sequencer();
            
            return Status;
        }

        private TestStatus Sequencer()
        {
            int[] amplList = { };
            string testName = "";

            ProgressBar1_Init(12 * numOfTRX * (hasKB036 ? 60 : 33));
#if DEBUG
            //goto meastests;
#endif
            SetHead1Text("GPRF CW Generator Tests");
            AddToResults(Environment.NewLine + "GPRF CW Generator Tests");

            /// -------------------------------------------------------------
            chartLimits3 = (",-0.8,-0.6,0,0.6,0.8");
            chartLimits6 = (",-1.4,-1.2,0,1.2,1.4");
            amplList = new int[] { -8, -44 };
#if DEBUG
            //amplList = new int[] { -8 };
#endif

            testName = "RF1COM_TX";

            if (ConnectionMessage(testName) == TestStatus.Abort)
               return GracefulExit(TestStatus.Abort);

            Write(cmw, "ROUTe:GPRF:GEN:SCENario:SALone RF1C, TX1");
            foreach (int ampl in amplList)
                if (Measure(testName, ampl, "") == TestStatus.Abort)
                   return GracefulExit(TestStatus.Abort);

            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    Write(cmw, "ROUTe:GPRF:GEN:SCENario:SALone RF1C, TX2");
                else
                    Write(cmw, "ROUTe:GPRF:GEN:SCENario:SALone RF1C, TX3");
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "  Path 2") == TestStatus.Abort)
                       return GracefulExit(TestStatus.Abort);
            }
#if DEBUG
            goto meastests;
#endif
            /// -------------------------------------------------------------
            chartLimits3 = (",-1.0,-0.8,0,0.8,1.0");
            chartLimits6 = (",-1.8,-1.6,0,1.6,1.8");
            amplList = new int[] { 0, -36 };
#if DEBUG
            amplList = new int[] { 0 };
#endif

            testName = "RF1OUT_TX";

            if (ConnectionMessage(testName) == TestStatus.Abort)
               return GracefulExit(TestStatus.Abort);

            Write(cmw, "ROUTe:GPRF:GEN:SCENario:SALone RF1O, TX1");
            foreach (int ampl in amplList)
                if (Measure(testName, ampl, "") == TestStatus.Abort)
                   return GracefulExit(TestStatus.Abort);

            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    Write(cmw, "ROUTe:GPRF:GEN:SCENario:SALone RF1O, TX2");
                else
                    Write(cmw, "ROUTe:GPRF:GEN:SCENario:SALone RF1O, TX3");
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

            Write(cmw, "ROUTe:GPRF:GEN:SCENario:SALone RF2C, TX1");
            foreach (int ampl in amplList)
                if (Measure(testName, ampl, "") == TestStatus.Abort)
                   return GracefulExit(TestStatus.Abort);

            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    Write(cmw, "ROUTe:GPRF:GEN:SCENario:SALone RF2C, TX2");
                else
                    Write(cmw, "ROUTe:GPRF:GEN:SCENario:SALone RF2C, TX3");
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

                Write(cmw, "ROUTe:GPRF:GEN:SCENario:SALone RF3C, TX2");
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "  Path 3") == TestStatus.Abort)
                       return GracefulExit(TestStatus.Abort);

                Write(cmw, "ROUTe:GPRF:GEN:SCENario:SALone RF3C, TX4");
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

                Write(cmw, "ROUTe:GPRF:GEN:SCENario:SALone RF3O, TX2");
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "  Path 3") == TestStatus.Abort)
                       return GracefulExit(TestStatus.Abort);

                Write(cmw, "ROUTe:GPRF:GEN:SCENario:SALone RF3O, TX4");
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

                Write(cmw, "ROUTe:GPRF:GEN:SCENario:SALone RF4C, TX2");
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "  Path 3") == TestStatus.Abort)
                       return GracefulExit(TestStatus.Abort);

                Write(cmw, "ROUTe:GPRF:GEN:SCENario:SALone RF4C, TX4");
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "  Path 4") == TestStatus.Abort)
                       return GracefulExit(TestStatus.Abort);
            }

            //
            // :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            // 
#if DEBUG
        meastests:
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

            Write(cmw, "ROUTe:GPRF:MEAS:SCENario:SALone RF1C, RX1");
            foreach (int ampl in amplList)
                if (Measure(testName, ampl, "") == TestStatus.Abort)
                    return GracefulExit(TestStatus.Abort);

            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    Write(cmw, "ROUTe:GPRF:MEAS:SCENario:SALone RF1C, RX2");
                else
                    Write(cmw, "ROUTe:GPRF:MEAS:SCENario:SALone RF1C, RX3");
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "  Path 2") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);
            }

            /// -------------------------------------------------------------
            testName = "RF2COM_RX";

            if (ConnectionMessage(testName) == TestStatus.Abort)
                return GracefulExit(TestStatus.Abort);
            InitMeasureSettings();

            Write(cmw, "ROUTe:GPRF:MEAS:SCENario:SALone RF2C, RX1");
            foreach (int ampl in amplList)
                if (Measure(testName, ampl, "") == TestStatus.Abort)
                    return GracefulExit(TestStatus.Abort);

            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    Write(cmw, "ROUTe:GPRF:MEAS:SCENario:SALone RF2C, RX2");
                else
                    Write(cmw, "ROUTe:GPRF:MEAS:SCENario:SALone RF2C, RX3");
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

                Write(cmw, "ROUTe:GPRF:MEAS:SCENario:SALone RF3C, RX2");
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "  Path 3") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);

                Write(cmw, "ROUTe:GPRF:MEAS:SCENario:SALone RF3C, RX4");
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "  Path 4") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);

                /// -------------------------------------------------------------
                testName = "RF4COM_RX";

                if (ConnectionMessage(testName) == TestStatus.Abort)
                    return GracefulExit(TestStatus.Abort);
                InitMeasureSettings();

                Write(cmw, "ROUTe:GPRF:MEAS:SCENario:SALone RF4C, RX2");
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "  Path 3") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);

                Write(cmw, "ROUTe:GPRF:MEAS:SCENario:SALone RF4C, RX4");
                foreach (int ampl in amplList)
                    if (Measure(testName, ampl, "  Path 4") == TestStatus.Abort)
                        return GracefulExit(TestStatus.Abort);
            }

            return GracefulExit(TestStatus.Complete);
        }

        private TestStatus Measure(string testName, int testAmpl, string path)
        {
            int minFreq = 70;
            int pmStatus = -1;
            int adjRounds = 0;
            double amplError = 0.0;
            double correction = 0.0;
            double cmwGenPower = 0.0;
            double cmwMeasPower = 0.0;
            double pmPower = 0.0;
            long currentFreq = 0;
            bool retry = false;
            string chartLimits = "";
            string testHeader = "";
            string visaResponse = "";
            string[] pmResponse = { };

            //SetDebugText("Waiting at Measure Start");
            mreMeasure.WaitOne();
            if (CancelTesting == true)
                return TestStatus.Abort;

            SetBtnCancelEnabled(true);

            testHeader = testName.Split('_')[0] + " @ " + testAmpl + " dBm  " + path;
            AddToResults(Environment.NewLine + testHeader);

        start:
            double maxError3 = 0.0;
            double maxError6 = 0.0;

            csvStream = OpenTempFile(out csvFileName);
            if (csvStream == null)
                return TestStatus.Abort;

            #region Config RX / TX

            /// setup sensor to read
            Write(cmw, "CONFigure:GPRF:MEAS:EPSensor:REPetition SINGleshot; TOUT 15; " +
                      "RESolution PD0; SCOunt " + OptionsForm.StatsCount + "; ATTenuation:STATe OFF");

            ///// setup measurement tests
            if (testName.Contains("RX"))
            {
                csvStream.WriteLine("    GPRF CW Measurement Tests - " + cmwID);
                Write(cmw, "INIT:GPRF:MEAS:POWer");
                Write(cmw, "CONFigure:GPRF:MEAS:RFSettings:ENPower 20");
                if (testName.Contains("1COM") || testName.Contains("2COM"))
                    Write(cmw, "ROUTe:GPRF:GEN:SCENario:SALone RF1O, TX1");
                else
                    Write(cmw, "ROUTe:GPRF:GEN:SCENario:SALone RF3O, TX2");
                cmwGenPower = testAmpl + 6.5;
                Write(cmw, "SOURce:GPRF:GEN:RFSettings:LEVel " + (cmwGenPower));
                minFreq = minRecvFreq;
            }
            else if (testName.Contains("TX"))
            {
                cmwGenPower = testAmpl;
                csvStream.WriteLine("    GPRF CW Generator Tests - " + cmwID);
                Write(cmw, "SOURce:GPRF:GEN:RFSettings:LEVel " + cmwGenPower);
            }

            csvStream.WriteLine("0," + chartLimits3);
            Write(cmw, "SOURce:GPRF:GEN:STATe ON");

            currentFreq = minFreq * (long)1e6;
            var endFreq = hasKB036 ? (long)6000e6 : (long)3300e6;

            string chart;
            if (hasKB036)
                chart = chartLimits6;
            else
                chart = chartLimits3;
            CreateGraph("Graph_" + (Convert.ToDouble(chart.Split(',')[5]) * 10), testHeader);
            #endregion

            do  ///// Main Loop
            {
                //SetDebugText("Waiting at Measure main loop start");
                mreMeasure.WaitOne();
                Status = TestStatus.InProgress;
                if (CancelTesting == true)
                    return TestStatus.Abort;

                #region Set up this loop - set freqs - get GPRF Measure Power
                ProgressBar1_Update(testAmpl);

                SetHead2Text((currentFreq / 1e6).ToString() + " MHz");

                Write(cmw, "SOURce:GPRF:GEN:RFSettings:FREQuency " + currentFreq);
                Write(cmw, "CONFigure:GPRF:MEAS:EPSensor:FREQuency " + currentFreq);
                if (testName.Contains("RX"))
                {
                    Write(cmw, "CONFigure:GPRF:MEAS:RFSettings:FREQuency " + currentFreq);
                    visaResponse = Query(cmw, "READ:GPRF:MEAS:POWer:AVERage?", 5000);
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
                    //SetDebugText("Waiting at Take sensor reading");
                    mreMeasure.WaitOne();
                    if (CancelTesting == true)
                        return TestStatus.Abort;

                    retry = false;
                    visaResponse = Query(cmw, "READ:GPRF:MEAS:EPSensor?", 20000);
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

                    if (pmStatus == 1)
                    {
                        retry = true;
                        ModalMessageBox("Measurement Timeout");
                    }
                    else if (pmStatus != 0)
                    {
                        Write(cmw, "SOURce:GPRF:GEN:STATe OFF");

                        ModalMessageBox("Recheck connections using the following diagram.", "Test Setup",
                                     MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        var img = new ConnectionImageForm(MessageBoxButtons.RetryCancel);
                        img.SetImage(testName + "_" + numOfFrontEnds);
                        Invoke(new MethodInvoker(() => img.ShowDialog(this)));

                        if (img.DialogResult == DialogResult.Abort)
                            return TestStatus.Abort;

                        retry = (img.DialogResult == DialogResult.Retry);
                        Write(cmw, "SOURce:GPRF:GEN:STATe ON");
                    }
                } while (retry);

                if (testName.Contains("RX"))
                {
                    adjRounds = 0;
                    do
                    {
                        correction = testAmpl - pmPower;
                        cmwGenPower = Math.Min(cmwGenPower + correction, 13.0);
                        Write(cmw, "SOURce:GPRF:GEN:RFSettings:LEVel " + (cmwGenPower));
                        visaResponse = Query(cmw, "READ:GPRF:MEAS:EPSensor?", 20000);
                        try
                        {
                            pmResponse = visaResponse.Split(',');
                            double.TryParse(pmResponse[2], out pmPower);
                        }
                        catch (Exception e)
                        {
                            ModalMessageBox(e.Message, e.GetType().ToString());
                        }
                        adjRounds += 1;
                    } while (Math.Abs(testAmpl - pmPower) >= 0.01 && adjRounds <= 2);
                    SetDebugText("Rounds: " + adjRounds.ToString());
                }

                if (testName.Contains("RX"))
                    amplError = cmwMeasPower - pmPower;
                else
                    amplError = pmPower - testAmpl;
                #endregion

                #region Handle excessive error
                // If error is excessive, assume improper connections and prompt to fix.
                if ((currentFreq <= 400e6) && (Math.Abs(amplError) > 3) && !ignoreAmplError)
                {
                    Write(cmw, "SOURce:GPRF:GEN:STATe OFF");
                    Write(cmw, "SYSTem:MEASurement:ALL:OFF");

                    ModalMessageBox("Re-check connections using the following diagram.", "Test Setup",
                                     MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    var connection = new ConnectionImageForm(MessageBoxButtons.AbortRetryIgnore);
                    connection.SetImage(testName + "_" + numOfFrontEnds);
                    Invoke(new MethodInvoker(() => connection.ShowDialog(this)));

                    //DialogResult resp = ModalMessageBox("(Retry) after fixing the connections" + Environment.NewLine +
                    //                                    "(Ignore) further level errors and continue test" + Environment.NewLine +
                    //                                    "(Abort) all testing",
                    //                                    "MEASURING - Check Connections",
                    //                                     MessageBoxButtons.AbortRetryIgnore,
                    //                                     MessageBoxIcon.Question,
                    //                                     MessageBoxDefaultButton.Button3);

                    ignoreAmplError = (connection.DialogResult == DialogResult.Ignore);

                    if (connection.DialogResult == DialogResult.Abort)
                        return TestStatus.Abort;

                    if (connection.DialogResult == DialogResult.Retry)
                    {
                        if (File.Exists(csvFileName))
                            try
                            {
                                csvStream.Dispose();
                                File.Delete(csvFileName);
                            }
                            catch
                            {
                                ModalMessageBox("Temp file delete Exception");
                            }
                        goto start;
                    }

                    if (ignoreAmplError)
                        Write(cmw, "SOURce:GPRF:GEN:STATe ON");
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
            Write(cmw, "SOURce:GPRF:GEN:STATe OFF");
            Write(cmw, "SYSTem:MEASurement:ALL:OFF");

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

            SetBtnCancelEnabled(false);

            return TestStatus.Success;
            #endregion
        }
    }
}
