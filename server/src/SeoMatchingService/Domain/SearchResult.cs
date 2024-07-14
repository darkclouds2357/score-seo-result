using SeoMatchingService.SearchEngine;
using System.Text.RegularExpressions;

namespace SeoMatchingService.Domain
{
    public class SearchResult(string searchedValue, CrawlResult item)
    {
        public string Url { get; } = item.Url;
        public string SearchedValue { get; } = searchedValue;
        public Uri Uri
        {
            get
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(Url))
                    {
                        var regex = new Regex(@"\/url\?q=(?<url>[^&]*)");
                        var match = regex.Match(Url);
                        if (match.Success)
                        {
                            var url = match.Groups["url"].Value;
                            return new Uri(url);
                        }
                        return new Uri(Url);
                    }
                }
                catch
                {

                }
                return null;
            }
        }
        public string Title { get; } = item.Title;
        public string Snippets { get; } = item.Snippets;
        public string SearchEngine { get; } = item.Enigne;
        public int Order { get; } = item.Order;
    }
}