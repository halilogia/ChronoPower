using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ChronoPower.Models;

namespace ChronoPower.Services;

public class PowerService : IPowerService
{
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool LockWorkStation();

    [DllImport("user32.dll", EntryPoint = "SendMessageW")]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    private const uint WM_SYSCOMMAND = 0x0112;
    private static readonly IntPtr SC_MONITORPOWER = new(0xF170);
    private static readonly IntPtr MONITOR_OFF = new(2);
    private static readonly IntPtr HWND_BROADCAST = new(0xFFFF);

    public void ExecuteAction(PowerActionType action, bool force)
    {
        try
        {
            switch (action)
            {
                case PowerActionType.Shutdown:
                    ExecuteProcess("shutdown.exe", force ? "/s /f /t 0" : "/s /t 0");
                    break;

                case PowerActionType.Restart:
                    ExecuteProcess("shutdown.exe", force ? "/r /f /t 0" : "/r /t 0");
                    break;

                case PowerActionType.Hibernate:
                    try
                    {
                        System.Windows.Forms.Application.SetSuspendState(PowerState.Hibernate, force, false);
                    }
                    catch
                    {
                        ExecuteProcess("shutdown.exe", "/h");
                    }
                    break;

                case PowerActionType.Sleep:
                    // Bilgisayarı uyku moduna alır
                    System.Windows.Forms.Application.SetSuspendState(PowerState.Suspend, force, false);
                    break;

                case PowerActionType.Lock:
                    LockWorkStation();
                    break;

                case PowerActionType.SignOut:
                    ExecuteProcess("shutdown.exe", force ? "/l /f" : "/l");
                    break;

                case PowerActionType.DisplayOff:
                    // Monitörü kapatır
                    SendMessage(HWND_BROADCAST, WM_SYSCOMMAND, (IntPtr)SC_MONITORPOWER, MONITOR_OFF);
                    break;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Güç işlemi yürütülürken hata: {ex.Message}");
        }
    }

    public void CancelWindowsScheduledShutdown()
    {
        try
        {
            ExecuteProcess("shutdown.exe", "/a");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Windows kapatma iptal edilirken hata: {ex.Message}");
        }
    }

    public void ScheduleWakeTimer(DateTime wakeTime)
    {
        try
        {
            // Windows Task Scheduler ile RTC / ACPI donanım alarmlı uyandırma görevi
            string formattedTime = wakeTime.ToString("yyyy-MM-ddTHH:mm:ss");
            string psCommand = "$action = New-ScheduledTaskAction -Execute 'cmd.exe' -Argument '/c echo ChronoPower Wake'; " +
                               $"$trigger = New-ScheduledTaskTrigger -Once -At (Get-Date '{formattedTime}'); " +
                               "$settings = New-ScheduledTaskSettingsSet -WakeToRun; " +
                               "Register-ScheduledTask -TaskName 'ChronoPower_WakeUp' -Action $action -Trigger $trigger -Settings $settings -Force";

            var startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -NonInteractive -WindowStyle Hidden -Command \"{psCommand}\"",
                CreateNoWindow = true,
                UseShellExecute = false
            };
            using var process = Process.Start(startInfo);
            process?.WaitForExit(4000);
            Debug.WriteLine($"ChronoPower Wake Timer ayarlandı: {formattedTime}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Wake timer kurulurken hata: {ex.Message}");
        }
    }

    public void CancelWakeTimer()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "schtasks.exe",
                Arguments = "/Delete /TN \"ChronoPower_WakeUp\" /F",
                CreateNoWindow = true,
                UseShellExecute = false
            };
            using var process = Process.Start(startInfo);
            process?.WaitForExit(2000);
            Debug.WriteLine("ChronoPower Wake Timer iptal edildi.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Wake timer iptal edilirken hata: {ex.Message}");
        }
    }

    private static void ExecuteProcess(string fileName, string arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            CreateNoWindow = true,
            UseShellExecute = false
        };
        Process.Start(startInfo);
    }
}
