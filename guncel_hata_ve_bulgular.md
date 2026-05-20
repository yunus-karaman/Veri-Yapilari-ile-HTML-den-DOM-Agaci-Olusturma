# Güncel Hata ve Bulgular

## İnceleme Tarihi

20 Mayıs 2026

## Genel Durum

Proje mevcut durumda `dotnet build` ile derlenebilir durumdadır. C# tarafında temel DOM parser, veri yapıları ve algoritmalar çalışır hale getirilmiştir. JavaScript tarafında da HTML girdisinden DOM ağacı oluşturma ve arayüzde görselleştirme akışı bulunmaktadır.

Bu dosya, mevcut rapordan bağımsız olarak güncel incelemede dikkat edilmesi gereken yeni sorunları, riskleri ve çözüm önerilerini takip etmek için oluşturulmuştur.

## 1. Parser ve HTML Ayrıştırma

### Problem

C# tarafındaki `Parser.cs` ve tarayıcı tarafındaki `dom-core.mjs` benzer HTML ayrıştırma mantığını ayrı ayrı uygulamaktadır. Bu durum iki tarafın zamanla farklı davranmasına yol açabilir.

### Etkilenen Dosyalar

- `Parser.cs`
- `dom-core.mjs`

### Risk

Aynı HTML girdisi C# tarafında farklı, JavaScript tarafında farklı DOM ağacı üretebilir. Bu da proje sunumunda veya test sürecinde tutarsız sonuçlara neden olabilir.

### Önerilen Çözüm

Ortak bir test senaryosu listesi hazırlanmalıdır. Aynı HTML örnekleri hem C# parser hem de JavaScript parser üzerinde denenmeli ve beklenen ağaç yapısı karşılaştırılmalıdır.

### Test Senaryoları

- Normal iç içe HTML etiketi
- `DOCTYPE` içeren HTML
- Yorum satırı içeren HTML
- `br`, `img`, `meta`, `input` gibi kapanış etiketi olmayan HTML etiketleri
- Hatalı kapanış etiketi içeren HTML

## 2. Veri Yapıları

### Problem

`HashTable`, `Stack` ve `Queue` yapıları temel olarak çalışmaktadır. Ancak bazı kenar durumlar için davranışlar daha net hale getirilmelidir.

### Etkilenen Dosyalar

- `HashTable.cs`
- `Stack.cs`
- `Queue.cs`
- `dom-core.mjs`

### Risk

Boş veya geçersiz anahtarlarla işlem yapılırsa beklenmeyen hata oluşabilir. Ayrıca hata mesajları ve davranışlar C# ile JavaScript tarafında tam olarak aynı değildir.

### Önerilen Çözüm

Veri yapıları için küçük bir test listesi oluşturulmalıdır. Boş stack, boş queue, aynı id değerinin tekrar eklenmesi ve hash çakışması gibi durumlar açıkça test edilmelidir.

### Test Senaryoları

- Boş stack üzerinden `Pop`
- Boş stack üzerinden `Peek`
- Boş queue üzerinden `Dequeue`
- Aynı id ile HashTable değer güncelleme
- Farklı id değerlerinde çakışma yönetimi

## 3. DOM Algoritmaları ve Ağaç Analizi

### Problem

`DomAlgorithms.cs` içindeki DFS ve BFS metotları sonucu liste olarak döndürmek yerine doğrudan konsola yazdırmaktadır. Bu yaklaşım test yazmayı ve sonuçları başka yerde kullanmayı zorlaştırır.

### Etkilenen Dosyalar

- `DomAlgorithms.cs`
- `TreeAnalyzer.cs`

### Risk

Algoritmalar çalışsa bile otomatik olarak doğrulanması zorlaşır. Proje büyüdüğünde veya arayüzle entegrasyon gerektiğinde bu yapı yetersiz kalabilir.

### Önerilen Çözüm

DFS ve BFS metotları ziyaret edilen düğümleri `List<DomNode>` olarak döndürmelidir. Konsola yazdırma işlemi algoritmanın içinde değil, çağıran tarafta yapılmalıdır.

### Test Senaryoları

- DFS sırasının beklenen sırayla eşleşmesi
- BFS sırasının beklenen sırayla eşleşmesi
- `null` kök düğüm için boş sonuç dönmesi
- Tag, class ve id aramalarının doğru düğümleri bulması

## 4. Derinlik Hesaplama Tutarlılığı

### Problem

`TreeAnalyzer.CalculateDepth` metodu ağacın yüksekliğini hesaplamaktadır. JavaScript tarafında ise `calculateDepth` bir düğümün köke olan uzaklığını hesaplamaktadır. Aynı isimli kavram iki tarafta farklı anlamda kullanılmaktadır.

### Etkilenen Dosyalar

- `TreeAnalyzer.cs`
- `dom-core.mjs`
- `script.mjs`

### Risk

Sunumda veya raporda "derinlik" kavramı karışabilir. Bir yerde ağaç yüksekliği, başka bir yerde düğüm seviyesi anlamına gelebilir.

### Önerilen Çözüm

Kavramlar ayrılmalıdır:

- `CalculateTreeHeight`: Ağacın toplam yüksekliği
- `CalculateNodeDepth`: Bir düğümün köke uzaklığı

Bu ayrım README ve rapor dosyalarında da açıklanmalıdır.

### Test Senaryoları

- Tek kök düğümlü ağaç
- Birden fazla seviyeli iç içe ağaç
- Yaprak düğümün derinlik hesabı
- Kök düğümün derinlik hesabı

## 5. Test ve Dokümantasyon Eksikliği

### Problem

Proje derlenebilir durumdadır, fakat davranışları kanıtlayan düzenli otomatik test yapısı bulunmamaktadır.

### Etkilenen Dosyalar

- `DomParser.csproj`
- `README.md`
- `hata_ve_bulgular_raporu.md`
- Yeni eklenecek test dosyaları

### Risk

Kodda yapılan değişiklikler daha sonra parser, arama veya görselleştirme tarafında fark edilmeden davranış bozukluğuna neden olabilir.

### Önerilen Çözüm

C# tarafı için ayrı bir test projesi eklenmelidir. JavaScript tarafı için de temel parser ve arama senaryoları ayrı test edilmelidir.

### Test Senaryoları

- Parser geçerli HTML için DOM ağacı oluşturmalı
- Parser hatalı HTML için kontrollü hata vermeli
- `id`, `class` ve `tag` aramaları doğru çalışmalı
- DFS ve BFS sıraları doğrulanmalı
- Arayüz boş girdi ve hatalı HTML durumunda bozulmamalı

## İş Bölümü Önerisi

Bu güncel bulgular 5 kişilik ekip için aşağıdaki şekilde bölüştürülebilir:

1. Parser ve HTML ayrıştırma sorumlusu
2. Veri yapıları sorumlusu
3. DOM algoritmaları ve arama sorumlusu
4. Arayüz ve görselleştirme sorumlusu
5. Test, dokümantasyon ve entegrasyon sorumlusu

Her kişi kendi alanındaki problemi, çözüm yaklaşımını ve test sonucunu bu dosyada güncel tutmalıdır.
