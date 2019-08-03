using System;

namespace storage.Domain.Queries
{
    public class GetSentencesQuery : IQuery
    {
        public string Keyword { get; set; }
        public DateTime YearMonth { get; set; }
    }
}