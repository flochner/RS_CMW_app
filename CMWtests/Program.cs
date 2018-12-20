using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CMWtests
{
    class Program
    {
        public MainForm Obj { get => obj; private set { } }
        private MainForm obj = null;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm obj = new MainForm();
            Application.Run(obj);
        }
    }
}
