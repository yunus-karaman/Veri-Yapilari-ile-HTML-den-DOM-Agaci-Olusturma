### DomNode'da TextContent Alanının Eksik Olması

**Etkilenen Dosyalar:**
- DomNode.cs

**Problem:**
`<p>Merhaba</p>` gibi HTML yapılarında metin saklanamıyordu.

**Uygulanan Çözüm:**
DomNode.cs'ye `TextContent` eklendi, constructor'da `string.Empty` ile başlatıldı.

### Queue .NET LinkedList Kullanıyordu

**Etkilenen Dosyalar:**
- Queue.cs
- QueueNode.cs

**Problem:**
Hazır LinkedList kullanılıyordu, sıfırdan yazılma şartı ihlal ediliyordu.

**Uygulanan Çözüm:**
Queue kendi linked list yapısıyla (`QueueNode<T>`) sıfırdan yazıldı.

### Queue'da Yanlış Exception Türü

**Etkilenen Dosyalar:**
- Queue.cs

**Problem:**
`System.Exception` kullanılıyordu.

**Uygulanan Çözüm:**
`InvalidOperationException` olarak değiştirildi.

### AddChild'de Null Kontrolü Yoktu

**Etkilenen Dosyalar:**
- DomNode.cs

**Problem:**
`AddChild(null)` çağrısı kontrolsüzdü.

**Uygulanan Çözüm:**
`ArgumentNullException` kontrolü eklendi.