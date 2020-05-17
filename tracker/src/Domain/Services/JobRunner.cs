using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using tracker.Domain.Models;
using tracker.Domain.Ports.Inbound;
using tracker.Domain.Ports.Outbound;

internal class JobRunner 
{
    private int executionCount = 0;
    private readonly ILogger<JobRunner> _logger;
    private Timer _timer;

    private ITracker _tracker;

    private IArticlesFileWriter _articlesFileWriter;

    public JobRunner(ILogger<JobRunner> logger, ITracker tracker, IArticlesFileWriter articlesFileWriter)
    {
        _logger = logger;
        _tracker = tracker;
        _articlesFileWriter = articlesFileWriter;
    }

    public Task StartAsync()
    {
        _logger.LogInformation("JobRunner started");
        _timer = new Timer(TrackAllSources, null, TimeSpan.Zero, 
            TimeSpan.FromHours(3));

        return Task.CompletedTask;
    }

    private void TrackAllSources(object state)
    {
        var count = Interlocked.Increment(ref executionCount);
         _logger.LogInformation(
            "JobRunner:TrackAllSources is running. Count: {Count}", count);

       var defaultSources = @"https://www.svt.se/;https://www.unt.se/nyheter/;https://www.aftonbladet.se/";
            var sources = Environment.GetEnvironmentVariable("SOURCES") ?? defaultSources;
            _logger.LogInformation("starting fetch from all sources");
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
             _logger.LogInformation("finished fetch from all sources");
    }
}