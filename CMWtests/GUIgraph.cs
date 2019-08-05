using System;
using System.Windows.Forms;
using System.Drawing;

namespace CMWtests
{
    public partial class MainForm
    {
        private int xScale;
        private int x1 = 89;
        private int y1;

        private void CreateGraph(string resource)
        {
            pictureBox1.Image = (Image)Properties.Resources.ResourceManager.GetObject(resource);
            Invoke(new MethodInvoker(() =>
            {
                pictureBox1.Show();
                pictureBox1.Invalidate();
            }));
        }

        private void PlotPoint(long currentFreq, double amplError)
        {
            using (Graphics g = pictureBox1.CreateGraphics())
            {
                xScale = (hasKB036 ? 5 : 17);

                int x2 = x1 + xScale;
                int y2 = 122 - Convert.ToInt32(94D * amplError / 2D);
                y2 = y2 > 280 ? 277 : y2;
                y2 = y2 < 0 ? 0 : y2;

                if (currentFreq < 200e6)
                {
                    x1 = x2 = 106;
                    y1 = y2;
                }

                g.DrawLine(new Pen(Color.CornflowerBlue, 2), x1, y1, x2, y2);

                x1 = x2;
                y1 = y2;
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            using (Font font = new Font("Calibri", 14))
            {
                e.Graphics.DrawString(testHeader, font, Brushes.Black, new Point(2, 2));
            }
        }
    }
}
