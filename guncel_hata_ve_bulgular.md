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

```markdown
### Sınıf Tanımlarının Tekrarlanması
**Etkilenen Dosyalar:**
- DomAlgorithms.cs

**Problem:**
Ana dallardaki (branch) veri yapıları (Queue<T>, Stack<T>, HashTable, DomNode vb.) DomAlgorithms.cs içerisinde tekrar tanımlanmıştı. Bu durum kodun ana dala (main) birleştirilmesi sırasında CS0101 (Aynı isim alanında çift tanımlama) derleme hatasına yol açıyordu.

**Uygulanan Çözüm:**

```Dosya içeriği temizlendi. Tekrarlanan tüm veri yapısı sınıfları DomAlgorithms.cs dosyasından çıkarılarak projede sadece tek bir tanım kalması sağlandı. İlgili veri yapıları referans alınarak (using) kullanılmaya başlandı.

### DFS ve BFS Algoritmalarının Test Edilememesi
**Etkilenen Dosyalar:**
- DomAlgorithms.cs

**Problem:**
DepthFirstSearch ve BreadthFirstSearch metotları void dönüş tipine sahipti ve sonuçları doğrudan Console'a yazdırıyordu. Bu yapı, algoritmaların UI entegrasyonunu ve otomatik birim testlerinin (Unit Test) yazılmasını engelliyordu.

**Uygulanan Çözüm:**

```Metot imzaları değiştirilerek gezinilen düğümlerin List<DomNode> olarak döndürülmesi sağlandı. Çıktı yönetimi algoritma içinden çıkarıldı ve çağıran (caller) katmanın sorumluluğuna bırakıldı.

### Düzenli İfade ile HTML Ayrıştırma Hatası
**Etkilenen Dosyalar:**
- DomAlgorithms.cs

**Problem:**
Ayrıştırma işlemi için kullanılan RegEx deseni, etiket özellikleri içindeki > karakterlerinde bozuluyor, ayrıca HTML yorum satırlarını (``) ve <!DOCTYPE> bildirimlerini hatalı bir şekilde düğüm olarak ağaca eklemeye çalışıyordu.

**Uygulanan Çözüm:**

```Tokenize metodundaki RegEx kuralı |<![^>]*>|</?[^>]+>|[^<]+ olarak güncellendi. Yorum satırları ve C-Data/Doctype bildirimleri ayrıştırma aşamasında atlanarak temiz bir DOM düğüm işleyişi sağlandı.

### Kendi Kendine Kapanan Elementlerin Ağacı Bozması
**Etkilenen Dosyalar:**
- DomAlgorithms.cs

**Problem:**
HTML5 standartlarında kapanış etiketi bulunmayan <br>, <img>, <meta> gibi elementler, normal açılış etiketi gibi Yığın'a ekleniyordu. Kapanış etiketi gelmediği için bu durum DOM hiyerarşisinin tamamen bozulmasına sebep oluyordu.

**Uygulanan Çözüm:**

TokenType enum yapısına SelfClosingTag eklendi. Tüm void elementler büyük/küçük harf duyarsız bir HashSet ($O(1)$ erişim) içinde tanımlandı. Token üretilirken bu düğümlerin stack'e push edilmeden doğrudan ebeveyn düğüme bağlanması sağlandı.

### Geçersiz Etiketlerde Dizi İndeksi Hatası 
**Etkilenen Dosyalar:**
- DomAlgorithms.cs

**Problem:**
Düğüm oluşturma aşamasında etiket içerikleri parçalanırken, içerik boş veya hatalı geldiğinde dizinin ilk elemanına (parts[0]) erişilmeye çalışılıyor ve uygulama tamamen çöküyordu.

**Uygulanan Çözüm:**

Dizi parçalamasından sonra parts.Length == 0 güvenlik kontrolü eklendi. Eğer içerik geçersizse çökme  yerine, hatanın kaynağını gösteren bir InvalidOperationException fırlatılması mantığı kuruldu.

### Rekürsif Aramalarda Aşırı Bellek Tüketimi
**Etkilenen Dosyalar:**
- DomAlgorithms.cs

**Problem:**
SearchByTagName metodu, ağaçta derine indikçe kendi kendini çağırdığı  her adımda bellekte yeni bir List<DomNode> objesi yaratıyor ve listeleri AddRange ile birleştiriyordu. N düğümlü bir ağaçta bu durum performansı ciddi oranda düşürüyordu.

**Uygulanan Çözüm:**

Metot tasarımı değiştirilerek dışarıdan verilen tek bir sonuç listesinin kullanılması sağlandı. Tüm rekürsif çağrılar aynı listeyi paylaşarak ekstra bellek tahsisi sorununu ortadan kaldırdı ve arama maliyeti saf O(N) seviyesine çekildi.

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
`<DIV>...</div>` gibi HTML açısından geçerli girdilerde kapanış etiketi karşılaştırması bire bir string eşitliğiyle yapıldığı için hatalı şekilde `Etiket uyusmazligi` hatası üretilebiliyordu.

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
