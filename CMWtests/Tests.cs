using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CMWgraph;

namespace CMWtests
{
    public partial class MainForm
    {
        private MBSession session = null;
        private int numOfFrontEnds = 0;
        private int numOfTRX = 0;
        private long minFreq = 0;
        private bool abort = false;
        private bool hasKB036 = false;
        private bool ignoreAmplError = false;
        private bool isFirstTest = true;
        private string chartLimits3 = null;
        private string chartLimits6 = null;
        private string cmwID = null;
        private string testName = null;

        private void SequencerAsync()
        {
            int[] amplList = null;
            string testHeader = null;

            ConnectIdentifyDUT();


            SetHead1Text("GPRF CW Measurement Tests");

            /// -------------------------------------------------------------
            chartLimits3 = ",-0.7,-0.5,0,0.5,0.7";
            chartLimits6 = ",-1.2,-1.0,0,1.0,1.2";
            amplList = new int[] { 0 }; //, -8, -20 };

            testName = "RF1COM_RX";
            Connection(testName);
            InitMeasureSettings();

            session.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF1C, RX1");
            foreach (int ampl in amplList)
            {
                testHeader = testName.Split('_')[0] + " @ " + ampl + " dBm";
                Measure(testHeader, ampl);
                //                // if (abort) return false;
            }



            /////
            return;// true;


            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    session.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF1C, RX2");
                else
                    session.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF1C, RX3");
                foreach (int ampl in amplList)
                {
                    testHeader = testName.Split('_')[0] + " @ " + ampl + " dBm  Path 2";
                    Measure(testHeader, ampl);
                    // if (abort) return false;
                }
            }

            /// -------------------------------------------------------------
            testName = "RF2COM_RX";
            Connection(testName);
            InitMeasureSettings();

            session.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF2C, RX1");
            foreach (int ampl in amplList)
            {
                testHeader = testName.Split('_')[0] + " @ " + ampl + " dBm";
                Measure(testHeader, ampl);
                // if (abort) return false;
            }

            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    session.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF2C, RX2");
                else
                    session.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF2C, RX3");
                foreach (int ampl in amplList)
                {
                    testHeader = testName.Split('_')[0] + " @ " + ampl + " dBm  Path 2";
                    Measure(testHeader, ampl);
                    // if (abort) return false;
                }
            }

            /// -------------------------------------------------------------
            if (numOfFrontEnds > 1)
            {
                testName = "RF3COM_RX";
                Connection(testName);
                InitMeasureSettings();

                session.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF3C, RX2");
                foreach (int ampl in amplList)
                {
                    testHeader = testName.Split('_')[0] + " @ " + ampl + " dBm  Path 3";
                    Measure(testHeader, ampl);
                    // if (abort) return false;
                }

                session.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF3C, RX4");
                foreach (int ampl in amplList)
                {
                    testHeader = testName.Split('_')[0] + " @ " + ampl + " dBm  Path 4";
                    Measure(testHeader, ampl);
                    // if (abort) return false;
                }

                /// -------------------------------------------------------------
                testName = "RF4COM_RX";
                Connection(testName);
                InitMeasureSettings();

                session.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF4C, RX2");
                foreach (int ampl in amplList)
                {
                    testHeader = testName.Split('_')[0] + " @ " + ampl + " dBm  Path 3";
                    Measure(testHeader, ampl);
                    // if (abort) return false;
                }

                session.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF4C, RX4");
                foreach (int ampl in amplList)
                {
                    testHeader = testName.Split('_')[0] + " @ " + ampl + " dBm  Path 4";
                    Measure(testHeader, ampl);
                    // if (abort) return false;
                }
            }

            ///
            /// :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            /// 

            SetHead1Text("GPRF CW Generator Tests");

            chartLimits3 = (",-0.8,-0.6,0,0.6,0.8");
            chartLimits6 = (",-1.4,-1.2,0,1.2,1.4");
            amplList = new int[] { -8, -44 };

            testName = "RF1COM_TX";
            Connection(testName);

            session.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF1C, TX1");
            foreach (int ampl in amplList)
            {
                testHeader = testName.Split('_')[0] + " @ " + ampl + " dBm";
                Measure(testHeader, ampl);
                // if (abort) return false;
            }

            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    session.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF1C, TX2");
                else
                    session.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF1C, TX3");
                foreach (int ampl in amplList)
                {
                    testHeader = testName.Split('_')[0] + " @ " + ampl + " dBm  Path 2";
                    Measure(testHeader, ampl);
                    // if (abort) return false;
                }
            }

            /// -------------------------------------------------------------
            chartLimits3 = (",-1.0,-0.8,0,0.8,1.0");
            chartLimits6 = (",-1.8,-1.6,0,1.6,1.8");
            amplList = new int[] { -0, -36 };

            testName = "RF1OUT_TX";
            Connection(testName);

            session.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF1O, TX1");
            foreach (int ampl in amplList)
            {
                testHeader = testName.Split('_')[0] + " @ " + ampl + " dBm";
                Measure(testHeader, ampl);
                // if (abort) return false;
            }

            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    session.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF1O, TX2");
                else
                    session.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF1O, TX3");
                foreach (int ampl in amplList)
                {
                    testHeader = testName.Split('_')[0] + " @ " + ampl + " dBm  Path 2";
                    Measure(testHeader, ampl);
                    // if (abort) return false;
                }
            }

            /// -------------------------------------------------------------
            chartLimits3 = (",-0.8,-0.6,0,0.6,0.8");
            chartLimits6 = (",-1.4,-1.2,0,1.2,1.4");
            amplList = new int[] { -8, -44 };

            testName = "RF2COM_TX";
            Connection(testName);

            session.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF2C, TX1");
            foreach (int ampl in amplList)
            {
                testHeader = testName.Split('_')[0] + " @ " + ampl + " dBm";
                Measure(testHeader, ampl);
                // if (abort) return false;
            }

            if (numOfTRX > 1)
            {
                if (numOfFrontEnds == 1)
                    session.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF2C, TX2");
                else
                    session.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF2C, TX3");
                foreach (int ampl in amplList)
                {
                    testHeader = testName.Split('_')[0] + " @ " + ampl + " dBm  Path 2";
                    Measure(testHeader, ampl);
                    // if (abort) return false;
                }
            }

            /// -------------------------------------------------------------
            if (numOfFrontEnds > 1)
            {
                chartLimits3 = (",-0.8,-0.6,0,0.6,0.8");
                chartLimits6 = (",-1.4,-1.2,0,1.2,1.4");
                amplList = new int[] { -8, -44 };

                testName = "RF3COM_TX";
                Connection(testName);

                session.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF3C, TX2");
                foreach (int ampl in amplList)
                {
                    testHeader = testName.Split('_')[0] + " @ " + ampl + " dBm  Path 3";
                    Measure(testHeader, ampl);
                    // if (abort) return false;
                }

                session.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF3C, TX4");
                foreach (int ampl in amplList)
                {
                    testHeader = testName.Split('_')[0] + " @ " + ampl + " dBm  Path 4";
                    Measure(testHeader, ampl);
                    // if (abort) return false;
                }

                /// -------------------------------------------------------------
                chartLimits3 = (",-1.0,-0.8,0,0.8,1.0");
                chartLimits6 = (",-1.8,-1.6,0,1.6,1.8");
                amplList = new int[] { -0, -36 };

                testName = "RF3OUT_TX";
                Connection(testName);

                session.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF3O, TX2");
                foreach (int ampl in amplList)
                {
                    testHeader = testName.Split('_')[0] + " @ " + ampl + " dBm  Path 3";
                    Measure(testHeader, ampl);
                    // if (abort) return false;
                }

                session.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF3O, TX4");
                foreach (int ampl in amplList)
                {
                    testHeader = testName.Split('_')[0] + " @ " + ampl + " dBm  Path 4";
                    Measure(testHeader, ampl);
                    // if (abort) return false;
                }

                /// -------------------------------------------------------------
                chartLimits3 = (",-0.8,-0.6,0,0.6,0.8");
                chartLimits6 = (",-1.4,-1.2,0,1.2,1.4");
                amplList = new int[] { -8, -44 };

                testName = "RF4COM_TX";
                Connection(testName);

                session.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF4C, TX2");
                foreach (int ampl in amplList)
                {
                    testHeader = testName.Split('_')[0] + " @ " + ampl + " dBm  Path 3";
                    Measure(testHeader, ampl);
                    // if (abort) return false;
                }

                session.Write("ROUTe:GPRF:MEAS:SCENario:SALone RF4C, TX4");
                foreach (int ampl in amplList)
                {
                    testHeader = testName.Split('_')[0] + " @ " + ampl + " dBm  Path 4";
                    Measure(testHeader, ampl);
                    // if (abort) return false;
                }
            }
            return;// true;
        }

        private void Measure(string testHeader, int testAmpl)
        {
            int pointsCount = 0;
            double amplError = 0.0;
            double cmwPower = 0.0;
            double maxError3 = 0.0;
            double maxError6 = 0.0;
            double pmPower = 0.0;
            double maxError = 0.0;
            long currentFreq = 0;
            long endFreq = 0;
            bool retry = false;
            string chartLimits = null;
            string csvfileName = null;
            string visaResponse = null;
            StreamWriter csvStream = null;

            AddToResults(Environment.NewLine + testHeader);

            csvfileName = GetTempFileName();
            csvStream = OpenTempFile(csvfileName);
            if (csvStream == null)
            {
                abort = true;
                return;
            }

            ///// setup sensor to read
            session.Write("CONFigure:GPRF:MEAS:EPSensor:REPetition SINGleshot");
            session.Write("CONFigure:GPRF:MEAS:EPSensor:TOUT 3");
            session.Write("CONFigure:GPRF:MEAS:EPSensor:SCOunt 1");
            session.Write("CONFigure:GPRF:MEAS:EPSensor:ATTenuation:STATe OFF");
            session.Write("CONFigure:GPRF:MEAS:EPSensor:RESolution PD2");

            ///// setup measurement tests
            if (testName.Contains("RX"))
            {
                csvStream.WriteLine("    GPRF CW Measurement Tests - " + cmwID);
                session.Write("INIT:GPRF:MEAS:POWer");
                session.Write("CONFigure:GPRF:MEAS:RFSettings:ENPower " + testAmpl);
                if (testName.Contains("1") || testName.Contains("2"))
                    session.Write("ROUTe:GPRF:GEN:SCENario:SALone RF1O, TX1");
                else
                    session.Write("ROUTe:GPRF:GEN:SCENario:SALone RF3O, TX1");
                session.Write("SOURce:GPRF:GEN:RFSettings:LEVel " + (testAmpl + 6.5));
            }
            else if (testName.Contains("TX"))
            {
                csvStream.WriteLine("    GPRF CW Generator Tests - " + cmwID);
                session.Write("SOURce:GPRF:GEN:RFSettings:LEVel " + testAmpl);
                minFreq = 70;
            }
            csvStream.WriteLine("0," + chartLimits3);
            session.Write("SOURce:GPRF:GEN:STATe ON");

            currentFreq = minFreq * (long)1e6;
            if (hasKB036)
                endFreq = (long)6000e6;
            else
                endFreq = (long)3300e6;
            
            do  ///// Main Loop
            {
                pointsCount += 1;
                SetHead2Text((currentFreq / 1e6).ToString() + " MHz");

                session.Write("SOURce:GPRF:GEN:RFSettings:FREQuency " + currentFreq);
                session.Write("CONFigure:GPRF:MEAS:EPSensor:FREQuency " + currentFreq);
                if (testName.Contains("RX"))
                {
                    session.Write("CONFigure:GPRF:MEAS:RFSettings:FREQuency " + currentFreq);
                    visaResponse = session.Query("READ:GPRF:MEAS:POWer:AVERage?", 5000);
                    cmwPower = Convert.ToDouble(visaResponse.Split(',')[1]);
                }

                do  ///// PM read
                {
                    visaResponse = session.Query("READ:GPRF:MEAS:EPSensor?", 5000);

                    if (visaResponse.Split(',')[2].Contains("INV"))
                    {
                        session.Write("SOURce:GPRF:GEN:STATe OFF");

                        MessageBox.Show("Re-check connections using the following diagram.");
                        var img = new ConnectionImageForm();
                        img.SetImage(testName + "-" + numOfFrontEnds);
                        img.ShowDialog();

                        string resp = MessageBox.Show("(Retry) after verifying the connections \n" +
                                                      "(Cancel) all testing",
                                                      "MEASURING - Check Connections",
                                                       MessageBoxButtons.RetryCancel,
                                                       MessageBoxIcon.Question,
                                                       MessageBoxDefaultButton.Button1).ToString();
                        abort = resp.Contains("Cancel");
                        retry = resp.Contains("Retry");

                        if (abort)
                        {
                            csvStream.Close();
                            return;
                        }
                        session.Write("SOURce:GPRF:GEN:STATe ON");
                    }
                } while (retry);
                retry = false;

                try
                {
                    pmPower = Convert.ToDouble(visaResponse.Split(',')[2]);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, e.Source);
                }

                if (testName.Contains("RX"))
                    amplError = cmwPower - pmPower;
                else
                    amplError = pmPower - testAmpl;

                // If error is excessive, assume improper connections and prompt to fix.
                if ((currentFreq < 300e6) && (Math.Abs(amplError) > 3) && !ignoreAmplError)
                {
                    session.Write("SOURce:GPRF:GEN:STATe OFF");
                    session.Write("SYSTem:MEASurement:ALL:OFF");

                    MessageBox.Show("Recheck connections using the following diagram.");
                    var img = new ConnectionImageForm();
                    img.SetImage(testName + "-" + numOfFrontEnds);
                    img.ShowDialog();

                    string resp = MessageBox.Show("(Retry) after fixing the connections \n" +
                                                  "(Ignore) further level errors and continue test \n" +
                                                  "(Abort) all testing",
                                                  "MEASURING - Check Connections",
                                                   MessageBoxButtons.AbortRetryIgnore,
                                                   MessageBoxIcon.Question,
                                                   MessageBoxDefaultButton.Button3).ToString();
                    ignoreAmplError = resp.Contains("Ignore");
                    abort = resp.Contains("Abort");
                    retry = resp.Contains("Retry");

                    if (ignoreAmplError)
                    {
                        session.Write("SOURce:GPRF:GEN:STATe ON");
                    }
                    else
                    {
                        csvStream.Close();
                        return;
                    }
                }

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

                // If current frequency is the minimum measurement frequency of CMW,
                // then the next freq is 200 MHz, otherwise it is increased by 100 MHz
                if (currentFreq == minFreq * 1e6)
                    currentFreq = (long)200e6;
                else
                    currentFreq += (long)100e6;

            } while (currentFreq <= endFreq);

            ///// Set instruments to standby.
            session.Write("SOURce:GPRF:GEN:STATe OFF");
            session.Write("SYSTem:MEASurement:ALL:OFF");

            // Push one frequency point beyond test to make whitespace in graph (0 MHz).
            csvStream.WriteLine(currentFreq / 1e6 + "," + chartLimits);
            csvStream.WriteLine(testHeader);
            csvStream.Close();

            SetHead2Text("");

            // maxErr tells graph to decide whether to use fixed Y-axis (2 dB),
            //   or dynamic axis if error exceeds 2 dB.
            maxError = Math.Max(Math.Abs(maxError3), Math.Abs(maxError6));

            // Create graph
            Graph.Create(cmwID, csvfileName, pointsCount, maxError, isFirstTest);

            isFirstTest = false;

            // Suppress connection error message until the next connection change.
            ignoreAmplError = true;

            abort = false;
            return;
        }

        public void Connection(string connection)
        {
            bool retryZero = false;
            string visaResponse = null;
            var img = new ConnectionImageForm();
            img.SetImage(connection + "-" + numOfFrontEnds);
            img.ShowDialog();

            do
            {
                session.Query("*RST; *OPC?", 4000);
                session.Query("*CLS; *OPC?", 4000);
                retryZero = abort = false;
                SetHead2Text("Zeroing Sensor...");
        //        session.Write("CALibration:GPRF:MEAS:EPSensor:ZERO", 20000);
        //        Thread.Sleep(5000);
                visaResponse = session.Query("CALibration:GPRF:MEAS:EPSensor:ZERO?", 20000);
                if (!visaResponse.Contains("PASS"))
                {

                    string resp = MessageBox.Show("Ensure sensor is not connected to an active source. \n\n" +
                                                  "(Retry) after verifying the connections \n" +
                                                  "(Cancel) all testing",
                                                  "Sensor Zero Failure",
                                                   MessageBoxButtons.RetryCancel,
                                                   MessageBoxIcon.Question,
                                                   MessageBoxDefaultButton.Button1).ToString();
                    retryZero = resp.Contains("Retry");
                    abort = resp.Contains("Cancel");
                }
            } while (retryZero);
            retryZero = false;

            SetHead2Text("");

            // if (abort) return;

            ignoreAmplError = false;
        }

        public bool ConnectIdentifyDUT()
        {
            string cmwModel = null;
            string cmwSerNum = null;
            string visaResponse = null;
            string[] identFields = null;
            string[] hwOptions = null;

            session = new MBSession("USB0::0x0AAD::0x0057::0142591::INSTR");

            //VISAresourceForm resource = new VISAresourceForm();
            //resource.ShowDialog();
            //string resourceString = resource.Selection;
            //resource.Dispose();
            //if (!string.Equals(resourceString, "No VISA resources found.") && resourceString != null)
            //{
            //    session = new MBSession(resourceString);
            //}
            //else
            //{
            //    AddToResults("No VISA resources found.");
            //    false;
            //}

            // CMW Identification
            session.Query("*RST; *OPC?", 4000);
            session.Query("*CLS; *OPC?", 4000);
            //session.Write(@"*RST", 4000);
            //visaResponse = session.Query(@"*OPC?", 4000);
            //session.Write(@"*CLS", 4000);
            //visaResponse = session.Query(@"*OPC?", 4000);
            visaResponse = session.Query(@"*IDN?");
            identFields = visaResponse.Split(',');
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
                return false;
            }
            else
            {
                MessageBox.Show("This is not a CMW.");
                return false;
            }

            cmwID = cmwModel + " " + cmwSerNum;
            AddToResults(cmwID);

            // CMW Options
            visaResponse = session.Query("SYSTem:BASE:OPTion:LIST? HWOPtion");
            AddToResults(visaResponse);
            hwOptions = visaResponse.Split(',');

            for (int i = 0; i < hwOptions.Length; i++)
            {
                hasKB036 = hwOptions[i].Contains("KB036") || hasKB036;
                if (hwOptions[i].Contains("H570"))
                    numOfTRX++;
                if (hwOptions[i].Contains("H590"))
                    numOfFrontEnds++;
            }
            //AddToResults("hasKB036: " + hasKB036.ToString());
            //AddToResults("numOfTRX: " + numOfTRX.ToString());
            //AddToResults("numOfFrontEnds: " + numOfFrontEnds.ToString());
            return true;
        }

        private static StreamWriter OpenTempFile(string fileName)
        {
            try
            {
                return new StreamWriter(fileName);
            }
            catch (IOException e)
            {
                MessageBox.Show(e.Message, "IO Exception");
                return null;
            }
        }

        private static string GetTempFileName()
        {
            int attempt = 0;
            while (true)
            {
                string fileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

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
            session.Write("CONFigure:GPRF:MEAS:POWer:MODE POWer");
            session.Write("CONFigure:GPRF:MEAS:POWer:SCOunt 50");
            session.Write("CONFigure:GPRF:MEAS:POWer:SLENgth 1000e-6");
            session.Write("CONFigure:GPRF:MEAS:POWer:MLENgth 950e-6");
            session.Write("TRIGger:GPRF:MEAS:POWer:OFFSet 10e-6");
        }
    }
}
