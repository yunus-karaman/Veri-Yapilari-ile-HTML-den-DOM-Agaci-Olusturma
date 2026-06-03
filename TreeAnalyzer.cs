using System;
using System.Collections.Generic;

namespace DomParser
{
    public class SubtreeAnalysis
    {
        public int Size { get; set; }
        public int TextNodeCount { get; set; }
        public int Height { get; set; }
        public int Depth { get; set; }
    }

    // Kapsam (Scope) hatasını çözmek için metotlar bir sınıf içine alındı
    public class TreeAnalyzer
    {
        // Geriye uyumluluk için mevcut derinlik API'si korunur.
        public static int CalculateDepth(DomNode node)
        {
            if (node == null) return 0;
            return GetHeight(node) + 1;
        }

        public static int GetHeight(DomNode node)
        {
            if (node == null) return -1;
            if (node.Children == null || node.Children.Count == 0) return 0;

            int maxChildHeight = -1;
            foreach (var child in node.Children)
            {
                int childHeight = GetHeight(child);
                if (childHeight > maxChildHeight)
                {
                    maxChildHeight = childHeight;
                }
            }
            return maxChildHeight + 1;
        }

        public static int GetDepth(DomNode node)
        {
            int depth = 0;
            DomNode cursor = node;
            while (cursor != null && cursor.Parent != null)
            {
                depth++;
                cursor = cursor.Parent;
            }
            return depth;
        }

        public static List<DomNode> GetSiblings(DomNode node)
        {
            List<DomNode> siblings = new List<DomNode>();
            if (node == null || node.Parent == null) return siblings;

            foreach (var child in node.Parent.Children)
            {
                if (!object.ReferenceEquals(child, node))
                {
                    siblings.Add(child);
                }
            }
            return siblings;
        }

        public static bool TryGetSiblings(DomNode node, out List<DomNode> siblings)
        {
            siblings = new List<DomNode>();
            if (node == null || node.Parent == null) return false;

            foreach (var child in node.Parent.Children)
            {
                if (!object.ReferenceEquals(child, node))
                {
                    siblings.Add(child);
                }
            }
            return true;
        }

        public static void FindElementsByTagName(DomNode node, string targetTag, List<DomNode> results)
        {
            if (node == null) return;
            if (results == null) return;
            if (string.IsNullOrEmpty(targetTag)) return;

            if (node.TagName != null && node.TagName.Equals(targetTag, StringComparison.OrdinalIgnoreCase))
            {
                results.Add(node);
            }

            if (node.Children == null) return;

            foreach (var child in node.Children)
            {
                FindElementsByTagName(child, targetTag, results);
            }
        }

        public static int CountSubtreeNodes(DomNode node)
        {
            if (node == null) return 0;

            int count = 1;
            if (node.Children == null) return count;

            foreach (var child in node.Children)
            {
                count += CountSubtreeNodes(child);
            }

            return count;
        }

        public static int CountTextNodes(DomNode node)
        {
            if (node == null) return 0;

            int count = string.Equals(node.TagName, "#text", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
            if (node.Children == null) return count;

            foreach (var child in node.Children)
            {
                count += CountTextNodes(child);
            }

            return count;
        }

        public static SubtreeAnalysis AnalyzeSubtree(DomNode node)
        {
            if (node == null)
            {
                return new SubtreeAnalysis
                {
                    Size = 0,
                    TextNodeCount = 0,
                    Height = -1,
                    Depth = 0
                };
            }

            return new SubtreeAnalysis
            {
                Size = CountSubtreeNodes(node),
                TextNodeCount = CountTextNodes(node),
                Height = GetHeight(node),
                Depth = GetDepth(node)
            };
        }
    }
}
