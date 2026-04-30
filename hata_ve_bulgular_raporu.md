# HATA VE BULGULAR



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
  * Stack Kapasite Yönetimi: HTML metninin ayrıştırılması ve hiyerarşik derinliğin takibi için kullanılacak Stack yapısı, statik sabit boyutlu bir dizi yerine dinamik liste altyapısıyla kurgulandı. Bu sayede, testlerde çok derin iç içe geçmiş HTML etiketleri geldiğinde Stack Overflow yaşanması engellendi.
  * Hash Table Çakışma Yönetimi: HTML elemanlarının id özelliklerini indekslerken iki farklı ID'nin aynı yuvaya düşmesi (collision) ihtimaline karşı "Separate Chaining" (Bağlı Liste Zincirleme) algoritması özel bir iç sınıf yazılarak çözüldü.
  * Zaman Karmaşıklığı Başarısı: Hash Table tasarımı sayesinde proje yönergesinde istenen getElementById işlemi test edildi ve düğüm sayısından bağımsız olarak ortalama $O(1)$ zaman karmaşıklığı hedefine ulaşıldı.

**2. Karşılaşılan Hatalar ve ÇözümlerHata**
   * Bug 01: Negatif Hash İndeksi ÜretimiDurum: Hash Table dizisi için indeks numarası üretilirken, bazı string ID değerlerinde sistemin yerleşik fonksiyonunun negatif değerler döndürdüğü ve bunun IndexOutOfRangeException hatasına yol açtığı tespit edildi.
   Çözüm: İndeks hesaplama fonksiyonuna mutlak değer Math.Abs() işlemi eklenerek dizinin her zaman geçerli ve pozitif bir yuvaya işaret etmesi sağlandı.
   * Bug 02: Boş Stackten Eleman Çıkarma RiskiDurum: Hatalı yazılmış bir HTML dokümanı simüle edildiğinde, boş olan Stack yapısından Pop veya Peek yapılmaya çalışılmasının sistemi çökerttiği gözlemlendi.
   Çözüm: Stack sınıfının Pop ve Peek metotlarına Count == 0 kontrolü eklendi. Sistem tamamen çökmek yerine durumu anlayıp kontrollü bir şekilde InvalidOperationException hatası fırlatacak hale getirildi.

### 3. `phase2-analysis-testing`

* **Son commit:** `65ff5ca`

* **Push Durumu:** uzak sunucuya push edildi.

**Yapılanlar:**
- DOM ağacının hiyerarşik analizi için `TreeAnalyzer.cs` sınıfı oluşturuldu.
- Ağaç derinliği hesaplama (`CalculateDepth`), kardeş düğümleri bulma (`GetSiblings`) ve etiket ismine göre arama (`FindElementsByTagName`) metotları eklendi.

**Bulgular ve Mimari Kararlar:**
- **Statik Erişim:** Ağaç analizi yapan tüm metotlar, projenin her yerinden instance (nesne) oluşturulmadan doğrudan kullanılabilmesi için `static` olarak yapılandırıldı.
- **Rekürsif (Özyineli) Yaklaşım:** `CalculateDepth` metodunda derinlik tespiti için ağaç yapısına en uygun olan rekürsif yaklaşım kullanıldı. Her alt düğüm kendi derinliğini hesaplayarak maksimum derinliği geriye döner.

**Karşılaşılan Hatalar ve Çözümler:**
- **Hata 01: Kök Düğümde (Root) Null Referans Hatası**
  - *Çözüm:* Metodun başlangıcına `if (node == null || node.Parent == null)` güvence kontrolü eklendi. Böylece üst düğümü olmayan hedef çağrılarında program çökmeden boş bir liste döndürüldü.

### 4. `phase2-traversal-search`


* **Son Commit:** `6f27eaf`

* **Push Durumu:** uzak sunucuya push edildi. 

## Çözülen Hatalar 
* **Kapsam (Scope) Hatası:** Sınıf dışında kalan `SearchByTagName` metodu `DomAlgorithms` sınıfı içerisine taşınarak projenin derlenmesini (compile) engelleyen kritik hata giderildi.
* **Büyük/Küçük Harf Duyarlılığı:** HTML standartlarına uymayan kesin eşitlik (`==`) araması yerine `StringComparison.OrdinalIgnoreCase` kullanıldı.
* **Null Referans Zafiyeti (NRE):** İlgili metotların başına `if (node == null) return;` kontrolü eklendi. Parser modülünden boş kök düğüm (root node) gelmesi durumunda uygulamanın çökmesi engellendi.


### 5. `phase3-ui-dom-visualizer`

**Son commit:** `13b054e`

**Push durumu:** Uzak repoya gonderildi.

**Tamamlanan baslica calismalar:**

- HTML girdi alanı ve DOM ağacı görüntüleme arayüzü eklendi.
- Arama cubugu ile `id`, `class` ve `tag` bazlı sorgu desteği eklendi.
- Düğüm seçimi, vurgulama, derinlik ve alt ağac bilgileri gosterildi.

