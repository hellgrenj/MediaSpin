using System.Collections.Generic;

namespace storage.Domain.Models
{
    public class Keyword
    {
        public int KeywordId { get; set; }
        public string Text { get; set; }
        public List<Sentence> Sentences { get; set; }
    }
}