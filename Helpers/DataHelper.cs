﻿using AzureADXNETCoreWebApp.Models;
using Kusto.Data;
using Kusto.Data.Common;
using Kusto.Data.Net.Client;
using Kusto.Ingest;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;

namespace AzureADXNETCoreWebApp.Helpers
{
    public class DataHelper : IDataHelper
    {
        private readonly ProjectOptions _options;
        private IHttpContextAccessor _httpContextAccessor;

        public DataHelper(IOptions<ProjectOptions> options, IHttpContextAccessor httpContextAccessor)
        {
            _options = options.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<StormEvent>> GetStormEvents(string userId, string searchText = null)
        {
            List<StormEvent> stormEvents = new List<StormEvent>();

            try
            {

                var userstates = await GetUserStates(userId);

                var kcsb = new KustoConnectionStringBuilder(_options.ADXCluster, _options.ADXDatabase)
                    .WithAadUserPromptAuthentication();

                using (var queryProvider = KustoClientFactory.CreateCslQueryProvider(kcsb))
                {
                    var query = "StormEvents| extend i = ingestion_time() | join(StormEvents | summarize i = max(ingestion_time()) by EventId) on $left.EventId == $right.EventId and $left.i ==$right.i | sort by StartTime desc | take 100 | where isnotnull(EventId)";

                    if (userstates != "")
                    {
                        query += " and State in (" + userstates + ") ";
                    }

                    if (searchText != null)
                    {
                        query += " and * has '" + searchText + "'";
                    }

                    // It is strongly recommended that each request has its own unique
                    // request identifier. This is mandatory for some scenarios (such as cancelling queries)
                    // and will make troubleshooting easier in others.
                    var clientRequestProperties = new ClientRequestProperties() { ClientRequestId = Guid.NewGuid().ToString() };
                    using (var reader = queryProvider.ExecuteQuery(query, clientRequestProperties))
                    {
                        while (reader.Read())
                        {
                            StormEvent se = ReflectPropertyInfo.ReflectType<StormEvent>(reader);
                            stormEvents.Add(se);
                        }
                    }
                }
            }
            catch
            {
            }
            return stormEvents;

        }

        public async Task<StormEvent> GetStormEvent(string userId, int eventId)
        {
            StormEvent se = null;
            try
            {
                var userstates = await GetUserStates(userId);

                var kcsb = new KustoConnectionStringBuilder(_options.ADXCluster, _options.ADXDatabase)
                    .WithAadUserPromptAuthentication();

                using (var queryProvider = KustoClientFactory.CreateCslQueryProvider(kcsb))
                {
                    var query = "StormEvents| extend i = ingestion_time() | join(StormEvents | summarize i = max(ingestion_time()) by EventId) on $left.EventId == $right.EventId and $left.i ==$right.i | where EventId ==\"" + eventId + "\"";

                    if (userstates != "")
                    {
                        query += " and State in (" + userstates + ") ";
                    }

                    // It is strongly recommended that each request has its own unique
                    // request identifier. This is mandatory for some scenarios (such as cancelling queries)
                    // and will make troubleshooting easier in others.
                    var clientRequestProperties = new ClientRequestProperties() { ClientRequestId = Guid.NewGuid().ToString() };
                    using (var reader = queryProvider.ExecuteQuery(query, clientRequestProperties))
                    {
                        while (reader.Read())
                        {
                            se = ReflectPropertyInfo.ReflectType<StormEvent>(reader);
                        }
                    }
                }
            }
            catch
            {
            }
            return se;
        }

        public async Task<bool> UpdateStormEvent(string update)
        {
            try
            {
                var kcsb = new KustoConnectionStringBuilder(_options.ADXCluster, _options.ADXDatabase)
                    .WithAadUserPromptAuthentication();

                using (var queryProvider = KustoIngestFactory.CreateDirectIngestClient(kcsb))
                {
                    // Ingest from a file according to the required properties
                    var kustoIngestionProperties = new KustoQueuedIngestionProperties(databaseName: _options.ADXDatabase, tableName: _options.ADXTable)
                    {
                        // Setting the report level to FailuresAndSuccesses will cause both successful and failed ingestions to be reported
                        // (Rather than the default "FailuresOnly" level - which is demonstrated in the
                        // 'Ingest From Local File(s) using KustoQueuedIngestClient and Ingestion Validation' section)
                        ReportLevel = IngestionReportLevel.FailuresAndSuccesses,
                        // Choose the report method of choice. 'Queue' is the default method.
                        // For the sake of the example, we will choose it anyway. 
                        ReportMethod = IngestionReportMethod.Queue,
                        Format = DataSourceFormat.json
                    };

                    StreamDescription sd = new StreamDescription
                    {
                        SourceId = Guid.NewGuid(),
                        Stream = GenericHelper.GenerateStreamFromString(update)
                    };

                    await queryProvider.IngestFromStreamAsync(sd, kustoIngestionProperties);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GetUserStates(string userId)
        {
            // Get the user states
            if (string.IsNullOrEmpty(_httpContextAccessor.HttpContext.Session.GetString("userstates")))
            {
                // REST API to get the user states
                if (_options.APIURL != "")
                {
                    string url = _options.APIURL + "&oid=" + userId;

                    using (HttpClient client = new HttpClient())
                    {
                        using (HttpResponseMessage res = await client.GetAsync(url))
                        {
                            using (HttpContent content = res.Content)
                            {
                                string data = await content.ReadAsStringAsync();
                                if (data != null)
                                {
                                    var currentuserstates = data.Replace("\"", "'").Replace("[", "").Replace("]", "");
                                    _httpContextAccessor.HttpContext.Session.SetString("userstates", currentuserstates);
                                }
                            }
                        }
                    }
                }
                else
                {
                    _httpContextAccessor.HttpContext.Session.SetString("userstates", "");
                }
            }
            return _httpContextAccessor.HttpContext.Session.GetString("userstates");
        }
    }
}
