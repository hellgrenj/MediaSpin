using HtmlAgilityPack;

namespace tracker.Domain.Services
{
    public interface IExtractor
    {
        string ExtractBodyTextFromArticleDocument(HtmlDocument articleHtmlDocument);
    }
}