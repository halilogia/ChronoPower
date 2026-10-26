# 🧠 Mühendislik Hafızası & Kararlar (KNOWLEDGE.md)

Bu dosya, ChronoPower projesine dair oturumlar arası kalıcı kararları (ADR) ve karşılaşılan kritik teknik tuzakları (Gotchas) içerir.

---

## 🏛️ Mimari Karar Kayıtları (ADR)

### [ADR-01] Doğrudan Win32 P/Invoke Entegrasyonu
- **Karar:** Güç eylemleri için harici `shutdown.exe` veya PowerShell süreçleri çalıştırmak yerine doğrudan `powrprof.dll`, `user32.dll` ve `kernel32.dll` P/Invoke çağrıları kullanıldı.
- **Gerekçe:** Siyah konsol pencerelerinin açılmasını önlemek, milisaniye hassasiyetinde zamanlama yapmak ve işlem durumunu doğrudan dönüş kodlarıyla denetlemek.

### [ADR-02] Waitable Timer ile Otomatik Uyandırma
- **Karar:** Uyku sonrası sistemin uyanması için Windows `CreateWaitableTimerExW` ve `SetWaitableTimer` çekirdek API'leri kullanıldı.
- **Gerekçe:** Standart uygulama zamanlayıcıları sistem uyku moduna girdiğinde durur; kernel düzeyinde timer ise anakart RTC'sini tetikleyebilir.

---

## ⚠️ Kritik Teknik Tuzaklar (Gotchas)

### [GOTCHA-01] Tepside Küçülmüş Pencereyi Öne Getirme
- **Tuzak:** `App.xaml.cs` tekil örnek çalışırken, arka plandaki pencereyi sadece `Activate()` ile öne getirmek tepside küçülmüş pencerelerde başarısız oluyordu.
- **Çözüm:** Önce `ShowWindow(handle, SW_RESTORE)` ile durum normale döndürülmeli, ardından `SetForegroundWindow(handle)` çağrılmalıdır.

### [GOTCHA-02] Boşta Kalma Süresi (GetLastInputInfo) Taşması
- **Tuzak:** `GetLastInputInfo` `cbSize` değeri doğru verilmezse çağrı başarısız olur; ayrıca sistem 49.7 günden fazla açık kaldığında `TickCount` 32-bit tamsayı taşmasına uğrayabilir.
- **Çözüm:** `cbSize` struct başlatılırken `Marshal.SizeOf` ile set edilir ve işaretli çıkarma işlemiyle fark hesaplanır.
