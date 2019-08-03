using System.Threading.Tasks;
using src.Domain.Models.DataStructures;

namespace src.Domain.Services
{
    public interface IAFINN
    {
        void Init();
        Task<AFINNPrediction> Predict(string sentence);
    }
}
