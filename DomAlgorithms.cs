using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DomParser
{
    
    public enum TokenType
    {
        OpenTag,
        CloseTag,
        SelfClosingTag, 
        Text
    }

    public class Token
    {
        public TokenType Type    { get; set; }
        public string    Content { get; set; }

        public Token(TokenType type, string content)
        {
            Type    = type;
            Content = content;
        }
    }

    public class HtmlParser
    {
        public HashTable ElementTable { get; private set; }

        // SORUN 4 ÇÖZÜMÜ: HTML spesifikasyonundaki tüm void element'ler.
        // HashSet + StringComparer.OrdinalIgnoreCase → O(1) arama, büyük/küçük harf duyarsız.
        private static readonly HashSet<string> VoidElements =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "area", "base", "br", "col", "embed",
                "hr", "img", "input", "link", "meta",
                "param", "source", "track", "wbr"
            };

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

                    // Id varsa hash tablosuna ekle → getElementById O(1)
                    if (!string.IsNullOrEmpty(node.Id))
                        ElementTable.Put(node.Id, node);

                    if (stack.IsEmpty)
                        root = node;          // İlk açılan tag → kök
                    else
                        stack.Peek().AddChild(node); // Üstteki ebeveyne ekle

                    stack.Push(node);         // Kapatılmayı beklemek için stack'e ekle
                }
                else if (token.Type == TokenType.SelfClosingTag)
                {
                   
                    DomNode node = CreateNodeFromTagContent(token.Content);

                    if (!string.IsNullOrEmpty(node.Id))
                        ElementTable.Put(node.Id, node);

                    if (!stack.IsEmpty)
                        stack.Peek().AddChild(node);
                    else
                        root = node; 
                }
                else if (token.Type == TokenType.CloseTag)
                {
                    if (!stack.IsEmpty)
                        stack.Pop(); 
                }
                
            }

            return root;
        }

      
        private List<Token> Tokenize(string html)
        {
            List<Token> tokens = new List<Token>();

           
            var matches = Regex.Matches(
                html,
                @"<!--[\s\S]*?-->|<![^>]*>|</?[^>]+>|[^<]+"
            );

            foreach (Match match in matches)
            {
                string value = match.Value.Trim();
                if (string.IsNullOrWhiteSpace(value)) continue;

                // Yorum veya DOCTYPE → tamamen atla
                if (value.StartsWith("<!--") || value.StartsWith("<!"))
                    continue;

                if (value.StartsWith("</"))
                {
                    // Kapanış tag'ı: </div>, </p> vb.
                    tokens.Add(new Token(TokenType.CloseTag, value));
                }
                else if (value.StartsWith("<"))
                {
                  
                    string tagName = ExtractTagName(value);

                    bool isSelfClosing = value.EndsWith("/>")
                                     || VoidElements.Contains(tagName);

                    TokenType type = isSelfClosing
                        ? TokenType.SelfClosingTag
                        : TokenType.OpenTag;

                    tokens.Add(new Token(type, value));
                }
                else
                {
                    tokens.Add(new Token(TokenType.Text, value));
                }
            }

            return tokens;
        }

        
        private string ExtractTagName(string tagContent)
        {
           
            string clean = tagContent.Trim('<', '>', '/').Trim();
            int spaceIndex = clean.IndexOf(' ');
            return spaceIndex >= 0
                ? clean.Substring(0, spaceIndex)
                : clean;
        }

 
        private DomNode CreateNodeFromTagContent(string tagContent)
        {
           
            string cleanContent = tagContent.Trim('<', '>', '/').Trim();

            string[] parts = cleanContent.Split(
                new[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries
            );

          
            if (parts.Length == 0)
                throw new InvalidOperationException(
                    $"Geçersiz tag içeriği: '{tagContent}'. " +
                    "Tag adı belirlenemiyor."
                );

            string tagName = parts[0]; // Artık güvenli
            DomNode node = new DomNode(tagName);

          
            var idMatch = Regex.Match(
                cleanContent,
                @"id\s*=\s*['""]([^'""]+)['""]"
            );
            if (idMatch.Success)
                node.SetId(idMatch.Groups[1].Value);

         
            var classMatch = Regex.Match(
                cleanContent,
                @"class\s*=\s*['""]([^'""]+)['""]"
            );
            if (classMatch.Success)
            {
                string[] classes = classMatch.Groups[1].Value
                    .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var c in classes)
                    node.AddClass(c);
            }

            return node;
        }
    }

 
    public class DomAlgorithms
    {
        
        public static List<DomNode> Dfs(DomNode root)
        {
           
            List<DomNode> result = new List<DomNode>();
            DfsHelper(root, result);
            return result;
        }

      
        private static void DfsHelper(DomNode node, List<DomNode> result)
        {
            if (node == null) return;

            result.Add(node); 
            foreach (var child in node.Children)
                DfsHelper(child, result); 
        }

      
        public static List<DomNode> Bfs(DomNode root)
        {
            List<DomNode> result = new List<DomNode>();
            if (root == null) return result;

          
            Queue<DomNode> queue = new Queue<DomNode>();
            queue.Enqueue(root);

            while (!queue.IsEmpty)
            {
                DomNode current = queue.Dequeue();
                result.Add(current);

                foreach (var child in current.Children)
                    queue.Enqueue(child);
            }

            return result;
        }

      
        public static void SearchByTagName(
            DomNode        root,
            string         tag,
            List<DomNode>  results)
        {
            if (root == null || results == null) return;

            if (root.TagName.Equals(tag, StringComparison.OrdinalIgnoreCase))
                results.Add(root);

            foreach (var child in root.Children)
                SearchByTagName(child, tag, results); 
        }
    }
}

