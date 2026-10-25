using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using ChronoPower.Services;
using ChronoPower.ViewModels;

namespace ChronoPower;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;
    private NotifyIcon? _notifyIcon;
    private bool _isExplicitExit;

    public MainWindow()
    {
        InitializeComponent();

        _viewModel = new MainViewModel();
        DataContext = _viewModel;

        InitializeTrayIcon();
        SetWindowIcon();

        StateChanged += MainWindow_StateChanged;
        Closing += MainWindow_Closing;
    }

    private void SetWindowIcon()
    {
        try
        {
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            var iconPath = Path.Combine(appDir, "ChronoPower.ico");
            var icon = AppIconGenerator.GetOrCreateAppIcon(iconPath);

            var iconSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            this.Icon = iconSource;
        }
        catch
        {
            // İkon yükleme hatası durumunda varsayılan ikon kalabilir
        }
    }

    private void InitializeTrayIcon()
    {
        try
        {
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            var iconPath = Path.Combine(appDir, "ChronoPower.ico");
            var icon = AppIconGenerator.GetOrCreateAppIcon(iconPath);

            _notifyIcon = new NotifyIcon
            {
                Icon = icon,
                Text = "ChronoPower — Akıllı Güç Asistanı",
                Visible = true
            };

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("ChronoPower'ı Aç", null, (s, e) => RestoreWindow());
            contextMenu.Items.Add("Görevi İptal Et", null, (s, e) => _viewModel.CancelTask());
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("Çıkış", null, (s, e) => ExitApplication());

            _notifyIcon.ContextMenuStrip = contextMenu;
            _notifyIcon.DoubleClick += (s, e) => RestoreWindow();
        }
        catch
        {
            // Bildirim alanı desteklenmiyorsa sessizce devam et
        }
    }

    private void MainWindow_StateChanged(object? sender, EventArgs e)
    {
        if (WindowState == WindowState.Minimized && _viewModel.MinimizeToTray)
        {
            Hide();
            _notifyIcon?.ShowBalloonTip(
                2000,
                "ChronoPower Arka Planda",
                "Zamanlayıcı saatin yanında çalışmaya devam ediyor.",
                ToolTipIcon.Info);
        }
    }

    private void MainWindow_Closing(object? sender, CancelEventArgs e)
    {
        if (_isExplicitExit)
        {
            Cleanup();
            return;
        }

        // Eğer aktif görev varsa ve tepsiye küçültme aktifse pencereyi gizle
        if (_viewModel.IsTaskActive && _viewModel.MinimizeToTray)
        {
            e.Cancel = true;
            WindowState = WindowState.Minimized;
            Hide();
            _notifyIcon?.ShowBalloonTip(
                2500,
                "ChronoPower Devam Ediyor",
                $"Aktif {_viewModel.SelectedAction.Title} göreviniz arka planda saymaya devam ediyor.",
                ToolTipIcon.Info);
            return;
        }

        // Eğer aktif görev varsa ve kullanıcı gerçekten çıkmak istiyorsa sor
        if (_viewModel.IsTaskActive)
        {
            var result = System.Windows.MessageBox.Show(
                "Aktif bir zamanlama görevi çalışıyor.\nÇıkarsanız planlanan işlem iptal edilecek. Çıkmak istiyor musunuz?",
                "ChronoPower — Görev İptali Onayı",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
                return;
            }

            _viewModel.CancelTask();
        }

        Cleanup();
    }

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    private const int SW_RESTORE = 9;

    public void RestoreWindow()
    {
        Show();
        if (WindowState == WindowState.Minimized)
        {
            WindowState = WindowState.Normal;
        }
        var handle = new WindowInteropHelper(this).Handle;
        if (handle != IntPtr.Zero)
        {
            ShowWindow(handle, SW_RESTORE);
            SetForegroundWindow(handle);
        }
        Activate();
        Topmost = true;
        Topmost = false;
        Focus();
    }

    private void ExitApplication()
    {
        _isExplicitExit = true;
        if (_viewModel.IsTaskActive)
        {
            _viewModel.CancelTask();
        }
        Cleanup();
        System.Windows.Application.Current.Shutdown();
    }

    private void Cleanup()
    {
        if (_notifyIcon != null)
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            _notifyIcon = null;
        }
    }
}