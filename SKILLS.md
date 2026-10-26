# 📜 SKILLS.md — ChronoPower Ajan ve Geliştirici Yönergeleri

Bu dosya, projede çalışacak yapay zeka ajanları ve katkıcılar için mimari sınırları, çalışma kurallarını ve tamamlanma kriterlerini (Definition of Done) belirler.

---

## 🎯 Temel İlkeler ve Sınırlar

- **Platform:** Yalnızca Windows 10/11 x64 ve .NET 9.0 (`net9.0-windows`).
- **Sıfır Dış Paket Politikası:** Harici UI veya helper paketleri (Newtonsoft, MaterialDesignInXaml vb.) eklenmemeli, saf .NET WPF & Win32 API yetenekleri kullanılmalıdır.
- **Güvenlik & Donanım:** Güç API çağrılarında (`ExitWindowsEx`, `SetSuspendState`) kullanıcı verisi kaybını önlemek için doğrudan zorlayıcı kapatma (`EWX_FORCE`) yapılmadan önce daima Grace Period tetiklenmelidir.

---

## 🛠️ Mimari Kurallar

- **Katı MVVM Ayrımı:** `MainWindow.xaml.cs` veya `App.xaml.cs` içine doğrudan iş mantığı yazılamaz. Tüm durumlar `MainViewModel` üzerinden bağlanır.
- **P/Invoke İzolasyonu:** Tüm `[DllImport]` bildirimleri `Services/` katmanında kalmalıdır.
- **Anti-Bloat:** Core Docs dosyaları 200 satırı, C# kaynak kodları mümkün mertebe 400 satırı aşamaz.

---

## ✅ Tamamlanma Kriteri (Definition of Done)

Bir özellik veya düzeltmenin tamamlanmış sayılması için:
1. `dotnet build -c Release` hatasız derlenmeli.
2. Yapılan değişiklikler `CHANGELOG.md` dosyasına eklenmeli.
3. `TASKS.md` üzerindeki ilgili görev `[x]` yapılmalı.
4. Pre-Push kapısı doğrulanarak commit alınıp GitHub'a push edilmeli.
