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
            foreach (var keyword in keywords)
            {
                Console.WriteLine(keyword);
            }

            return keywords;
        }

        [HttpGet("[action]")]
        public async Task<List<String>> AllYearMonths()
        {

            var yearMonths = await _storageClient.GetAllYearMonths();
            var yearMonthStrings = new List<string>();
            foreach (var yearMonth in yearMonths)
            {
                Console.WriteLine(yearMonth.ToShortDateString());
                var month = yearMonth.Month < 10 ? $"0{yearMonth.Month}" : yearMonth.Month.ToString();
                yearMonthStrings.Add($"{yearMonth.Year}-{month}");
            }

            return yearMonthStrings;
        }

        [HttpPost("[action]")]
        public async Task<List<Sentence>> Sentences([FromBody]SentencesReq query)
        {

            // FluentValidation for asp.net core 3 out yet?
            if (query.Date == null || query.Keyword == null || query.Keyword == string.Empty)
            {
                return new List<Sentence>();
            }
            var sentences = await _storageClient.GetSentencesAsync(query.Keyword, query.Date);
            return sentences;
        }
    }
}
