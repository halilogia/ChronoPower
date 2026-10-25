namespace ChronoPower.Models;

public enum ScheduleModeType
{
    Countdown, // Geri Sayım (örn: 30 dakika sonra)
    AtTime,    // Belirli bir saatte (örn: 23:45'te)
    Idle       // Boşta kalınca (örn: 20 dakika işlem yapılmazsa)
}
