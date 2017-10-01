using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TranslucentTB
{
    class Translucent
    {
        [DllImport("user32.dll")]
        internal static extern IntPtr SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);
        [DllImport("user32.dll")]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [StructLayout(LayoutKind.Sequential)]
        internal struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        internal enum WindowCompositionAttribute
        {
            WCA_ACCENT_POLICY = 19
        }

        internal enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_INVALID_STATE = 4
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }

        public static void TrueOn(string[] args)
        {
            AccentPolicy accentPolicy = new AccentPolicy();
            accentPolicy.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;
            accentPolicy.AccentFlags =  0x20 | 0x40 | 0x80 | 0x100;
            accentPolicy.GradientColor = 125;
            accentPolicy.AnimationId = 0;

            IntPtr accentPtr = Marshal.AllocHGlobal(Marshal.SizeOf(accentPolicy));
            Marshal.StructureToPtr(accentPolicy, accentPtr, false);
            WindowCompositionAttributeData data = new WindowCompositionAttributeData();
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = Marshal.SizeOf(accentPolicy);
            data.Data = accentPtr;

            IntPtr taskbar = FindWindow("Shell_TrayWnd", null);
            IntPtr secondTaskbar = FindWindow("Shell_SecondaryTrayWnd", null);
            IntPtr Button = FindWindow("Button", null);
            IntPtr ToolbarWindow32 = FindWindow("ToolbarWindow32", null);
            SetWindowCompositionAttribute(taskbar, ref data);
            SetWindowCompositionAttribute(secondTaskbar, ref data);
            SetWindowCompositionAttribute(Button, ref data);
            SetWindowCompositionAttribute(ToolbarWindow32, ref data);
        }
    }
    public class T
    {
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        [StructLayout(LayoutKind.Sequential)]
        internal struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        internal enum WindowCompositionAttribute
        {
            // ...
            WCA_ACCENT_POLICY = 19
            // ...
        }

        internal enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_INVALID_STATE = 4
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }

        public static void EnableBlur()
        {
            var accent = new AccentPolicy();
            var accentStructSize = Marshal.SizeOf(accent);
            accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;
            accent.GradientColor = 0;

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData();
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;
            SetWindowCompositionAttribute(Process.GetCurrentProcess().MainWindowHandle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }

    }

}