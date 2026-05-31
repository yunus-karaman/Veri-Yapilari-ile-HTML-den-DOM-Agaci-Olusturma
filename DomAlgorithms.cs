using System.Collections.Generic;

namespace DomParser
{
    public class DomAlgorithms
    {
      

        public static List<DomNode> DepthFirstSearch(DomNode root)
        {
            List<DomNode> visited = new List<DomNode>();
            if (root == null) return visited;

            Stack<DomNode> stack = new Stack<DomNode>();
            stack.Push(root);

            while (stack.Count > 0)
            {
                DomNode current = stack.Pop();
                visited.Add(current);

                if (current.Children == null) continue;

                for (int i = current.Children.Count - 1; i >= 0; i--)
                    stack.Push(current.Children[i]);
            }

            return visited;
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

                if (current.Children == null) continue;

                foreach (var child in current.Children)
                    queue.Enqueue(child);
            }

            return visited;
        }

   

        public static List<DomNode> SearchByTagName(DomNode root, string targetTag)
        {
            List<DomNode> result = new List<DomNode>();
            if (string.IsNullOrEmpty(targetTag)) return result;
            SearchByTagNameInternal(root, targetTag, result);
            return result;
        }

        private static void SearchByTagNameInternal(
            DomNode node, string targetTag, List<DomNode> result)
        {
            if (node == null) return;

            if (string.Equals(node.TagName, targetTag, StringComparison.OrdinalIgnoreCase))
                result.Add(node);

            if (node.Children == null) return;

            foreach (var child in node.Children)
                SearchByTagNameInternal(child, targetTag, result);
        }

        public static DomNode SearchById(DomNode root, string targetId)
        {
            if (root == null || string.IsNullOrEmpty(targetId)) return null;

            if (string.Equals(root.Id, targetId, StringComparison.Ordinal))
                return root;

            if (root.Children == null) return null;

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
            if (string.IsNullOrEmpty(targetClass)) return result;
            SearchByClassNameInternal(root, targetClass, result);
            return result;
        }

        private static void SearchByClassNameInternal(
            DomNode node, string targetClass, List<DomNode> result)
        {
            if (node == null) return;

            if (node.Classes != null &&
                node.Classes.Exists(c =>
                    string.Equals(c, targetClass, StringComparison.OrdinalIgnoreCase)))
                result.Add(node);

            if (node.Children == null) return;

            foreach (var child in node.Children)
                SearchByClassNameInternal(child, targetClass, result);
        }
    }
}

