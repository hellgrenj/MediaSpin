using System;
using System.Collections.Generic;

namespace visualizer2.Model
{
    public class Sentence
    {
        public int SentenceId { get; set; }
        public int SourceId { get; set; }
        public int KeywordId { get; set; }
        public string Text { get; set; }
        public bool Positive { get; set; }
        public string SourceArticleUrl { get; set; }
        public string SourceArticleHeader { get; set; }
        public DateTime Received { get; set; }
        public Keyword Keyword { get; set; }
        public Source Source { get; set; }

    }
}