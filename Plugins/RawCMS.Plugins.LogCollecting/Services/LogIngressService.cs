﻿using RawCMS.Library.Schema;
using RawCMS.Library.Service;
using RawCMS.Plugins.FullText.Core;
using RawCMS.Plugins.LogCollecting.Controllers;
using RawCMS.Plugins.LogCollecting.Model;
using RawCMS.Plugins.LogCollecting.Models;
using System.Collections.Generic;
using System.Linq;

namespace RawCMS.Plugins.LogCollecting.Services
{
    public class LogService
    {
        private readonly FullTextService fullTextService;
        private readonly LogQueue logQueue;
        private readonly CRUDService crudService;

        private const int LOG_PROCESSING_SIZE = 100000;
        private const int PROCESSING_ENTRY_COUNT = 1000;

        public LogService(FullTextService fullTextService, CRUDService crudService)
        {
            this.fullTextService = fullTextService;
            this.crudService = crudService;
            this.logQueue = new LogQueue();
        }

        public void PersistLog(string applicationId, LogEntity data)
        {
            this.fullTextService.AddDocumentRaw(applicationId, data);
        }

        public List<LogStatistic> GetStatistic(string applicationId = null)
        {
            var applications = this.crudService.Query<Application>("application", new Library.DataModel.DataQuery
            {
                PageSize = int.MaxValue
            }).Items.ToList();

            var logs = this.logQueue.GetQueueStatistic(applicationId);

            return applications.Join(logs,
                x => x.PublicId.ToString(),
                y => y.ApplicationId,
                (x, y) => new LogStatistic
                {
                    ApplicationId = y.ApplicationId,
                    ApplicationName = x.Name,
                    Count = y.Count,
                    Time = y.Time
                }).ToList();
        }

        public void EnqueueLog(string applicationId, List<LogEntity> data)
        {
            data.ForEach(x =>
            {
                x.ApplicationId = applicationId;
                logQueue.Enqueue(x);
            }
                );

            logQueue.AppendLoadValue();
        }

        public void PersistLog()
        {
            var applications = this.crudService.Query<Application>("application", new Library.DataModel.DataQuery
            {
                PageSize = int.MaxValue
            })
                .Items
                .Where(x => x.PublicId != null).ToList();

            int processedLog = 0;
            List<LogEntity> batch;
            string indexname = "";
            while (processedLog < LOG_PROCESSING_SIZE && (batch = this.logQueue.Dequeue(PROCESSING_ENTRY_COUNT)).Count > 0)
            {
                var appsId = batch.Select(x => x.ApplicationId).Distinct();
                foreach (var appId in appsId)
                {
                    indexname = "log_" + applications.FirstOrDefault(x => x.PublicId.ToString().Equals(appId)).Id;
                    this.fullTextService.BulkAddDocument<LogEntity>(indexname, batch.Where(x => x.ApplicationId.Equals(appId)).ToList());
                }
            }
        }
    }
}