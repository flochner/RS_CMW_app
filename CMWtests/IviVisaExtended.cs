//This project contains Ivi VISA extention functions for:
//- error checking
//- string, integer, double and boolean types querying
//- reading the single-precision binary data

// This project is then referenced in all other Examples

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Ivi.Visa; //This .NET assembly is installed with your NI VISA installation

namespace IviVisaExtended
{
    /// <summary>
    /// New type of Exception thrown when the instrument error checking detects an error 
    /// </summary>
    public class InstrumentErrorException : Exception
    {
        /// <summary>
        /// Instrument Error Exception with no specified error
        /// </summary>
        public InstrumentErrorException() : base("Instrument error(s) occured")
        {
        }
        /// <summary>
        /// Instrument Error Exception with one error
        /// </summary>
        public InstrumentErrorException(string error) : base(error)
        {
        }

        /// <summary>
        /// Instrument Error Exception with list of errors
        /// </summary>
        public InstrumentErrorException(List<string> errors) : base(String.Join("\n", errors))
        {
        }
    }
    
    /// <summary>
    /// New type of Exception thrown when the instrument OPC timeout (by STB polling) is reached
    /// </summary>
    public class InstrumentOPCtimeoutException : Exception
    {
        /// <summary>
        /// Instrument OPC Timeout Exception
        /// </summary>
        public InstrumentOPCtimeoutException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// Extention class for Ivi.Visa IMessageBasedSession interface
    /// </summary>
    public static class IVI_VISA_MessageBasedSession_Extended
    {
        /// <summary>
        /// Combined function for writing a command and optionally sending *OPC? query
        /// </summary>
        /// <param name="io"></param>
        public static void Write(this IMessageBasedSession io, string command, bool waitForOPC = false)
        {
            io.RawIO.Write(command);
            if (waitForOPC == true)
            {
                io.WaitForOPC();
            }
        }

        /// <summary>
        /// Combined method for sending and reading string responses
        /// </summary>
        /// <param name="io"></param>
        /// <param name="query">Query to be send to the instrument</param>
        /// <returns></returns>
        public static string QueryString(this IMessageBasedSession io, string query)
        {
            io.RawIO.Write(query);
            var response = io.RawIO.ReadString();
            response = response.TrimEnd('\n');
            return response;
        }

        /// <summary>
        /// Combined method for querying integer responses
        /// </summary>
        /// <param name="io"></param>
        /// <param name="query">Query to be send to the instrument</param>
        /// <returns></returns>
        public static int QueryInteger(this IMessageBasedSession io, string query)
        {
            return Int32.Parse(io.QueryString(query));
        }

        /// <summary>
        /// Combined method for querying double responses
        /// </summary>
        /// <param name="io"></param>
        /// <param name="query">Query to be send to the instrument</param>
        /// <returns></returns>
        public static double QueryDouble(this IMessageBasedSession io, string query)
        {
            return Convert.ToDouble(io.QueryString(query), System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Combined method for querying boolean responses
        /// </summary>
        /// <param name="io"></param>
        /// <param name="query">Query to be send to the instrument</param>
        /// <returns></returns>
        public static bool QueryBool(this IMessageBasedSession io, string query)
        {
            var response = io.QueryString(query);
            var result = (response.ToUpper(System.Globalization.CultureInfo.InvariantCulture) == "TRUE" || response == "1") ? true : false;
            return result;
        }

        /// <summary>
        /// Method for querying long data of unknown length - the reading is done by blocks
        /// </summary>
        /// <param name="io"></param>
        /// <param name="query">Query to be send to the instrument</param>
        /// <returns></returns>
        public static string QueryLongString(this IMessageBasedSession io, string query, long segmentSize = 65536)
        {
            io.RawIO.Write(query);
            byte[] segment = new byte[segmentSize];
            StringBuilder output = new StringBuilder();
            ReadStatus status;
            while (true)
            {
                segment = io.RawIO.Read(segmentSize, out status);
                output.Append(Encoding.ASCII.GetString(segment));
                if (status != ReadStatus.MaximumCountReached)
                {
                    break;
                }
            }
            // Trim the LF at the end
            if (output.Length > 0 && output[output.Length - 1] == 10)
            {
                output.Length--;
            }
            return output.ToString();
        }

        /// <summary>
        /// This method waits until the instrument responds that the operation is complete
        /// </summary>
        static void WaitForOPC(this IMessageBasedSession io)
        {
            io.QueryString("*OPC?");
        }

        /// <summary>
        /// This method checks the Status Byte bit 2 and if it is True, it reads all the entries in the instrument Error Queue
        /// </summary>
        public static List<string> ReadErrorQueue(this IMessageBasedSession io)
        {
            var errors = new List<string>();

            if ((io.QueryInteger("*STB?") & 4) > 0)
            {
                while (true)
                {
                    var response = io.QueryString("SYST:ERR?");
                    if (response.ToLower().Contains("\"no error\""))
                    {
                        break;
                    }
                    errors.Add(response);

                    // safety stop
                    if (errors.Count > 50)
                    {
                        errors.Add("Cannot clear the error queue");
                        break;
                    }
                }
            }
            return errors;
        }

        /// <summary>
        /// Sends *CLS command and clears the Error Queue if necessary
        /// </summary>
        /// <param name="io"></param>
        public static void ClearStatus(this IMessageBasedSession io)
        {
            io.QueryString("*CLS;*OPC?");
            io.ReadErrorQueue();
        }

        /// <summary>
        ///This method will call ReadErrorQueue and throw InstrumentErrorException if any error occured
        ///If you want to only check for error without generating the exception, use the ReadErrorQueue() function
        /// </summary>
        public static void ErrorChecking(this IMessageBasedSession io)
        {
            var errors = io.ReadErrorQueue();
            if (errors.Count > 0)
            {
                throw new InstrumentErrorException(errors);
            }
        }

        /// <summary>
        /// Parse the binary data block e.g. "#41001" and return the size e.g. 1001
        /// </summary>
        /// <param name="io"></param>
        /// <returns>Size of the binary block</returns>
        public static Int64 ParseBinaryDataSizeHeader(this IMessageBasedSession io)
        {
            var hash = io.RawIO.ReadString(1);
            if (hash != "#")
            {
                throw new Ivi.Visa.VisaException(String.Format("The binary block header was not found in the response. Expected '#', received character '{0}'", hash));
            }
            var sizeOfsize = Int32.Parse(io.RawIO.ReadString(1));
            var size = Int64.Parse(io.RawIO.ReadString(sizeOfsize));
            return size;
        }

        /// <summary>
        /// Read IEEE binary data block
        /// </summary>
        /// <param name="io"></param>
        /// <returns>byte array with the binary data content</returns>
        public static byte[] ReadBinaryDataBlock(this IMessageBasedSession io)
        {
            ReadStatus readStatus;
            var blockSize = io.ParseBinaryDataSizeHeader();
            var binaryData = io.RawIO.Read(blockSize, out readStatus);

            //if the read indicates that more data are available, discard the rest (usually one LF character)
            if (readStatus == ReadStatus.MaximumCountReached)
            {
                io.FormattedIO.ReadUntilEnd();
            }

            return binaryData;
        }
        
        /// <summary>
        /// Reads float32 - single precision array of numbers in binary format from the instrument
        /// </summary>
        /// <param name="io"></param>
        /// <param name="query">Query to be sent to the instrument</param>
        /// <param name="endianessFit">True(default) no need to change the bytes order. R&S instruments use little-endian, Intel processors too.</param>
        /// <returns>double-precision array of values</returns>
        public static double[] QueryBinaryFloatData(this IMessageBasedSession io, string query, bool endianessFit=true)
        {
            io.RawIO.Write(query);
            byte[] waveformBytes = io.ReadBinaryDataBlock();
            int samples = waveformBytes.Length / 4; //binary waveform is return as 4-byte/single-precision float
            var values = new double[samples];

            if (endianessFit) //no need to change the byte order, the target PC and the instrument use the same endian
            {
                for (int i = 0; i < samples; i++)
                    values[i] = BitConverter.ToSingle(waveformBytes, i * 4);
            }
            else //change the byte order, the target PC and the instrument use different endians
            {
                var tempArray = new byte[4];
                for (int i = 0; i < samples; i++)
                {
                    tempArray[0] = waveformBytes[i * 4 + 3];
                    tempArray[1] = waveformBytes[i * 4 + 2];
                    tempArray[2] = waveformBytes[i * 4 + 1];
                    tempArray[3] = waveformBytes[i * 4 + 0];
                    values[i] = BitConverter.ToSingle(tempArray, 0);
                }
            }

            return values;
        }

        /// <summary>
        /// Reads binary data from the instrument and saves them into the file
        /// </summary>
        /// <param name="io"></param>
        /// <param name="PCfilePath">PC file path to save the binary data response</param>
        public static void ReadBinaryDataToFile(this IMessageBasedSession io, string PCfilePath)
        {
            var fileContent = io.ReadBinaryDataBlock();
            System.IO.File.WriteAllBytes(PCfilePath, fileContent);
        }

        /// <summary>
        /// Sends a query, reads the binary data response from the instrument and saves them into the file
        /// </summary>
        /// <param name="io"></param>
        /// /// <param name="query">Query to be sent to the instrument</param>
        /// <param name="filePath">File to save the binary data</param>
        public static void QueryBinaryDataToFile(this IMessageBasedSession io, string query, string filePath)
        {
            io.RawIO.Write(query);
            var fileContent = io.ReadBinaryDataBlock();
            System.IO.File.WriteAllBytes(filePath, fileContent);
        }
        /// <summary>
        /// Sends a command synchronised with the STB polling mechanism
        /// </summary>
        /// <param name="io"></param>
        /// <param name="command">command to be sent to the instrument</param>
        /// <param name="timeout">timeout in milliseconds</param>
        public static void WriteWithSTBpollSync(this IMessageBasedSession io, string command, int timeout)
        {
            io.QueryInteger("*ESR?"); //Clear the Event Status Register
            io.Write(command + ";*OPC");
            string exMessage = String.Format("WriteWithSTBpollSync - Timeout occured. Command: \"{0}\", timeout {1} ms", command, timeout);
            io._STBpolling(exMessage, timeout);
            io.QueryInteger("*ESR?"); //Clear the Event Status Register
        }

        /// <summary>
        /// Sends a query synchronised with the STB polling mechanism
        /// </summary>
        /// <param name="io"></param>
        /// <param name="query">query to be sent to the instrument</param>
        /// <param name="timeout">timeout in milliseconds</param>
        /// /// <returns>string response</returns>
        public static string QueryWithSTBpollSync(this IMessageBasedSession io, string query, int timeout)
        {
            io.QueryInteger("*ESR?"); //Clear the Event Status Register
            io.Write(query + ";*OPC");
            string exMessage = String.Format("QueryWithSTBpollSync - Timeout occured. Query: \"{0}\", timeout {1} ms", query, timeout);
            io._STBpolling(exMessage, timeout);
            var response = io.RawIO.ReadString();
            response = response.TrimEnd('\n');
            io.QueryInteger("*ESR?"); //Clear the Event Status Register
            return response;
        }

        /// <summary>
        /// Sends a command synchronised with the Service Request mechanism
        /// </summary>
        /// <param name="io"></param>
        /// <param name="command">command to be sent to the instrument</param>
        /// <param name="timeout">timeout in milliseconds</param>
        public static void WriteWithSRQsync(this IMessageBasedSession io, string command, int timeout)
        {
            io.QueryInteger("*ESR?"); //Clear the Event Status Register
            io.DiscardEvents(EventType.ServiceRequest);
            io.EnableEvent(EventType.ServiceRequest);
            io.Write(command + ";*OPC");
            try
            {
                io.WaitOnEvent(EventType.ServiceRequest, timeout);
            }
            catch (Ivi.Visa.NativeVisaException e) when (e.ErrorCode == Ivi.Visa.NativeErrorCode.Timeout)
            {
                string exMessage = String.Format("WriteWithSRQsync - Timeout occured. Command: \"{0}\", timeout {1} ms", command, timeout);
                throw new InstrumentOPCtimeoutException(exMessage);
            }
            
            io.QueryInteger("*ESR?"); //Clear the Event Status Register
        }

        /// <summary>
        /// Sends a query synchronised with the Service Request mechanism
        /// </summary>
        /// <param name="io"></param>
        /// <param name="query">query to be sent to the instrument</param>
        /// <param name="timeout">timeout in milliseconds</param>
        /// /// <returns>string response</returns>
        public static string QueryWithSRQsync(this IMessageBasedSession io, string query, int timeout)
        {
            io.QueryInteger("*ESR?"); //Clear the Event Status Register
            io.DiscardEvents(EventType.ServiceRequest);
            io.EnableEvent(EventType.ServiceRequest);
            io.Write(query + ";*OPC");
            try
            {
                io.WaitOnEvent(EventType.ServiceRequest, timeout);
            }
            catch (Ivi.Visa.NativeVisaException e) when (e.ErrorCode == Ivi.Visa.NativeErrorCode.Timeout)
            {
                string exMessage = String.Format("QueryWithSRQsync - Timeout occured. Command: \"{0}\", timeout {1} ms", query, timeout);
                throw new InstrumentOPCtimeoutException(exMessage);
            }
            var response = io.RawIO.ReadString();
            response = response.TrimEnd('\n');
            io.QueryInteger("*ESR?"); //Clear the Event Status Register
            return response;
        }

        /// <summary>
        /// Sends a command with registration of the service request event handler.
        /// </summary>
        /// <param name="io"></param>
        /// <param name="command">command to be sent to the instrument</param>
        /// <param name="handler">method to be called when Service Request is invoked</param>
        public static void WriteWithSRQevent(this IMessageBasedSession io, string command, EventHandler<VisaEventArgs> handler)
        {
            io.ServiceRequest -= handler; //unregister even if it does not exist
            io.ServiceRequest += handler; //register again, this prevents double registrations
            io.QueryInteger("*ESR?"); //Clear the Event Status Register
            io.Write(command + ";*OPC");
        }
        
        private static void _STBpolling(this IMessageBasedSession io, string exMessage, int timeout)
        {
            var start = DateTime.Now;
            var stop = start.AddMilliseconds(timeout);
            var sleep10ms = start.AddMilliseconds(10);
            var sleep100ms = DateTime.Now.AddMilliseconds(100);
            var sleep1000ms = DateTime.Now.AddMilliseconds(1000);
            var sleep5000ms = DateTime.Now.AddMilliseconds(5000);
            var sleep10000ms = DateTime.Now.AddMilliseconds(10000);
            var sleep30000ms = DateTime.Now.AddMilliseconds(30000);

            // STB polling loop
            while (true)
            {
                var stb = io.ReadStatusByte();
                if (stb.HasFlag(StatusByteFlags.EventStatusRegister))
                    break;

                if (DateTime.Now > stop)
                    throw new InstrumentOPCtimeoutException(exMessage);

                if      (DateTime.Now < sleep10ms) { } //Full speed
                else if (DateTime.Now < sleep100ms) Thread.Sleep(1);
                else if (DateTime.Now < sleep1000ms) Thread.Sleep(10);
                else if (DateTime.Now < sleep5000ms) Thread.Sleep(50);
                else if (DateTime.Now < sleep10000ms) Thread.Sleep(100);
                else if (DateTime.Now < sleep30000ms) Thread.Sleep(500);
                else Thread.Sleep(1000);
            }
        }
    }
}