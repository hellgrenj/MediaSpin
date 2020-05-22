using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using src.Domain.Ports.Outbound;
using storage.Domain.Models;
using storage.Domain.Ports.Outbound;

namespace storage.Domain.Services
{
    public class StoreAnalyzedSentenceService : IStoreAnalyzedSentenceService
    {
        private readonly ILogger<StoreAnalyzedSentenceService> _logger;
        private readonly IDatabaseAccess _databaseAccess;

        private readonly IPipeline _pipeline;
        public StoreAnalyzedSentenceService(IDatabaseAccess databaseAccess, ILogger<StoreAnalyzedSentenceService> logger,
        IPipeline pipeline)
        {
            _databaseAccess = databaseAccess;
            _logger = logger;
            _pipeline = pipeline;
        }
        public async Task StoreAsync(AnalyzedSentence analyzedSentence)
        {
            var source = new Source() { Url = analyzedSentence.Source };
            var sourceId = await _databaseAccess.EnsureExistAsync(source);

            var keyword = new Keyword() { Text = analyzedSentence.Keyword };
            var keywordId = await _databaseAccess.EnsureExistAsync(keyword);

            var sentence = new Sentence()
            {
                KeywordId = keywordId,
                SourceId = sourceId,
                Text = analyzedSentence.Sentence,
                Positive = analyzedSentence.Positive,
                SourceArticleHeader = analyzedSentence.ArticleHeader,
                SourceArticleUrl = analyzedSentence.ArticleUrl,
                Received = DateTime.Now
            };
            var result = await _databaseAccess.SaveSentenceAsync(sentence);
            if (result.saved)
            {
                _pipeline.SendToBot(analyzedSentence);
            }

        }
    }
}