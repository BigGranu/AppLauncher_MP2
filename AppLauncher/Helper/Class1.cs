using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppLauncher.Helper
{
  public class KeyHook
  {
    private delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

    //Declare hook handle as int.
    static int hHook = 0;

    public static List<Keys> KeyCodes = new List<Keys>();

    //Declare keyboard hook constant.
    //For other hook types, you can obtain these values from Winuser.h in Microsoft SDK.
    const int WH_KEYBOARD_LL = 13;

    HookProc KeyboardHookProcedure;

    [StructLayout(LayoutKind.Sequential)]
    private class keyboardHookStruct
    {
      public int vkCode;
      public int scanCode;
      public int flags;
      public int time;
      public int dwExtraInfo;
    }

    //Import for SetWindowsHookEx function.
    //Use this function to install thread-specific hook.
    [DllImport("user32.dll", CharSet = CharSet.Auto,
     CallingConvention = CallingConvention.StdCall)]
    private static extern int SetWindowsHookEx(int idHook, HookProc lpfn,
    IntPtr hInstance, int threadId);

    //Import for UnhookWindowsHookEx.
    //Call this function to uninstall the hook.
    [DllImport("user32.dll", CharSet = CharSet.Auto,
     CallingConvention = CallingConvention.StdCall)]
    private static extern bool UnhookWindowsHookEx(int idHook);

    //Import for CallNextHookEx.
    //Use this function to pass the hook information to next hook procedure in chain.
    [DllImport("user32.dll", CharSet = CharSet.Auto,
     CallingConvention = CallingConvention.StdCall)]
    private static extern int CallNextHookEx(int idHook, int nCode,
    IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll")]
    static extern IntPtr LoadLibrary(string lpFileName);

    public KeyHook()
    {
      Hook();
    }

    ~KeyHook()
    {
      UnHook();
    }

    public int Hook()
    {
      KeyboardHookProcedure = new HookProc(KeyHook.KeyboardHookProc);

      hHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookProcedure, (IntPtr)LoadLibrary("User32"), 0);
      return hHook;
    }

    public bool UnHook()
    {
      bool ret = UnhookWindowsHookEx(hHook);
      if (ret)
        hHook = 0;
      return ret;
    }

    private static int KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
    {
      if (nCode < 0)
      {
        return CallNextHookEx(hHook, nCode, wParam, lParam);
      }
      else
      {

        if (((int)wParam == 256) || ((int)wParam == 260))
        {
          keyboardHookStruct MyKeyboardHookStruct = (keyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(keyboardHookStruct));
          KeyCodes.Add((Keys)MyKeyboardHookStruct.vkCode);
          Console.WriteLine("hhhhh");
        }
        return 1;
      }
    }

  }
}
