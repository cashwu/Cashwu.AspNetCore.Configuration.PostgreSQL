using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Cashwu.AspNetCore.Configuration.PostgreSQL
{
    internal class PostgreSqlConfigurationSource : IConfigurationSource
    {
        private readonly DbContextOptions<ConfigurationContext> _dbOptions;
        private readonly PostgreSqlConfigurationOptions _efOptions;

        public PostgreSqlConfigurationSource(DbContextOptions<ConfigurationContext> dbOptions, PostgreSqlConfigurationOptions efOptions)
        {
            _dbOptions = dbOptions;
            _efOptions = efOptions;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new PostgreSqlConfigurationProvider(_dbOptions, _efOptions);
        }
    }
}