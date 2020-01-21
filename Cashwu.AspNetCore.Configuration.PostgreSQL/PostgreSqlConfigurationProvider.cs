using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Cashwu.AspNetCore.Configuration.PostgreSQL
{
    internal class PostgreSqlConfigurationProvider : ConfigurationProvider, IDisposable
    {
        private static readonly object LockObject = new object();
        private static DateTime _lastRequested;
        private readonly DbContextOptions<ConfigurationContext> _dbOptions;
        private readonly PostgreSqlConfigurationOptions _efOptions;
        private readonly CancellationTokenSource _cancellationToken;
        private Task _backgroundWorker;

        public PostgreSqlConfigurationProvider(DbContextOptions<ConfigurationContext> dbOptions, PostgreSqlConfigurationOptions efOptions)
        {
            _cancellationToken = new CancellationTokenSource();
            _dbOptions = dbOptions;
            _efOptions = efOptions;
        }

        public override async void Load()
        {
            using (var dbContext = new ConfigurationContext(_dbOptions))
            {
                _lastRequested = DateTime.UtcNow;
                Data = await GetFromDatabase(dbContext);
            }
            
            _backgroundWorker = Task.Run(async () =>
            {
                while (true)
                {
                    if (_cancellationToken.IsCancellationRequested)
                    {
                       break; 
                    }
                    
                    if (HasChanged)
                    {
                        await UpdateFromDatabase();
                    }

                    await Task.Delay(_efOptions.PollingInterval);
                }
            });
        }

        private bool HasChanged
        {
            get
            {
                try
                {
                    using (var context = new ConfigurationContext(_dbOptions))
                    {
                        var now = DateTime.UtcNow;

                        var lastUpdated = context.ConfigurationValue
                                                 .Where(c => c.LastUpdated <= now)
                                                 .OrderByDescending(v => v.LastUpdated)
                                                 .Select(v => v.LastUpdated)
                                                 .FirstOrDefault();

                        var hasChanged = lastUpdated > _lastRequested;

                        _lastRequested = lastUpdated;

                        return hasChanged;
                    }
                }
                catch (NpgsqlException)
                {
                    return false;
                }
            }
        }

        private async Task UpdateFromDatabase()
        {
            using (var dbContext = new ConfigurationContext(_dbOptions))
            {
                var data = await GetFromDatabase(dbContext);

                lock (LockObject)
                {
                    Data = data;
                }

                OnReload();
            }
        }

        private async Task<Dictionary<string, string>> GetFromDatabase(ConfigurationContext dbContext)
        {
            try
            {
                return await dbContext.ConfigurationValue.ToDictionaryAsync(c => c.Key, c => c.Value);
            }
            catch (NpgsqlException)
            {
                return new Dictionary<string, string>();
            }
        }

        public void Dispose()
        {
            _cancellationToken.Cancel();
            _backgroundWorker?.Dispose();
            _backgroundWorker = null;
        }
    }
}