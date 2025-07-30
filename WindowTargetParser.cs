using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowKiller
{
    public class WindowData
    {
        public required string Title { get; set; }
        public required string ClassName { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public required Process Process { get; set; }
    }
    public static class WindowTargetParser
    {
        public static WindowData? GetWindowTargetUnderCursor(IntPtr overlayHandle)
        {
            Point mousePos = Cursor.Position;
            IntPtr hWnd = WindowFromPoint(mousePos);

            // Ignore the overlay window itself
            if (hWnd == overlayHandle)
            {
                hWnd = GetNextWindowUnderCursor(mousePos, overlayHandle);
            }

            if (hWnd != IntPtr.Zero)
            {
                string title = GetWindowTextSafe(hWnd);
                string className = GetClassNameSafe(hWnd);

                RECT rect;
                int x = 0, y = 0, width = 0, height = 0;
                if (GetWindowRect(hWnd, out rect))
                {
                    x = rect.Left;
                    y = rect.Top;
                    width = rect.Right - rect.Left;
                    height = rect.Bottom - rect.Top;
                }

                Process? process = null;
                try
                {
                    uint pid;
                    GetWindowThreadProcessId(hWnd, out pid);
                    if (pid != 0)
                        process = Process.GetProcessById((int)pid);
                }
                catch
                {
                }

                if(process == null)
                {
                    return null; // All or nothing
                }

                return new WindowData
                {
                    Title = title,
                    ClassName = className,
                    X = x,
                    Y = y,
                    Width = width,
                    Height = height,
                    Process = process
                };
            }
            else
            {
                return null;
            }
        }

        // Helper: Get the next window under the cursor, skipping the specified handle
        private static IntPtr GetNextWindowUnderCursor(Point pt, IntPtr skipHandle)
        {
            IntPtr hWnd = IntPtr.Zero;
            IntPtr found = IntPtr.Zero;

            // Enumerate all windows in Z order (top to bottom)
            hWnd = GetTopWindow(IntPtr.Zero);
            while (hWnd != IntPtr.Zero)
            {
                if (IsWindowVisible(hWnd) && hWnd != skipHandle)
                {
                    RECT rect;
                    if (GetWindowRect(hWnd, out rect))
                    {
                        if (rect.Left <= pt.X && pt.X < rect.Right &&
                            rect.Top <= pt.Y && pt.Y < rect.Bottom)
                        {
                            found = hWnd;
                            break;
                        }
                    }
                }
                hWnd = GetWindow(hWnd, GW_HWNDNEXT);
            }
            return found;
        }

        // Helper: Get window text safely
        private static string GetWindowTextSafe(IntPtr hWnd)
        {
            var buffer = new System.Text.StringBuilder(256);
            GetWindowText(hWnd, buffer, buffer.Capacity);
            return buffer.ToString();
        }

        // Helper: Get class name safely
        private static string GetClassNameSafe(IntPtr hWnd)
        {
            var buffer = new System.Text.StringBuilder(256);
            GetClassName(hWnd, buffer, buffer.Capacity);
            return buffer.ToString();
        }

        // Win32 API imports and constants
        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point pt);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetTopWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr hWnd, System.Text.StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);


        private const uint GW_HWNDNEXT = 2;

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
}
