using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using StorageEndpoint;
using visualizer.Shared;


namespace visualizer.Server.Services
{
    public class StorageClient : IStorageClient
    {
        public async Task<List<String>> GetAllKeywordsAsync()
        {
            var channel = new Channel("storage:7878", ChannelCredentials.Insecure);
            var client = new Storage.StorageClient(channel);

            var keywords = await client.GetKeywordsAsync(new GetKeywordsRequest { });
            await channel.ShutdownAsync();
            return keywords.Keywords.ToList<string>();
        }

        public async Task<IEnumerable<DateTime>> GetAllYearMonths()
        {
            var channel = new Channel("storage:7878", ChannelCredentials.Insecure);
            var client = new Storage.StorageClient(channel);

            var result = await client.GetYearMonthsAsync(new GetYearMonthsRequest { });
            await channel.ShutdownAsync();
            var yearMonths = new List<DateTime>();
            foreach (var timestamp in result.Yearmonths)
            {
                yearMonths.Add(timestamp.ToDateTime());
            }
            return yearMonths;
        }

        public async Task<List<Sentence>> GetSentencesAsync(string keyword, DateTime yearMonth)
        {
            var channel = new Channel("storage:7878", ChannelCredentials.Insecure);
            var client = new Storage.StorageClient(channel);
            
            var result = await client.GetSentencesAsync(new GetSentencesRequest { Keyword = keyword, YearMonth = Timestamp.FromDateTime(yearMonth.ToUniversalTime()) });
            await channel.ShutdownAsync();
            var yearMonths = new List<DateTime>();
            var sentences = new List<visualizer.Shared.Sentence>();
            foreach (var s in result.Sentences)
            {
                sentences.Add(new Sentence()
                {
                    Keyword = new Keyword()
                    {
                        Text = s.Keyword.Text
                    },
                    Source = new Source()
                    {
                        Url = s.Source.Url
                    },
                    Text = s.Text,
                    Positive = s.Positive,
                    Received = s.Received.ToDateTime(),
                    SourceArticleHeader = s.Sourcearticleheader,
                    SourceArticleUrl = s.Sourcearticleurl
                });
            }
            return sentences;
        }
    }
}