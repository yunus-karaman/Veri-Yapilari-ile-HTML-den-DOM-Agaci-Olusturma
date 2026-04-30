using System;
using System.Collections.Generic;

namespace DomParser
{
    // Kapsam (Scope) hatasını çözmek için metotlar bir sınıf içine alındı
    public class TreeAnalyzer
    {
        // Her yerden çağrılabilmesi için metotlar static yapıldı
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

            // Eğer düğüm boşsa veya kök düğümse (ebeveyni yoksa) boş liste döner
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

            // Etiket adını büyük/küçük harf duyarsız olarak karşılaştırıyoruz
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