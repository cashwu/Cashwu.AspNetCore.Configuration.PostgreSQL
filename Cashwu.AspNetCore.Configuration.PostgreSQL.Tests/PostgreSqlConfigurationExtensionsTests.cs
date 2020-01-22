using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Cashwu.AspNetCore.Configuration.PostgreSQL.Tests
{
    public class PostgreSqlConfigurationExtensionsTests
    {
        private static string InitConfigValue = "init";
        private static string InitConfigKey = "test";
        private static int DelayInterval = 2000;

        [Fact]
        public void Wrong_connectionName_should_throw_exception()
        {
            var config = new KeyValuePair<string, string>("ConnectionStrings:DefaultConnection", "SomeConnectionString");

            Action action = () =>
                new ConfigurationBuilder()
                    .AddInMemoryCollection(new List<KeyValuePair<string, string>> { config })
                    .AddPostgreSqlEntityFrameworkValues(options =>
                    {
                        options.ConnectionStringName = "error key";
                    });

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Current_connectionName_should_not_throw_exception()
        {
            var connectionKey = "conn";
            
            var config = new KeyValuePair<string, string>($"ConnectionStrings:{connectionKey}", "SomeConnectionString");

            Action action = () =>
                new ConfigurationBuilder()
                    .AddInMemoryCollection(new List<KeyValuePair<string, string>> { config })
                    .AddPostgreSqlEntityFrameworkValues(options =>
                    {
                        options.ConnectionStringName = connectionKey;
                    });

            action.Should().NotThrow<ArgumentException>();
        }

        [Fact]
        public void GetConfigurationValue()
        {
            var dbOptions = InitTestDatabase();
            var configuration = GivenConfiguration(dbOptions);

            configuration.GetValue<string>(InitConfigKey).Should().Be(InitConfigValue);
        }

        [Fact]
        public async Task ReloadConfigurationValue()
        {
            var ChangeConfigValue = "change";

            var dbOptions = InitTestDatabase();
            var configuration = GivenConfiguration(dbOptions);

            var context = new ConfigurationContext(dbOptions.Options);
            var configurationValue = context.ConfigurationValue.Single(a => a.Key == InitConfigKey);
            configurationValue.Value = ChangeConfigValue;
            configurationValue.LastUpdated = DateTime.UtcNow;
            context.SaveChanges();

            await Task.Delay(DelayInterval);

            configuration.GetValue<string>(InitConfigKey).Should().Be(ChangeConfigValue);
        }

        private static DbContextOptionsBuilder<ConfigurationContext> InitTestDatabase()
        {
            var dbOptions = new DbContextOptionsBuilder<ConfigurationContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            using var context = new ConfigurationContext(dbOptions.Options);

            context.ConfigurationValue.Add(new ConfigurationValue
            {
                Key = InitConfigKey,
                Value = InitConfigValue,
                LastUpdated = DateTime.UtcNow
            });

            context.SaveChanges();

            return dbOptions;
        }

        private static IConfigurationRoot GivenConfiguration(DbContextOptionsBuilder<ConfigurationContext> dbOptions)
        {
            return new ConfigurationBuilder()
                   .AddTestPostgreSqlEntityFrameworkValues(dbOptions)
                   .Build();
        }
    }

    internal static class ExtensionFeatures
    {
        internal static IConfigurationBuilder AddTestPostgreSqlEntityFrameworkValues(this IConfigurationBuilder builder,
                                                                                     DbContextOptionsBuilder<ConfigurationContext> dbOptions,
                                                                                     int pollingInterval = 1000)
        {
            var efOptions = new PostgreSqlConfigurationOptions
            {
                ConnectionStringName = "DefaultConnection",
                PollingInterval = pollingInterval
            };

            return builder.Add(new PostgreSqlConfigurationSource(dbOptions.Options, efOptions));
        }
    }
}