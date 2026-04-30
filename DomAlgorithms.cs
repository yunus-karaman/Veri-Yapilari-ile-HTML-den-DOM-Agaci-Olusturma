using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DomParser
{
  
    public class DomNode
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; }
        public DomNode Parent { get; set; }
        public List<DomNode> Children { get; set; }

        public DomNode(string tagName)
        {
            TagName = tagName;
            Id = null;
            Classes = new List<string>();
            Parent = null;
            Children = new List<DomNode>();
        }

        public void AddChild(DomNode child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        public void SetId(string id)
        {
            Id = id;
        }

        public void AddClass(string className)
        {
            Classes.Add(className);
        }
    }


    public class Queue<T>
    {
        private LinkedList<T> list = new LinkedList<T>();

        public void Enqueue(T item)
        {
            list.AddLast(item);
        }

        public T Dequeue()
        {
            if (list.Count == 0)
                throw new Exception("Queue boş");

            T value = list.First.Value;
            list.RemoveFirst();
            return value;
        }

        public T Peek()
        {
            if (list.Count == 0)
                throw new Exception("Queue boş");

            return list.First.Value;
        }

        public int Count
        {
            get { return list.Count; }
        }

        public bool IsEmpty
        {
            get { return list.Count == 0; }
        }
    }

    public class Stack<T>
    {
        private List<T> elements = new List<T>();

        public void Push(T item)
        {
            elements.Add(item);
        }

        public T Pop()
        {
            if (elements.Count == 0)
                throw new InvalidOperationException("Stack boş, çıkarılacak eleman yok!");

            int lastIndex = elements.Count - 1;
            T item = elements[lastIndex];
            elements.RemoveAt(lastIndex);
            return item;
        }

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

    public class HashTable
    {
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

        public HashTable(int capacity = 100)
        {
            this.capacity = capacity;
            buckets = new HashNode[capacity];
        }

        private int GetBucketIndex(string key)
        {
            int hashCode = key.GetHashCode();
            int index = hashCode % capacity;
            return Math.Abs(index);
        }

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

        public DomNode GetElementById(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;

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

    public enum TokenType
    {
        OpenTag,
        CloseTag,
        Text
    }

    public class Token
    {
        public TokenType Type { get; set; }
        public string Content { get; set; }

        public Token(TokenType type, string content)
        {
            Type = type;
            Content = content;
        }
    }

    public class HtmlParser
    {
        public HashTable ElementTable { get; private set; }

        public HtmlParser()
        {
            ElementTable = new HashTable(100);
        }

        public DomNode Parse(string html)
        {
            List<Token> tokens = Tokenize(html);
            Stack<DomNode> stack = new Stack<DomNode>();
            DomNode root = null;

            foreach (var token in tokens)
            {
                if (token.Type == TokenType.OpenTag)
                {
                    DomNode node = CreateNodeFromTagContent(token.Content);

                
                    if (!string.IsNullOrEmpty(node.Id))
                    {
                        ElementTable.Put(node.Id, node);
                    }

                    if (stack.IsEmpty)
                    {
                        root = node;
                    }
                    else
                    {
                        stack.Peek().AddChild(node); 
                    }
                    
                    stack.Push(node); 
                }
                else if (token.Type == TokenType.CloseTag)
                {
                    if (!stack.IsEmpty)
                    {
                        stack.Pop(); 
                    }
                }
              
            }

            return root;
        }

        private List<Token> Tokenize(string html)
        {
            List<Token> tokens = new List<Token>();
            
         
            var matches = Regex.Matches(html, @"(</?[^>]+>)|([^<]+)");
            
            foreach (Match match in matches)
            {
                string value = match.Value.Trim();
                if (string.IsNullOrWhiteSpace(value)) continue;

                if (value.StartsWith("</"))
                {
                    tokens.Add(new Token(TokenType.CloseTag, value));
                }
                else if (value.StartsWith("<"))
                {
                    tokens.Add(new Token(TokenType.OpenTag, value));
                }
                else
                {
                    tokens.Add(new Token(TokenType.Text, value));
                }
            }
            return tokens;
        }

        private DomNode CreateNodeFromTagContent(string tagContent)
        {
           
            string cleanContent = tagContent.Trim('<', '>').Trim();
            
           
            string[] parts = cleanContent.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string tagName = parts[0];
            
            DomNode node = new DomNode(tagName);

           
            var idMatch = Regex.Match(cleanContent, @"id\s*=\s*['""]([^'""]+)['""]");
            if (idMatch.Success)
            {
                node.SetId(idMatch.Groups[1].Value);
            }

           
            var classMatch = Regex.Match(cleanContent, @"class\s*=\s*['""]([^'""]+)['""]");
            if (classMatch.Success)
            {
                string classString = classMatch.Groups[1].Value;
                string[] classes = classString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var c in classes)
                {
                    node.AddClass(c);
                }
            }

            return node;
        }
    }

   
    public class DomAlgorithms
    {
        public static void DepthFirstSearch(DomNode node)
        {
            if (node == null) return;
            
            Console.WriteLine("DFS Ziyaret: " + node.TagName + (node.Id != null ? $" (id: {node.Id})" : ""));
            
            foreach (var child in node.Children)
            {
                DepthFirstSearch(child);
            }
        }

        public static void BreadthFirstSearch(DomNode root)
        {
            if (root == null) return;
            
            Queue<DomNode> queue = new Queue<DomNode>();
            queue.Enqueue(root);

            while (!queue.IsEmpty)
            {
                DomNode current = queue.Dequeue();
                Console.WriteLine("BFS Ziyaret: " + current.TagName + (current.Id != null ? $" (id: {current.Id})" : ""));
                
                foreach (var child in current.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }

      
        public static List<DomNode> SearchByTagName(DomNode root, string targetTag)
        {
            List<DomNode> foundNodes = new List<DomNode>();
            if (root == null) return foundNodes;

            if (root.TagName.Equals(targetTag, StringComparison.OrdinalIgnoreCase))
            {
                foundNodes.Add(root);
            }

            foreach (var child in root.Children)
            {
                foundNodes.AddRange(SearchByTagName(child, targetTag));
            }

            return foundNodes;
        }
    }

    public class TreeAnalyzer
    {
        public static int CalculateDepth(DomNode node)
        {
            if (node == null) return 0;
            if (node.Children.Count == 0) return 1; 

            int maxChildDepth = 0;
            foreach (var child in node.Children)
            {
                int childDepth = CalculateDepth(child); 
                if (childDepth > maxChildDepth)
                {
                    maxChildDepth = childDepth;
                }
            }

            return maxChildDepth + 1; 
        }

        public static List<DomNode> GetSiblings(DomNode node)
        {
            List<DomNode> siblings = new List<DomNode>();

            if (node == null || node.Parent == null) 
                return siblings;

            foreach (var child in node.Parent.Children)
            {
                if (child != node) 
                {
                    siblings.Add(child);
                }
            }
            return siblings;
        }

        public static void FindElementsByTagName(DomNode node, string targetTag, List<DomNode> results)
        {
            if (node == null) return;

            if (node.TagName.Equals(targetTag, StringComparison.OrdinalIgnoreCase))
            {
                results.Add(node);
            }

            foreach (var child in node.Children)
            {
                FindElementsByTagName(child, targetTag, results);
            }
        }
    }
}
