using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using storage.Domain.Commands;
using storage.Domain.Ports.Inbound;
using storage.Domain.Queries;

namespace storage.Domain.Services
{
    public class Mediator : IMediator
    {
        private readonly ILogger<Mediator> _logger;
        private readonly IStoreAnalyzedSentenceService _storeAnalyzedSentenceService;
        private readonly IGetKeywordsService _getKeywordsService;
        private readonly IGetYearMonthsService _getYearMonthsService;
        private readonly IGetSentencesService _getSentencesService;

        public Mediator(ILogger<Mediator> logger, 
        IStoreAnalyzedSentenceService storedAnalyzedSentenceService,
        IGetKeywordsService getKeywordsService, 
        IGetYearMonthsService getYearMonthsService, 
        IGetSentencesService getSentencesService)
        {
            _logger = logger;
            _storeAnalyzedSentenceService = storedAnalyzedSentenceService;
            _getKeywordsService = getKeywordsService;
            _getYearMonthsService = getYearMonthsService;
            _getSentencesService = getSentencesService;
        }

        public async Task SendAsync(ICommand command)
        {
            switch (command)
            {
                case StoreAnalyzedSentenceCommand storeAnalyzedSentenceCommand:
                    _logger.LogInformation($"Received StoreAnalyzedSentenceCommand for the sentence {storeAnalyzedSentenceCommand.AnalyzedSentence.Sentence}");
                    await _storeAnalyzedSentenceService.StoreAsync(storeAnalyzedSentenceCommand.AnalyzedSentence);
                    break;
                default:
                    _logger.LogError($"found no service for this command");
                    break;
            }
        }

        public async Task<TResponse> SendAsync<TResponse>(IQuery query)
        {
            switch (query)
            {
                case GetSentencesQuery getSentencesQuery:
                    _logger.LogInformation($"Received GetSentencesQuery");
                    var sentences = await _getSentencesService.GetAsync(getSentencesQuery);
                    return (TResponse)Convert.ChangeType(sentences, typeof(TResponse));

                case GetAllKeywordsQuery getAllKeywordQuery:
                    _logger.LogInformation($"Received GetAllKeywordsQuery");
                    var keywords = await _getKeywordsService.GetAsync();
                    return (TResponse)Convert.ChangeType(keywords, typeof(TResponse));

                case GetAllYearMonthsQuery getAllYearMonthsQuery:
                    _logger.LogInformation($"Received GetAllYearMonths");
                    var yearMonths = await _getYearMonthsService.GetAsync();
                    return (TResponse)Convert.ChangeType(yearMonths, typeof(TResponse));

                default:
                    _logger.LogError("found no service for command");
                    throw new NotImplementedException("found no service for this command");
            }
        }
    }
}