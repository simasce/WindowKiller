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
            // Ensure only one instance is running
            using var mutex = new Mutex(true, "WindowKillerAppMutex", out bool isNewInstance);
            if (!isNewInstance)
            {
                MessageBox.Show(
                    "Window Killer is already running.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Setup notification bar icon
            SetupNotificationBarIcon();

            // Start the global keyboard hook
            GlobalKeyboardHook.Start();

            Application.Run();

            // Clean up the hook on exit
            GlobalKeyboardHook.Stop();
        }

        private static void SetupNotificationBarIcon()
        {
            var icon = new NotifyIcon();
            icon.Visible = true;
            icon.Icon = MainWindowHandler.GetWindowIcon();
            icon.Text = "Window Killer";
            icon.MouseClick += Icon_MouseClick;

            // Setup context menu for the notification bar icon
            var contextMenu = new ContextMenuStrip();

            var titleMenuItem = new ToolStripMenuItem("Window Killer");
            titleMenuItem.Enabled = false;
            contextMenu.Items.Add(titleMenuItem);

            // Add a separator
            contextMenu.Items.Add(new ToolStripSeparator());

            // Add a quit option
            var quitMenuItem = new ToolStripMenuItem("Quit");
            quitMenuItem.Click += (s, e) =>
            {
                icon.Visible = false;
                MainWindowHandler.Close();
                Application.Exit();
            };
            contextMenu.Items.Add(quitMenuItem);
            icon.ContextMenuStrip = contextMenu;
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