namespace ChronoPower.Models;

public enum PowerActionType
{
    Shutdown,
    Sleep,
    Restart,
    Hibernate,
    Lock,
    SignOut,
    DisplayOff
}

public class PowerAction
{
    public PowerActionType Type { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;
    public string IconData { get; init; } = string.Empty;
    public string AccentColor { get; init; } = "#6366F1";
    public bool SupportsForce { get; init; }

    public static List<PowerAction> GetAllActions()
    {
        return
        [
            new PowerAction
            {
                Type = PowerActionType.Shutdown,
                Title = "Bilgisayarı Kapat",
                Subtitle = "Sistemi tamamen ve güvenle kapatır",
                AccentColor = "#EF4444", // Kırmızı
                SupportsForce = true,
                // Güç açma/kapama ikonu SVG Path
                IconData = "M12 2v10m-6.364-7.636a9 9 0 1 0 12.728 0"
            },
            new PowerAction
            {
                Type = PowerActionType.Sleep,
                Title = "Uyku Modu (Sleep)",
                Subtitle = "RAM'i korur, düşük güçte bekler, anında uyanır",
                AccentColor = "#8B5CF6", // Mor
                SupportsForce = false,
                // Hilal / Ay ikonu
                IconData = "M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z"
            },
            new PowerAction
            {
                Type = PowerActionType.Restart,
                Title = "Yeniden Başlat",
                Subtitle = "Sistemi kapatıp temiz şekilde tekrar açar",
                AccentColor = "#3B82F6", // Mavi
                SupportsForce = true,
                // Yenileme döngü ikonu
                IconData = "M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"
            },
            new PowerAction
            {
                Type = PowerActionType.Hibernate,
                Title = "Hazırda Beklet",
                Subtitle = "Belleği diske yazar, sıfır güç tüketir",
                AccentColor = "#06B6D4", // Camgöbeği
                SupportsForce = false,
                // Kar tanesi / Dondurma ikonu
                IconData = "M12 3v18m9-9H3m15.364-6.364l-12.728 12.728m0-12.728l12.728 12.728"
            },
            new PowerAction
            {
                Type = PowerActionType.Lock,
                Title = "Ekranı Kilitle",
                Subtitle = "Oturumu kilitler, arkadaki işler devam eder",
                AccentColor = "#F59E0B", // Kehribar sarısı
                SupportsForce = false,
                // Asma kilit ikonu
                IconData = "M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"
            },
            new PowerAction
            {
                Type = PowerActionType.DisplayOff,
                Title = "Monitörü Kapat",
                Subtitle = "PC açık kalır (indirme/render), ekran sinyali kesilir",
                AccentColor = "#10B981", // Zümrüt yeşili
                SupportsForce = false,
                // Ekran ikonu
                IconData = "M9.75 17L9 20l-1 1h8l-1-1-.75-3M3 13h18M5 17h14a2 2 0 002-2V5a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"
            },
            new PowerAction
            {
                Type = PowerActionType.SignOut,
                Title = "Oturumu Kapat",
                Subtitle = "Mevcut Windows kullanıcısından çıkış yapar",
                AccentColor = "#EC4899", // Pembe
                SupportsForce = false,
                // Kullanıcı / Çıkış ikonu
                IconData = "M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"
            }
        ];
    }
}
