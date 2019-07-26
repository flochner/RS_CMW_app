using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace CMWtests
{
    class GUIgraph : MainForm
    {
        public GUIgraph()
        {
            Pix();
        }

        public void Pix()
        {
            pictureBox1.CreateGraphics();
        }

    }
}
