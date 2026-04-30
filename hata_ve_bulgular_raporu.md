# HATA VE BULGULAR RAPORU

Bu dokuman iki parcadan olusur:

- Ilk bolumde repoda daha once yer alan orijinal hata ve bulgular korunmustur.
- Ikinci bolumde yeni inceleme ve duzeltme surecinde eklenen guncel bulgular yer alir.

Kontrol edilen uzak branch'lar:

- `main`
- `phase1-data-structures`
- `phase1-dom-parser`
- `phase2-analysis-testing`
- `phase2-traversal-search`
- `phase3-ui-dom-visualizer`

## Bolum A - Orijinal Rapor Icerigi

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
- Stack Kapasite Yönetimi: HTML metninin ayrıştırılması ve hiyerarşik derinliğin takibi için kullanılacak Stack yapısı, statik sabit boyutlu bir dizi yerine dinamik liste altyapısıyla kurgulandı. Bu sayede, testlerde çok derin iç içe geçmiş HTML etiketleri geldiğinde Stack Overflow yaşanması engellendi.
- Hash Table Çakışma Yönetimi: HTML elemanlarının id özelliklerini indekslerken iki farklı ID'nin aynı yuvaya düşmesi (collision) ihtimaline karşı "Separate Chaining" (Bağlı Liste Zincirleme) algoritması özel bir iç sınıf yazılarak çözüldü.
- Zaman Karmaşıklığı Başarısı: Hash Table tasarımı sayesinde proje yönergesinde istenen getElementById işlemi test edildi ve düğüm sayısından bağımsız olarak ortalama `O(1)` zaman karmaşıklığı hedefine ulaşıldı.

**2. Karşılaşılan Hatalar ve Çözümler**
- Bug 01: Negatif Hash İndeksi Üretimi
  Durum: Hash Table dizisi için indeks numarası üretilirken, bazı string ID değerlerinde sistemin yerleşik fonksiyonunun negatif değerler döndürdüğü ve bunun `IndexOutOfRangeException` hatasına yol açtığı tespit edildi.
  Çözüm: İndeks hesaplama fonksiyonuna mutlak değer `Math.Abs()` işlemi eklenerek dizinin her zaman geçerli ve pozitif bir yuvaya işaret etmesi sağlandı.
- Bug 02: Boş Stackten Eleman Çıkarma Riski
  Durum: Hatalı yazılmış bir HTML dokümanı simüle edildiğinde, boş olan Stack yapısından `Pop` veya `Peek` yapılmaya çalışılmasının sistemi çökerttiği gözlemlendi.
  Çözüm: Stack sınıfının `Pop` ve `Peek` metotlarına `Count == 0` kontrolü eklendi. Sistem tamamen çökmek yerine durumu anlayıp kontrollü bir şekilde `InvalidOperationException` hatası fırlatacak hale getirildi.

### 3. `phase2-analysis-testing`

**Son commit:** `65ff5ca`

**Push durumu:** uzak sunucuya push edildi.

**Yapılanlar:**
- DOM ağacının hiyerarşik analizi için `TreeAnalyzer.cs` sınıfı oluşturuldu.
- Ağaç derinliği hesaplama (`CalculateDepth`), kardeş düğümleri bulma (`GetSiblings`) ve etiket ismine göre arama (`FindElementsByTagName`) metotları eklendi.

**Bulgular ve Mimari Kararlar:**
- Statik Erişim: Ağaç analizi yapan tüm metotlar, projenin her yerinden instance (nesne) oluşturulmadan doğrudan kullanılabilmesi için `static` olarak yapılandırıldı.
- Rekürsif (Özyineli) Yaklaşım: `CalculateDepth` metodunda derinlik tespiti için ağaç yapısına en uygun olan rekürsif yaklaşım kullanıldı. Her alt düğüm kendi derinliğini hesaplayarak maksimum derinliği geriye döner.

**Karşılaşılan Hatalar ve Çözümler:**
- Hata 01: Kök Düğümde (Root) Null Referans Hatası
  Çözüm: Metodun başlangıcına `if (node == null || node.Parent == null)` güvence kontrolü eklendi. Böylece üst düğümü olmayan hedef çağrılarında program çökmeden boş bir liste döndürüldü.

### 4. `phase2-traversal-search`

**Son commit:** `6f27eaf`

**Push durumu:** uzak sunucuya push edildi.

**Çözülen Hatalar**
- Kapsam (Scope) Hatası: Sınıf dışında kalan `SearchByTagName` metodu `DomAlgorithms` sınıfı içerisine taşınarak projenin derlenmesini (compile) engelleyen kritik hata giderildi.
- Büyük/Küçük Harf Duyarlılığı: HTML standartlarına uymayan kesin eşitlik (`==`) araması yerine `StringComparison.OrdinalIgnoreCase` kullanıldı.
- Null Referans Zafiyeti (NRE): İlgili metotların başına `if (node == null) return;` kontrolü eklendi. Parser modülünden boş kök düğüm (root node) gelmesi durumunda uygulamanın çökmesi engellendi.

### 5. `phase3-ui-dom-visualizer`

**Son commit:** `13b054e`

**Push durumu:** Uzak repoya gonderildi.

**Tamamlanan baslica calismalar:**
- HTML girdi alanı ve DOM ağacı görüntüleme arayüzü eklendi.
- Arama cubugu ile `id`, `class` ve `tag` bazlı sorgu desteği eklendi.
- Düğüm seçimi, vurgulama, derinlik ve alt ağac bilgileri gosterildi.

## Bolum B - Guncel Inceleme ve Yeni Duzeltmeler

Not: README icindeki eski `develop`, `phase-1`, `phase-2`, `phase-3` adlandirmasi repo gercegiyle uyusmuyordu. Dokumantasyon, uzak repodaki mevcut branch adlarina gore duzeltildi.

### 1. `phase1-data-structures`

**Durum:** Temel veri yapilari mevcut ve entegrasyonda kullaniliyor.

**Kontrol sonucu:**
- `DomNode`, `Stack`, `Queue` ve `HashTable` siniflari projede aktif olarak kullaniliyor.
- `HashTable` icin indeks hesabi daha guvenli hale getirildi.

**Yeni duzeltme:**
- Negatif hash degeri olusmasi halinde gecersiz indeks riski daha guvenli bir yontemle kapatildi.

**Uygulanan cozum:**
- Indeks hesabi `hashCode & 0x7fffffff` yaklasimina cevrildi.

### 2. `phase1-dom-parser`

**Durum:** Bu fazdaki en kritik eksik burada bulundu ve kapatildi.

**Tespit edilen hatalar:**
- `HtmlParser.Parse` hicbir zaman DOM olusturmuyordu.
- `Tokenize` bos liste donduruyordu.
- `CreateNodeFromTagContent` hic dugum uretmiyordu.
- Projede `.csproj` olmadigi icin C# tarafi dogrudan build edilemiyordu.

**Uygulanan cozumler:**
- Calisan bir `HtmlParser` uygulandi.
- Tokenizer; yorumlari ve `DOCTYPE` bildirimlerini atlayacak sekilde tamamlandi.
- `br`, `meta`, `img` gibi bos HTML etiketleri parser tarafinda desteklendi.
- `id` ve `class` ayrisma mantigi eklendi, `ElementTable` indekslemesi aktif hale getirildi.
- `DomParser.csproj` eklenerek proje `dotnet build` ile derlenebilir duruma getirildi.
- Metin dugumleri icin `DomNode` sinifina `TextContent` alani eklendi.

### 3. `phase2-analysis-testing`

**Durum:** Analiz metotlari calisiyor; entegrasyonu bloke eden yeni hata gorulmedi.

**Kontrol sonucu:**
- `TreeAnalyzer` metotlari mevcut veri modeliyle uyumlu.
- Parser tarafi calisir hale geldigi icin bu fazin metotlari artik gercek DOM agaci uzerinde kullanilabilir.

### 4. `phase2-traversal-search`

**Durum:** DFS, BFS ve arama metotlari entegrasyon icinde calisiyor.

**Kontrol sonucu:**
- `DomAlgorithms` sinifi derleme mantigi acisindan tutarli.
- Buyuk/kucuk harf duyarsiz tag aramasi korunuyor.
- Null kontrolleri mevcut.

### 5. `phase3-ui-dom-visualizer`

**Durum:** Arayuzun ayristirma cekirdeginde iki kritik uyumluluk hatasi duzeltildi.

**Tespit edilen hatalar:**
- Standart `<!DOCTYPE html>` girdileri ayrismiyordu.
- HTML bos etiketleri yalnizca XHTML tarzi `/>` ile calisiyordu.

**Uygulanan cozumler:**
- `dom-core.mjs` icinde `DOCTYPE` ve benzeri bildirimler token akisindan cikarildi.
- HTML bos etiketleri icin `VOID_ELEMENTS` listesi eklendi.
- Kapanis etiketi karsilastirmasi kucuk-buyuk harf duyarsiz hale getirildi.
- Beklenmeyen kapanis etiketlerinde daha kontrollu hata akisi saglandi.

## Genel Sonuc

Bu guncelleme sonrasinda:

- Orijinal rapor korunmus oldu.
- Yeni hata ve cozumler ayri bir bolumde belgelenmis oldu.
- JavaScript tabanli DOM gorsellestirici standart HTML girdilerine daha uyumlu calisiyor.
- C# parser artik gercekten DOM agaci uretiyor.
- C# kaynaklari proje dosyasi ile birlikte derlenebilir durumda.
- Branch dokumantasyonu repo gercegiyle uyumlu hale getirildi.

