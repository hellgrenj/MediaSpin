using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using storage.Domain.Models;
using storage.Domain.Ports.Outbound;
using storage.Domain.Queries;

namespace storage.Domain.Services
{
    public class GetSentencesService : IGetSentencesService
    {
        private readonly ILogger<GetSentencesService> _logger;
        private readonly IDatabaseAccess _databaseAccess;
        public GetSentencesService(IDatabaseAccess databaseAccess, ILogger<GetSentencesService> logger)
        {
            _databaseAccess = databaseAccess;
            _logger = logger;
        }
        public async Task<List<Sentence>> GetAsync(GetSentencesQuery query)
        {
            _logger.LogInformation("fetching sentences by keyword and month");
            return await _databaseAccess.GetSentencesAsync(query);
        }

    }
}