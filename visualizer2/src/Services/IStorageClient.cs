using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using visualizer2.Model;

namespace visualizer2.Services
{
    public interface IStorageClient {
        Task<List<String>> GetAllKeywordsAsync();
        Task<IEnumerable<DateTime>> GetAllYearMonths();

        Task<List<Sentence>> GetSentencesAsync(string keyword, DateTime yearMonth);
    }
}