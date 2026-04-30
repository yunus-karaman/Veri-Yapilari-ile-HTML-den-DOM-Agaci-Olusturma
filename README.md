# Veri Yapıları ile HTML'den DOM Ağacı Oluşturma

Bu depo, basitlestirilmis bir HTML metnini ayrıştırıp hiyerarsik bir DOM agacina donusturen proje icin
faz bazli calisma yapisini ve Faz 3 gorsellestirme arayuzunu icerir.

## Faz 3 Arayuzu

Arayuz dosyalari:

- `index.html`
- `styles.css`
- `dom-core.mjs`
- `script.mjs`

Saglanan ozellikler:

- Sol panelde kod editoru benzeri HTML girdi alani
- Sag panelde acilir-kapanir DOM agaci
- `id="header"`, `class="item"`, `#header`, `.item` ve `div` benzeri sorgularla arama
- Eslesen dugumleri agac uzerinde vurgulama
- Dugum ayrintisi, alt agac boyutu ve derinlik gostergeleri

## Calistirma

### Faz 3 Arayuzu

1. Depoyu ac.
2. `index.html` dosyasini bir tarayicida calistir.
3. Ornek HTML yukleyebilir veya kendi HTML metnini girip `DOM Olustur` butonuna basabilirsin.

### C# Kutuphane

1. Proje klasorunde `dotnet build` calistir.
2. `HtmlParser` sinifi ile HTML metnini DOM agacina cevir.
3. `HashTable`, `DomAlgorithms` ve `TreeAnalyzer` siniflari ile agac uzerinde arama ve analiz yap.

## Branch Akisi

- `main`: Tum fazlarin entegre edildigi ana dal
- `phase1-data-structures`: `DomNode`, `Queue`, `Stack`, `HashTable`
- `phase1-dom-parser`: C# tabanli HTML parser
- `phase2-analysis-testing`: `TreeAnalyzer` ve analiz adimlari
- `phase2-traversal-search`: DFS, BFS ve arama algoritmalari
- `phase3-ui-dom-visualizer`: HTML/JS arayuzu ve DOM gorsellestirme
