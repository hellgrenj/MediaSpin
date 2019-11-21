using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using storage.Domain.Models;
using storage.Domain.Ports.Outbound;

namespace storage.Domain.Services
{
    public class GetKeywordsService : IGetKeywordsService
    {
        private readonly ILogger<GetKeywordsService> _logger;
        private readonly IDatabaseAccess _databaseAccess;
        public GetKeywordsService(IDatabaseAccess databaseAccess, ILogger<GetKeywordsService> logger)
        {
            _databaseAccess = databaseAccess;
            _logger = logger;
        }
        public async Task<List<Keyword>> GetAsync()
        {
            _logger.LogInformation("fetching all keywords, DEMO DEMO");
            return await _databaseAccess.GetAllKeywordsAsync();
        }

    }
}