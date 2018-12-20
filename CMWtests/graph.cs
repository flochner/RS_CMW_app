using System;
using System.IO;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;

namespace CMWgraph
{
    /// <summary>
    /// Class for creating CMW graphs.
    /// </summary>
    public static class Graph
    {
        [STAThread]
        public static void Create(string dut, string fileName, int maxFreq, double maxError, bool isFirstTest)
        {
            FileInfo csvFile = new FileInfo(fileName);
            string bookName = Environment.GetEnvironmentVariable("USERPROFILE") + @"\Desktop\" + dut + ".xlsx";
            FileInfo book = new FileInfo(bookName);

            if (book.Exists)
            {
                CheckOpenFile(book);
                if (isFirstTest)
                {
                    book.Delete();
                    book = new FileInfo(bookName);
                }
            }

            ExcelPackage package = new ExcelPackage(book);
            ExcelWorksheet sheet = package.Workbook.Worksheets.Add((package.Workbook.Worksheets.Count + 1).ToString());
            ExcelRangeBase csvText = sheet.Cells.LoadFromText(csvFile);

            sheet.Cells["B1:E1"].Clear();
            sheet.Cells["B2"].Clear();
            sheet.Cells[maxFreq + 3, 2].Clear();
            sheet.Cells[maxFreq + 4, 2, maxFreq + 3, 7].Clear();
            sheet.Cells["A1"].Style.Font.Size = 22;
            sheet.Row(1).Merged = true;

            ExcelChart chart = sheet.Drawings.AddChart("chart1", eChartType.Line);
            for (int col = 6; col >= 1; col--)
                chart.Series.Add(csvText.Offset(1, col, maxFreq + 2, 1), csvText.Offset(1, 0, maxFreq + 2, 1));

            chart.Title.Text = sheet.Cells[maxFreq + 4, 1].Value.ToString();
            chart.SetPosition(44, 6);
            chart.SetSize(820, 270);
            chart.DisplayBlanksAs = eDisplayBlanksAs.Gap;
            chart.Legend.Add();
            chart.Legend.Position = eLegendPosition.TopRight;

            RemoveGridlines(chart);

            chart.XAxis.Crosses = eCrosses.Min;
            chart.XAxis.MajorTickMark = eAxisTickMark.Out;
            chart.XAxis.MinorTickMark = eAxisTickMark.None;
            chart.XAxis.MinValue = 0;
            chart.XAxis.MaxValue = maxFreq + 3;
            chart.XAxis.MajorUnit = 2;
            chart.XAxis.Title.Text = "Frequency (MHz)";
            chart.XAxis.Title.Font.Size = 12;

            chart.YAxis.MajorTickMark = eAxisTickMark.Out;
            chart.YAxis.MinorTickMark = eAxisTickMark.None;
            chart.YAxis.Format = "0.0";
            chart.YAxis.Title.Text = "Error (dB)";
            chart.YAxis.Title.Font.Size = 12;
            chart.YAxis.CrossBetween = eCrossBetween.MidCat;
            if (Math.Max(Convert.ToDouble(sheet.Cells[maxFreq, 7].Value), maxError) < 2.0)
            {
                chart.YAxis.MinValue = -2.0;
                chart.YAxis.MaxValue = 2.0;
            }

            var hiLimit24 = (ExcelLineChartSerie)chart.Series[0];
            hiLimit24.LineWidth = 1;
            hiLimit24.LineColor = Color.Red;
            hiLimit24.Header = "24 mo.";

            var hiLimit12 = (ExcelLineChartSerie)chart.Series[1];
            hiLimit12.LineWidth = 1;
            hiLimit12.LineColor = Color.Gold;
            hiLimit12.Header = "12 mo.";

            var zero = (ExcelLineChartSerie)chart.Series[2];
            zero.LineWidth = 1;
            zero.LineColor = Color.Silver;
            zero.Header = "0 dB";

            var loLimit12 = (ExcelLineChartSerie)chart.Series[3];
            loLimit12.LineWidth = 1;
            loLimit12.LineColor = Color.Gold;
            loLimit12.Header = "12 mo.";

            var loLimit24 = (ExcelLineChartSerie)chart.Series[4];
            loLimit24.LineWidth = 1;
            loLimit24.LineColor = Color.Red;
            loLimit24.Header = "24 mo.";

            var data = (ExcelLineChartSerie)chart.Series[5];
            data.Smooth = false;
            data.LineColor = Color.CornflowerBlue;
            data.Header = "data";

            sheet.PrinterSettings.PrintArea = sheet.Cells["A1:M15"];
            sheet.PrinterSettings.Orientation = eOrientation.Landscape;

            package.Save();
            package.Dispose();
            try
            {
                csvFile.Delete();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, e.Source);
            }
        }

        private static void RemoveGridlines(ExcelChart chart)
        {
            var chartXml = chart.ChartXml;
            var nsuri = chartXml.DocumentElement.NamespaceURI;
            var nsm = new XmlNamespaceManager(chartXml.NameTable);
            nsm.AddNamespace("c", nsuri);

            var valAxisNodes = chartXml.SelectNodes("c:chartSpace/c:chart/c:plotArea/c:valAx", nsm);
            if (valAxisNodes != null && valAxisNodes.Count > 0)
                foreach (XmlNode valAxisNode in valAxisNodes)
                {
                    var major = valAxisNode.SelectSingleNode("c:majorGridlines", nsm);
                    if (major != null)
                        valAxisNode.RemoveChild(major);

                    var minor = valAxisNode.SelectSingleNode("c:minorGridlines", nsm);
                    if (minor != null)
                        valAxisNode.RemoveChild(minor);
                }
        }

        private static void CheckOpenFile(FileInfo file)
        {
            FileStream stream = null;
            bool fileIsOpen;

            do
            {
                fileIsOpen = false;
                try
                {
                    stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                }
                catch (IOException)
                {
                    MessageBox.Show("Close the Workbook!", "CMW graphs",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    fileIsOpen = true;
                }
                finally
                {
                    if (stream != null)
                        stream.Close();
                }
            }
            while (fileIsOpen);
        }
    }
}
