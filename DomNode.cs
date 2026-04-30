using System;
using System.Collections.Generic;

namespace DomParser
{
    public class DomNode
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public string TextContent { get; set; }
        public List<string> Classes { get; set; }
        public DomNode Parent { get; set; }
        public List<DomNode> Children { get; set; }

        public DomNode(string tagName)
        {
            TagName = tagName;
            Id = null;
            TextContent = string.Empty;
            Classes = new List<string>();
            Parent = null;
            Children = new List<DomNode>();
        }

        public void AddChild(DomNode child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        public void SetId(string id)
        {
            Id = id;
        }

        public void AddClass(string className)
        {
            Classes.Add(className);
        }
    }
}
