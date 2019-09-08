using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using src.Domain.Models;
using src.Domain.Models.DataStructures;
using src.Domain.Ports.Outbound;
using src.Domain.Services;
using Xunit;

namespace unittests.Domain.Services
{
    public class EngineTests
    {

        [Fact]
        public void Init_initializes_both_ml_model_and_afinn()
        {
            var afinnMock = new Mock<IAFINN>();
            var mlModelMock = new Mock<IMLModel>();
            var pipelineMock = new Mock<IPipeline>();
            var loggerMock = new Mock<ILogger<Engine>>();
            var engine = new Engine(loggerMock.Object, pipelineMock.Object, mlModelMock.Object, afinnMock.Object);
            engine.Init();
            afinnMock.Verify(afinn => afinn.Init(), Times.Exactly(1));
            mlModelMock.Verify(ml => ml.Init(), Times.Exactly(1));
        }

        [Fact]
        public void Analyze_doesnt_analyze_sentences_that_doesnt_contain_keyword()
        {
            var afinnMock = new Mock<IAFINN>();
            afinnMock.Setup(afinn => afinn.Predict(It.IsAny<string>())).ReturnsAsync(new AFINNPrediction(){ Score = -1 });
            var mlModelMock = new Mock<IMLModel>();
            var neutralPrediction = new PredictionResult();
            neutralPrediction.Sentence = "doesnt matter for this test";
            neutralPrediction.SentimentPrediction = new SentimentPrediction() { Prediction = true, Probability = 0.550F, Score = 0 };
            mlModelMock.Setup(ml => ml.Predict(It.IsAny<string>()))
            .Returns(neutralPrediction);
            var pipelineMock = new Mock<IPipeline>();
            var loggerMock = new Mock<ILogger<Engine>>();
            var engine = new Engine(loggerMock.Object, pipelineMock.Object, mlModelMock.Object, afinnMock.Object);

            var testArticle = new Article();
            testArticle.ArticleUrl = "http://madeupnews.com/article-one";
            testArticle.Header = "Article number one";
            testArticle.Keywords = new List<string>() { "magicKeyword" };
            testArticle.Source = "http://madeupnews.com";
            testArticle.Text = @"This is one sentence. This is another sentence but non of these two contains
            the keyword.";

            engine.Analyze(testArticle);
            afinnMock.Verify(a => a.Predict(It.IsAny<string>()), Times.Exactly(0));
            mlModelMock.Verify(m => m.Predict(It.IsAny<string>()), Times.Exactly(0));
            pipelineMock.Verify(p => p.SendForStorage(It.IsAny<AnalyzedSentence>()), Times.Exactly(0));
        }

        [Fact]
        public void Analyze_doesnt_send_sentence_for_storage_if_ml_model_predicts_neutral()
        {
            var afinnMock = new Mock<IAFINN>();
            afinnMock.Setup(afinn => afinn.Predict(It.IsAny<string>())).ReturnsAsync(new AFINNPrediction(){ Score = -1 });
            var mlModelMock = new Mock<IMLModel>();
            var neutralPrediction = new PredictionResult();
            neutralPrediction.Sentence = "doesnt matter for this test";
            neutralPrediction.SentimentPrediction = new SentimentPrediction() { Prediction = true, Probability = 0.550F, Score = 0 };
            mlModelMock.Setup(ml => ml.Predict(It.IsAny<string>()))
            .Returns(neutralPrediction);
            var pipelineMock = new Mock<IPipeline>();
            var loggerMock = new Mock<ILogger<Engine>>();
            var engine = new Engine(loggerMock.Object, pipelineMock.Object, mlModelMock.Object, afinnMock.Object);

            var testArticle = new Article();
            testArticle.ArticleUrl = "http://madeupnews.com/article-one";
            testArticle.Header = "Article number one";
            testArticle.Keywords = new List<string>() { "magicKeyword" };
            testArticle.Source = "http://madeupnews.com";
            testArticle.Text = @"This is one sentence. This is another sentence but non of the first two contains
            the keyword. This, third sentence, however does contain magicKeyword, which is what we are looking for";

            engine.Analyze(testArticle);
            afinnMock.Verify(a => a.Predict(It.IsAny<string>()), Times.Exactly(1));
            mlModelMock.Verify(m => m.Predict(It.IsAny<string>()), Times.Exactly(1));
            pipelineMock.Verify(p => p.SendForStorage(It.IsAny<AnalyzedSentence>()), Times.Exactly(0));
        }

        [Fact]
        public void Analyze_sends_sentence_for_storage_if_ml_model_and_afinn_agrees_on_negative()
        {
            var afinnMock = new Mock<IAFINN>();
            afinnMock.Setup(afinn => afinn.Predict(It.IsAny<string>())).ReturnsAsync(new AFINNPrediction(){ Score = -2 });
            var mlModelMock = new Mock<IMLModel>();
            var neutralPrediction = new PredictionResult();
            neutralPrediction.Sentence = "doesnt matter for this test";
            neutralPrediction.SentimentPrediction = new SentimentPrediction() { Prediction = true, Probability = 0.99F, Score = 0 };
            mlModelMock.Setup(ml => ml.Predict(It.IsAny<string>()))
            .Returns(neutralPrediction);
            var pipelineMock = new Mock<IPipeline>();
            var loggerMock = new Mock<ILogger<Engine>>();
            var engine = new Engine(loggerMock.Object, pipelineMock.Object, mlModelMock.Object, afinnMock.Object);

            var testArticle = new Article();
            testArticle.ArticleUrl = "http://madeupnews.com/article-one";
            testArticle.Header = "Article number one";
            testArticle.Keywords = new List<string>() { "magicKeyword" };
            testArticle.Source = "http://madeupnews.com";
            testArticle.Text = @"This is one sentence. This is another sentence but non of the first two contains
            the keyword. This, third sentence, however does contain magicKeyword, which is what we are looking for";

            engine.Analyze(testArticle);
            afinnMock.Verify(a => a.Predict(It.IsAny<string>()), Times.Exactly(1));
            mlModelMock.Verify(m => m.Predict(It.IsAny<string>()), Times.Exactly(1));
            pipelineMock.Verify(p => p.SendForStorage(It.IsAny<AnalyzedSentence>()), Times.Exactly(1));
        }

        [Fact]
        public void Analyze_sends_sentence_for_storage_if_ml_model_and_afinn_agrees_on_positive()
        {
            var afinnMock = new Mock<IAFINN>();
            afinnMock.Setup(afinn => afinn.Predict(It.IsAny<string>())).ReturnsAsync(new AFINNPrediction(){ Score = 3 });
            var mlModelMock = new Mock<IMLModel>();
            var neutralPrediction = new PredictionResult();
            neutralPrediction.Sentence = "doesnt matter for this test";
            neutralPrediction.SentimentPrediction = new SentimentPrediction() { Prediction = false, Probability = 0.005F, Score = 0 };
            mlModelMock.Setup(ml => ml.Predict(It.IsAny<string>()))
            .Returns(neutralPrediction);
            var pipelineMock = new Mock<IPipeline>();
            var loggerMock = new Mock<ILogger<Engine>>();
            var engine = new Engine(loggerMock.Object, pipelineMock.Object, mlModelMock.Object, afinnMock.Object);

            var testArticle = new Article();
            testArticle.ArticleUrl = "http://madeupnews.com/article-one";
            testArticle.Header = "Article number one";
            testArticle.Keywords = new List<string>() { "magicKeyword" };
            testArticle.Source = "http://madeupnews.com";
            testArticle.Text = @"This is one sentence. This is another sentence but non of the first two contains
            the keyword. This, third sentence, however does contain magicKeyword, which is what we are looking for";

            engine.Analyze(testArticle);
            afinnMock.Verify(a => a.Predict(It.IsAny<string>()), Times.Exactly(1));
            mlModelMock.Verify(m => m.Predict(It.IsAny<string>()), Times.Exactly(1));
            pipelineMock.Verify(p => p.SendForStorage(It.IsAny<AnalyzedSentence>()), Times.Exactly(1));
        }

        [Fact]
        public void Analyze_doesnt_send_sentence_for_storage_if_ml_model_predicts_positive_but_afinn_disagrees()
        {
            var afinnMock = new Mock<IAFINN>();
            // predicts negative
            afinnMock.Setup(afinn => afinn.Predict(It.IsAny<string>())).ReturnsAsync(new AFINNPrediction(){ Score = -3 });
            var mlModelMock = new Mock<IMLModel>();
            var neutralPrediction = new PredictionResult();
            neutralPrediction.Sentence = "doesnt matter for this test";
            // predicts positive
            neutralPrediction.SentimentPrediction = new SentimentPrediction() { Prediction = false, Probability = 0.005F, Score = 0 };
            mlModelMock.Setup(ml => ml.Predict(It.IsAny<string>()))
            .Returns(neutralPrediction);
            var pipelineMock = new Mock<IPipeline>();
            var loggerMock = new Mock<ILogger<Engine>>();
            var engine = new Engine(loggerMock.Object, pipelineMock.Object, mlModelMock.Object, afinnMock.Object);

            var testArticle = new Article();
            testArticle.ArticleUrl = "http://madeupnews.com/article-one";
            testArticle.Header = "Article number one";
            testArticle.Keywords = new List<string>() { "magicKeyword" };
            testArticle.Source = "http://madeupnews.com";
            testArticle.Text = @"This is one sentence. This is another sentence but non of the first two contains
            the keyword. This, third sentence, however does contain magicKeyword, which is what we are looking for";

            engine.Analyze(testArticle);
            afinnMock.Verify(a => a.Predict(It.IsAny<string>()), Times.Exactly(1));
            mlModelMock.Verify(m => m.Predict(It.IsAny<string>()), Times.Exactly(1));
            pipelineMock.Verify(p => p.SendForStorage(It.IsAny<AnalyzedSentence>()), Times.Exactly(0));
        }

        [Fact]
        public void Analyze_doesnt_send_sentence_for_storage_if_ml_model_predicts_negative_but_afinn_disagrees()
        {
            var afinnMock = new Mock<IAFINN>();
            // predicts positive
            afinnMock.Setup(afinn => afinn.Predict(It.IsAny<string>())).ReturnsAsync(new AFINNPrediction(){ Score = 3 });
            var mlModelMock = new Mock<IMLModel>();
            var neutralPrediction = new PredictionResult();
            neutralPrediction.Sentence = "doesnt matter for this test";
            // predicts negative
            neutralPrediction.SentimentPrediction = new SentimentPrediction() { Prediction = true, Probability = 0.99F, Score = 0 };
            mlModelMock.Setup(ml => ml.Predict(It.IsAny<string>()))
            .Returns(neutralPrediction);
            var pipelineMock = new Mock<IPipeline>();
            var loggerMock = new Mock<ILogger<Engine>>();
            var engine = new Engine(loggerMock.Object, pipelineMock.Object, mlModelMock.Object, afinnMock.Object);

            var testArticle = new Article();
            testArticle.ArticleUrl = "http://madeupnews.com/article-one";
            testArticle.Header = "Article number one";
            testArticle.Keywords = new List<string>() { "magicKeyword" };
            testArticle.Source = "http://madeupnews.com";
            testArticle.Text = @"This is one sentence. This is another sentence but non of the first two contains
            the keyword. This, third sentence, however does contain magicKeyword, which is what we are looking for";

            engine.Analyze(testArticle);
            afinnMock.Verify(a => a.Predict(It.IsAny<string>()), Times.Exactly(1));
            mlModelMock.Verify(m => m.Predict(It.IsAny<string>()), Times.Exactly(1));
            pipelineMock.Verify(p => p.SendForStorage(It.IsAny<AnalyzedSentence>()), Times.Exactly(0));
        }


    }
}
