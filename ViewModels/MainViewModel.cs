using System.Collections.ObjectModel;
using System.Media;
using System.Windows.Threading;
using ChronoPower.Models;
using ChronoPower.Services;

namespace ChronoPower.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IPowerService _powerService;
    private readonly DispatcherTimer _timer;

    private PowerAction _selectedAction;
    private ScheduleModeType _selectedMode = ScheduleModeType.Countdown;
    private bool _isTaskActive;
    private bool _isGracePeriod;
    private bool _forceCloseApps;
    private bool _playAlertSound = true;
    private bool _minimizeToTray = true;

    // Otomatik Uyandırma (Wake Timer) ayarları
    private bool _enableWakeTimer;
    private int _wakeAfterMinutes = 60;
    private string _wakeInfoText = "Otomatik uyandırma kapalı.";

    // Geri sayım ayarları
    private int _countdownHours;
    private int _countdownMinutes = 30;

    // Belirli saat ayarları
    private int _targetHour;
    private int _targetMinute;

    // Boşta kalma ayarları
    private int _idleThresholdMinutes = 20;
    private TimeSpan _currentIdleTime = TimeSpan.Zero;

    // Aktif görev durumları
    private int _totalSeconds;
    private int _remainingSeconds;
    private double _progressPercentage;
    private DateTime? _targetDateTime;
    private string _formattedRemainingTime = "00 : 30 : 00";
    private string _statusMessage = "Hazır. Bir işlem seçip başlatabilirsiniz.";

    public MainViewModel(IPowerService? powerService = null)
    {
        _powerService = powerService ?? new PowerService();
        AvailableActions = new ObservableCollection<PowerAction>(PowerAction.GetAllActions());
        _selectedAction = AvailableActions[0]; // Kapat varsayılan

        // Belirli saat başlangıç değeri: Şu anki saat + 1 saat
        var nextHour = DateTime.Now.AddHours(1);
        _targetHour = nextHour.Hour;
        _targetMinute = (nextHour.Minute / 5) * 5; // 5 dakikalık yuvarlama

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += OnTimerTick;

        // Komutlar
        StartTaskCommand = new RelayCommand(StartTask, () => !IsTaskActive);
        CancelTaskCommand = new RelayCommand(CancelTask, () => IsTaskActive);
        ExecuteImmediatelyCommand = new RelayCommand(ExecuteImmediately);
        AddMinutesCommand = new RelayCommand(p =>
        {
            if (p != null && int.TryParse(p.ToString(), out int mins))
                AddMinutes(mins);
        }, _ => IsTaskActive);
        SetPresetDurationCommand = new RelayCommand(p =>
        {
            if (p != null && int.TryParse(p.ToString(), out int mins))
                SetPresetDuration(mins);
        }, _ => !IsTaskActive);
        SetPresetWakeDurationCommand = new RelayCommand(p =>
        {
            if (p != null && int.TryParse(p.ToString(), out int mins))
                WakeAfterMinutes = mins;
        }, _ => !IsTaskActive);
        SelectActionCommand = new RelayCommand<PowerAction>(action =>
        {
            if (action != null) SelectedAction = action;
        });
        SelectModeCommand = new RelayCommand(p =>
        {
            if (p is ScheduleModeType mode) SelectedMode = mode;
            else if (p != null && Enum.TryParse<ScheduleModeType>(p.ToString(), out var parsed))
                SelectedMode = parsed;
        });

        UpdateWakePreview();
    }

    #region Properties

    public ObservableCollection<PowerAction> AvailableActions { get; }

    public PowerAction SelectedAction
    {
        get => _selectedAction;
        set
        {
            if (SetProperty(ref _selectedAction, value))
            {
                OnPropertyChanged(nameof(SupportsWakeTimer));
                UpdateWakePreview();
            }
        }
    }

    public ScheduleModeType SelectedMode
    {
        get => _selectedMode;
        set
        {
            if (SetProperty(ref _selectedMode, value))
            {
                OnPropertyChanged(nameof(IsCountdownMode));
                OnPropertyChanged(nameof(IsAtTimeMode));
                OnPropertyChanged(nameof(IsIdleMode));
                UpdateWakePreview();
            }
        }
    }

    public bool IsCountdownMode => SelectedMode == ScheduleModeType.Countdown;
    public bool IsAtTimeMode => SelectedMode == ScheduleModeType.AtTime;
    public bool IsIdleMode => SelectedMode == ScheduleModeType.Idle;

    public bool IsTaskActive
    {
        get => _isTaskActive;
        set
        {
            if (SetProperty(ref _isTaskActive, value))
            {
                OnPropertyChanged(nameof(IsTaskInactive));
            }
        }
    }

    public bool IsTaskInactive => !IsTaskActive;

    public bool IsGracePeriod
    {
        get => _isGracePeriod;
        set => SetProperty(ref _isGracePeriod, value);
    }

    public bool ForceCloseApps
    {
        get => _forceCloseApps;
        set => SetProperty(ref _forceCloseApps, value);
    }

    public bool PlayAlertSound
    {
        get => _playAlertSound;
        set => SetProperty(ref _playAlertSound, value);
    }

    public bool MinimizeToTray
    {
        get => _minimizeToTray;
        set => SetProperty(ref _minimizeToTray, value);
    }

    public bool SupportsWakeTimer =>
        SelectedAction?.Type == PowerActionType.Hibernate ||
        SelectedAction?.Type == PowerActionType.Sleep;

    public bool EnableWakeTimer
    {
        get => _enableWakeTimer;
        set
        {
            if (SetProperty(ref _enableWakeTimer, value))
            {
                UpdateWakePreview();
                if (!_enableWakeTimer && IsTaskActive)
                {
                    _powerService.CancelWakeTimer();
                }
                else if (_enableWakeTimer && IsTaskActive && SupportsWakeTimer)
                {
                    var wakeTime = (TargetDateTime ?? DateTime.Now).AddMinutes(WakeAfterMinutes);
                    _powerService.ScheduleWakeTimer(wakeTime);
                }
            }
        }
    }

    public int WakeAfterMinutes
    {
        get => _wakeAfterMinutes;
        set
        {
            if (SetProperty(ref _wakeAfterMinutes, Math.Clamp(value, 5, 1440)))
            {
                UpdateWakePreview();
                if (EnableWakeTimer && IsTaskActive && SupportsWakeTimer)
                {
                    var wakeTime = (TargetDateTime ?? DateTime.Now).AddMinutes(_wakeAfterMinutes);
                    _powerService.ScheduleWakeTimer(wakeTime);
                }
            }
        }
    }

    public string WakeInfoText
    {
        get => _wakeInfoText;
        set => SetProperty(ref _wakeInfoText, value);
    }

    public int CountdownHours
    {
        get => _countdownHours;
        set
        {
            if (SetProperty(ref _countdownHours, Math.Clamp(value, 0, 99)))
            {
                UpdateFormattedPreview();
                UpdateWakePreview();
            }
        }
    }

    public int CountdownMinutes
    {
        get => _countdownMinutes;
        set
        {
            if (SetProperty(ref _countdownMinutes, Math.Clamp(value, 0, 59)))
            {
                UpdateFormattedPreview();
                UpdateWakePreview();
            }
        }
    }

    public int TargetHour
    {
        get => _targetHour;
        set
        {
            if (SetProperty(ref _targetHour, Math.Clamp(value, 0, 23)))
            {
                UpdateWakePreview();
            }
        }
    }

    public int TargetMinute
    {
        get => _targetMinute;
        set
        {
            if (SetProperty(ref _targetMinute, Math.Clamp(value, 0, 59)))
            {
                UpdateWakePreview();
            }
        }
    }

    public int IdleThresholdMinutes
    {
        get => _idleThresholdMinutes;
        set
        {
            if (SetProperty(ref _idleThresholdMinutes, Math.Clamp(value, 1, 300)))
            {
                UpdateWakePreview();
            }
        }
    }

    public TimeSpan CurrentIdleTime
    {
        get => _currentIdleTime;
        set => SetProperty(ref _currentIdleTime, value);
    }

    public string FormattedRemainingTime
    {
        get => _formattedRemainingTime;
        set => SetProperty(ref _formattedRemainingTime, value);
    }

    public double ProgressPercentage
    {
        get => _progressPercentage;
        set => SetProperty(ref _progressPercentage, value);
    }

    public DateTime? TargetDateTime
    {
        get => _targetDateTime;
        set
        {
            if (SetProperty(ref _targetDateTime, value))
            {
                OnPropertyChanged(nameof(TargetDateTimeText));
                UpdateWakePreview();
            }
        }
    }

    public string TargetDateTimeText
    {
        get
        {
            if (!TargetDateTime.HasValue) return string.Empty;
            var dt = TargetDateTime.Value;
            var dayText = dt.Date == DateTime.Today ? "Bugün" : "Yarın";
            return $"Planlanan Zaman: {dayText} {dt:HH:mm:ss}";
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    #endregion

    #region Commands

    public RelayCommand StartTaskCommand { get; }
    public RelayCommand CancelTaskCommand { get; }
    public RelayCommand ExecuteImmediatelyCommand { get; }
    public RelayCommand AddMinutesCommand { get; }
    public RelayCommand SetPresetDurationCommand { get; }
    public RelayCommand SetPresetWakeDurationCommand { get; }
    public RelayCommand<PowerAction> SelectActionCommand { get; }
    public RelayCommand SelectModeCommand { get; }

    #endregion

    #region Command Handlers

    public void StartTask()
    {
        int secondsToRun = 0;
        DateTime now = DateTime.Now;

        switch (SelectedMode)
        {
            case ScheduleModeType.Countdown:
                secondsToRun = (CountdownHours * 3600) + (CountdownMinutes * 60);
                if (secondsToRun <= 0)
                {
                    StatusMessage = "Lütfen 0'dan büyük bir süre belirleyin.";
                    return;
                }
                TargetDateTime = now.AddSeconds(secondsToRun);
                break;

            case ScheduleModeType.AtTime:
                var target = new DateTime(now.Year, now.Month, now.Day, TargetHour, TargetMinute, 0);
                if (target <= now)
                {
                    // Eğer seçilen saat geçmişte kalmışsa, yarınki o saat kastedilmiştir
                    target = target.AddDays(1);
                }
                secondsToRun = (int)(target - now).TotalSeconds;
                TargetDateTime = target;
                break;

            case ScheduleModeType.Idle:
                // Boşta kalma eşiği
                secondsToRun = IdleThresholdMinutes * 60;
                TargetDateTime = null;
                break;
        }

        _totalSeconds = secondsToRun;
        _remainingSeconds = secondsToRun;
        IsGracePeriod = false;
        ProgressPercentage = 0;

        UpdateRemainingDisplay();
        IsTaskActive = true;

        if (EnableWakeTimer && SupportsWakeTimer)
        {
            var wakeTime = (TargetDateTime ?? DateTime.Now.AddSeconds(secondsToRun)).AddMinutes(WakeAfterMinutes);
            _powerService.ScheduleWakeTimer(wakeTime);
            StatusMessage = $"{SelectedAction.Title} geri sayımı başladı. (Otomatik uyandırma: {wakeTime:HH:mm})";
        }
        else
        {
            StatusMessage = $"{SelectedAction.Title} için geri sayım başladı.";
        }

        if (PlayAlertSound)
        {
            SystemSounds.Asterisk.Play();
        }

        _timer.Start();
    }

    public void CancelTask()
    {
        _timer.Stop();
        IsTaskActive = false;
        IsGracePeriod = false;
        ProgressPercentage = 0;
        TargetDateTime = null;

        _powerService.CancelWindowsScheduledShutdown();
        _powerService.CancelWakeTimer();

        StatusMessage = "Planlanan görev iptal edildi.";
        UpdateFormattedPreview();
        UpdateWakePreview();

        if (PlayAlertSound)
        {
            SystemSounds.Hand.Play();
        }
    }

    public void ExecuteImmediately()
    {
        _timer.Stop();
        IsTaskActive = false;
        if (EnableWakeTimer && SupportsWakeTimer)
        {
            var wakeTime = DateTime.Now.AddMinutes(WakeAfterMinutes);
            _powerService.ScheduleWakeTimer(wakeTime);
            StatusMessage = $"{SelectedAction.Title} hemen uygulanıyor... (Uyandırma: {wakeTime:HH:mm})";
        }
        else
        {
            StatusMessage = $"{SelectedAction.Title} hemen uygulanıyor...";
        }
        _powerService.ExecuteAction(SelectedAction.Type, ForceCloseApps);
    }

    public void AddMinutes(int minutes)
    {
        if (!IsTaskActive || SelectedMode == ScheduleModeType.Idle) return;

        int extraSeconds = minutes * 60;
        _remainingSeconds += extraSeconds;
        _totalSeconds += extraSeconds;
        if (TargetDateTime.HasValue)
        {
            TargetDateTime = TargetDateTime.Value.AddMinutes(minutes);
        }

        if (EnableWakeTimer && SupportsWakeTimer && TargetDateTime.HasValue)
        {
            var updatedWake = TargetDateTime.Value.AddMinutes(WakeAfterMinutes);
            _powerService.ScheduleWakeTimer(updatedWake);
        }

        if (_remainingSeconds > 60)
        {
            IsGracePeriod = false;
        }

        UpdateRemainingDisplay();
        StatusMessage = $"{minutes} dakika süre eklendi.";

        if (PlayAlertSound)
        {
            SystemSounds.Beep.Play();
        }
    }

    public void SetPresetDuration(int minutes)
    {
        CountdownHours = minutes / 60;
        CountdownMinutes = minutes % 60;
    }

    #endregion

    #region Timer Logic

    private void OnTimerTick(object? sender, EventArgs e)
    {
        if (SelectedMode == ScheduleModeType.Idle)
        {
            // Boşta kalma modunda kullanıcı aktivitesini dinle
            var idleTime = IdleDetector.GetIdleTime();
            CurrentIdleTime = idleTime;

            var threshold = TimeSpan.FromMinutes(IdleThresholdMinutes);
            if (idleTime >= threshold)
            {
                TriggerAction();
            }
            else
            {
                var remaining = threshold - idleTime;
                _remainingSeconds = (int)remaining.TotalSeconds;
                _totalSeconds = (int)threshold.TotalSeconds;
                UpdateRemainingDisplay();
                StatusMessage = $"Boşta kalma süresi: {idleTime:mm\\:ss} / {threshold:mm\\:ss}";
            }
            return;
        }

        // Countdown veya AtTime modu
        if (_remainingSeconds > 0)
        {
            _remainingSeconds--;
            UpdateRemainingDisplay();

            // Son 60 saniye uyarısı
            if (_remainingSeconds <= 60 && !IsGracePeriod)
            {
                IsGracePeriod = true;
                StatusMessage = $"DİKKAT: 1 dakika içinde {SelectedAction.Title} gerçekleşecek!";
                if (PlayAlertSound)
                {
                    SystemSounds.Exclamation.Play();
                }
            }
        }
        else
        {
            TriggerAction();
        }
    }

    private void TriggerAction()
    {
        _timer.Stop();
        IsTaskActive = false;
        if (EnableWakeTimer && SupportsWakeTimer)
        {
            var wakeTime = DateTime.Now.AddMinutes(WakeAfterMinutes);
            _powerService.ScheduleWakeTimer(wakeTime);
            StatusMessage = $"{SelectedAction.Title} uygulanıyor... (Uyandırma: {wakeTime:HH:mm})";
        }
        else
        {
            StatusMessage = $"{SelectedAction.Title} uygulanıyor...";
        }

        if (PlayAlertSound)
        {
            SystemSounds.Exclamation.Play();
        }

        _powerService.ExecuteAction(SelectedAction.Type, ForceCloseApps);
    }

    private void UpdateRemainingDisplay()
    {
        var ts = TimeSpan.FromSeconds(Math.Max(0, _remainingSeconds));
        FormattedRemainingTime = $"{(int)ts.TotalHours:D2} : {ts.Minutes:D2} : {ts.Seconds:D2}";

        if (_totalSeconds > 0)
        {
            double elapsed = _totalSeconds - _remainingSeconds;
            ProgressPercentage = Math.Clamp((elapsed / _totalSeconds) * 100.0, 0, 100);
        }
    }

    private void UpdateFormattedPreview()
    {
        if (!IsTaskActive)
        {
            FormattedRemainingTime = $"{CountdownHours:D2} : {CountdownMinutes:D2} : 00";
        }
    }

    private DateTime CalculateEstimatedActionTime()
    {
        var now = DateTime.Now;
        if (IsTaskActive && TargetDateTime.HasValue)
            return TargetDateTime.Value;

        return SelectedMode switch
        {
            ScheduleModeType.Countdown => now.AddHours(CountdownHours).AddMinutes(CountdownMinutes),
            ScheduleModeType.AtTime => (new DateTime(now.Year, now.Month, now.Day, TargetHour, TargetMinute, 0) <= now)
                ? new DateTime(now.Year, now.Month, now.Day, TargetHour, TargetMinute, 0).AddDays(1)
                : new DateTime(now.Year, now.Month, now.Day, TargetHour, TargetMinute, 0),
            ScheduleModeType.Idle => now.AddMinutes(IdleThresholdMinutes),
            _ => now
        };
    }

    private void UpdateWakePreview()
    {
        if (!SupportsWakeTimer)
        {
            WakeInfoText = "💡 Otomatik uyandırma özelliği 'Hazırda Beklet' ve 'Uyku' modlarında kullanılabilir.";
            return;
        }

        if (!EnableWakeTimer)
        {
            WakeInfoText = "Otomatik uyandırma kapalı. İhtiyaç halinde kutucuğu işaretleyebilirsiniz.";
            return;
        }

        var actionTime = CalculateEstimatedActionTime();
        var wakeTime = actionTime.AddMinutes(WakeAfterMinutes);
        string dayStr = wakeTime.Date == DateTime.Today ? "Bugün" : "Yarın";
        int hours = WakeAfterMinutes / 60;
        int mins = WakeAfterMinutes % 60;
        string durStr = hours > 0 ? (mins > 0 ? $"{hours} sa {mins} dk" : $"{hours} saat") : $"{mins} dakika";

        WakeInfoText = $"⚡ {SelectedAction?.Title ?? "Eylem"} sonrası {durStr} sonra ({dayStr} saat {wakeTime:HH:mm}'de) PC otomatik uyandırılacak.";
    }

    #endregion
}
