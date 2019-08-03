
using src.Domain.Models;

namespace src.Domain.Ports.Outbound
{
    public interface IPipeline
    {
        void SendForStorage(AnalyzedSentence AnalyzedSentence);
        void Open();
        void Close();
    }
}