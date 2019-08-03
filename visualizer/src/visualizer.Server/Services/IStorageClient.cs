using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using visualizer.Shared;

namespace visualizer.Server.Services
{
    public interface IStorageClient {
        Task<List<String>> GetAllKeywordsAsync();
        Task<IEnumerable<DateTime>> GetAllYearMonths();

        Task<List<Sentence>> GetSentencesAsync(string keyword, DateTime yearMonth);
    }
}