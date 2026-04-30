using System;
using System.Collections.Generic;

namespace DomParser
{
    public class DomAlgorithms
    {
        public static void DepthFirstSearch(DomNode node)
        {
            if (node == null) return;
            
            Console.WriteLine("DFS Ziyaret: " + node.TagName);
            
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
                Console.WriteLine("BFS Ziyaret: " + current.TagName);
                
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

       
        public static DomNode SearchById(DomNode root, string targetId)
        {
            if (root == null || string.IsNullOrEmpty(targetId)) return null;

            if (root.Id == targetId)
            {
                return root;
            }

            foreach (var child in root.Children)
            {
                DomNode result = SearchById(child, targetId);
                if (result != null)
                {
                    return result; 
                }
            }

            return null;
        }

       
        public static List<DomNode> SearchByClassName(DomNode root, string targetClass)
        {
            List<DomNode> foundNodes = new List<DomNode>();
            
            if (root == null || string.IsNullOrEmpty(targetClass)) return foundNodes;

            if (root.Classes.Contains(targetClass))
            {
                foundNodes.Add(root);
            }

            foreach (var child in root.Children)
            {
                foundNodes.AddRange(SearchByClassName(child, targetClass));
            }

            return foundNodes;
        }
    }
}
