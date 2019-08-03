using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using tracker.Domain.Ports.Outbound;
using tracker.Domain.Models;
using tracker.Domain.Ports.Inbound;

namespace tracker.Domain.Services
{
    public class Tracker : ITracker
    {
        private readonly IPipeline _pipeline;
        private readonly ISpider _spider;
        private readonly ILogger _logger;

        private readonly IValidator _validator;

        private readonly IExtractor _extractor;

        private List<string> _keywords { get; set; } = new List<string>();
        public Tracker(IPipeline pipeline, ISpider spider, ILogger<Tracker> logger, IValidator validator, IExtractor extractor)
        {
            _logger = logger;
            _pipeline = pipeline;
            _spider = spider;
            _validator = validator;
            _extractor = extractor;
            var keywords = Environment.GetEnvironmentVariable("KEYWORDS") ?? "Liberalerna;Kristdemokraterna;Socialdemokraterna;Sverige;Brand";
            _keywords.AddRange(keywords.Split(";"));
        }
        public void Scan(string baseUrl, List<Article> allArticles)
        {
            _logger.LogInformation($"Extracting articles for analysis from {baseUrl.ToUpper()}");
            SearchArticles(baseUrl, allArticles);
        }

        private void SearchArticles(string baseUrl, List<Article> allArticles, int headerSize = 1)
        {
            var htmlDoc = _spider.LoadPage(baseUrl);
            var headers = _spider.GetHeadersOfSize(htmlDoc, headerSize);
            foreach (var header in headers)
            {
                try
                {
                    header.InnerText.Trim();
                    if (!_validator.ConsideredArticleHeader(header.InnerText))
                        continue;
    
                    var (articleUrl, articleHtmlDocument) = _spider.DownloadArticleByHeader(baseUrl, header);
                    if (articleUrl == null && articleHtmlDocument == null)
                        continue;

                    var articleBodyText = _extractor.ExtractBodyTextFromArticleDocument(articleHtmlDocument);
                    var article = CreateArticle(articleHtmlDocument, articleBodyText, allArticles, baseUrl, articleUrl, header.InnerText);
                    var foundKeywords = ScanArticleForKeyWords(article);
                    if (foundKeywords.Count() > 0)
                    {
                        article.Keywords = foundKeywords;
                        _pipeline.SendForAnalysis(article);
                    }

                }
                catch (Exception e)
                {
                    _logger.LogError($"Failed to download and process article {header.InnerText} with the following exception {e.Message} stack trace: {e.StackTrace}");
                    continue;
                }


            }
            headerSize++;
            if (headerSize <= 4)
                SearchArticles(baseUrl, allArticles, headerSize);
        }
    
        private Article CreateArticle(HtmlDocument articleHtmlDocument, string articleBodyText, List<Article> allArticles, string baseUrl, string articleUrl, string header)
        {
            articleHtmlDocument.Text = articleBodyText;
            var article = new Article() { Source = baseUrl, Text = articleHtmlDocument.Text.Trim(), Header = header.Trim(), Keywords = new List<string>(), ArticleUrl = articleUrl };
            allArticles.Add(article);
            _logger.LogDebug($"article is {articleHtmlDocument.Text.Length} characters long");
            return article;
        }

        private List<string> ScanArticleForKeyWords(Article article)
        {
            var foundKeywords = new List<string>();
            foreach (var keyword in _keywords)
            {
                if (article.Text.Contains(keyword))
                {
                    _logger.LogDebug($"This article contains the keyphrase {keyword}");
                    foundKeywords.Add(keyword);
                }
            }
            return foundKeywords;
        }
    }
}