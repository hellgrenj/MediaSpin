namespace storage.Domain.Models
{
    public class AnalyzedSentence
    {
        public string Source { get; set; }
        public string ArticleHeader { get; set; }
        public string ArticleUrl { get; set; }
        public bool Positive { get; set; }
        public string Keyword { get; set; }
        public string Sentence { get; set; }
    }
}