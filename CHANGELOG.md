# 📝 Değişiklik Günlüğü (CHANGELOG.md)

Bu projedeki tüm önemli değişiklikler bu dosyada belgelenmektedir.  
Format [Keep a Changelog](https://keepachangelog.com/tr/1.0.0/) standardına dayanır ve [SemVer](https://semver.org/lang/tr/) kullanır.

---

## [1.0.0] - 2026-10-25

### Eklendi (Added)
- **Güç Eylemleri:** Kapatma, uyku, yeniden başlatma, hazırda bekletme ve oturum kilitleme Win32 entegrasyonları.
- **Zamanlama Seçenekleri:** Geri sayım sayacı, belirli bir saate ayarlama ve sistem boşta kalma süresi (idle detector).
- **Akıllı Uyarı Sistemi:** Kapatma öncesinde kullanıcıya iptal veya erteleme fırsatı veren 30 saniyelik sesli ve görsel grace period.
- **Otomatik Uyandırma:** Windows Waitable Timer API kullanılarak uyku modundan otomatik uyanma desteği.
- **Sistem Tepsisi:** Arka planda kesintisiz çalışma ve tepsiye küçülme modu.
- **Tekil Örnek Koruması:** Uygulamanın arka arkaya birden fazla pencere açmasını engelleyen Mutex ve pencere öne getirme mantığı.
- **Temel Dokümantasyon:** GPL-3.0 lisansı, mimari, görevler ve yol haritası belgeleri eklendi.
