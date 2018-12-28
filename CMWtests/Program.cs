using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CMWtests
{
    class Program
    {
        public MainForm Obj { get => _obj; private set { } }
        private readonly MainForm _obj = null;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm _obj = new MainForm();
            Application.Run(_obj);
        }
    }
}
