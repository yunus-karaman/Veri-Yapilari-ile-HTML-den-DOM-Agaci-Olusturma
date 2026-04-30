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

        public HtmlParser()
        {
            // İstenen kapasitede tabloyu başlat
            ElementTable = new HashTable(100);
        }

        //PARSER - Token listesini okuyup Stack yardımıyla DOM Ağacı inşa etme
        public DomNode Parse(string html)
        {
            
            return null; 
        }

        //TOKENIZER - HTML metnini basit bir ayrıştırma yaklaşımı ile işleme
        private List<Token> Tokenize(string html)
        {
    
            return new List<Token>();
        }

        // YARDIMCI METOT: Etiket içeriğinden DomNode üretme
        private DomNode CreateNodeFromTagContent(string tagContent)
        {
            
            return null;
        }
        
    }
}