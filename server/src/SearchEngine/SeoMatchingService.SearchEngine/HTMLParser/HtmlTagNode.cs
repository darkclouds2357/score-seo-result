using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SeoMatchingService.SearchEngine.HTMLParser
{
    public class HtmlTagNode(string tagName)
    {
        private record TagPath(string TagName, string Attribute, string[] AttributeValues);

        private readonly Regex _tagRegex = new(@"(?<tag>[a-zA-Z0-9_-]+)\[@(?<attribute>[a-zA-Z0-9_-]+)='(?<value>[^']+)'\]");

        public string TagName { get; set; } = tagName;
        public IReadOnlyDictionary<string, string[]> Attributes { get; set; } = new Dictionary<string, string[]>();
        public List<HtmlTagNode> Children { get; set; } = [];
        public string Content { get; set; } = string.Empty;

        public List<string> SkipContent { get; set; } = [];
        public List<string> SkipComment { get; set; } = [];

        private static IEnumerable<HtmlTagNode> FindNodes(IEnumerable<HtmlTagNode> nodes, LinkedListNode<TagPath> tag)
        {
            var result = new List<HtmlTagNode>();

            Func<HtmlTagNode, bool> filter = n => n.TagName == tag.Value.TagName && (string.IsNullOrWhiteSpace(tag.Value.Attribute) || n.Attributes.TryGetValue(tag.Value.Attribute, out var attributes) && tag.Value.AttributeValues.All(a => attributes.Contains(a)));

            foreach (var node in nodes)
            {
                if (filter(node))
                {
                    yield return node;
                }
                else if (node.Children?.Any() ?? false)
                {
                    var childMatches = FindNodes(node.Children, tag);
                    foreach (var child in childMatches)
                    {
                        yield return child;
                    }
                }

            }

        }

        public IReadOnlyList<HtmlTagNode> FindXPath(string xpath)
        {
            var tags = new LinkedList<TagPath>(xpath.Split('/').Select(path =>
            {
                var match = _tagRegex.Match(path);
                if (match.Success)
                {
                    var tag = match.Groups["tag"].Value;
                    var attribute = match.Groups["attribute"].Value;
                    var values = match.Groups["value"].Value.Split(' ').ToArray();
                    return new TagPath(tag, attribute, values);
                }
                return new TagPath(path, null, null);
            }));

            var result = new List<HtmlTagNode>();

            var source = Children.AsEnumerable();

            var tag = tags.First;

            while (tag != null)
            {
                var nextTag = tag.Next;

                if (nextTag != null && string.IsNullOrWhiteSpace(tag.Value.TagName) && string.IsNullOrWhiteSpace(nextTag.Value.TagName))
                {
                    tag = nextTag.Next;
                }
                else
                {
                    source = FindNodes(source, tag);
                    tag = nextTag;
                }
            }

            return source.ToList();


        }
    }

}
