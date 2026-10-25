using System.Threading;
using System.Windows;

namespace ChronoPower;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    private const string MutexName = "ChronoPower_SingleInstance_Mutex_98a72b";
    private const string EventName = "ChronoPower_RestoreWindow_Event_98a72b";

    private Mutex? _mutex;
    private EventWaitHandle? _eventWaitHandle;
    private RegisteredWaitHandle? _registeredWait;
    private bool _ownsMutex;

    protected override void OnStartup(StartupEventArgs e)
    {
        _mutex = new Mutex(true, MutexName, out bool createdNew);

        if (!createdNew)
        {
            try
            {
                if (EventWaitHandle.TryOpenExisting(EventName, out var existing))
                {
                    existing.Set();
                    existing.Dispose();
                }
            }
            catch
            {
                // Mevcut kopya tetiklenemezse bile ikinci kopya kapanmalı
            }

            _mutex.Dispose();
            _mutex = null;
            Shutdown();
            return;
        }

        _ownsMutex = true;
        _eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, EventName);
        _registeredWait = ThreadPool.RegisterWaitForSingleObject(
            _eventWaitHandle,
            (state, timedOut) =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    (MainWindow as MainWindow)?.RestoreWindow();
                });
            },
            null,
            Timeout.Infinite,
            false);

        base.OnStartup(e);

        var mainWindow = new MainWindow();
        MainWindow = mainWindow;
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _registeredWait?.Unregister(null);
        _eventWaitHandle?.Dispose();
        if (_mutex != null)
        {
            if (_ownsMutex)
            {
                try { _mutex.ReleaseMutex(); } catch (ApplicationException) { }
            }
            _mutex.Dispose();
        }
        base.OnExit(e);
    }
}
