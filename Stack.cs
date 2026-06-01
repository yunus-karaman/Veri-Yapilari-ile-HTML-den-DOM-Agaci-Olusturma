using System;

namespace DomParser
{
    public class Stack<T>
    {
        // Kendi bağlı liste düğümümüz
        private class Node
        {
            public T Value;
            public Node Next;

            public Node(T value, Node next)
            {
                Value = value;
                Next = next;
            }
        }

        private Node head;
        private int count;

        public void Push(T item)
        {
            head = new Node(item, head);
            count++;
        }

        public T Pop()
        {
            if (head == null)
                throw new InvalidOperationException("Stack boş, çıkarılacak eleman yok!");

            T value = head.Value;
            head = head.Next;
            count--;
            return value;
        }

        public T Peek()
        {
            if (head == null)
                throw new InvalidOperationException("Stack boş!");

            return head.Value;
        }

        public int Count
        {
            get { return count; }
        }

        public bool IsEmpty
        {
            get { return head == null; }
        }
    }
}
