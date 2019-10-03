using System;
using System.Drawing;
using System.Drawing.Text;

namespace CMWtests
{
    public partial class MainForm
    {
        private float xMin, xMax, yMin, yMax;
        private float xPoints;
        private float yBase;
        private float x1, y1;

        private void CreateGraph(string resource)
        {
            xMin = (hasKB036 ? 91 : 105);
            xMax = (hasKB036 ? 675 : 647);
            xPoints = (hasKB036 ? 59 : 32);

            yMin = (hasKB036 ? -88.25F : -94.5F );
            yMax = (hasKB036 ? 86.5F : 93.5F);
            yBase = (hasKB036 ? 116 : 121);

            pictureBoxGraph.Image = (Image)Properties.Resources.ResourceManager.GetObject(resource);

            using (Font font = new Font("Calibri", 14))
            {
                StringFormat sf = new StringFormat()
                { Alignment = StringAlignment.Center };

                Graphics e = Graphics.FromImage(pictureBoxGraph.Image);
                e.TextRenderingHint = TextRenderingHint.AntiAlias;
                e.DrawString(testHeader, font, Brushes.Black, new RectangleF(0, 8, 710, 30), sf);
            }
        }

        private void PlotPoint(long currentFreq, double amplError)
        {
            currentFreq = (currentFreq < (long)200e6 ? (long)100e6 : currentFreq);
            using (Graphics g = Graphics.FromImage(pictureBoxGraph.Image))
            {
                float x2, y2;
                if (Math.Abs(amplError) > 2)
                    amplError = 2 * Math.Sign(amplError);

                x2 = xMin + ((xMax - xMin) / xPoints * ((float)(currentFreq / 1e8) - 1));

                if (amplError > 0)
                    y2 = yBase - (yMax * (float)amplError / 2F);
                else
                    y2 = yBase + (yMin * (float)amplError / 2F);

                if (currentFreq < 200e6)
                {
                    x1 = x2;
                    y1 = y2;
                }

                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.DrawLine(new Pen(Color.CornflowerBlue, 2), x1, y1, x2, y2);

                x1 = x2;
                y1 = y2;
            }
            pictureBoxGraph.Invalidate();
        }
    }
}
