# HATA VE BULGULAR



### 1. `phase1-data-structures`



### 2. `phase1-dom-parser`



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

