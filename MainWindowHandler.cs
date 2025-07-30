using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WindowKiller
{
    public static class MainWindowHandler
    {
        public static Form1? CurrentWindow { get; private set; } = null;

        public static bool IsOpen
        {
            get
            {
                return CurrentWindow != null;
            }
        }

        public static void Open()
        {
            if (IsOpen)
                return;
            CurrentWindow = new Form1();
            CurrentWindow.FormClosed += CurrentWindow_FormClosed;
            CurrentWindow.Show();
        }

        public static void Close()
        {
            CurrentWindow?.Close();
            DisposeWindow();
        }

        public static void Toggle()
        {
            if (!IsOpen)
                Open();
            else
                Close();
        }

        public static void TriggerKillProcess()
        {
            if(CurrentWindow != null)
            {
                CurrentWindow.TriggerKillProcess();
            }
        }
        public static Icon GetWindowIcon()
        {
            // Load WindowSlash.ico from embedded resources
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("WindowKiller.WindowSlash.ico");
            if (stream != null)
            {
                return new Icon(stream);
            }
            else
            {
                return SystemIcons.Application; // fallback
            }
        }


        private static void CurrentWindow_FormClosed(object? sender, FormClosedEventArgs e)
        {
            DisposeWindow();
        }

        private static void DisposeWindow()
        {
            CurrentWindow?.Dispose();
            CurrentWindow = null;
        }
    }
}
