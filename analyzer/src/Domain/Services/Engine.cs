using System;
using src.Domain.Models;
using Microsoft.Extensions.Logging;
using src.Domain.Ports.Outbound;
using Microsoft.AspNetCore.NodeServices;
using System.Threading.Tasks;
using src.Domain.Models.DataStructures;

namespace src.Domain.Services
{
    public class Engine : IEngine
    {
        private readonly ILogger _logger;
        private readonly IMLModel mainPredictor;
        private readonly IAFINN secondOpinionPredictor;
        private readonly IPipeline _pipeline;
        public Engine(ILogger<Engine> logger, IPipeline pipeline, IMLModel mlModel, IAFINN afinn)
        {
            mainPredictor = mlModel;
            secondOpinionPredictor = afinn;
            _logger = logger;
            _pipeline = pipeline;
        }
        public void Init()
        {
            secondOpinionPredictor.Init();
            mainPredictor.Init();
        }
        public void Analyze(Article article)
        {
            foreach (var sentence in article.Text.Split("."))
            {
                foreach (var keyword in article.Keywords)
                {
                    if (sentence.Contains(keyword))
                    {
                        _logger.LogDebug($"Found sentence containing keyword {keyword}");
                        var result = mainPredictor.Predict(sentence);
                        var secondOpinonTask = secondOpinionPredictor.Predict(sentence);
                        Task.WaitAll(secondOpinonTask);
                        
                        if (HasNegativeSentiment(sentence, result, secondOpinonTask.Result))
                        {
                            var analyzedSentence = CreateAnalyzedSentence(article, sentence, false, keyword);
                            SendForStorage(analyzedSentence);
                        }
                        else if (HasPositiveSentiment(sentence, result, secondOpinonTask.Result))
                        {
                            var analyzedSentence = CreateAnalyzedSentence(article, sentence, true, keyword);
                            SendForStorage(analyzedSentence);
                        }
                        else
                        {
                            _logger.LogDebug($"Sentence {sentence} considered neutral");
                        }
                    }
                }
            }
        }
        private bool HasNegativeSentiment(string sentence, PredictionResult result, AFINNPrediction secondOpinion)
        {

            if (Convert.ToBoolean(result.SentimentPrediction.Prediction) && result.SentimentPrediction.Probability > 0.97)
            {
                if (secondOpinion.Score < -2)
                {
                    return true; // both predictors agree on negative sentiment 
                }
                else
                {
                    _logger.LogInformation($"AFINN server disagrees with sentence {sentence} being negative");
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        private bool HasPositiveSentiment(string sentence, PredictionResult result, dynamic secondOpinion)
        {
            if (!Convert.ToBoolean(result.SentimentPrediction.Prediction) && result.SentimentPrediction.Probability < 0.02)
            {
                if (secondOpinion.Score > 2)
                {
                    return true; // both predictors agree on positive sentiment 
                }
                else
                {
                    _logger.LogInformation($"AFINN server disagrees with sentence {sentence} being positive");
                    return false;
                }
            }
            else
            {
                return false;
            }

        }
        private AnalyzedSentence CreateAnalyzedSentence(Article article, string sentence, bool positive, string foundKeyword)
        {
            var analyzedSentence = new AnalyzedSentence()
            {
                Source = article.Source,
                ArticleHeader = article.Header,
                ArticleUrl = article.ArticleUrl,
                Positive = positive,
                Keyword = foundKeyword,
                Sentence = sentence
            };
            return analyzedSentence;
        }
        private void SendForStorage(AnalyzedSentence analyzedSentence)
        {
            _logger.LogInformation($"sending the following sentence for storage: {analyzedSentence.Sentence}");
            _pipeline.SendForStorage(analyzedSentence);
        }
    }
}