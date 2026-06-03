# Güncel Hata ve Bulgular Yazım Formatı

## main

```markdown
### Bulgu Başlığı

**Etkilenen Dosyalar:**

**Problem:**

**Uygulanan Çözüm:**

```

## phase1-data-structures

```markdown
### Bulgu Başlığı

**Etkilenen Dosyalar:**

**Problem:**


**Uygulanan Çözüm:**

```

## phase1-dom-parser

```markdown
### Bulgu Başlığı


**Etkilenen Dosyalar:**

**Problem:**


**Uygulanan Çözüm:**

```

## phase2-analysis-testing

```markdown
### Bulgu Başlığı

**Etkilenen Dosyalar:**

**Problem:**

**Uygulanan Çözüm:**

```

## phase2-traversal-search


### Sınıf Tanımlarının Tekrarlanması

**Etkilenen Dosyalar:DomAlgorithms.cs**

**Problem: Ana dallardaki veri yapıları (Queue<T>, Stack<T>, HashTable, DomNode vb.) DomAlgorithms.cs içerisinde tekrar tanımlanmıştı. Bu durum kodun ana dala (main) birleştirilmesi sırasında CS0101 (Aynı isim alanında çift tanımlama) derleme hatasına yol açıyordu.**

**Uygulanan Çözüm: Dosya içeriği temizlendi. Tekrarlanan tüm veri yapısı sınıfları DomAlgorithms.cs dosyasından çıkarılarak projede sadece tek bir tanım kalması sağlandı. İlgili veri yapıları referans alınarak (using) kullanılmaya başlandı.**

### DFS ve BFS Algoritmalarının Test Edilememesi

**Etkilenen Dosyalar:DomAlgorithms.cs**

**Problem:DepthFirstSearch ve BreadthFirstSearch metotları void dönüş tipine sahipti ve sonuçları doğrudan Console'a yazdırıyordu. Bu yapı, algoritmaların UI entegrasyonunu ve otomatik birim testlerinin (Unit Test) yazılmasını engelliyordu.**

**Uygulanan Çözüm:Metot imzaları değiştirilerek gezinilen düğümlerin List<DomNode> olarak döndürülmesi sağlandı. Çıktı yönetimi algoritma içinden çıkarıldı ve çağıran (caller) katmanın sorumluluğuna bırakıldı.**

### Düzenli İfade (RegEx) ile HTML Ayrıştırma Hatası
**Etkilenen Dosyalar:DomAlgorithms.cs**

**Problem:Ayrıştırma işlemi için kullanılan RegEx deseni, etiket özellikleri (attribute) içindeki > karakterlerinde bozuluyor, ayrıca HTML yorum satırlarını (``) ve <!DOCTYPE> bildirimlerini hatalı bir şekilde düğüm olarak ağaca eklemeye çalışıyordu.**

**Uygulanan Çözüm:Tokenize metodundaki RegEx kuralı |<![^>]*>|</?[^>]+>|[^<]+ olarak güncellendi. Yorum satırları ve C-Data/Doctype bildirimleri ayrıştırma aşamasında atlanarak temiz bir DOM düğüm işleyişi sağlandı.**

### Kendi Kendine Kapanan (Void) Elementlerin Ağacı Bozması

**Etkilenen Dosyalar:DomAlgorithms.cs**

**Problem:HTML5 standartlarında kapanış etiketi bulunmayan <br>, <img>, <meta> gibi elementler, normal açılış etiketi gibi Yığına (Stack<DomNode>) ekleniyordu. Kapanış etiketi gelmediği için bu durum DOM hiyerarşisinin tamamen bozulmasına sebep oluyordu.**

**Uygulanan Çözüm:TokenType enum yapısına SelfClosingTag eklendi. Tüm void elementler büyük/küçük harf duyarsız bir HashSet içinde tanımlandı. Token üretilirken bu düğümlerin stack'e push edilmeden doğrudan ebeveyn düğüme bağlanması sağlandı.**

### Geçersiz Etiketlerde Dizi İndeksi Hatası (IndexOutOfRange)

**Etkilenen Dosyalar:DomAlgorithms.cs**

**Problem:Düğüm oluşturma aşamasında etiket içerikleri parçalanırken (Split), içerik boş veya hatalı geldiğinde dizinin ilk elemanına (parts[0]) erişilmeye çalışılıyor ve uygulama tamamen çöküyordu.**

**Uygulanan Çözüm:Dizi parçalamasından sonra parts.Length == 0 güvenlik kontrolü eklendi. Eğer içerik geçersizse çökme (crash) yerine, hatanın kaynağını gösteren bir InvalidOperationException fırlatılması mantığı kuruldu.**

### Rekürsif Aramalarda Aşırı Bellek Tüketimi (Allocation)

**Etkilenen Dosyalar:DomAlgorithms.cs**

**Problem:SearchByTagName metodu, ağaçta derine indikçe kendi kendini çağırdığı (rekürsif) her adımda bellekte yeni bir List<DomNode> objesi yaratıyor ve listeleri AddRange ile birleştiriyordu. N düğümlü bir ağaçta bu durum performansı ciddi oranda düşürüyordu.**

**Uygulanan Çözüm: Metot tasarımı değiştirilerek dışarıdan verilen tek bir sonuç listesinin (referans olarak) kullanılması sağlandı (void SearchByTagName(DomNode, string, List<DomNode>)). Tüm rekürsif çağrılar aynı listeyi paylaşarak ekstra bellek tahsisi sorununu ortadan kaldırdı ve arama maliyeti O(N) seviyesine çekildi.**





## phase3-ui-dom-visualizer

### Modül Seviyesindeki Node ID Sayacının İzole Edilmemesi

**Etkilenen Dosyalar:**
- `dom-core.mjs`
- `phase3-ui.test.mjs`

**Problem:**
`nextNodeId` modül seviyesinde tutulduğu için `buildDomTree` çağrıları aynı sayaç durumunu paylaşıyordu. Bu durum eş zamanlı veya arka arkaya parse senaryolarında düğüm id üretimini dış duruma bağımlı hale getiriyor ve unit test yazımını zorlaştırıyordu.

**Uygulanan Çözüm:**
`nextNodeId`, `buildDomTree` içine lokal değişken olarak taşındı. `createElementNode` ve `createTextNode` fonksiyonları id üretimini closure üzerinden alan factory yapısına çevrildi. Test dosyasında birden fazla parse çağrısının tekrar `node-1` ile başladığı doğrulandı.

### Kapanış Etiketi Karşılaştırmasının Büyük/Küçük Harfe Duyarlı Olması

**Etkilenen Dosyalar:**
- `dom-core.mjs`
- `phase3-ui.test.mjs`

**Problem:**
`<DIV>...</div>` gibi HTML açısından geçerli girdilerde kapanış etiketi karşılaştırması bire bir string eşitliğiyle yapıldığı için hatalı şekilde `Etiket uyuşmazlığı` hatası üretilebiliyordu.

**Uygulanan Çözüm:**
Node oluşturulurken tag adları lower-case olarak normalize edildi. Kapanış etiketi kontrolü de lower-case karşılaştırmaya çekildi. Testte mixed-case tag ve void element kombinasyonu doğrulandı.

### ID Aramasında Kullanılan Stratejinin UI'da Görünmemesi

**Etkilenen Dosyalar:**
- `dom-core.mjs`
- `script.mjs`
- `phase3-ui.test.mjs`

**Problem:**
ID aramalarında `auto` stratejisi hash index ile O(1) çalışırken, kullanıcı BFS veya DFS seçtiğinde O(n) ağaç taraması yapılıyordu. Bu davranış eğitim amaçlı olsa da UI'da görünmediği için kullanıcı hangi maliyetle arama yaptığını anlayamıyordu.

**Uygulanan Çözüm:**
`resolveSearchPlan` fonksiyonu eklendi. Arama özetinde `Hash indeks O(1)`, `BFS O(n)` veya `DFS O(n)` bilgisi gösterilmeye başlandı. ID araması BFS/DFS ile çalıştırıldığında konsola uyarı verilerek hash index'in kullanılmadığı açık hale getirildi.

### Metrik Hesabında Her Düğüm İçin Tekrar Derinlik Hesaplanması

**Etkilenen Dosyalar:**
- `dom-core.mjs`
- `script.mjs`
- `phase3-ui.test.mjs`

**Problem:**
`updateMetrics`, tüm düğümleri dolaştıktan sonra her düğüm için tekrar `calculateDepth` çağırıyordu. Bu yaklaşım derin ağaçlarda toplam maliyeti O(N*H) seviyesine çıkarıyordu.

**Uygulanan Çözüm:**
`flattenNodesWithDepth` fonksiyonu eklendi ve DFS sırasında düğüm derinliği tek geçişte taşındı. `updateMetrics` bu listeyi kullanacak şekilde güncellendi. Render sırasında açık/kapalı ağaç durumu için de yeniden `calculateDepth` çağırmak yerine recursive depth parametresi kullanılmaya başlandı.

### DOCTYPE ve Void Element Ayrımının Tokenize Sırasında Eksik Kalması

**Etkilenen Dosyalar:**
- `dom-core.mjs`
- `phase3-ui.test.mjs`

**Problem:**
DOCTYPE bildirimi ve `br`, `img`, `meta` gibi kapanış etiketi gerektirmeyen HTML void elementleri parser davranışında özel ele alınmadığında ağaç yapısı bozulabiliyor veya gereksiz kapanış etiketi beklenebiliyordu.

**Uygulanan Çözüm:**
Tokenize aşamasında yorumlar ve `<!...>` bildirimleri atlanmaya devam edecek şekilde korundu. Tag adı `VOID_ELEMENTS` listesiyle karşılaştırılarak `/>` yazımı olmasa bile void elementler `selfClosingTag` olarak işlenir hale getirildi. Testte `<BR>` ve `<IMG>` girdilerinin kapanış etiketi olmadan parse edildiği doğrulandı.

### İlk Parse Hatasında UI Durumunun Belirsiz Kalması

**Etkilenen Dosyalar:**
- `script.mjs`
- `phase3-ui.test.mjs`

**Problem:**
Sayfa açılışında örnek HTML parse edilirken hata oluşursa kullanıcı yalnızca boş sonuç durumunu görebiliyor, hatanın nedeni yeterince açık görünmüyordu.

**Uygulanan Çözüm:**
Başlangıç yükleme akışı `initialize` fonksiyonuna alındı ve try/catch ile sarıldı. Başlangıç örneği yüklenemezse durum alanında somut hata mesajı gösterilecek hale getirildi. Örnek HTML'in parse edildiği statik test eklendi.

### Düğüm Seçiminde Tüm Ağacın Her Tıklamada Taranması

**Etkilenen Dosyalar:**
- `dom-core.mjs`
- `script.mjs`
- `phase3-ui.test.mjs`

**Problem:**
`selectNode`, tıklanan düğümü bulmak için `flattenNodes(state.root).find(...)` ile tüm ağacı dolaşıyordu. Büyük ağaçlarda her tıklama O(n) maliyetli hale geliyordu.

**Uygulanan Çözüm:**
`buildDomTree` sırasında `uidIndex` oluşturuldu ve her düğüm `uid -> node` eşlemesiyle index'e yazıldı. UI state içine `uidIndex` eklendi. `selectNode`, düğümü `state.uidIndex.get(nodeId)` ile O(1) erişimle bulacak şekilde güncellendi.

### Arama Bölümünde Erişilebilirlik Bildirimlerinin Eksik Olması

**Etkilenen Dosyalar:**
- `index.html`
- `script.mjs`

**Problem:**
Arama alanı semantik olarak `role=search` içinde değildi ve arama sonucundaki canlı durum değişiklikleri screen reader kullanıcılarına yeterince açık iletilmiyordu.

**Uygulanan Çözüm:**
Arama kontrolleri `role="search"` taşıyan form yapısına çevrildi. Arama butonu `submit` davranışına alındı ve JS tarafında form submit event'i yakalanarak arama çalıştırıldı. Durum çubuğuna `aria-live="polite"` ve `aria-atomic="true"` eklendi.

## phase1-data-structures - PR #8

### DomNode'da TextContent Alanının Eksik Olması

**Etkilenen Dosyalar:**
- `DomNode.cs`

**Problem:**
`<p>Merhaba</p>` gibi HTML yapılarında metin saklanamıyordu.

**Uygulanan Çözüm:**
`DomNode.cs` dosyasına `TextContent` alanı eklendi ve constructor içinde `string.Empty` ile başlatıldı.

### Queue .NET LinkedList Kullanıyordu

**Etkilenen Dosyalar:**
- `Queue.cs`
- `QueueNode.cs`

**Problem:**
Queue implementasyonu hazır `LinkedList<T>` kullanıyordu.

**Uygulanan Çözüm:**
Queue, kendi linked list düğümü olan `QueueNode<T>` ile sıfırdan yazıldı.

### Queue'da Yanlış Exception Türü

**Etkilenen Dosyalar:**
- `Queue.cs`

**Problem:**
Boş queue işlemlerinde genel `System.Exception` fırlatılıyordu.

**Uygulanan Çözüm:**
Boş queue işlemlerinde `InvalidOperationException` kullanılacak şekilde düzeltildi.

### AddChild'de Null Kontrolü Yoktu

**Etkilenen Dosyalar:**
- `DomNode.cs`

**Problem:**
`AddChild(null)` çağrısı kontrolsüzdü.

**Uygulanan Çözüm:**
`AddChild` içinde `ArgumentNullException` kontrolü eklendi.

## main - Son Merge ve Test Bulguları

### PR #8 ve PR #10 Entegrasyonunun Yerelde Doğrulanması

**Etkilenen Dosyalar:**
- `DomNode.cs`
- `Queue.cs`
- `QueueNode.cs`
- `TreeAnalyzer.cs`
- `guncel_hata_ve_bulgular.md`

**Problem:**
PR #8, `guncel_hata_ve_bulgular.md` dosyasında çakışma oluşturuyordu. PR #10 ise `Parser.cs` üzerinde güncel `main` dalındaki çalışan parser ile çakışıyordu. Doğrudan merge edilirse çalışan parser davranışının geriye gitme riski vardı.

**Uygulanan Çözüm:**
PR #8'deki veri yapısı değişiklikleri korundu ve rapor çakışması mevcut dosyaya ek bölüm olarak çözüldü. PR #10'da `Parser.cs` için güncel `main` sürümü korundu; `TreeAnalyzer.cs` iyileştirmeleri geriye uyumlu şekilde alındı. `CalculateDepth` metodu korunarak `GetHeight`, `GetDepth` ve `TryGetSiblings` eklendi.

### PR #9'un Bu Haliyle Merge Edilememesi

**Etkilenen Dosyalar:**
- `DomAlgorithms.cs`
- `Parser.cs`

**Problem:**
PR #9'daki `DomAlgorithms.cs`, `TokenType`, `Token` ve `HtmlParser` gibi parser sınıflarını tekrar tanımlıyordu. Bu değişiklik seçildiğinde proje `CS0101` ve `CS0111` hatalarıyla derlenmiyordu.

**Uygulanan Çözüm:**
PR #9 ana dala alınmadı. Bu PR'ın merge edilebilmesi için `DomAlgorithms.cs` içindeki tekrar parser/veri yapısı tanımları kaldırılmalı ve dosya yalnızca DFS, BFS ve arama algoritmalarını içerecek şekilde güncel `main` dalına göre yeniden düzenlenmelidir.

### Arayüz Modüllerinin Yanlış MIME Tipiyle Servis Edilmesi

**Etkilenen Dosyalar:**
- `README.md`
- `Dockerfile`
- `nginx.conf`
- `index.html`
- `script.mjs`
- `dom-core.mjs`

**Problem:**
Arayüz `.mjs` modülleri kullanıyor. Bazı basit statik sunucular `.mjs` dosyalarını `text/plain` olarak servis ettiği için tarayıcı modülleri çalıştırmıyor; bu durumda sayfa açılıyor ancak örnek HTML yüklenmiyor, DOM ağacı oluşmuyor ve butonlar işlevsiz gibi görünüyor.

**Uygulanan Çözüm:**
Docker runtime aşaması Nginx tabanlı hale getirildi. `nginx.conf` içinde `.mjs` dosyalarının `text/javascript` MIME tipiyle servis edilmesi sağlandı. README'deki çalıştırma akışı Docker Compose odaklı olarak güncellendi.

### Son Doğrulama Testleri

**Etkilenen Dosyalar:**
- `DomParser.csproj`
- `Parser.cs`
- `DomAlgorithms.cs`
- `TreeAnalyzer.cs`
- `dom-core.mjs`
- `script.mjs`
- `phase3-ui.test.mjs`

**Problem:**
Son merge sonrasında projenin Proje Konu 2 gereksinimlerine göre çalışıp çalışmadığı net olarak doğrulanmalıydı.

**Uygulanan Çözüm:**
`dotnet build`, C# parser/veri yapısı doğrulama senaryoları, `node phase3-ui.test.mjs`, 250 elemanlı büyük DOM JavaScript testi ve tarayıcı arayüz akışı çalıştırıldı. Geçerli HTML parse edildi, `#header` sorgusu hash index ile O(1) bulundu, `.container` sorgusu BFS ile iki eşleşme döndürdü, hatalı HTML için kullanıcıya hata mesajı gösterildi.

### Teslim Paketi İçin Kalan Dokümantasyon Eksikleri

**Etkilenen Dosyalar:**
- Proje teslim paketi

**Problem:**
Kod ve arayüz Proje Konu 2'nin ana çalışma amacını karşılıyor; ancak PDF'teki genel teslim standartlarında Docker konfigürasyonu, kapsamlı proje raporu, UML/Big-O analizleri ve demo videosu gibi ek teslim öğeleri de isteniyor.

**Uygulanan Çözüm:**
Bu maddeler kod çalışmasını engelleyen hata değildir. Teslimden önce ayrıca hazırlanması gereken paket işleri olarak not edildi.

## main - Docker, Arayüz ve Dokümantasyon Güncellemeleri

### Docker ile Tek Komutla Çalıştırma Eksikliği

**Etkilenen Dosyalar:**
- `Dockerfile`
- `docker-compose.yml`
- `.dockerignore`
- `README.md`
- `nginx.conf`

**Problem:**
Proje yerelde çalışsa da teslim kriterlerinde istenen tek komutla ayağa kaldırma akışı eksikti. Ayrıca `.mjs` modüllerinin doğru MIME tipiyle sunulması gerektiği için sıradan dosya açma yöntemi arayüzü güvenilir şekilde çalıştırmıyordu.

**Uygulanan Çözüm:**
Çok aşamalı Dockerfile eklendi. Build aşamasında .NET 9 SDK ile proje derleniyor, runtime aşamasında Nginx arayüzü servis ediyor. `docker-compose.yml` ile uygulama `4174` portundan tek komutla çalıştırılabilir hale getirildi.

### Farklı Düğüm Sayılarıyla Test Etme Akışının Belirsiz Olması

**Etkilenen Dosyalar:**
- `index.html`
- `script.mjs`
- `dom-core.mjs`
- `styles.css`
- `phase3-ui.test.mjs`

**Problem:**
Kullanıcı farklı düğüm sayılarıyla deneme yapmak istediğinde elle HTML üretmesi gerekiyordu. Ayrıca toplam düğüm sayısının element düğümleri ile metin düğümlerinin toplamı olduğu arayüzde anlaşılması zor olabiliyordu.

**Uygulanan Çözüm:**
Arayüze `Düğüm sayısı` alanı ve `HTML Üret` butonu eklendi. `generateSyntheticHtml` fonksiyonu ile 1-1000 arası sentetik HTML üretimi sağlandı. Test dosyasına 1, 10, 100 ve 500 düğümlü senaryolar eklendi. README'ye toplam düğüm sayısının element ve metin düğümlerini birlikte kapsadığı notu eklendi.

### Düğüm Ayrıntısı Panelinin Boş Kalması

**Etkilenen Dosyalar:**
- `script.mjs`

**Problem:**
DOM ağacı oluşturulduktan sonra kullanıcı bir düğüm seçmediği sürece `Düğüm Ayrıntısı` paneli boş kalıyordu. Arama sonuçlarında da eşleşen düğüm vurgulansa bile ayrıntı paneli her zaman otomatik güncellenmiyordu.

**Uygulanan Çözüm:**
DOM oluşturulduğunda ilk görünür düğüm otomatik seçilecek şekilde `firstVisibleNode` akışı eklendi. Arama sonucu varsa ilk eşleşme otomatik seçiliyor ve `updateNodeDetails` ile ayrıntı paneli dolduruluyor. Tıklama hedefindeki kararsızlığı azaltmak için aynı `data-node-id` bilgisinin hem `details` hem `summary` üzerinde tekrar edilmesi kaldırıldı.

### Arayüzde Türkçe Karakterlerin Eksik Kullanılması

**Etkilenen Dosyalar:**
- `index.html`
- `script.mjs`
- `dom-core.mjs`
- `nginx.conf`
- `README.md`

**Problem:**
Arayüzde ve durum mesajlarında `Agaci`, `Gorsellestirici`, `Dugum`, `Hazir`, `Sonuc`, `uyusmazligi` gibi Türkçe karakterleri eksik metinler bulunuyordu. Bu durum kullanıcı deneyimini zayıflatıyor ve profesyonel görünümü bozuyordu.

**Uygulanan Çözüm:**
Kullanıcıya görünen başlık, buton, panel, metrik, durum, hata ve uyarı metinleri Türkçe karakterlerle güncellendi. README profesyonel bir proje dokümanı olarak yeniden düzenlendi.

### Son Test ve Doğrulama Durumu

**Etkilenen Dosyalar:**
- `DomParser.csproj`
- `phase3-ui.test.mjs`
- `Dockerfile`
- `docker-compose.yml`
- `nginx.conf`

**Problem:**
Docker, arayüz metinleri, sentetik düğüm üretimi ve düğüm ayrıntısı düzeltmeleri sonrasında tüm projenin yeniden doğrulanması gerekiyordu.

**Uygulanan Çözüm:**
`dotnet build`, `node phase3-ui.test.mjs` ve `docker compose up --build -d` çalıştırıldı. Docker build sırasında .NET Release derlemesi başarılı oldu. Container üzerinden `index.html`, `script.mjs` ve `dom-core.mjs` dosyalarının `200` döndüğü ve `.mjs` dosyalarının `text/javascript` MIME tipiyle servis edildiği doğrulandı.

### Python Yerel Sunucu Dosyasının Gereksiz Hale Gelmesi

**Etkilenen Dosyalar:**
- `local_server.py`
- `Dockerfile`
- `docker-compose.yml`
- `nginx.conf`
- `README.md`

**Problem:**
Docker Compose tek komutla çalıştırma akışı eklendikten sonra `python local_server.py` komutu ve bu dosyaya bağlı runtime yapısı gereksiz tekrar oluşturuyordu. Kullanıcı açısından iki ayrı çalıştırma yöntemi kafa karıştırıcıydı.

**Uygulanan Çözüm:**
`local_server.py` kaldırıldı. Docker runtime aşaması Nginx'e taşındı. README, Docker Compose'u tek resmi çalıştırma yolu olarak gösterecek şekilde sadeleştirildi. Compose içindeki Python'a özel `HOST` ve `PORT` ortam değişkenleri kaldırıldı.

## main - Dayanıklılık Testleri Sonrası Düzeltmeler

### Metin ve Attribute İçindeki `<` / `>` Karakterlerinin Parser'ı Bozması

**Etkilenen Dosyalar:**
- `dom-core.mjs`
- `Parser.cs`

**Problem:**
Regex tabanlı token üretimi, metin içindeki `<` karakterini etiket başlangıcı sanabiliyor ve attribute değeri içinde geçen `>` karakterinde etiketi erken kapatıyordu. Bu nedenle `<p>5 < 10 ve 20 > 3</p>` hatalı şekilde reddediliyor, `<div title="5 > 3">...</div>` ise sessizce yanlış DOM ağacı üretiyordu.

**Uygulanan Çözüm:**
Regex tabanlı etiket okuma yerine quote durumunu takip eden durum-bazlı tokenizer eklendi. Artık `>` karakteri yalnızca attribute quote dışında olduğunda etiketi kapatıyor; geçerli etiket başlangıcı olmayan `<` karakterleri metin olarak korunuyor.

### Script, Style, Textarea ve Title İçeriklerinin Yanlış Ayrıştırılması

**Etkilenen Dosyalar:**
- `dom-core.mjs`
- `Parser.cs`

**Problem:**
`script`, `style`, `textarea` ve `title` içindeki `<` / `>` karakterleri normal HTML etiketi gibi ayrıştırılmaya çalışılıyordu. Bu durum script/style içeriklerinde hataya, textarea/title içeriklerinde ise semantik olarak yanlış ağaç üretimine yol açıyordu.

**Uygulanan Çözüm:**
Bu elementler raw text kapsamına alındı. Açılış etiketi görüldükten sonra ilgili kapanış etiketine kadar olan içerik tek metin düğümü olarak işleniyor.

### Aşırı Derin DOM Ağaçlarında Çağrı Yığını Taşması

**Etkilenen Dosyalar:**
- `dom-core.mjs`
- `script.mjs`

**Problem:**
DFS arama, subtree analizi, düğüm düzleştirme ve arayüz render işlemlerinde recursive yaklaşım kullanılıyordu. 10.000 seviye iç içe HTML gibi uç örneklerde JavaScript çağrı yığını sınırı aşılabiliyordu.

**Uygulanan Çözüm:**
`depthFirstSearch`, `analyzeSubtree` ve `flattenNodesWithDepth` iteratif stack yapısına taşındı. Arayüzde seçili düğüm yolu parent zinciriyle çıkarılıyor ve ağaç render işlemi recursive çağrı yerine manuel stack ile yapılıyor.

### Favicon İsteğinin Yanlışlıkla HTML Döndürmesi

**Etkilenen Dosyalar:**
- `nginx.conf`

**Problem:**
`/favicon.ico` isteği SPA fallback nedeniyle `index.html` döndürebiliyordu. Bu kritik çalışma hatası değildi ancak dağıtım çıktısını temiz olmayan hale getiriyordu.

**Uygulanan Çözüm:**
Nginx konfigürasyonuna `/favicon.ico` için `204` dönen özel location eklendi.

### Dayanıklılık Retest Sonucu

**Etkilenen Dosyalar:**
- `dom-core.mjs`
- `script.mjs`
- `Parser.cs`
- `nginx.conf`

**Problem:**
Senior test geçişinde bulunan zayıf noktaların gerçekten kapandığının doğrulanması gerekiyordu.

**Uygulanan Çözüm:**
`dotnet build`, `node phase3-ui.test.mjs`, özel stres testi ve `docker compose up --build -d` yeniden çalıştırıldı. Metin içindeki `<`, attribute içindeki `>`, `script/style/textarea/title` raw text içerikleri, 10.000 derinlikli ağaç, 10.000 düğüm DFS araması, 1000 düğümlü sentetik üretim ve geçersiz HTML reddi başarıyla doğrulandı. Canlı Chrome testinde console hatası olmadan ilgili arayüz senaryoları geçti.
