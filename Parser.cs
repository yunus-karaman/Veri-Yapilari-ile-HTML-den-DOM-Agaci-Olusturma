using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DomParser
{
    // Token tiplerini belirten Enum
    public enum TokenType
    {
        OpenTag,
        SelfClosingTag,
        CloseTag,
        Text
    }

    // HTML metninden koparılan her bir anlamlı parçayı temsil eden sınıf
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

    // --- ANA PARSER SINIFI ---

    public class HtmlParser
    {
        private static readonly HashSet<string> VoidElements = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "area", "base", "br", "col", "embed", "hr", "img",
            "input", "link", "meta", "param", "source", "track", "wbr"
        };

        // O(1) hızında id ile arama yapabilmek için kullanılacak Hash Tablosu
        public HashTable ElementTable { get; private set; }

        public HtmlParser()
        {
            // İstenen kapasitede tabloyu başlat
            ElementTable = new HashTable(100);
        }

        //PARSER - Token listesini okuyup Stack yardımıyla DOM Ağacı inşa etme
        public DomNode Parse(string html)
        {
            ElementTable = new HashTable(100);

            DomNode root = new DomNode("document");
            Stack<DomNode> stack = new Stack<DomNode>();
            stack.Push(root);

            foreach (Token token in Tokenize(html))
            {
                if (token.Type == TokenType.Text)
                {
                    string text = token.Content.Trim();
                    if (text.Length == 0)
                    {
                        continue;
                    }

                    DomNode textNode = new DomNode("#text");
                    textNode.TextContent = text;
                    stack.Peek().AddChild(textNode);
                    continue;
                }

                if (token.Type == TokenType.OpenTag || token.Type == TokenType.SelfClosingTag)
                {
                    DomNode node = CreateNodeFromTagContent(token.Content);
                    stack.Peek().AddChild(node);

                    if (!string.IsNullOrWhiteSpace(node.Id))
                    {
                        ElementTable.Put(node.Id, node);
                    }

                    if (token.Type == TokenType.OpenTag)
                    {
                        stack.Push(node);
                    }

                    continue;
                }

                string closingTag = token.Content
                    .Replace("</", string.Empty)
                    .Replace(">", string.Empty)
                    .Trim();

                if (stack.Count == 1)
                {
                    throw new InvalidOperationException($"Etiket uyusmazligi: </{closingTag}> beklenmeyen konumda.");
                }

                DomNode current = stack.Pop();
                if (!current.TagName.Equals(closingTag, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException($"Etiket uyusmazligi: </{closingTag}> beklenmeyen konumda.");
                }
            }

            if (stack.Count != 1)
            {
                throw new InvalidOperationException("HTML yapisi tamamlanmamis. Acik kalan etiketler var.");
            }

            return root;
        }

        //TOKENIZER - HTML metnini basit bir ayrıştırma yaklaşımı ile işleme
        private List<Token> Tokenize(string html)
        {
            List<Token> tokens = new List<Token>();

            if (string.IsNullOrWhiteSpace(html))
            {
                return tokens;
            }

            string pattern = @"<!--[\s\S]*?-->|<![^>]*>|</?[^>]+>|[^<]+";
            MatchCollection matches = Regex.Matches(html, pattern);

            foreach (Match match in matches)
            {
                string value = match.Value;

                if (value.StartsWith("<!--", StringComparison.Ordinal) ||
                    value.StartsWith("<!", StringComparison.Ordinal))
                {
                    continue;
                }

                if (value.StartsWith("</", StringComparison.Ordinal))
                {
                    tokens.Add(new Token(TokenType.CloseTag, value));
                    continue;
                }

                if (value.StartsWith("<", StringComparison.Ordinal))
                {
                    string tagName = ExtractTagName(value);
                    TokenType tokenType = value.EndsWith("/>", StringComparison.Ordinal) || VoidElements.Contains(tagName)
                        ? TokenType.SelfClosingTag
                        : TokenType.OpenTag;

                    tokens.Add(new Token(tokenType, value));
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(value))
                {
                    tokens.Add(new Token(TokenType.Text, value));
                }
            }

            return tokens;
        }

        // YARDIMCI METOT: Etiket içeriğinden DomNode üretme
        private DomNode CreateNodeFromTagContent(string tagContent)
        {
            string normalized = tagContent
                .Trim()
                .TrimStart('<')
                .TrimEnd('>')
                .TrimEnd('/')
                .Trim();

            string[] parts = normalized.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                throw new InvalidOperationException("Gecersiz etiket bulundu.");
            }

            DomNode node = new DomNode(parts[0].ToLowerInvariant());
            MatchCollection attributeMatches = Regex.Matches(
                normalized,
                "([A-Za-z_:][-A-Za-z0-9_:.]*)\\s*=\\s*(\"([^\"]*)\"|'([^']*)'|([^\\s\"'=<>`]+))");

            foreach (Match attributeMatch in attributeMatches)
            {
                string attributeName = attributeMatch.Groups[1].Value;
                string attributeValue =
                    attributeMatch.Groups[3].Success ? attributeMatch.Groups[3].Value :
                    attributeMatch.Groups[4].Success ? attributeMatch.Groups[4].Value :
                    attributeMatch.Groups[5].Value;

                if (attributeName.Equals("id", StringComparison.OrdinalIgnoreCase))
                {
                    node.SetId(attributeValue);
                    continue;
                }

                if (attributeName.Equals("class", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (string className in attributeValue.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        node.AddClass(className);
                    }
                }
            }

            return node;
        }

        private string ExtractTagName(string tagContent)
        {
            string normalized = tagContent
                .Trim()
                .TrimStart('<')
                .TrimEnd('>')
                .TrimEnd('/')
                .Trim();

            string[] parts = normalized.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return parts.Length == 0 ? string.Empty : parts[0].ToLowerInvariant();
        }
    }
}
