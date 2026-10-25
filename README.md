# ⚡ ChronoPower

> **Modern, Planlı Bilgisayar Kapatma, Uyku ve Güç Yönetimi Asistanı**  
> *Modern & Intelligent Power Scheduler for Windows (.NET 9 WPF)*

---

## 🌟 Özellikler (Features)

- ⏱️ **Esnek Zamanlama Modları:**
  - **Geri Sayım:** Belirlediğiniz süre (saat / dakika) sonunda otomatik işlem.
  - **Belirli Saat:** Günün belirli bir saatinde (HH:mm) tetiklenme.
  - **Boşta Kalma (Idle Detection):** Kullanıcı bilgisayar başında olmadığında otomatik devreye girme.
- ⚡ **Güç Eylemleri:**
  - Bilgisayarı Kapat (Shutdown)
  - Uyku Modu (Sleep)
  - Yeniden Başlat (Restart)
  - Hazırda Beklet (Hibernate)
  - Kilitle (Lock Screen)
- 🔔 **Akıllı Uyarı Sistemi:**
  - Son saniyelerde sesli ve görsel bildirim (Grace period).
  - Kullanıcıya işlemi iptal etme veya erteleme fırsatı.
- ⏰ **Otomatik Uyandırma (Wake Timers):**
  - Uyku modundan belirlenen süre sonra otomatik uyanma desteği.
- 🗔 **Modern Arayüz & Sistem Tepsisi (Tray):**
  - Fluent/modern koyu tema tasarımı.
  - Arka planda sessiz çalışma, sistem tepsisine küçülme ve tek tıkla geri yükleme.

---

## 🛠️ Teknolojiler & Gereksinimler

- **Platform:** Windows 10 / 11
- **Framework:** .NET 9.0 (`net9.0-windows`)
- **Mimari:** WPF (Windows Presentation Foundation) & MVVM

---

## 🚀 Derleme ve Çalıştırma

```bash
# Projeyi derleyin
dotnet build -c Release

# Çalıştırın
dotnet run
```

---

## 📄 Lisans

Bu proje MIT lisansı ile lisanslanmıştır.
