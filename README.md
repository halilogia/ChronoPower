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

## 📚 Dokümantasyon (Core Docs)

| Doküman | Açıklama |
| :--- | :--- |
| [ARCHITECTURE.md](ARCHITECTURE.md) | Katmanlar, Win32 P/Invoke API'leri ve MVVM mimari tasarımı |
| [ROADMAP.md](ROADMAP.md) | Gelecek sürümler, özellik hedefleri ve kilometre taşları |
| [CHANGELOG.md](CHANGELOG.md) | Sürüm geçmişi ve değişiklik dökümü (SemVer) |
| [KNOWLEDGE.md](KNOWLEDGE.md) | Mühendislik hafızası, kritik mimari kararlar (ADR) ve Gotchas |
| [TASKS.md](TASKS.md) | Aktif sprint görevleri ve tamamlanma durumu |
| [SKILLS.md](SKILLS.md) | Ajan anayasası, Definition of Done ve geliştirici kuralları |
| [LICENSE](LICENSE) | GNU General Public License v3.0 (GPL-3.0) tam metni |

---

## 📄 Lisans

Bu proje **GNU General Public License v3.0 (GPL-3.0)** ile lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakabilirsiniz.
