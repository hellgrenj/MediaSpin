using System.Threading.Tasks;
using HtmlAgilityPack;

namespace tracker.Domain.Services
{
    public interface ISpider
    {
        HtmlDocument LoadPage(string url);
        HtmlNodeCollection GetHeadersOfSize(HtmlDocument htmlDoc, int headerSize);

        void RemoveNodesFromDocument(HtmlDocument document, string expression);
        Task<string> HtmlToTextAsync(string html);

        (string articleUrl, HtmlDocument article) DownloadArticleByHeader(string baseUrl, HtmlNode node);
    }
}