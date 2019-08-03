namespace tracker.Domain.Services
{
    public interface IValidator
    {
        bool ConsideredArticleHeader(string header);
        bool ConsideredBodyText(string sentence);
    }
}