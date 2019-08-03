using Microsoft.Extensions.Logging;
using Microsoft.ML;
using src.Domain.Models;
using src.Domain.Models.DataStructures;

namespace src.Domain.Services
{
    public class MLModel : IMLModel
    {
        private MLContext _mlContext { get; set; } = new MLContext();
        private ITransformer _trainedModel { get; set; }

        private readonly ILogger _logger;

        public void Init()
        {
            // alias for triggering construction (used as singleton) and therefore loading pre-trained model
        }
        public MLModel(ILogger<MLModel> logger)
        {
            _logger = logger;
            DataViewSchema modelSchema;
            if (_trainedModel == null)
            {
                _logger.LogInformation("Loading pre-trained model");
                _trainedModel = _mlContext.Model.Load("./MLModel/SentimentModel.zip", out modelSchema);
            }
        }
        public PredictionResult Predict(string sentence)
        {
            var sentimentIssue = new SentimentIssue { Text = sentence };
            var predEngine = _mlContext.Model.CreatePredictionEngine<SentimentIssue, SentimentPrediction>(_trainedModel);
            var resultprediction = predEngine.Predict(sentimentIssue);
            return new PredictionResult() { SentimentPrediction = resultprediction, Sentence = sentence };
        }
    }
}