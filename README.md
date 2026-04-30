# Veri Yapilari ile HTML'den DOM Agaci Olusturma

Bu proje, basitlestirilmis HTML girdisini ayrıştırarak DOM agaci olusturmak, bu agac uzerinde veri yapilari ve algoritmalarla islem yapmak ve sonucu bir arayuzde gorsellestirmek icin hazirlandi.

Repo iki ana parcadan olusur:

- C# tarafinda DOM dugumu, parser, hash table, stack, queue ve analiz algoritmalari
- JavaScript tarafinda HTML girdisini ayrıştıran ve DOM agacini gorsellestiren arayuz

## Proje Kapsami

Projede su yetenekler bulunur:

- HTML etiketlerinden hiyerarsik DOM agaci olusturma
- `id`, `class` ve `tag` bazli arama
- DFS ve BFS ile dugum dolasimi
- Agac derinligi, kardes dugumler ve alt agac boyutu analizi
- Tarayici uzerinde DOM agaci gorsellestirme

## Dosya Yapisi

### C# cekirdegi

- `DomNode.cs`: DOM dugum modeli
- `Parser.cs`: HTML parser
- `HashTable.cs`: `id` indeksleme yapisi
- `Stack.cs`: parser icin stack
- `Queue.cs`: BFS icin queue
- `DomAlgorithms.cs`: DFS, BFS ve arama algoritmalari
- `TreeAnalyzer.cs`: derinlik, kardes ve alt agac analizleri
- `DomParser.csproj`: .NET proje dosyasi

### Arayuz

- `index.html`: uygulama iskeleti
- `styles.css`: arayuz stilleri
- `dom-core.mjs`: tarayici tarafindaki parser ve agac mantigi
- `script.mjs`: arayuz etkilesimi ve render akisi

## Calistirma

### Arayuz

1. Repo klasorunu ac.
2. `index.html` dosyasini tarayicida ac.
3. Ornek HTML yukleyebilir veya kendi HTML metnini girip `DOM Olustur` butonuna basabilirsin.

### C# kutuphanesi

1. Proje klasorunde `dotnet build` calistir.
2. `HtmlParser` ile HTML metnini DOM agacina cevir.
3. `DomAlgorithms` ve `TreeAnalyzer` ile agac uzerinde arama ve analiz yap.

## Guncel Durum

Son guncellemelerle birlikte:

- C# parser artik calisir durumda
- `DOCTYPE` bildirimleri destekleniyor
- `br`, `meta`, `img` gibi bos HTML etiketleri parser tarafinda duzgun ele aliniyor
- Proje `dotnet build` ile derlenebiliyor

## Branch Yapisi

Uzak repodaki mevcut branch'lar:

- `main`: tum fazlarin entegre edildigi ana dal
- `phase1-data-structures`: temel veri yapilari
- `phase1-dom-parser`: C# parser gelistirmeleri
- `phase2-analysis-testing`: agac analizi ve testleme adimlari
- `phase2-traversal-search`: DFS, BFS ve arama algoritmalari
- `phase3-ui-dom-visualizer`: HTML/JS arayuzu ve gorsellestirme

Detayli hata ve bulgular icin `hata_ve_bulgular_raporu.md` dosyasina bakabilirsin.
