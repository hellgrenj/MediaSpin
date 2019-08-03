using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using storage.Domain.Models;
using storage.Domain.Queries;

namespace storage.Domain.Services
{
    public interface IGetSentencesService
    {
        Task<List<Sentence>> GetAsync(GetSentencesQuery query);

    }
}