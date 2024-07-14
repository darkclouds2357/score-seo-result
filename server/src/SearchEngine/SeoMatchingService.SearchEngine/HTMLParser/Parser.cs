using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace SeoMatchingService.SearchEngine.HTMLParser
{
    public class Parser(string html)
    {
        private const string COMMENT_START = "<!--";
        private const string COMMENT_END = "-->";
        private int _index = 0;

        private HtmlTagNode _currentNode = new("root");

        private Stack<HtmlTagNode> _lastParentNodes = new();

        private readonly string[] _skipTag = [
            "script",
            "style",
            "head",
            "header"

        ];

        private readonly string[] _tagWithoutClosing = [
            "img",
            "meta"
        ];

        private char _currentValue => html[_index];
        private char _nextValue => html[_index + 1];
        private int _htmlLength => html.Length;

        private bool _isNotEnding => _index < _htmlLength;

        private bool _isSkipTag => _skipTag.Any(t => html[_index..].StartsWith($"<{t}"));

        private bool _isCommentStart => html[_index..].StartsWith(COMMENT_START);

        //private bool _isValidTag => _currentValue == '<' && _isNotEnding && (Char.IsLetter(_currentValue) || _currentValue == '/' || _currentValue == '?' || _currentValue == '!' || _currentValue == '%');

        public HtmlTagNode Parse()
        {
            while (_isNotEnding)
            {
                if (_isCommentStart)
                {
                    SkipComment();
                    continue;
                }
                else if (_currentValue == '<')
                {
                    if (_isSkipTag)
                    {
                        SkipTag();
                        continue;
                    }
                    else if (_nextValue == '/')
                    {
                        _index += 2; // Skip '</'
                        var closingTagName = ReadTagName();

                        if (_currentNode.TagName == closingTagName)
                        {
                            _currentNode = _lastParentNodes.Pop();
                            continue;
                        }
                    }
                    else
                    {
                        _index++;
                        var tagName = ReadTagName();
                        HtmlTagNode childNode = new(tagName)
                        {
                            Attributes = ReadAttributes()
                        };

                        _currentNode.Children.Add(childNode);

                        _lastParentNodes.Push(_currentNode);
                        _currentNode = childNode;
                    }
                }
                else if (_currentValue == '>')
                {
                    _index++; // Skip '>'

                    if (_tagWithoutClosing.Contains(_currentNode.TagName))
                    {
                        _currentNode = _lastParentNodes.Pop();
                        continue;
                    }
                }
                else if (char.IsLetter(_currentValue))
                {
                    _currentNode.Content = ReadTextContent();
                }
                else
                {
                    _index++;
                }
            }

            while (_lastParentNodes.Any())
            {
                _currentNode = _lastParentNodes.Pop();
            }


            return _currentNode;
        }

        private string ReadTagName()
        {
            var start = _index;
            while (_isNotEnding && _currentValue != '>' && !char.IsWhiteSpace(_currentValue))
                _index++;
            return html[start.._index];
        }

        private IReadOnlyDictionary<string, string[]> ReadAttributes()
        {
            var attributes = new Dictionary<string, string[]>();
            while (_isNotEnding && _currentValue != '>')
            {
                if (!char.IsWhiteSpace(_currentValue))
                {
                    var attributeName = ReadAttributeName();
                    var attributeValue = ReadAttributeValue();
                    attributes[attributeName] = attributeValue.Split(' ').Where(a => !string.IsNullOrWhiteSpace(a)).ToArray();
                }
                else
                {
                    _index++; // Skip whitespace
                }
            }
            return attributes;
        }

        private string ReadAttributeName()
        {
            var start = _index;
            while (_isNotEnding && _currentValue != '=' && !char.IsWhiteSpace(_currentValue))
                _index++;
            return html[start.._index];
        }

        private string ReadAttributeValue()
        {
            while (_isNotEnding && (char.IsWhiteSpace(_currentValue) || _currentValue == '='))
                _index++;
            var quote = _currentValue;
            _index++; // Skip quote (either ' or ")
            var start = _index;
            while (_isNotEnding && _currentValue != quote)
                _index++;
            var attributeValue = html[start.._index];
            _index++; // Skip quote
            return attributeValue;
        }

        private string ReadTextContent()
        {
            var start = _index;
            while (_isNotEnding && _currentValue != '<')
                _index++;
            return html[start.._index];
        }

        private void SkipComment()
        {

            _index += COMMENT_START.Length;
            var start = _index;
            while (_isNotEnding && !html[_index..].StartsWith(COMMENT_END))
            {
                _index++;
            }
            _currentNode.SkipComment.Add(html[start.._index]);

            _index += COMMENT_END.Length;
        }
        private void SkipTag()
        {
            var start = _index;
            _index++; // Skip '<'

            var tagName = ReadTagName();

            // Skip until the close of the tag
            //while (_isStillRead && _indexValue != '>')
            while (_isNotEnding && !html[_index..].StartsWith($"</{tagName}"))
            {
                _index++;
            }

            // skip close tag
            while (_isNotEnding && _currentValue != '>')
            {
                _index++;
            }

            _currentNode.SkipContent.Add(html[start.._index]);

            _index++; // Skip '>'
        }
    }

}
