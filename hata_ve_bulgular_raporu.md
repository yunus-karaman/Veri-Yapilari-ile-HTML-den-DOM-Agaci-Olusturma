# HATA VE BULGULAR RAPORU

Bu rapor iki bölümden oluşmaktadır;

- İlk bolümde repoda daha önce yer alan hata ve bulgular raporlanmıştır.
- Ikinci bolumde yeni inceleme ve duzeltme surecinde eklenen guncel bulgular yer alir.


## ilk bölüm hata ve bulguları

### 1. `phase1-data-structures`

**Yapılanlar:**
- DomNode.cs (N-ary Tree) eklendi.
- Queue.cs (Queue) eklendi.


### 2. `phase1-dom-parser`

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

**Not:** Bu commit bilgisi, korunmus tarihsel rapor metninden gelmektedir. Guncel uzak branch basi `origin/phase2-traversal-search` icin `b2c8290` commit'idir.
**Çözülen Hatalar**
- Kapsam (Scope) Hatası: Sınıf dışında kalan `SearchByTagName` metodu `DomAlgorithms` sınıfı içerisine taşınarak projenin derlenmesini (compile) engelleyen kritik hata giderildi.
- Büyük/Küçük Harf Duyarlılığı: HTML standartlarına uymayan kesin eşitlik (`==`) araması yerine `StringComparison.OrdinalIgnoreCase` kullanıldı.
- Null Referans Zafiyeti (NRE): İlgili metotların başına `if (node == null) return;` kontrolü eklendi. Parser modülünden boş kök düğüm (root node) gelmesi durumunda uygulamanın çökmesi engellendi.

### 5. `phase3-ui-dom-visualizer`


**Tamamlanan başlıca çalışmalar:**
- HTML girdi alanı ve DOM ağacı görüntüleme arayüzü eklendi.
- Arama çubuğu ile `id`, `class` ve `tag` bazlı sorgu desteği eklendi.
- Düğüm seçimi, vurgulama, derinlik ve alt ağac bilgileri gösterildi.

##  Guncel Inceleme ve Yeni Duzeltmeler


### 1. `phase1-data-structures`

**Durum:** Temel veri yapıları mevcut ve entegrasyonda kullanılıyor.

**Kontrol sonucu:**
- `DomNode`, `Stack`, `Queue` ve `HashTable` siniflari projede aktif olarak kullaniliyor.
- `HashTable` icin indeks hesabi daha guvenli hale getirildi.

**Yeni duzeltme:**
- Negatif hash değeri oluşması halinde geçersiz indeks riski daha güvenli bir yöntemle kapatıldı.

**Uygulanan cozum:**
- Indeks hesabı `hashCode & 0x7fffffff` yaklaşımına çevrildi.

### 2. `phase1-dom-parser`


**Tespit edilen hatalar:**
- `HtmlParser.Parse` hiçbir zaman DOM oluşturmuyordu.
- `Tokenize` boş liste döndürüyordu.
- `CreateNodeFromTagContent` hic dugum üretmiyordu.
- Projede `.csproj` olmadığı için C# tarafı doğrudan build edilemiyordu.

**Uygulanan cozumler:**
- Calişan bir `HtmlParser` uygulandı.
- Tokenizer; yorumları ve `DOCTYPE` bildirimlerini atlayacak sekilde tamamlandı.
- `br`, `meta`, `img` gibi bos HTML etiketleri parser tarafinda desteklendi.
- `id` ve `class` ayrişma mantiği eklendi, `ElementTable` indekslemesi aktif hale getirildi.
- `DomParser.csproj` eklenerek proje `dotnet build` ile derlenebilir duruma getirildi.
- Metin duğumleri için `DomNode` sınıfına `TextContent` alanı eklendi.

### 3. `phase2-analysis-testing`

**Durum:** Analiz metotları calişıyor; entegrasyonu bloke eden yeni hata gorulmedi.

**Kontrol sonucu:**
- `TreeAnalyzer` metotlari mevcut veri modeliyle uyumlu.
- Parser tarafi çalışır hale geldiği için bu fazın metotlari artık gerçek DOM ağaci üzerinde kullanilabilir.

### 4. `phase2-traversal-search`

**Durum:** DFS, BFS ve arama metotlari entegrasyon içinde calışıyor.

**Kontrol sonucu:**
- `DomAlgorithms` sınıfı derleme mantigi açısından tutarlı.
- Büyük/küçük harf duyarsız tag araması korunuyor.
- Null kontrolleri mevcut.

### 5. `phase3-ui-dom-visualizer`

**Durum:** Arayüzün ayrıştırma çekirdeğinde iki kritik uyumluluk hatasi duzeltildi.

**Tespit edilen hatalar:**
- Standart `<!DOCTYPE html>` girdileri ayrışmıyordu.
- HTML bos etiketleri yalnızca XHTML tarzi `/>` ile calışıyordu.

**Uygulanan cozumler:**
- `dom-core.mjs` içinde `DOCTYPE` ve benzeri bildirimler token akışından çıkarıldı.
- HTML boş etiketleri için `VOID_ELEMENTS` listesi eklendi.
- Kapanis etiketi karsilastirmasi küçük-büyük harf duyarsız hale getirildi.
- Beklenmeyen kapaniş etiketlerinde daha kontrollü hata akışı sağlandı.

## Genel Sonuc

- JavaScript tabanlı DOM gorsellestirici standart HTML girdilerine daha uyumlu calışıyor.
- C# parser artık gercekten DOM ağacı üretiyor.
- C# kaynaklari proje dosyasi ile birlikte derlenebilir durumda.

