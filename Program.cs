using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Lab4
{
    static class Program
    {
        [DllImport("kernel32.dll")] static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AttachConsole(ATTACH_PARENT_PROCESS);
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Console.WriteLine();
            Application.Run(new MainWindow());
            //UParser parser = new UParser(new ExpressionHandler());
            //KeyValuePair<Decimal, string> result = parser.Evaluate("sqrt(36) + 17% - 23 * sqrt(25)% + 6");
            //Console.WriteLine("{0}; {1}", result.Key, result.Value);
        }
    }
}