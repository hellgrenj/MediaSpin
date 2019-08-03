using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using storage.Domain.Models;
using storage.Domain.Ports.Outbound;

namespace storage.Domain.Services
{
    public class StoreAnalyzedSentenceService : IStoreAnalyzedSentenceService
    {
        private readonly ILogger<StoreAnalyzedSentenceService> _logger;
        private readonly IDatabaseAccess _databaseAccess;
        public StoreAnalyzedSentenceService(IDatabaseAccess databaseAccess, ILogger<StoreAnalyzedSentenceService> logger)
        {
            _databaseAccess = databaseAccess;
            _logger = logger;
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
            await _databaseAccess.SaveSentenceAsync(sentence);
        }
    }
}