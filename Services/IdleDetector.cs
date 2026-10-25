using System.Runtime.InteropServices;

namespace ChronoPower.Services;

public class IdleDetector
{
    [StructLayout(LayoutKind.Sequential)]
    private struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

    [DllImport("kernel32.dll")]
    private static extern uint GetTickCount();

    /// <summary>
    /// Kullanıcının klavye veya fareye dokunmadığı süreyi döner.
    /// </summary>
    public static TimeSpan GetIdleTime()
    {
        var lastInputInfo = new LASTINPUTINFO();
        lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);

        if (!GetLastInputInfo(ref lastInputInfo))
        {
            return TimeSpan.Zero;
        }

        uint currentTick = GetTickCount();
        uint idleTicks = currentTick - lastInputInfo.dwTime;

        return TimeSpan.FromMilliseconds(idleTicks);
    }
}
