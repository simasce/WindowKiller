using System.Runtime.InteropServices;
using WindowKiller;

public static class GlobalKeyboardHook
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    private static IntPtr _hookID = IntPtr.Zero;
    private static LowLevelKeyboardProc _proc = HookCallback;
    private static bool ctrlDown = false;

    public static void Start()
    {
        if (_hookID == IntPtr.Zero)
            _hookID = SetHook(_proc);
    }

    public static void Stop()
    {
        if (_hookID != IntPtr.Zero)
        {
            UnhookWindowsHookEx(_hookID);
            _hookID = IntPtr.Zero;
        }
    }

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            Keys key = (Keys)vkCode;

            if (wParam == (IntPtr)WM_KEYDOWN)
            {
                if (key == Keys.LControlKey || key == Keys.RControlKey)
                {
                    ctrlDown = true;
                }
                else if (key == Keys.Home && ctrlDown)
                {
                    MainWindowHandler.Toggle();
                }
                else if(key == Keys.Escape)
                {
                    if(MainWindowHandler.IsOpen)
                    {
                        MainWindowHandler.Close();
                    }
                }
            }
            else if (wParam == (IntPtr)WM_KEYUP)
            {
                if (key == Keys.LControlKey || key == Keys.RControlKey)
                {
                    ctrlDown = false;
                }
            }
        }
        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
        using (var curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                GetModuleHandle(curModule.ModuleName), 0);
        }
    }
}