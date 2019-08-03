using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using storage.Domain.Models;
using storage.Domain.Ports.Inbound;
using storage.Domain.Queries;
using StorageEndpoint;

namespace storage.Infrastructure
{
    public class StorageService : Storage.StorageBase
    {
        private readonly IMediator _mediator;

        public StorageService(IMediator mediator)
        {
            _mediator = mediator;
        }
        public override async Task<GetSentencesResponse> GetSentences(GetSentencesRequest req, ServerCallContext context)
        {
            var query = new GetSentencesQuery();
            query.Keyword = req.Keyword;
            query.YearMonth = req.YearMonth.ToDateTime();
            var sentences = await _mediator.SendAsync<List<Sentence>>(query);
            var response = new GetSentencesResponse();
            foreach (var sentence in sentences)
            {
                var s = new StorageEndpoint.GetSentencesResponse.Types.Sentence()
                {
                    Keyword = new StorageEndpoint.GetSentencesResponse.Types.Sentence.Types.Keyword()
                    {
                        Text = sentence.Keyword.Text
                    },
                    Source = new StorageEndpoint.GetSentencesResponse.Types.Sentence.Types.Source()
                    {
                        Url = sentence.Source.Url
                    },
                    Positive = sentence.Positive,
                    Received = Timestamp.FromDateTime(sentence.Received.ToUniversalTime()),
                    Sourcearticleheader = sentence.SourceArticleHeader,
                    Sourcearticleurl = sentence.SourceArticleUrl,
                    Text = sentence.Text
                };


                response.Sentences.Add(s);
            }
            return response;

        }
        public override async Task<GetKeywordsResponse> GetKeywords(GetKeywordsRequest req, ServerCallContext context)
        {
            var keywords = await _mediator.SendAsync<List<Keyword>>(new GetAllKeywordsQuery());
            var response = new GetKeywordsResponse();
            foreach (var keyword in keywords)
            {
                response.Keywords.Add(keyword.Text);
            }
            return response;
        }

        public override async Task<GetYearMonthsResponse> GetYearMonths(GetYearMonthsRequest req, ServerCallContext context)
        {
            var yearMonths = await _mediator.SendAsync<List<DateTime>>(new GetAllYearMonthsQuery());
            var response = new GetYearMonthsResponse();
            foreach (var yearMonth in yearMonths)
            {
                response.Yearmonths.Add(Timestamp.FromDateTime(yearMonth.ToUniversalTime()));
            }
            return response;
        }
    }
}
