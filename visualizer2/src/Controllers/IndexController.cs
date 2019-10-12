using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using visualizer2.Model;
using visualizer2.Services;

namespace visualizer2.Controllers
{
    [Route("api/[controller]")]
    public class IndexController : Controller
    {
        private readonly IStorageClient _storageClient;
        public IndexController(IStorageClient storageClient)
        {
            _storageClient = storageClient;
        }
        [HttpGet("[action]")]
        public async Task<List<String>> AllKeywords()
        {
            var keywords = await _storageClient.GetAllKeywordsAsync();
            return keywords;
        }

        [HttpGet("[action]")]
        public async Task<List<String>> AllYearMonths()
        {

            var yearMonths = await _storageClient.GetAllYearMonths();
            var yearMonthStrings = new List<string>();
            foreach (var yearMonth in yearMonths)
            {
                var month = yearMonth.Month < 10 ? $"0{yearMonth.Month}" : yearMonth.Month.ToString();
                yearMonthStrings.Add($"{yearMonth.Year}-{month}");
            }

            return yearMonthStrings;
        }

        [HttpPost("[action]")]
        public async Task<List<Sentence>> Sentences([FromBody]SentencesReq query)
        {
            if (string.IsNullOrEmpty(query.Date) || string.IsNullOrEmpty(query.Keyword))
            {
                return new List<Sentence>();
            }
            var sentences = await _storageClient.GetSentencesAsync(query.Keyword, DateTime.Parse($"{query.Date} 23:00"));
            return sentences;
        }
    }
}
