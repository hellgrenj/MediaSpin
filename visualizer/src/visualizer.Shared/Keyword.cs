using System.Collections.Generic;

namespace visualizer.Shared
{
    public class Keyword
    {
        public int KeywordId { get; set; }
        public string Text { get; set; }

        public List<Sentence> Sentences { get; set; }
    }
}