# Veri Yapıları ile HTML'den DOM Ağacı Oluşturma

Bu proje, basitleştirilmiş HTML girdisini ayrıştırarak DOM ağacı oluşturmak, bu ağaç üzerinde veri yapıları ve algoritmalarla işlem yapmak ve sonucu bir arayüzde görselleştirmek için hazırlandı.

Repo iki ana parçadan oluşur:

- C# tarafında DOM düğümü, parser, hash table, stack, queue ve analiz algoritmaları
- JavaScript tarafında HTML girdisini ayrıştıran ve DOM ağacını görselleştiren arayüz

## Proje Kapsamı

Projede şu yetenekler bulunur:

- HTML etiketlerinden hiyerarşik DOM ağacı oluşturma
- `id`, `class` ve `tag` bazlı arama
- DFS ve BFS ile düğüm dolaşımı
- Ağaç derinliği, kardeş düğümler ve alt ağaç boyutu analizi
- Tarayıcı üzerinde DOM ağacı görselleştirme

## Dosya Yapısı

### C# çekirdeği

- `DomNode.cs`: DOM düğüm modeli
- `Parser.cs`: HTML parser
- `HashTable.cs`: `id` indeksleme yapısı
- `Stack.cs`: parser için stack
- `Queue.cs`: BFS için queue
- `DomAlgorithms.cs`: DFS, BFS ve arama algoritmaları
- `TreeAnalyzer.cs`: derinlik, kardeş ve alt ağaç analizleri
- `DomParser.csproj`: .NET proje dosyası

### Arayüz

- `index.html`: uygulama iskeleti
- `styles.css`: arayüz stilleri
- `dom-core.mjs`: tarayıcı tarafındaki parser ve ağaç mantığı
- `script.mjs`: arayüz etkileşimi ve render akışı

## Çalıştırma

### Arayüz

1. Repo klasörünü aç.
2. `index.html` dosyasını tarayıcıda aç.
3. Örnek HTML yükleyebilir veya kendi HTML metnini girip `DOM Oluştur` butonuna basabilirsin.

### C# kütüphanesi

1. Proje klasöründe `dotnet build` çalıştır.
2. `HtmlParser` ile HTML metnini DOM ağacına çevir.
3. `DomAlgorithms` ve `TreeAnalyzer` ile ağaç üzerinde arama ve analiz yap.

## Güncel Durum

Son güncellemelerle birlikte:

- C# parser artık çalışır durumda.
- `DOCTYPE` bildirimleri destekleniyor.
- `br`, `meta`, `img` gibi boş HTML etiketleri parser tarafında düzgün ele alınıyor.
- Proje `dotnet build` ile derlenebiliyor.

## Branch Yapısı

Uzak repodaki mevcut branch'ler:

- `main`: tüm fazların entegre edildiği ana dal
- `phase1-data-structures`: temel veri yapıları
- `phase1-dom-parser`: C# parser geliştirmeleri
- `phase2-analysis-testing`: ağaç analizi ve testleme adımları
- `phase2-traversal-search`: DFS, BFS ve arama algoritmaları
- `phase3-ui-dom-visualizer`: HTML/JS arayüzü ve görselleştirme
