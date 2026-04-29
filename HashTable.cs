using System;

namespace DomParser
{
    public class HashTable
    {
        // Çakışmaları önlemek için kendi içinde bir linked list düğümü
        private class HashNode
        {
            public string Key { get; set; }     // ID değeri
            public DomNode Value { get; set; }  // İlgili DOM düğümü
            public HashNode Next { get; set; }  // Çakışma olursa bir sonraki düğüm

            public HashNode(string key, DomNode value)
            {
                Key = key;
                Value = value;
                Next = null;
            }
        }

        private HashNode[] buckets;
        private int capacity;

        public HashTable(int capacity = 100)
        {
            this.capacity = capacity;
            buckets = new HashNode[capacity];
        }

        // Gelen ID ye göre bir indeks numarası üreten hash fonksiyonu
        private int GetBucketIndex(string key)
        {
            int hashCode = key.GetHashCode();
            int index = hashCode % capacity;
            return Math.Abs(index);
        }

        // Tabloya yeni bir ID ve DomNode çifti ekleme
        public void Put(string key, DomNode value)
        {
            int index = GetBucketIndex(key);
            HashNode head = buckets[index];

            HashNode current = head;
            while (current != null)
            {
                if (current.Key == key)
                {
                    current.Value = value;
                    return;
                }
                current = current.Next;
            }

            HashNode newNode = new HashNode(key, value);
            newNode.Next = head;
            buckets[index] = newNode;
        }

        // Dokümanda istenen getElementById fonksiyonu
        public DomNode GetElementById(string key)
        {
            int index = GetBucketIndex(key);
            HashNode head = buckets[index];

            HashNode current = head;
            while (current != null)
            {
                if (current.Key == key)
                {
                    return current.Value;
                }
                current = current.Next;
            }

            return null;
        }
    }
}