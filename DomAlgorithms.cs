using System;
using System.Collections.Generic;

namespace DomParser
{
    public class DomAlgorithms
    {
        public static List<DomNode> DepthFirstSearch(DomNode root)
        {
            List<DomNode> visited = new List<DomNode>();
            DfsInternal(root, visited);
            return visited;
        }

        private static void DfsInternal(DomNode node, List<DomNode> visited)
        {
            if (node == null) return;
            visited.Add(node);
            foreach (var child in node.Children)
                DfsInternal(child, visited);
        }

        public static List<DomNode> BreadthFirstSearch(DomNode root)
        {
            List<DomNode> visited = new List<DomNode>();
            if (root == null) return visited;

            Queue<DomNode> queue = new Queue<DomNode>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                DomNode current = queue.Dequeue();
                visited.Add(current);
                foreach (var child in current.Children)
                    queue.Enqueue(child);
            }

            return visited;
        }

        public static List<DomNode> SearchByTagName(DomNode root, string targetTag)
        {
            List<DomNode> result = new List<DomNode>();
            SearchByTagNameInternal(root, targetTag, result);
            return result;
        }

        private static void SearchByTagNameInternal(
            DomNode node, string targetTag, List<DomNode> result)
        {
            if (node == null) return;

            if (node.TagName.Equals(targetTag, StringComparison.OrdinalIgnoreCase))
                result.Add(node);

            foreach (var child in node.Children)
                SearchByTagNameInternal(child, targetTag, result);
        }

        public static DomNode SearchById(DomNode root, string targetId)
        {
            if (root == null || string.IsNullOrEmpty(targetId)) return null;

            if (string.Equals(root.Id, targetId, StringComparison.Ordinal))
                return root;

            foreach (var child in root.Children)
            {
                DomNode result = SearchById(child, targetId);
                if (result != null) return result;
            }

            return null;
        }

        public static List<DomNode> SearchByClassName(DomNode root, string targetClass)
        {
            List<DomNode> result = new List<DomNode>();
            SearchByClassNameInternal(root, targetClass, result);
            return result;
        }

        private static void SearchByClassNameInternal(
            DomNode node, string targetClass, List<DomNode> result)
        {
            if (node == null || string.IsNullOrEmpty(targetClass)) return;

            if (node.Classes.Contains(targetClass))
                result.Add(node);

            foreach (var child in node.Children)
                SearchByClassNameInternal(child, targetClass, result);
        }
    }
}

        }
    }
}
