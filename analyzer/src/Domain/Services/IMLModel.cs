using src.Domain.Models;

namespace src.Domain.Services
{
    public interface IMLModel
    {
        void Init();
        PredictionResult Predict(string sentence);
    }
}