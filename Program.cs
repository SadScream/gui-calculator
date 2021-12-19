using System;
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
            //Application.Run(new MainWindow());
            UParser parser = new UParser(new ExpressionHandler());
            Console.WriteLine(parser.Evaluate("10 + 120 * 10%"));
        }
    }
}