using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Cashwu.AspNetCore.Configuration.PostgreSQL.Tests")]

namespace Cashwu.AspNetCore.Configuration.PostgreSQL
{
    public class ConfigurationValue
    {
        public string Key { get; set; }

        public string Value { get; set; }

        private DateTime? _lastUpdated;

        public DateTime LastUpdated
        {
            internal set => _lastUpdated = value;
            get => _lastUpdated ?? DateTime.UtcNow;
        }
    }
}