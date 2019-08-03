using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using storage.Domain.Models;

namespace storage.Domain.Services
{
    public interface IGetYearMonthsService
    {
        Task<List<DateTime>> GetAsync();

    }
}