﻿using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RawCMS.Library.Core;
using RawCMS.Library.Core.Attributes;
using RawCMS.Library.Core.Interfaces;
using RawCMS.Library.Service;
using RawCMS.Library.UI;
using RawCMS.Plugins.LogCollecting.Config;
using RawCMS.Plugins.LogCollecting.Jobs;
using RawCMS.Plugins.LogCollecting.Services;
using System;

namespace RawCMS.Plugins.LogCollecting
{
    [PluginInfo(4)]
    public class LogsPlugin : Library.Core.Extension.Plugin, IConfigurablePlugin<LogsPluginConfig>
    {
        public override string Name => "LogsCollecting";

        public override string Description => "";

        public override string Slug => "logs";

        public LogsPlugin(AppEngine appEngine, ILogger<LogsPlugin> logger) : base(appEngine, logger)
        { }

        public override void Configure(IApplicationBuilder app)
        {
            RecurringJob.AddOrUpdate("prova", () => Console.WriteLine("PROVA"), Cron.Minutely);
        }

        public override void ConfigureMvc(IMvcBuilder builder)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            //var sp = services.BuildServiceProvider();
            //var fs = sp.GetService<FullText.Core.ElasticFullTextService>();
            //var fs2 = sp.GetService<FullText.Core.FullTextService>();
            services.AddSingleton<LogService, LogService>();

            services.AddSingleton<LogCollectingIngestor, LogCollectingIngestor>((x) =>
            {
                var logService = x.GetService<LogService>();
                var crud = x.GetService<CRUDService>();
                return new LogCollectingIngestor(logService, crud);
            });
        }

        public override void Setup(IConfigurationRoot configuration)
        {
        }

        public override UIMetadata GetUIMetadata()
        {
            return new UIMetadata();
        }
    }
}