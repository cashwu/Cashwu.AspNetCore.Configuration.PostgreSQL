using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Cashwu.AspNetCore.Configuration.PostgreSQL
{
    public static class PostgreSqlConfigurationExtensions
    {
        public static IConfigurationBuilder AddPostgreSqlEntityFrameworkValues(this IConfigurationBuilder builder,
                                                                               Action<PostgreSqlConfigurationOptions> optionsAction = null)
        {
            var connectionStringConfig = builder.Build();

            var efOptions = new PostgreSqlConfigurationOptions
            {
                ConnectionStringName = "DefaultConnection",
                PollingInterval = 1000
            };

            optionsAction?.Invoke(efOptions);

            var dbOptions = new DbContextOptionsBuilder<ConfigurationContext>();

            dbOptions = dbOptions.UseNpgsql(connectionStringConfig.GetConnectionString(efOptions.ConnectionStringName));

            return builder.Add(new PostgreSqlConfigurationSource(dbOptions.Options, efOptions));
        }
    }
}