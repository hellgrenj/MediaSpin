using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Logging;

namespace tracker.Domain.Services
{
    public class Spider : ISpider
    {
        private readonly ILogger _logger;
        private readonly INodeServices _nodeService;
        public Spider(ILogger<Spider> logger, INodeServices nodeServices)
        {
            _logger = logger;
            _nodeService = nodeServices;
        }

        public HtmlDocument LoadPage(string url)
        {
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(url);
            return htmlDoc;
        }

        public HtmlNodeCollection GetHeadersOfSize(HtmlDocument htmlDoc, int headerSize)
        {
            var emptyCollection = new HtmlNodeCollection(null);
            var headers = htmlDoc.DocumentNode.SelectNodes($"//h{headerSize}");
            if (headers != null && headers.Count > 0)
            {
                return headers;
            }
            else
            {
                return emptyCollection;
            }
        }
        public void RemoveNodesFromDocument(HtmlDocument document, string expression)
        {
            var nodes = document.DocumentNode.SelectNodes(expression);
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    node.Remove();
                }
            }
        }

        public async Task<string> HtmlToTextAsync(string html)
        {
            return await _nodeService.InvokeAsync<string>(@"./NodeScripts/html-to-text.js", html);
        }

        public (string articleUrl, HtmlDocument article) DownloadArticleByHeader(string baseUrl, HtmlNode node)
        {
            if (node.Ancestors("a").Count() == 0)
            {
                _logger.LogDebug("Could not find article link");
                return (null, null);
            }
            var link = node.Ancestors("a").First().Attributes["href"].Value;
            if (link[0] != '/')
            {
                link = "/" + link;
            }
            var articleUrl = baseUrl + link;
            var article = LoadPage(articleUrl);
            return (articleUrl, article);
        }
    }
}