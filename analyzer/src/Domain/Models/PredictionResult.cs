using src.Domain.Models.DataStructures;

namespace src.Domain.Models
{
    public class PredictionResult
    {
        public SentimentPrediction SentimentPrediction { get; set; }
        public string Sentence { get; set; }
    }
}