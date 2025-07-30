using System.Reflection;

namespace WindowKiller
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            var icon = new NotifyIcon();

            icon.Visible = true;
            icon.Icon = MainWindowHandler.GetWindowIcon();
            icon.Text = "Simasce's Window Killer";
            icon.MouseClick += Icon_MouseClick;

            // Start the global keyboard hook
            GlobalKeyboardHook.Start();

            Application.Run();

            // Clean up the hook on exit
            GlobalKeyboardHook.Stop();
        }

        private static void Icon_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MainWindowHandler.Toggle();
            }
        }
    }
}