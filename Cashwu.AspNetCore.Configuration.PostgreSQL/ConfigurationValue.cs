using System;

namespace Cashwu.AspNetCore.Configuration.PostgreSQL
{
    public class ConfigurationValue
    {
        public string Key { get; set; }

        public string Value { get; set; }

#pragma warning disable 649
        private DateTime? _lastUpdated;
#pragma warning restore 649
        // ReSharper disable once ConvertToAutoProperty

        public DateTime LastUpdated => _lastUpdated ?? DateTime.UtcNow;
    }
}