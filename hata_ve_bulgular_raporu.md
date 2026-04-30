# HATA VE BULGULAR



### 1. `phase1-data-structures`
#### Muhsin - N-ary Tree + Queue

**Yapılanlar:**
- DomNode.cs (N-ary Tree) eklendi
- Queue.cs (Queue) eklendi
- Pull Request açıldı

**Karşılaşılan Hatalar:**
- Henüz yok

**Çözümler:**
- Yok



### 2. `phase1-dom-parser`

**Son commit:** `2d23be1`

**Push durumu:** Uzak repoya gonderildi.

**1. Temel Bulgular ve Mimari Kararlar**
  * Stack Kapasite Yönetimi: HTML metninin ayrıştırılması ve hiyerarşik derinliğin takibi için kullanılacak Stack yapısı, statik sabit boyutlu bir dizi yerine dinamik liste altyapısıyla kurgulandı. Bu sayede, testlerde çok derin iç içe geçmiş HTML etiketleri geldiğinde Stack Overflow yaşanması engellendi.
  * Hash Table Çakışma Yönetimi: HTML elemanlarının id özelliklerini indekslerken iki farklı ID'nin aynı yuvaya düşmesi (collision) ihtimaline karşı "Separate Chaining" (Bağlı Liste Zincirleme) algoritması özel bir iç sınıf yazılarak çözüldü.
  * Zaman Karmaşıklığı Başarısı: Hash Table tasarımı sayesinde proje yönergesinde istenen getElementById işlemi test edildi ve düğüm sayısından bağımsız olarak ortalama $O(1)$ zaman karmaşıklığı hedefine ulaşıldı.

**2. Karşılaşılan Hatalar ve ÇözümlerHata**
   * Bug 01: Negatif Hash İndeksi ÜretimiDurum: Hash Table dizisi için indeks numarası üretilirken, bazı string ID değerlerinde sistemin yerleşik fonksiyonunun negatif değerler döndürdüğü ve bunun IndexOutOfRangeException hatasına yol açtığı tespit edildi.
   Çözüm: İndeks hesaplama fonksiyonuna mutlak değer Math.Abs() işlemi eklenerek dizinin her zaman geçerli ve pozitif bir yuvaya işaret etmesi sağlandı.
   * Bug 02: Boş Stackten Eleman Çıkarma RiskiDurum: Hatalı yazılmış bir HTML dokümanı simüle edildiğinde, boş olan Stack yapısından Pop veya Peek yapılmaya çalışılmasının sistemi çökerttiği gözlemlendi.
   Çözüm: Stack sınıfının Pop ve Peek metotlarına Count == 0 kontrolü eklendi. Sistem tamamen çökmek yerine durumu anlayıp kontrollü bir şekilde InvalidOperationException hatası fırlatacak hale getirildi.

### 3. `phase2-analysis-testing`



### 4. `phase2-traversal-search`

Bug: DomAlgorithms sınıfında kapsam hatası ve eksik HtmlParser implementasyonu

## Tespit Edilen Sorunlar

1. **Kapsam (Scope) Hatasinin Giderilmesi:**
   * `DomAlgorithms` sinifinin disinda kalan `SearchByTagName` metodu sinif icine alinarak derleme hatasi cozuldu.
   * `SearchByTagName` metoduna buyuk/kucuk harf duyarsizligi (`StringComparison.OrdinalIgnoreCase`) eklenerek arama iyilestirildi.

2. **HtmlParser Sinifinin Aktiflestirilmesi:**
   * **Tokenizer:** HTML metnindeki etiketleri (`<tag>`) ve metinleri ayirmak icin Regex tabanli bir parcalayici (Tokenizer) yazildi.
   * **DOM Agaci Insasi:** Acilis ve kapanis etiketlerini eslestirmek ve parent-child (ebeveyn-cocuk) hiyerarsisini kurmak icin ozel `Stack` yapisi `Parse` metodunda kullanildi.
   * **Nitelik (Attribute) Yonetimi:** Etiketlerdeki `id` ve `class` degerleri ayristirildi. Hizli arama, yani O(1) karmasikligi saglamasi icin `id` degerleri `HashTable` (`ElementTable`) yapisina eklendi.

### 5. `phase3-ui-dom-visualizer`

**Son commit:** `13b054e`

**Push durumu:** Uzak repoya gonderildi.

**Tamamlanan baslica calismalar:**

- HTML girdi alanı ve DOM ağacı görüntüleme arayüzü eklendi.
- Arama cubugu ile `id`, `class` ve `tag` bazlı sorgu desteği eklendi.
- Düğüm seçimi, vurgulama, derinlik ve alt ağac bilgileri gosterildi.

