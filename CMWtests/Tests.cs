using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CMWgraph;
using RsVisaLoader;

namespace CMWtests
{
    public class Tests
    {

        private ViSession session = null;
        private ViStatus status = 0;
        private int vi = 0;

        public enum TestStatus : int { Abort = -1, Success, Completed };
        private TestStatus exitStatus = TestStatus.Success;
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
            TestStatus exitStatus;

            exitStatus = Sequencer();

            if (exitStatus == TestStatus.Abort)
                _parent.AddToResults(Environment.NewLine + "Tests Aborted.");
            else if (exitStatus == TestStatus.Success)
                _parent.AddToResults(Environment.NewLine + "Tests Complete.");
        }

        private TestStatus Sequencer()
        {
            int[] amplList = { };
            string testName = "";

            if (ConnectIdentifyDUT() == TestStatus.Abort)
            {
                _parent.SetBtnBeginEnabled(true);
                return TestStatus.Abort;
            }

            _parent.SetBtnCancelEnabled(true);
            _parent.SetHead1Text("GPRF CW Measurement Tests");

            /// -------------------------------------------------------------
            chartLimits3 = ",-0.7,-0.5,0,0.5,0.7";
            chartLimits6 = ",-1.2,-1.0,0,1.0,1.2";
            amplList = new int[] { 0 };//, -8 };//, -20 };

            testName = "RF1COM_RX";
            if (ConnectionMessage(testName) == TestStatus.Abort)
                return GracefulExit();

            InitMeasureSettings();

            session.Write(vi, "ROUTe:GPRF:MEAS:SCENario:SALone RF1C, RX1");
            foreach (int ampl in amplList)
                if (Measure(testName, ampl, "") == TestStatus.Abort)
                    return GracefulExit();



            /////
            return GracefulExit();

            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    session.Write(vi, "ROUTe:GPRF:MEAS:SCENario:SALone RF1C, RX2");
                else
                    session.Write(vi, "ROUTe:GPRF:MEAS:SCENario:SALone RF1C, RX3");
                foreach (int ampl in amplList)
                    Measure(testName, ampl, "Path 2");
            }

            /// -------------------------------------------------------------
            testName = "RF2COM_RX";
            if (ConnectionMessage(testName) == TestStatus.Abort)
                return GracefulExit();
            InitMeasureSettings();

            session.Write(vi, "ROUTe:GPRF:MEAS:SCENario:SALone RF2C, RX1");
            foreach (int ampl in amplList)
                Measure(testName, ampl, "");

            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    session.Write(vi, "ROUTe:GPRF:MEAS:SCENario:SALone RF2C, RX2");
                else
                    session.Write(vi, "ROUTe:GPRF:MEAS:SCENario:SALone RF2C, RX3");
                foreach (int ampl in amplList)
                    Measure(testName, ampl, "Path 2");
            }

            /// -------------------------------------------------------------
            if (numOfFrontEnds > 1)
            {
                testName = "RF3COM_RX";
                if (ConnectionMessage(testName) == TestStatus.Abort)
                    return GracefulExit();
                InitMeasureSettings();

                session.Write(vi, "ROUTe:GPRF:MEAS:SCENario:SALone RF3C, RX2");
                foreach (int ampl in amplList)
                    Measure(testName, ampl, "Path 3");

                session.Write(vi, "ROUTe:GPRF:MEAS:SCENario:SALone RF3C, RX4");
                foreach (int ampl in amplList)
                    Measure(testName, ampl, "Path 4");

                /// -------------------------------------------------------------
                testName = "RF4COM_RX";
                if (ConnectionMessage(testName) == TestStatus.Abort)
                    return GracefulExit();
                InitMeasureSettings();

                session.Write(vi, "ROUTe:GPRF:MEAS:SCENario:SALone RF4C, RX2");
                foreach (int ampl in amplList)
                    Measure(testName, ampl, "Path 3");

                session.Write(vi, "ROUTe:GPRF:MEAS:SCENario:SALone RF4C, RX4");
                foreach (int ampl in amplList)
                    Measure(testName, ampl, "Path 4");
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
                return GracefulExit();

            session.Write(vi, "ROUTe:GPRF:MEAS:SCENario:SALone RF1C, TX1");
            foreach (int ampl in amplList)
                Measure(testName, ampl, "");

            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    session.Write(vi, "ROUTe:GPRF:MEAS:SCENario:SALone RF1C, TX2");
                else
                    session.Write(vi, "ROUTe:GPRF:MEAS:SCENario:SALone RF1C, TX3");
                foreach (int ampl in amplList)
                    Measure(testName, ampl, "Path 2");
            }

            /// -------------------------------------------------------------
            chartLimits3 = (",-1.0,-0.8,0,0.8,1.0");
            chartLimits6 = (",-1.8,-1.6,0,1.6,1.8");
            amplList = new int[] { -0, -36 };

            testName = "RF1OUT_TX";
            if (ConnectionMessage(testName) == TestStatus.Abort)
                return GracefulExit();

            session.Write(vi, "ROUTe:GPRF:MEAS:SCENario:SALone RF1O, TX1");
            foreach (int ampl in amplList)
                Measure(testName, ampl, "");

            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    session.Write(vi, "ROUTe:GPRF:MEAS:SCENario:SALone RF1O, TX2");
                else
                    session.Write(vi, "ROUTe:GPRF:MEAS:SCENario:SALone RF1O, TX3");
                foreach (int ampl in amplList)
                    Measure(testName, ampl, "Path 2");
            }

            /// -------------------------------------------------------------
            chartLimits3 = (",-0.8,-0.6,0,0.6,0.8");
            chartLimits6 = (",-1.4,-1.2,0,1.2,1.4");
            amplList = new int[] { -8, -44 };

            testName = "RF2COM_TX";
            if (ConnectionMessage(testName) == TestStatus.Abort)
                return GracefulExit();

            session.Write(vi, "ROUTe:GPRF:MEAS:SCENario:SALone RF2C, TX1");
            foreach (int ampl in amplList)
                Measure(testName, ampl, "");

            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    session.Write(vi, "ROUTe:GPRF:MEAS:SCENario:SALone RF2C, TX2");
                else
                    session.Write(vi, "ROUTe:GPRF:MEAS:SCENario:SALone RF2C, TX3");
                foreach (int ampl in amplList)
                    Measure(testName, ampl, "Path 2");
            }

            /// -------------------------------------------------------------
            if (numOfFrontEnds > 1)
            {
                chartLimits3 = (",-0.8,-0.6,0,0.6,0.8");
                chartLimits6 = (",-1.4,-1.2,0,1.2,1.4");
                amplList = new int[] { -8, -44 };

                testName = "RF3COM_TX";
                if (ConnectionMessage(testName) == TestStatus.Abort)
                    return GracefulExit();

                session.Write(vi, "ROUTe:GPRF:MEAS:SCENario:SALone RF3C, TX2");
                foreach (int ampl in amplList)
                    Measure(testName, ampl, "Path 3");

                session.Write(vi, "ROUTe:GPRF:MEAS:SCENario:SALone RF3C, TX4");
                foreach (int ampl in amplList)
                    Measure(testName, ampl, "Path 4");

                /// -------------------------------------------------------------
                chartLimits3 = (",-1.0,-0.8,0,0.8,1.0");
                chartLimits6 = (",-1.8,-1.6,0,1.6,1.8");
                amplList = new int[] { -0, -36 };

                testName = "RF3OUT_TX";
                if (ConnectionMessage(testName) == TestStatus.Abort)
                    return GracefulExit();

                session.Write(vi, "ROUTe:GPRF:MEAS:SCENario:SALone RF3O, TX2");
                foreach (int ampl in amplList)
                    Measure(testName, ampl, "Path 3");

                session.Write(vi, "ROUTe:GPRF:MEAS:SCENario:SALone RF3O, TX4");
                foreach (int ampl in amplList)
                    Measure(testName, ampl, "Path 4");

                /// -------------------------------------------------------------
                chartLimits3 = (",-0.8,-0.6,0,0.6,0.8");
                chartLimits6 = (",-1.4,-1.2,0,1.2,1.4");
                amplList = new int[] { -8, -44 };

                testName = "RF4COM_TX";
                if (ConnectionMessage(testName) == TestStatus.Abort)
                    return GracefulExit();

                session.Write(vi, "ROUTe:GPRF:MEAS:SCENario:SALone RF4C, TX2");
                foreach (int ampl in amplList)
                    Measure(testName, ampl, "Path 3");

                session.Write(vi, "ROUTe:GPRF:MEAS:SCENario:SALone RF4C, TX4");
                foreach (int ampl in amplList)
                    Measure(testName, ampl, "Path 4");
            }
            return TestStatus.Success;
        }

        private TestStatus Measure(string testName, int testAmpl, string path)
        {
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
            session.Write(vi, "CONFigure:GPRF:MEAS:EPSensor:REPetition SINGleshot");
            session.Write(vi, "CONFigure:GPRF:MEAS:EPSensor:TOUT 3");
            session.Write(vi, "CONFigure:GPRF:MEAS:EPSensor:SCOunt 1");
            session.Write(vi, "CONFigure:GPRF:MEAS:EPSensor:ATTenuation:STATe OFF");
            session.Write(vi, "CONFigure:GPRF:MEAS:EPSensor:RESolution PD2");

            ///// setup measurement tests
            if (testName.Contains("RX"))
            {
                _csvStream.WriteLine("    GPRF CW Measurement Tests - " + cmwID);
                session.Write(vi, "INIT:GPRF:MEAS:POWer");
                session.Write(vi, "CONFigure:GPRF:MEAS:RFSettings:ENPower " + testAmpl);
                if (testName.Contains("1") || testName.Contains("2"))
                    session.Write(vi, "ROUTe:GPRF:GEN:SCENario:SALone RF1O, TX1");
                else
                    session.Write(vi, "ROUTe:GPRF:GEN:SCENario:SALone RF3O, TX1");
                session.Write(vi, "SOURce:GPRF:GEN:RFSettings:LEVel " + (testAmpl + 6.5));
            }
            else if (testName.Contains("TX"))
            {
                _csvStream.WriteLine("    GPRF CW Generator Tests - " + cmwID);
                session.Write(vi, "SOURce:GPRF:GEN:RFSettings:LEVel " + testAmpl);
                minFreq = 70;
            }

            _csvStream.WriteLine("0," + chartLimits3);
            session.Write(vi, "SOURce:GPRF:GEN:STATe ON");

            currentFreq = minFreq * (long)1e6;
            if (hasKB036)
                endFreq = (long)6000e6;
            else
                endFreq = (long)3300e6;
            #endregion

            MessageBox.Show("tol set for 0.003");
            do  ///// Main Loop
            {
                if (_cts.IsCancellationRequested)
                    return GracefulExit();

                #region Set up loop
                pointsCount += 1;
                _parent.SetHead2Text((currentFreq / 1e6).ToString() + " MHz");

                session.Write(vi, "SOURce:GPRF:GEN:RFSettings:FREQuency " + currentFreq);
                session.Write(vi, "CONFigure:GPRF:MEAS:EPSensor:FREQuency " + currentFreq);
                if (testName.Contains("RX"))
                {
                    session.Write(vi, "CONFigure:GPRF:MEAS:RFSettings:FREQuency " + currentFreq);
                    status = session.Query(vi, "READ:GPRF:MEAS:POWer:AVERage?", out visaResponse);
                    try
                    {
                        cmwMeasPower = Convert.ToDouble(visaResponse.Split(',')[1]);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, e.GetType().ToString());
                    }
                }
                #endregion

                #region Take sensor reading
                do
                {
                    retry = false;
                    status = session.Query(vi, "READ:GPRF:MEAS:EPSensor?", out visaResponse);
                    if (status < ViStatus.VI_SUCCESS) ShowErrorText("Measure.SensorReading", status);

                    try
                    {
                        visaResponse.Split('~');
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        exitStatus = TestStatus.Abort;
                        return GracefulExit();
                    }

                    if (visaResponse.Split(',')[2].Contains("INV") ||
                        visaResponse.Split(',')[2].Contains("NAV"))
                    {
                        session.Write(vi, "SOURce:GPRF:GEN:STATe OFF");

                        MessageBox.Show("Re-check connections using the following diagram.",
                                        "Test Setup",
                                         MessageBoxButtons.OK,
                                         MessageBoxIcon.Asterisk);

                        var img = new ConnectionImageForm(MessageBoxButtons.RetryCancel);
                        img.SetImage(testName + "-" + numOfFrontEnds);
                        img.ShowDialog();

                        if (img.DialogResult == DialogResult.Abort)
                            return TestStatus.Abort;

                        retry = (img.DialogResult == DialogResult.Retry);
                        session.Write(vi, "SOURce:GPRF:GEN:STATe ON");
                    }
                } while (retry);

                try
                {
                    pmPower = Convert.ToDouble(visaResponse.Split(',')[2]);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, e.Source);
                }

                if (testName.Contains("RX"))
                    amplError = cmwMeasPower - pmPower;
                else
                    amplError = pmPower - testAmpl;
                #endregion

                #region Handle excessive error
                // If error is excessive, assume improper connections and prompt to fix.
                if ((currentFreq <= 200e6) && (Math.Abs(amplError) > 3) && !_ignoreAmplError)
                {
                    session.Write(vi, "SOURce:GPRF:GEN:STATe OFF");
                    session.Write(vi, "SYSTem:MEASurement:ALL:OFF");

                    MessageBox.Show("Re-check connections using the following diagram.",
                                    "Test Setup",
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Asterisk);

                    var img = new ConnectionImageForm(MessageBoxButtons.AbortRetryIgnore);
                    img.SetImage(testName + "-" + numOfFrontEnds);
                    img.ShowDialog();

                    DialogResult resp = MessageBox.Show("(Retry) after fixing the connections" + Environment.NewLine +
                                                        "(Ignore) further level errors and continue test" + Environment.NewLine +
                                                        "(Abort) all testing",
                                                        "MEASURING - Check Connections",
                                                         MessageBoxButtons.AbortRetryIgnore,
                                                         MessageBoxIcon.Question,
                                                         MessageBoxDefaultButton.Button3);

                    _ignoreAmplError = (img.DialogResult == DialogResult.Ignore);

                    if (img.DialogResult == DialogResult.Abort)
                        return TestStatus.Abort;

                    if (img.DialogResult == DialogResult.Retry)
                        goto start;

                    if (_ignoreAmplError)
                        session.Write(vi, "SOURce:GPRF:GEN:STATe ON");
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
            session.Write(vi, "SOURce:GPRF:GEN:STATe OFF");
            session.Write(vi, "SYSTem:MEASurement:ALL:OFF");

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
            bool retryZero = false;
            string visaResponse = "";

            _parent.SetBtnCancelEnabled(false);

            do
            {
                retryZero = false;
                status = session.Query(vi, "*RST;*OPC?", out visaResponse);
                if (status < ViStatus.VI_SUCCESS) ShowErrorText("Connection.*RST, status", status);
                status = session.Query(vi, "*CLS;*OPC?", out visaResponse);
                if (status < ViStatus.VI_SUCCESS) ShowErrorText("Connection.*CLS, status", status);

                var img = new ConnectionImageForm(MessageBoxButtons.OKCancel);
                img.SetImage(connection + "-" + numOfFrontEnds);
                img.ShowDialog();
                if (img.DialogResult == DialogResult.Abort)
                    return TestStatus.Abort;

                _parent.SetHead2Text("Zeroing Sensor...");
                status = session.Query(vi, "CALibration:GPRF:MEAS:EPSensor:ZERO", out visaResponse);
                if (status < ViStatus.VI_SUCCESS) ShowErrorText("Zero.", status);


                Thread.Sleep(10000);
                status = session.Query(vi, "CALibration:GPRF:MEAS:EPSensor:ZERO?", out visaResponse);
                if (status < ViStatus.VI_SUCCESS) ShowErrorText("Zero?", status);
                //visaResponse = "PASS";


                if (!visaResponse.Contains("PASS"))
                {
                    var result = MessageBox.Show("Ensure sensor is not connected to an active source." + Environment.NewLine + Environment.NewLine +
                                                 "(Retry) after verifying the connections" + Environment.NewLine +
                                                 "(Cancel) all testing",
                                                 "Sensor Zero Failure",
                                                  MessageBoxButtons.RetryCancel,
                                                  MessageBoxIcon.Exclamation,
                                                  MessageBoxDefaultButton.Button1);

                    retryZero = (result == DialogResult.Retry);
                    if (result == DialogResult.Cancel)
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
            string visaResponse = "";
            string[] identFields = { };
            string[] hwOptions = { };
            string resource = "";

            session = new ViSession();

            var resForm = new VISAresourceForm(session.ResourceMgr);
            resForm.ShowDialog();
            resource = resForm.Resource;
            if (resForm.Status == TestStatus.Abort || resource == null)
            {
                MessageBox.Show("No resource selected.");
                return TestStatus.Abort;
            }
            status = session.OpenSession(resource, out vi);
            if (status < ViStatus.VI_SUCCESS)
            {
                ShowErrorText("ConnectIdentify.OpenSession", status);
                return TestStatus.Abort;
            }
            resForm.Dispose();

            // CMW Identification
            status = session.Query(vi, "*RST;*OPC?", out visaResponse);
            if (status < ViStatus.VI_SUCCESS) ShowErrorText("ConnectIdentify.*RST", status);
            status = session.Query(vi, "*CLS;*OPC?", out visaResponse);
            if (status < ViStatus.VI_SUCCESS) ShowErrorText("ConnectIdentify.*CLS", status);

            session.Query(vi, "*IDN?", out visaResponse);
            identFields = visaResponse.Split(',');

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
                MessageBox.Show("DUT Not Yet Covered under this procedure.");
                return TestStatus.Abort;
            }
            else
            {
                MessageBox.Show("This is not a CMW.");
                return TestStatus.Abort;
            }

            cmwID = cmwModel + " " + cmwSerNum;
            _parent.AddToResults(cmwID);

            // CMW Options
            session.Query(vi, "SYSTem:BASE:OPTion:LIST? HWOPtion", out visaResponse);
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
                MessageBox.Show(e.Message, "IO Exception");
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
            session.Write(vi, "CONFigure:GPRF:MEAS:POWer:MODE POWer");
            session.Write(vi, "CONFigure:GPRF:MEAS:POWer:SCOunt 50");
            session.Write(vi, "CONFigure:GPRF:MEAS:POWer:SLENgth 1000e-6");
            session.Write(vi, "CONFigure:GPRF:MEAS:POWer:MLENgth 950e-6");
            session.Write(vi, "TRIGger:GPRF:MEAS:POWer:OFFSet 10e-6");
        }

        private TestStatus GracefulExit()
        {
            if (exitStatus == TestStatus.Completed)
                return TestStatus.Completed;

            _parent.SetBtnCancelEnabled(false);
            _parent.SetHead1Text("");
            _parent.SetHead2Text("");

            //_parent.AddToResults("In graceful");

            status = session.Query(vi, "*RST;*OPC?", out string visaResponse);
            if (status < ViStatus.VI_SUCCESS) ShowErrorText("GracefulExit.*RST", status);
            status = session.Query(vi, "*CLS;*OPC?", out visaResponse);
            if (status < ViStatus.VI_SUCCESS) ShowErrorText("GracefulExit.*CLS", status);

            status = session.CloseSession(vi);
            if (status < ViStatus.VI_SUCCESS) ShowErrorText("GracefulExit.CloseSesion", status);
            session.CloseResMgr();

            if (_csvStream != null)
                try { _csvStream.Dispose(); }
                catch { MessageBox.Show("Dispose csvStream Exception"); }

            if (File.Exists(csvFileName))
                try { File.Delete(csvFileName); }
                catch { MessageBox.Show("Temp file delete Exception"); }

            try
            {
                _cts.Dispose();
                _parent.SetBtnBeginEnabled(true);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, exc.GetType().ToString());
            }

            exitStatus = TestStatus.Completed;
            return TestStatus.Abort;
        }

        private void ShowErrorText(string source, ViStatus status)
        {
            StringBuilder text = new StringBuilder(visa32.VI_FIND_BUFLEN);
            ViStatus err = visa32.viStatusDesc(session.ResourceMgr, status, text);
            _parent.AddToResults(Environment.NewLine + source + Environment.NewLine + text.ToString());
        }
    }
}
