using System;

namespace DomParser
{
    public class HashTable
    {
        // Çakışmalar için zincirleme (chaining) düğümü
        private class HashNode
        {
            public string Key { get; set; }
            public DomNode Value { get; set; }
            public HashNode Next { get; set; }

            public HashNode(string key, DomNode value)
            {
                Key = key;
                Value = value;
                Next = null;
            }
        }

        private HashNode[] buckets;
        private int capacity;
        private int count;
        private const double LoadFactorLimit = 0.75;

        public HashTable(int capacity = 100)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be positive");

            this.capacity = capacity;
            buckets = new HashNode[capacity];
            count = 0;
        }

        public int Count
        {
            get { return count; }
        }

        // Kendi yazdığımız hash fonksiyonu (polynomial rolling hash)
        private int GetBucketIndex(string key, int bucketCount)
        {
            int hash = 0;
            foreach (char c in key)
            {
                hash = hash * 31 + c;
            }
            return (hash & 0x7FFFFFFF) % bucketCount;   // negatif/taşma olmasın diye maske
        }

        public void Put(string key, DomNode value)
        {
            ValidateKey(key);

            int index = GetBucketIndex(key, capacity);
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
            count++;

            // tablo dolunca büyüt
            if ((double)count / capacity > LoadFactorLimit)
            {
                Rehash();
            }
        }

        public DomNode GetElementById(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            int index = GetBucketIndex(key, capacity);
            HashNode current = buckets[index];

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

        public bool Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;

            int index = GetBucketIndex(key, capacity);
            HashNode current = buckets[index];
            HashNode previous = null;

            while (current != null)
            {
                if (current.Key == key)
                {
                    if (previous == null)
                        buckets[index] = current.Next;
                    else
                        previous.Next = current.Next;

                    count--;
                    return true;
                }
                previous = current;
                current = current.Next;
            }

            return false;
        }

        // Kapasiteyi 2 katına çıkarıp elemanları yeniden dağıtır
        private void Rehash()
        {
            int newCapacity = capacity * 2;
            HashNode[] newBuckets = new HashNode[newCapacity];

            for (int i = 0; i < buckets.Length; i++)
            {
                HashNode current = buckets[i];
                while (current != null)
                {
                    HashNode next = current.Next;

                    int index = GetBucketIndex(current.Key, newCapacity);
                    current.Next = newBuckets[index];
                    newBuckets[index] = current;

                    current = next;
                }
            }

            buckets = newBuckets;
            capacity = newCapacity;
        }

        private static void ValidateKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("HashTable key cannot be null or empty", nameof(key));
        }
    }
}
