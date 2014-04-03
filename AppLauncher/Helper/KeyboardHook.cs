using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppLauncher.Helper
{
  public class KeyboardHook : IDisposable
  {

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, HookHandlerDelegate lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
    private static extern short GetKeyState(int keyCode);

    // needed for Keypressd Time
    private DateTime _start;
    private bool _pressed;

    // Internal parameters
    private bool PassAllKeysToNextApp = false;
    private bool AllowAltTab = false;
    private bool AllowWindowsKey = false;

    // Keyboard API constants
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    private const int HC_ACTION = 0;

    //Variables used in the call to SetWindowsHookEx
    private HookHandlerDelegate proc;
    private IntPtr hookID = IntPtr.Zero;

    /// <summary>
    /// Delegate for KeyboardHook event handling.
    /// </summary>
    public delegate void KeyboardHookEventHandler(KeyboardHookEventArgs e);
    public delegate IntPtr HookHandlerDelegate(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

    public event KeyboardHookEventHandler SendExit;

    // Structure returned by the hook whenever a key is pressed
    public struct KBDLLHOOKSTRUCT
    {
      public int vkCode;
      int scanCode;
      public int flags;
      int time;
      int dwExtraInfo;
    }

    #region Constructors
    /// <summary>
    /// Sets up a keyboard hook to trap all keystrokes without 
    /// passing any to other applications.
    /// </summary>
    public KeyboardHook()
    {
      proc = HookCallback;
      using (Process curProcess = new Process())
      using (ProcessModule curModule = curProcess.MainModule)
      {
        hookID = SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        Console.WriteLine("Hook started :" + curModule.ModuleName);
      }
    }

    /// <summary>
    /// Releases the keyboard hook.
    /// </summary>
    public void Dispose()
    {
      UnhookWindowsHookEx(hookID);
    }

    #endregion

    #region Hook Callback Method
    /// <summary>
    /// Processes the key event captured by the hook.
    /// </summary>
    private IntPtr HookCallback(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
    {
      Console.WriteLine("Hooked ");

      if (nCode < HC_ACTION) return CallNextHookEx(hookID, nCode, wParam, ref lParam);

      if (wParam == (IntPtr)WM_KEYDOWN)
      {
        // if Key = ESC
        if (lParam.vkCode == 27)
        {
          if (_pressed == false)
          {
            _pressed = true;
            _start = DateTime.Now;
          }
        }
      }

      if (wParam == (IntPtr)WM_KEYUP)
      {
        // if Key = ESC
        if (lParam.vkCode != 27) return CallNextHookEx(hookID, nCode, wParam, ref lParam);

        _pressed = false;

        var pressedTime = DateTime.Now - _start;
        if (pressedTime.TotalMilliseconds >= 2000)
        {

          SendExitCommand(new KeyboardHookEventArgs(lParam.vkCode, true));

        }
      }
      //Pass key to next application
      return CallNextHookEx(hookID, nCode, wParam, ref lParam);
    }
    #endregion

    #region Event Handling

    public void SendExitCommand(KeyboardHookEventArgs e)
    {
      if (SendExit != null)
        SendExit(e);
    }

    #endregion

    /// <summary>
    /// Event arguments for the KeyboardHook class's KeyIntercepted event.
    /// </summary>
    public class KeyboardHookEventArgs : EventArgs
    {

      private readonly string _keyName;
      private readonly int _keyCode;
      private readonly bool _passThrough;

      /// <summary>
      /// The name of the key that was pressed.
      /// </summary>
      public string KeyName
      {
        get { return _keyName; }
      }

      /// <summary>
      /// The virtual key code of the key that was pressed.
      /// </summary>
      public int KeyCode
      {
        get { return _keyCode; }
      }

      /// <summary>
      /// True if this key combination was passed to other applications,
      /// false if it was trapped.
      /// </summary>
      public bool PassThrough
      {
        get { return _passThrough; }
      }

      public KeyboardHookEventArgs(int evtKeyCode, bool evtPassThrough)
      {
        _keyName = ((Keys)evtKeyCode).ToString();
        _keyCode = evtKeyCode;
        _passThrough = evtPassThrough;
      }

    }

  }
}
