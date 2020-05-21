using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using src.Domain.Ports.Outbound;
using src.Infrastructure;
using storage.Domain.Ports.Inbound;
using storage.Domain.Ports.Outbound;
using storage.Domain.Services;
using storage.Infrastructure;
using storage.Persistence;

namespace storage
{
    public class Startup
    {

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();

            services.AddTransient<IMediator, Mediator>();
            services.AddTransient<IDatabaseAccess, DatabaseAccess>();
            services.AddTransient<IStoreAnalyzedSentenceService, StoreAnalyzedSentenceService>();
            services.AddTransient<IGetKeywordsService, GetKeywordsService>();
            services.AddSingleton<IPipeline, RabbitClient>();
            services.AddTransient<IGetYearMonthsService, GetYearMonthsService>();
            services.AddTransient<IGetSentencesService, GetSentencesService>();

            // postgres
            string user = Environment.GetEnvironmentVariable("POSTGRES_USER");
            string password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
            string db = Environment.GetEnvironmentVariable("POSTGRES_DB");
            var sqlConnectionString = $@"Server=postgres;Database={db};User Id={user};Password={password}";
            services.AddDbContext<StorageDbContext>(options => options.UseNpgsql(sqlConnectionString));
            // rabbit
            services.AddHostedService<RabbitEndpoint>();
            
        }
        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<StorageDbContext>())
                {
                    context.Database.Migrate();
                }
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Microsoft.Extensions.Hosting.IHostApplicationLifetime lifetime,
        IPipeline pipeline)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<StorageService>();
            });
            UpdateDatabase(app);
            pipeline.Open();
        }
    }
}
