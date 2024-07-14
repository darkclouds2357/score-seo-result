using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SeoMatchingService.SearchEngine.HTMLParser;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace SeoMatchingService.SearchEngine
{
    public record CrawlerParserOption
    {
        public string WrapperNodeXpath { get; init; }
        public string UrlNodeXpath { get; init; }
        public string TitleNodeXpath { get; init; }
        public string SnippetNodeXpath { get; init; }
    }

    public class CrawlerParser(Dictionary<string, CrawlerParserOption> crawlerOptions, ILogger<CrawlerParser> logger)
    {
        private Dictionary<string, CrawlerParserOption> _crawlerOptions = crawlerOptions;
        private ILogger<CrawlerParser> _logger = logger;

        public CrawlResult[] HTMLContentParser(string engine, string content)
        {
            var result = new List<CrawlResult>();

            if (_crawlerOptions.TryGetValue(engine, out var crawlerOptions))
            {
                content = content.Replace("<!doctype html>", string.Empty);
                var parser = new Parser(content);

                var htmlNode = parser.Parse();


                var nodes = htmlNode.FindXPath(crawlerOptions.WrapperNodeXpath);


                _logger.LogInformation("Wrapper Nodes Count {count}, xpath {xpath}", nodes.Count, crawlerOptions.WrapperNodeXpath);


                for (int i = 0; i < nodes.Count; i++)
                {
                    var node = nodes[i];

                    var urlNode = node.FindXPath(crawlerOptions.UrlNodeXpath).FirstOrDefault();
                    var titleNode = node.FindXPath(crawlerOptions.TitleNodeXpath).FirstOrDefault();
                    var snippetNode = node.FindXPath(crawlerOptions.SnippetNodeXpath).FirstOrDefault();

                    var crawlResult = new CrawlResult
                    (
                        engine,
                        urlNode != null && urlNode.Attributes.TryGetValue("href", out var urls) && urls.Any() ? urls.FirstOrDefault() : null,
                        titleNode?.Content,
                        snippetNode?.Content,
                        i
                     );
                    result.Add(crawlResult);
                }
            }

            return [.. result];
        }
    }
}