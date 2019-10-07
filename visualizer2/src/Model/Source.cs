using System.Collections.Generic;

namespace visualizer2.Model
{
    public class Source
    {
        public int SourceId { get; set; }
        public string Url { get; set; }

        public List<Sentence> Sentences { get; set; }

    }
}