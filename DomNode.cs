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
            if (string.IsNullOrWhiteSpace(tagName))
                throw new ArgumentException("Tag name cannot be empty", nameof(tagName));

            TagName = tagName;
            Id = null;
            TextContent = string.Empty;
            Classes = new List<string>();
            Parent = null;
            Children = new List<DomNode>();
        }

        public void AddChild(DomNode child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child), "Child cannot be null");

            child.Parent = this;
            Children.Add(child);
        }

        public void SetId(string id)
        {
            Id = id;
        }

        public void AddClass(string className)
        {
            if (string.IsNullOrWhiteSpace(className))
                return;

            Classes.Add(className);
        }
    }
}
