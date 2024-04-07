using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Interop;

namespace TakeControlPC
{
    internal class Commands
    {
        private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        private const int APPCOMMAND_VOLUME_UP = 0xA0000;
        private const int APPCOMMAND_VOLUME_DOWN = 0x90000;
        private const int WM_APPCOMMAND = 0x319;

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        [DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg,
            IntPtr wParam, IntPtr lParam);
        internal static void SetVolumeUp(MainWindow mainWindow)
        {
            IntPtr intPtr = new WindowInteropHelper(mainWindow).Handle;
            SendMessageW(intPtr, WM_APPCOMMAND, intPtr,
                (IntPtr)APPCOMMAND_VOLUME_UP);
        }

        internal static void SetVolumeDown(MainWindow mainWindow)
        {
            IntPtr intPtr = new WindowInteropHelper(mainWindow).Handle;
            SendMessageW(intPtr, WM_APPCOMMAND, intPtr,
                (IntPtr)APPCOMMAND_VOLUME_DOWN);
        }

        internal static void SetVolumeMute(MainWindow mainWindow)
        {
            IntPtr intPtr = new WindowInteropHelper(mainWindow).Handle;
            SendMessageW(intPtr, WM_APPCOMMAND, intPtr,
                (IntPtr)APPCOMMAND_VOLUME_MUTE);
        }

        internal static void PausePlay() => keybd_event((byte)Keys.MediaPlayPause, 0, 0, 0);
        internal static void NextTrack() => keybd_event((byte)Keys.MediaNextTrack, 0, 0, 0);
        internal static void PreviousTrack() => keybd_event((byte)Keys.MediaPreviousTrack, 0, 0, 0);
        internal static void StopTrack() => keybd_event((byte)Keys.MediaStop, 0, 0, 0);
        internal static void NextStepTrack() => keybd_event((byte)Keys.Right, 0, 0, 0);
        internal static void PreviousStepTrack() => keybd_event((byte)Keys.Left, 0, 0, 0);
        internal static void SleepPC() => SetSuspendState(false, true, true);
        internal static void RestartPC() => Process.Start("shutdown.exe", "-r -t 0");
        internal static void PowerOffPC() => Process.Start("shutdown.exe", "-s -t 0");
        internal static void LaunchApp(string path)
        {
            try
            {
                Process.Start(new ProcessStartInfo("cmd", $"/c start {path}") { CreateNoWindow = true });
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
