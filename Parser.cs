using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DomParser
{
    // Token tiplerini belirten Enum
    public enum TokenType
    {
        OpenTag,
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
        // O(1) hızında id ile arama yapabilmek için kullanılacak Hash Tablosu
        public HashTable ElementTable { get; private set; }

        // Self-closing (void) HTML etiketleri — bunlar stack'e push edilmez
        private static readonly HashSet<string> VoidElements = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase)
        {
            "area", "base", "br", "col", "embed", "hr", "img",
            "input", "link", "meta", "param", "source", "track", "wbr"
        };

        public HtmlParser()
        {
            // İstenen kapasitede tabloyu başlat
            ElementTable = new HashTable(100);
        }

        //PARSER - Token listesini okuyup Stack yardımıyla DOM Ağacı inşa etme
        public DomNode Parse(string html)
        {
            List<Token> tokens = Tokenize(html);

            // Sanal kök düğüm oluştur
            DomNode root = new DomNode("document");
            Stack<DomNode> stack = new Stack<DomNode>();
            stack.Push(root);

            foreach (var token in tokens)
            {
                switch (token.Type)
                {
                    case TokenType.OpenTag:
                        DomNode node = CreateNodeFromTagContent(token.Content);
                        if (node != null)
                        {
                            // Mevcut ebeveyne çocuk olarak ekle
                            stack.Peek().AddChild(node);

                            // Self-closing veya void element değilse stack'e push et
                            bool isSelfClosing = token.Content.TrimEnd().EndsWith("/>");
                            bool isVoidElement = VoidElements.Contains(node.TagName);

                            if (!isSelfClosing && !isVoidElement)
                            {
                                stack.Push(node);
                            }
                        }
                        break;

                    case TokenType.CloseTag:
                        // Stack'te sadece kök kalmamalı — güvenlik kontrolü
                        if (stack.Count > 1)
                        {
                            stack.Pop();
                        }
                        break;

                    case TokenType.Text:
                        string trimmedText = token.Content.Trim();
                        if (!string.IsNullOrEmpty(trimmedText))
                        {
                            // Metin düğümü oluştur ve ebeveyne ekle
                            DomNode textNode = new DomNode("#text");
                            stack.Peek().AddChild(textNode);
                        }
                        break;
                }
            }

            return root;
        }

        //TOKENIZER - HTML metnini Regex ile token'lara ayırma
        private List<Token> Tokenize(string html)
        {
            List<Token> tokens = new List<Token>();

            // Desen: HTML yorumları | DOCTYPE/CDATA | Etiketler (açılış/kapanış) | Metin
            string pattern = @"<!--[\s\S]*?-->|<![^>]*>|</?[^>]+>|[^<]+";

            MatchCollection matches = Regex.Matches(html, pattern);

            foreach (Match match in matches)
            {
                string value = match.Value;

                // HTML yorumlarını atla (<!-- ... -->)
                if (value.StartsWith("<!--"))
                    continue;

                // DOCTYPE bildirimlerini atla (<! ... >)
                if (value.StartsWith("<!"))
                    continue;

                // Kapanış etiketi (</div>, </p> vb.)
                if (value.StartsWith("</"))
                {
                    tokens.Add(new Token(TokenType.CloseTag, value));
                    continue;
                }

                // Açılış etiketi (<div>, <p class="x"> vb.)
                if (value.StartsWith("<"))
                {
                    tokens.Add(new Token(TokenType.OpenTag, value));
                    continue;
                }

                // Metin düğümü — boş olmayan metinleri ekle
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
            // < ve > karakterlerini kaldır, self-closing "/" karakterini temizle
            string content = tagContent
                .TrimStart('<')
                .TrimEnd('>')
                .TrimEnd('/')
                .Trim();

            if (string.IsNullOrEmpty(content))
                return null;

            // Tag adını çıkar (ilk kelime, harf ile başlamalı)
            Match tagMatch = Regex.Match(content, @"^[a-zA-Z][a-zA-Z0-9]*");
            if (!tagMatch.Success)
                return null;

            string tagName = tagMatch.Value;
            DomNode node = new DomNode(tagName);

            // id attribute'unu çıkar — id="value" veya id='value'
            Match idMatch = Regex.Match(content, @"id\s*=\s*[""']([^""']+)[""']");
            if (idMatch.Success)
            {
                string idValue = idMatch.Groups[1].Value;
                node.SetId(idValue);
                // Hash tablosuna kaydet — O(1) erişim için
                ElementTable.Put(idValue, node);
            }

            // class attribute'unu çıkar — class="cls1 cls2" veya class='cls1 cls2'
            Match classMatch = Regex.Match(content, @"class\s*=\s*[""']([^""']+)[""']");
            if (classMatch.Success)
            {
                string[] classes = classMatch.Groups[1].Value
                    .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string cls in classes)
                {
                    node.AddClass(cls);
                }
            }

            return node;
        }
        
    }
}