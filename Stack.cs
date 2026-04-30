using System;
using System.Collections.Generic;

namespace DomParser
{
    public class Stack<T>
    {
        
        private List<T> elements = new List<T>();

        // Stackin en üstüne eleman ekler açılış etiketleri için
        public void Push(T item)
        {
            elements.Add(item);
        }

        // Stackin en üstündeki elemanı çıkarır ve döndürür kapanış etiketleri için
        public T Pop()
        {
            if (elements.Count == 0)
                throw new InvalidOperationException("Stack boş, çıkarılacak eleman yok!");

            int lastIndex = elements.Count - 1;
            T item = elements[lastIndex];
            elements.RemoveAt(lastIndex);
            return item;
        }

        // Stackin en üstündeki elemanı çıkarmadan sadece değerine bakar
        public T Peek()
        {
            if (elements.Count == 0)
                throw new InvalidOperationException("Stack boş!");

            return elements[elements.Count - 1];
        }

        public int Count
        {
            get { return elements.Count; }
        }

        public bool IsEmpty
        {
            get { return elements.Count == 0; }
        }
    }
}