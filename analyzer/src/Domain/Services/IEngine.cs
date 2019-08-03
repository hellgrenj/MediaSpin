using src.Domain.Models;

namespace src.Domain.Services
{
    public interface IEngine
    {
        void Analyze(Article article);
        void Init();
    }
}