using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using storage.Domain.Models;
using storage.Domain.Ports.Outbound;
using storage.Domain.Queries;

namespace storage.Persistence
{
    public class DatabaseAccess : IDatabaseAccess
    {
        private readonly StorageDbContext _storageDbContext;

        private readonly ILogger<DatabaseAccess> _logger;
        public DatabaseAccess(StorageDbContext storageDbContext, ILogger<DatabaseAccess> logger)
        {
            _logger = logger;
            _storageDbContext = storageDbContext;
        }
        public async Task<int> EnsureExistAsync(Source source)
        {
            // TODO handle unique constraint exception 
            var existing = await _storageDbContext.Sources.Where(s => s.Url == source.Url).SingleOrDefaultAsync();
            if (existing == null)
            {
                _storageDbContext.Sources.Add(source);
                await _storageDbContext.SaveChangesAsync();
                _logger.LogInformation($"stored the source {source.Url}");
                return source.SourceId;
            }
            else
            {
                _logger.LogInformation($"source {source.Url} already exist, returning id for existing one");
                return existing.SourceId;
            }
        }
        public async Task<int> EnsureExistAsync(Keyword keyword)
        {
            // TODO handle unique constraint exception 
            var existing = await _storageDbContext.Keywords.Where(w => w.Text == keyword.Text).SingleOrDefaultAsync();
            if (existing == null)
            {
                _storageDbContext.Keywords.Add(keyword);
                await _storageDbContext.SaveChangesAsync();
                _logger.LogInformation($"stored the keyword {keyword.Text}");
                return keyword.KeywordId;
            }
            else
            {
                _logger.LogInformation($"keyword {keyword.Text} already exist, returning id for existing one");
                return existing.KeywordId;
            }
        }
        public async Task<int> SaveSentenceAsync(Sentence sentence)
        {
            /* <Johan Thinks> 
            storage is a worker on a queue and only works with
            one message at a time. Even when scaled one message 
            only ever goes to one worker. 
            very unlikely that this will be a problem.. i.e the 
            read then write (anti)pattern.. in this scenario. 
            I could wrap this in a serializable transaction or 
            define this complex unique constraint 
            and handle violations... </Johan Thinks> */
            
            var existing = await _storageDbContext.Sentences.Where(
                s => s.Text == sentence.Text &&
                s.KeywordId == sentence.KeywordId &&
                s.SourceId == sentence.SourceId &&
                s.SourceArticleHeader == sentence.SourceArticleHeader &&
                s.SourceArticleUrl == sentence.SourceArticleUrl &&
                s.Received.Date == sentence.Received.Date).SingleOrDefaultAsync();

            if (existing == null)
            {
                _storageDbContext.Sentences.Add(sentence);
                await _storageDbContext.SaveChangesAsync();
                _logger.LogInformation($"stored the sentence {sentence.Text}");
                return sentence.SentenceId;
            }
            else
            {
                _logger.LogInformation($"sentence {sentence.Text} already exist for the same keyword, source, article and day");
                return existing.SentenceId;
            }

        }

        public async Task<List<Keyword>> GetAllKeywordsAsync()
        {
            return await _storageDbContext.Keywords.ToListAsync();
        }

        public async Task<List<DateTime>> GetAllYearMonthsAsync()
        {
            var DistinctYearMonths = await _storageDbContext.Sentences
            .Select(s => new { s.Received.Year, s.Received.Month })
            .Distinct()
            .OrderByDescending(s => s.Year)
            .OrderByDescending(s => s.Month)
            .ToListAsync();
            
            List<DateTime> yearMonths = DistinctYearMonths.Select(x => new DateTime(x.Year, x.Month, 1)).ToList();
            return yearMonths;
        }
        public async Task<List<Sentence>> GetSentencesAsync(GetSentencesQuery query)
        {
            var sentences = await _storageDbContext.Sentences.Where(s => s.Keyword.Text == query.Keyword
            && s.Received.Year == query.YearMonth.Year
            && s.Received.Month == query.YearMonth.Month)
            .Include(s => s.Keyword)
            .Include(s => s.Source)
            .OrderByDescending(s => s.Received)
            .ToListAsync();

            return sentences;
        }

    }
}