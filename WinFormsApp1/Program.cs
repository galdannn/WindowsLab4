using System;
using System.Windows.Forms;
using PdfSharp.Fonts;

namespace WinFormsApp1
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            GlobalFontSettings.FontResolver = new MyFontResolver();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Initialize database
            DatabaseHelper.InitializeDatabase();
            Application.Run(new LoginForm());
        }
    }
}