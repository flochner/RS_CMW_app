using System;
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

    public static class StringExt
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}
