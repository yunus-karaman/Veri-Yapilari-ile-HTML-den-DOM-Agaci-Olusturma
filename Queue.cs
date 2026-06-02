using System;

namespace DomParser
{
    public class Queue<T>
    {
        private QueueNode<T> _head;
        private QueueNode<T> _tail;
        private int _count;

        public Queue()
        {
            _head = null;
            _tail = null;
            _count = 0;
        }

        public void Enqueue(T item)
        {
            QueueNode<T> newNode = new QueueNode<T>(item);

            if (_tail == null)
            {
                _head = newNode;
                _tail = newNode;
            }
            else
            {
                _tail.Next = newNode;
                _tail = newNode;
            }

            _count++;
        }

        public T Dequeue()
        {
            if (_head == null)
                throw new InvalidOperationException("Queue boş");

            T value = _head.Value;
            _head = _head.Next;

            if (_head == null)
                _tail = null;

            _count--;
            return value;
        }

        public T Peek()
        {
            if (_head == null)
                throw new InvalidOperationException("Queue boş");

            return _head.Value;
        }

        public int Count
        {
            get { return _count; }
        }

        public bool IsEmpty
        {
            get { return _count == 0; }
        }
    }
}