using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace tracker.Domain.Services
{
    public class Extractor : IExtractor
    {
        private readonly ISpider _spider;

        private readonly IValidator _validator;
        public Extractor(ISpider spider, IValidator validator)
        {
            _spider = spider;
            _validator = validator;
        }

        public string ExtractBodyTextFromArticleDocument(HtmlDocument articleHtmlDocument)
        {
            RemoveHeadersFromDocument(articleHtmlDocument);
            RemoveLinksFromDocument(articleHtmlDocument);
            RemoveUnorderedListsFromDocument(articleHtmlDocument);
            RemoveScriptsFromDocument(articleHtmlDocument);
            if (articleHtmlDocument?.DocumentNode?.OuterHtml == null)
            {
                return String.Empty;
            }
            var cleanedHtml = articleHtmlDocument.DocumentNode.OuterHtml;
            var htmlToTextConversion = _spider.HtmlToTextAsync(cleanedHtml);
            Task.WaitAll(htmlToTextConversion);

            if (htmlToTextConversion.IsCompletedSuccessfully)
            {
                var articleText = htmlToTextConversion.Result.Replace("\n", " ");
                var finalArticleText = RemoveNonBodyTextSentences(articleText);
                return finalArticleText;
            }
            else
            {
                throw new Exception($"could not convert the following html to text {cleanedHtml}");
            }
        }

        private void RemoveHeadersFromDocument(HtmlDocument articleHtmlDocument, int headerSize = 1)
        {
            _spider.RemoveNodesFromDocument(articleHtmlDocument, $"//h{headerSize}");
            headerSize++;
            if (headerSize <= 4)
                RemoveHeadersFromDocument(articleHtmlDocument, headerSize);
        }
        private void RemoveLinksFromDocument(HtmlDocument articleHtmlDocument)
        {
            _spider.RemoveNodesFromDocument(articleHtmlDocument, "//a[@href]");
        }
        private void RemoveUnorderedListsFromDocument(HtmlDocument articleHtmlDocument)
        {
            _spider.RemoveNodesFromDocument(articleHtmlDocument, "//ul");
        }
        private void RemoveScriptsFromDocument(HtmlDocument articleHtmlDocument)
        {
            _spider.RemoveNodesFromDocument(articleHtmlDocument, "//script");
        }
        private string RemoveNonBodyTextSentences(string articleText)
        {
            List<string> articleSentenceArray = articleText.Split(".").ToList();
            articleSentenceArray.RemoveAll(sentence => !_validator.ConsideredBodyText(sentence));
            return String.Join(".", articleSentenceArray.ToArray());
        }
    }
}