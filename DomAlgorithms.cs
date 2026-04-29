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
    }
}


        public static List<DomNode> SearchByTagName(DomNode root, string targetTag)
        {
            List<DomNode> foundNodes = new List<DomNode>();
            
            if (root == null) return foundNodes;

           
            if (root.TagName == targetTag)
            {
                foundNodes.Add(root);
            }

           
            foreach (var child in root.Children)
            {
           
                foundNodes.AddRange(SearchByTagName(child, targetTag));
            }

            return foundNodes;
        }
