using storage.Domain.Models;

namespace storage.Domain.Commands
{
    public class StoreAnalyzedSentenceCommand : ICommand
    {
        public AnalyzedSentence AnalyzedSentence { get; set; }
    }
}