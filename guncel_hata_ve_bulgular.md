# Güncel Hata ve Bulgular Taslağı

## Amaç

Bu dosya, proje üzerinde yapılacak güncel incelemelerde bulunan hata, eksik, risk ve çözüm önerilerini düzenli şekilde kaydetmek için hazırlanmış bir taslaktır.

Bu taslakta gerçek hata veya bulgu yer almaz. Her ekip üyesi kendi sorumluluk alanında yaptığı inceleme sonucunda ilgili bölümü doldurmalıdır.

## Kullanım Kuralları

- Her bulgu açık, kısa ve teknik olarak anlaşılır yazılmalıdır.
- Aynı problem birden fazla yerde tekrar edilmemelidir.
- Bir bulgu yazılırken mutlaka etkilenen dosya veya modül belirtilmelidir.
- Çözüm önerisi uygulanabilir ve test edilebilir olmalıdır.
- Her çözümden sonra test sonucu eklenmelidir.
- Emin olunmayan konular kesin hata gibi yazılmamalı, "risk" veya "kontrol edilmeli" olarak belirtilmelidir.

## Bulgu Yazım Formatı

Her yeni bulgu aşağıdaki formatla eklenmelidir:

```markdown
### Bulgu Başlığı

**Kategori:** Hata / Eksik / Risk / İyileştirme

**Sorumlu Kişi:** Ad Soyad

**Etkilenen Dosyalar:**
- Dosya adı veya modül adı

**Problem:**
Problemin kısa ve net açıklaması.

**Neden Önemli:**
Bu problemin projeye etkisi.

**Önerilen Çözüm:**
Uygulanacak çözüm yaklaşımı.

**Uygulanan Çözüm:**
Çözüm tamamlandıysa yapılan değişikliklerin özeti.

**Test Sonucu:**
Hangi kontrollerin yapıldığı ve sonucun ne olduğu.

**Durum:** Açık / Devam Ediyor / Çözüldü / Kontrol Edilecek
```

## 1. Parser ve HTML Ayrıştırma

Bu bölüm HTML girdisinin ayrıştırılması, token üretimi, etiket eşleştirme, hatalı HTML kontrolü ve DOM ağacı oluşturma süreci için kullanılmalıdır.

### Eklenecek Bulgular

```markdown
### Bulgu Başlığı

**Kategori:**

**Sorumlu Kişi:**

**Etkilenen Dosyalar:**

**Problem:**

**Neden Önemli:**

**Önerilen Çözüm:**

**Uygulanan Çözüm:**

**Test Sonucu:**

**Durum:**
```

## 2. Veri Yapıları

Bu bölüm Stack, Queue, HashTable ve DOM düğüm modeli gibi temel veri yapılarıyla ilgili incelemeler için kullanılmalıdır.

### Eklenecek Bulgular

```markdown
### Bulgu Başlığı

**Kategori:**

**Sorumlu Kişi:**

**Etkilenen Dosyalar:**

**Problem:**

**Neden Önemli:**

**Önerilen Çözüm:**

**Uygulanan Çözüm:**

**Test Sonucu:**

**Durum:**
```

## 3. DOM Algoritmaları ve Arama

Bu bölüm DFS, BFS, id arama, class arama, tag arama ve algoritma çıktılarının doğrulanması için kullanılmalıdır.

### Eklenecek Bulgular

```markdown
### Bulgu Başlığı

**Kategori:**

**Sorumlu Kişi:**

**Etkilenen Dosyalar:**

**Problem:**

**Neden Önemli:**

**Önerilen Çözüm:**

**Uygulanan Çözüm:**

**Test Sonucu:**

**Durum:**
```

## 4. Arayüz ve Görselleştirme

Bu bölüm HTML giriş alanı, DOM ağacı gösterimi, arama arayüzü, hata mesajları ve kullanıcı deneyimiyle ilgili incelemeler için kullanılmalıdır.

### Eklenecek Bulgular

```markdown
### Bulgu Başlığı

**Kategori:**

**Sorumlu Kişi:**

**Etkilenen Dosyalar:**

**Problem:**

**Neden Önemli:**

**Önerilen Çözüm:**

**Uygulanan Çözüm:**

**Test Sonucu:**

**Durum:**
```

## 5. Test, Dokümantasyon ve Entegrasyon

Bu bölüm test senaryoları, proje dokümantasyonu, branch birleştirme süreci, raporlama ve genel proje kontrolü için kullanılmalıdır.

### Eklenecek Bulgular

```markdown
### Bulgu Başlığı

**Kategori:**

**Sorumlu Kişi:**

**Etkilenen Dosyalar:**

**Problem:**

**Neden Önemli:**

**Önerilen Çözüm:**

**Uygulanan Çözüm:**

**Test Sonucu:**

**Durum:**
```

## İş Bölümü Takibi

| Alan | Sorumlu Kişi | Durum |
| --- | --- | --- |
| Parser ve HTML Ayrıştırma |  |  |
| Veri Yapıları |  |  |
| DOM Algoritmaları ve Arama |  |  |
| Arayüz ve Görselleştirme |  |  |
| Test, Dokümantasyon ve Entegrasyon |  |  |

## Genel Kontrol Listesi

- [ ] Her ekip üyesi kendi alanını inceledi.
- [ ] Bulgular standart formata göre yazıldı.
- [ ] Etkilenen dosyalar belirtildi.
- [ ] Çözüm önerileri eklendi.
- [ ] Uygulanan çözümler yazıldı.
- [ ] Test sonuçları eklendi.
- [ ] Son kontrol yapıldı.
