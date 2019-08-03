using System.Threading.Tasks;
using storage.Domain.Models;

namespace storage.Domain.Services
{
    public interface IStoreAnalyzedSentenceService
    {
        Task StoreAsync(AnalyzedSentence analyzedSentence);

    }
}