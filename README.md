# Veri Yapıları ile HTML'den DOM Ağacı Oluşturma

Bu proje, ham HTML metnini basitleştirilmiş bir parser ile ayrıştırarak bellekte DOM benzeri hiyerarşik bir ağaç yapısı oluşturur. Amaç tam kapsamlı bir tarayıcı motoru geliştirmek değil; DOM oluşturma sürecini veri yapıları, arama algoritmaları ve görselleştirme üzerinden anlaşılır şekilde modellemektir.

Parser kontrollü bir HTML alt kümesini hedefler. Tarayıcıların yaptığı CSS hesaplama, JavaScript çalıştırma, layout/render motoru davranışı veya hatalı HTML'i otomatik onarma bu projenin kapsamı değildir.

## Temel Özellikler

- HTML etiketlerinden N-ary tree tabanlı DOM ağacı oluşturma
- Ebeveyn-çocuk ilişkilerini ve metin düğümlerini saklama
- `id`, `class` ve tag adına göre arama
- `id` aramalarında hash index ile ortalama `O(1)` erişim
- DFS ve BFS ile ağaç dolaşımı
- Ağaç derinliği, kardeş düğümler ve alt ağaç boyutu analizi
- Açılır-kapanır DOM ağacı görselleştirmesi
- Farklı düğüm sayıları için sentetik HTML üretimi
- Çok satırlı, tab veya newline içeren attribute yazımlarını ayrıştırma
- `br`, `img`, `input`, `meta` gibi void elementleri kapanış etiketi beklemeden işleme
- Docker Compose ile tek komutla çalıştırma

## Mimari

### C# Çekirdeği

| Dosya | Sorumluluk |
| --- | --- |
| `DomNode.cs` | DOM düğüm modeli, metin içeriği, ebeveyn ve çocuk ilişkileri |
| `Parser.cs` | HTML tokenizer ve stack tabanlı DOM ağacı üretimi |
| `Stack.cs` | Parser için sıfırdan yazılmış yığıt |
| `Queue.cs`, `QueueNode.cs` | BFS için sıfırdan yazılmış kuyruk |
| `HashTable.cs` | `id` tabanlı hızlı erişim için hash table |
| `DomAlgorithms.cs` | DFS, BFS, tag/class/id arama algoritmaları |
| `TreeAnalyzer.cs` | Derinlik, kardeş düğüm ve alt ağaç analizleri |

C# tarafında `Stack`, `Queue` ve `HashTable` sınıfları sıfırdan yazılmıştır. Parser açılış/kapanış etiketlerini işlerken custom `Stack` kullanır. BFS custom `Queue`, DFS custom `Stack` kullanır. `HashTable` polynomial rolling hash, separate chaining ve `0.75` yük faktörü sonrası rehash ile çalışır.

### Arayüz

| Dosya | Sorumluluk |
| --- | --- |
| `index.html` | Uygulama iskeleti ve erişilebilir form yapısı |
| `styles.css` | Görsel düzen, panel yapısı ve DOM ağacı stilleri |
| `dom-core.mjs` | Tarayıcı tarafı parser, veri yapıları ve arama mantığı |
| `script.mjs` | Arayüz etkileşimleri, render akışı ve düğüm ayrıntıları |
| `phase3-ui.test.mjs` | Arayüz çekirdeği için JavaScript doğrulama testleri |

JavaScript tarafında da `Stack`, `Queue` ve `HashTable` sıfırdan uygulanmıştır. UI parser custom `Stack` ile DOM ağacını kurar, BFS custom `Queue` kullanır, DFS ve alt ağaç dolaşımları custom `Stack` kullanır. `idIndex` ve `uidIndex` hash tabloları separate chaining ve rehash destekler.

## Gereksinimler

Docker ile çalıştırmak için:

- Docker Desktop
- Docker Compose

Yerel geliştirme için:

- .NET SDK 9
- Node.js

Projede `npm install` veya `pip install` gerektiren ek bağımlılık yoktur.

## Docker ile Çalıştırma

Docker Desktop'ı başlattıktan sonra repo klasöründe şu komutu çalıştır:

```powershell
docker compose up --build
```

Arka planda çalıştırmak için:

```powershell
docker compose up --build -d
```

Uygulama adresi:

```text
http://127.0.0.1:4174/index.html
```

Kapatmak için:

```powershell
docker compose down
```

## Test ve Doğrulama

C# derlemesi:

```powershell
dotnet build
```

JavaScript arayüz çekirdeği testleri:

```powershell
node phase3-ui.test.mjs
```

Bu testler parser whitespace davranışını, void elementleri, hatalı kapanışları, HashTable rehash sonrasını, custom Stack/Queue dolaşım sıralarını, `O(1)` id aramasını ve sentetik HTML düğüm sayısını doğrular.

Docker doğrulaması:

```powershell
docker compose config
docker compose build
docker compose up --build -d
```

## Kullanım

1. Sol paneldeki HTML girdisini düzenle veya `Örnek HTML` butonunu kullan.
2. İstersen `Düğüm sayısı` alanına değer girip `HTML Üret` ile sentetik veri oluştur.
3. `DOM Oluştur` ile ağacı üret.
4. Sağ panelde DOM ağacını incele.
5. Arama alanında `#header`, `.container`, `id="brand"`, `class="item"` veya `section` gibi sorgular kullan.
6. Seçilen veya bulunan düğümün ayrıntıları sağdaki `Düğüm Ayrıntısı` panelinde görünür.

## Notlar ve Sınırlamalar

- Parser kontrollü ve basitleştirilmiş HTML alt kümesi için tasarlanmıştır.
- Yorum satırları ve `DOCTYPE` bildirimleri atlanır.
- `br`, `img`, `meta` gibi void elementler kapanış etiketi olmadan işlenir.
- Toplam düğüm sayısı element düğümleri ile metin düğümlerinin toplamıdır.
- Çok derin ve olağan dışı iç içe yapılarda tarayıcı tarafındaki rekürsif işlemler sınıra yaklaşabilir; küçük ve orta ölçekli veri setleri hedeflenmiştir.
- CSS selector motoru yoktur; arama `id`, `class` ve tag adı ile sınırlıdır.
- CSS hesaplama, layout, gerçek tarayıcı uyumluluğu ve JavaScript çalıştırma desteklenmez.
- Hatalı HTML'i tarayıcı gibi otomatik tamir etmek hedeflenmez; uyumsuz veya açık kalan etiketler kontrollü hata üretir.

## Teslim Notları

- TODO: Demo videosu linki eklenecek.
- TODO: UML/rapor dosyası teslim kapsamına ayrıca eklenecekse bağlantısı burada belirtilecek.
- AI kullanım notları / prompt dökümü: TODO, gerçek kullanılan prompt ve yardım adımları teslim formatına göre eklenecek. Sahte prompt üretilmemiştir.

## Branch Yapısı

- `main`: Entegre ve test edilmiş ana dal
- `phase1-data-structures`: Temel veri yapıları
- `phase1-dom-parser`: C# parser geliştirmeleri
- `phase2-analysis-testing`: Ağaç analizi ve testleme
- `phase2-traversal-search`: DFS, BFS ve arama algoritmaları
- `phase3-ui-dom-visualizer`: HTML/JS arayüzü ve görselleştirme
