# Veri Yapıları ile HTML'den DOM Ağacı Oluşturma

Bu proje, ham HTML metnini basitleştirilmiş bir parser ile ayrıştırarak bellekte DOM benzeri hiyerarşik bir ağaç yapısı oluşturur. Amaç tam kapsamlı bir tarayıcı motoru geliştirmek değil; DOM oluşturma sürecini veri yapıları, arama algoritmaları ve görselleştirme üzerinden anlaşılır şekilde modellemektir.

## Temel Özellikler

- HTML etiketlerinden N-ary tree tabanlı DOM ağacı oluşturma
- Ebeveyn-çocuk ilişkilerini ve metin düğümlerini saklama
- `id`, `class` ve tag adına göre arama
- `id` aramalarında hash index ile ortalama `O(1)` erişim
- DFS ve BFS ile ağaç dolaşımı
- Ağaç derinliği, kardeş düğümler ve alt ağaç boyutu analizi
- Açılır-kapanır DOM ağacı görselleştirmesi
- Farklı düğüm sayıları için sentetik HTML üretimi
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

### Arayüz

| Dosya | Sorumluluk |
| --- | --- |
| `index.html` | Uygulama iskeleti ve erişilebilir form yapısı |
| `styles.css` | Görsel düzen, panel yapısı ve DOM ağacı stilleri |
| `dom-core.mjs` | Tarayıcı tarafı parser, veri yapıları ve arama mantığı |
| `script.mjs` | Arayüz etkileşimleri, render akışı ve düğüm ayrıntıları |
| `phase3-ui.test.mjs` | Arayüz çekirdeği için JavaScript doğrulama testleri |

## Gereksinimler

Docker ile çalıştırmak için:

- Docker Desktop
- Docker Compose

Yerel geliştirme için:

- .NET SDK 9
- Node.js
- Python 3.11 veya üzeri

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

## Yerel Olarak Çalıştırma

Arayüz `.mjs` modülleri kullandığı için doğrudan `index.html` dosyasını açmak yerine yerel sunucu kullanılmalıdır.

```powershell
python local_server.py
```

Sonra tarayıcıda şu adresi aç:

```text
http://127.0.0.1:4174/index.html
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

Yerel sunucu söz dizimi kontrolü:

```powershell
python -m py_compile local_server.py
```

Docker doğrulaması:

```powershell
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

## Branch Yapısı

- `main`: Entegre ve test edilmiş ana dal
- `phase1-data-structures`: Temel veri yapıları
- `phase1-dom-parser`: C# parser geliştirmeleri
- `phase2-analysis-testing`: Ağaç analizi ve testleme
- `phase2-traversal-search`: DFS, BFS ve arama algoritmaları
- `phase3-ui-dom-visualizer`: HTML/JS arayüzü ve görselleştirme
