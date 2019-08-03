using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using storage.Domain.Models;
using storage.Domain.Ports.Outbound;

namespace storage.Domain.Services
{
    public class GetYearMonthsService : IGetYearMonthsService
    {
        private readonly ILogger<GetYearMonthsService> _logger;
        private readonly IDatabaseAccess _databaseAccess;
        public GetYearMonthsService(IDatabaseAccess databaseAccess, ILogger<GetYearMonthsService> logger)
        {
            _databaseAccess = databaseAccess;
            _logger = logger;
        }
        public async Task<List<DateTime>> GetAsync()
        {
            _logger.LogInformation("fetching all year-month's");
            return await _databaseAccess.GetAllYearMonthsAsync();
        }

    }
}