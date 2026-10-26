# 🗺️ ChronoPower Yol Haritası (ROADMAP.md)

---

## 🚀 v1.0 (MVP - Tamamlandı)
- [x] Modern Fluent/Dark temalı WPF kullanıcı arayüzü
- [x] Temel güç eylemleri: Kapat (Shutdown), Uyku (Sleep), Yeniden Başlat (Restart), Hazırda Beklet (Hibernate), Kilitle (Lock)
- [x] 3 Zamanlama Modu: Geri Sayım, Belirli Saat, Boşta Kalma (Idle Detection)
- [x] Sistem tepsisi (System Tray / Minimize to Tray) entegrasyonu
- [x] Son saniyelerde sesli ve görsel iptal penceresi (Grace Period)
- [x] Windows Waitable Timer ile uyku sonrası otomatik uyanma (Wake Timer) desteği
- [x] Tekil örnek (Single-instance Mutex) koruması

---

## ⚡ v1.1 (Aktif Planlama)
- [ ] Belirli bir işlem/program (Process) kapandığında otomatik güç eylemi tetikleme (Örn: Render veya oyun bitince)
- [ ] Çoklu dil desteği (Türkçe & İngilizce dinamik geçiş)
- [ ] Özel ses dosyası (.wav) seçebilme
- [ ] Zamanlanmış haftalık/günlük yinelenen görevler

---

## 🔮 v2.0 (Gelecek Vizyon)
- [ ] Ağ üzerinden uzaktan güç yönetimi (Local Webhook / Mobil tetikleme)
- [ ] Pil yüzdesine göre otomatik tasarruf / uyku profilleri (Laptop desteği)
- [ ] Windows başlangıcında otomatik ve sessiz başlama (Startup Task)
