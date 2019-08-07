using System;
using System.Collections.Specialized;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using tracker.Domain.Ports.Outbound;
using tracker.Domain.Services;
using tracker.Infrastructure.Quartz.Jobs;

namespace tracker.Infrastructure.Quartz
{
    public class ScheduleService
    {
        private readonly IScheduler scheduler;
        public ScheduleService()
        {
            NameValueCollection props = new NameValueCollection
        {
            { "quartz.serializer.type", "binary" },
            { "quartz.scheduler.instanceName", "tracker_scheduler" },
            { "quartz.jobStore.type", "Quartz.Simpl.RAMJobStore, Quartz" },
            { "quartz.threadPool.threadCount", "1" }
        };
            StdSchedulerFactory factory = new StdSchedulerFactory(props);
            scheduler = factory.GetScheduler().ConfigureAwait(false).GetAwaiter().GetResult();
        }
        public void Start(ISpider spider, IPipeline pipeline, ILogger<Tracker> logger, IArticlesFileWriter fileWriter, IValidator validator, IExtractor extractor)
        {
            scheduler.Start().ConfigureAwait(false).GetAwaiter().GetResult();

            ScheduleJobs(spider, pipeline, logger, fileWriter, validator, extractor);
        }
        public void ScheduleJobs(ISpider spider, IPipeline pipeline, ILogger<Tracker> logger, IArticlesFileWriter fileWriter, IValidator validator, IExtractor extractor)
        {
            TrackAllSources._tracker = new Tracker(pipeline, spider, logger, validator, extractor);
            TrackAllSources._articlesFileWriter = fileWriter;
            IJobDetail job = JobBuilder.Create<TrackAllSources>()
                .WithIdentity("FetchFromAllSources", "default")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("EveryOtherHour", "default")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(3)
                    .RepeatForever())
                .Build();

            // Tell quartz to schedule the job using our trigger
            scheduler.ScheduleJob(job, trigger).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        public void Stop()
        {
            scheduler.Shutdown().ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}