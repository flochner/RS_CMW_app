using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace CMWtests
{
    public partial class MainForm
    {
        private double lastAmplError;

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
        { }
        
    }
}
