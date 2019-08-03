using System.Collections.Generic;

namespace src.Domain.Models
{
    public class Article
    {
        public string Source { get; set; }
        public string Text { get; set; }
        public string Header { get; set; }
        public string ArticleUrl { get; set; }
        public List<string> Keywords { get; set; } = new List<string>();
    }
}