using Quartz;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using tracker.Domain.Ports.Outbound;
using tracker.Domain.Services;
using tracker.Domain.Models;
using tracker.Domain.Ports.Inbound;

namespace tracker.Infrastructure.Quartz.Jobs
{
    [DisallowConcurrentExecution]
    public class TrackAllSources : IJob
    {
        // TODO: ugly static property injection, feels overkill tho with a Quartz factory for just construct DI for ONE job...?
        public static ITracker _tracker { get; set; }
        public static IArticlesFileWriter _articlesFileWriter { get; set; }
        public Task Execute(IJobExecutionContext context)
        {
            var defaultSources = @"https://www.svt.se/;https://www.unt.se/nyheter/;https://www.aftonbladet.se/";
            var sources = Environment.GetEnvironmentVariable("SOURCES") ?? defaultSources;
            Log.Information("starting fetch from all sources");
            var allArticles = new List<Article>();

            foreach (var source in sources.Split(";"))
            {
                _tracker.Scan(source, allArticles);
            }

            var writeArticlesToFile = Environment.GetEnvironmentVariable("WRITE_ARTICLES_TO_FILE") ?? "false";
            if (writeArticlesToFile == "true")
            {
                _articlesFileWriter.WriteToJSON(allArticles);
            }
            Log.Information("finished fetch from all sources");

            return Task.CompletedTask;
        }
    }
}