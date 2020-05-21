using storage.Domain.Models;

namespace src.Domain.Ports.Outbound
{
    public interface IPipeline
    {
        void SendToBot(AnalyzedSentence sentence);
        void Open();
        void Close();
    }
}