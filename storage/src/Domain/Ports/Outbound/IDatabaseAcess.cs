using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using storage.Domain.Models;
using storage.Domain.Queries;

namespace storage.Domain.Ports.Outbound
{
    // lets start with a simple IDatabaseAccess .. and see what kind of 
    // abstraction we will need later on (if any). Perhaps a generic repository pattern or what have you..
    public interface IDatabaseAccess
    {
        Task<int> EnsureExistAsync(Source source);
        Task<int> EnsureExistAsync(Keyword keyword);
        Task<(int sentenceId, bool saved)> SaveSentenceAsync(Sentence sentence);
        Task<List<Keyword>> GetAllKeywordsAsync();
        Task<List<DateTime>> GetAllYearMonthsAsync();
        Task<List<Sentence>> GetSentencesAsync(GetSentencesQuery query);
    }
}