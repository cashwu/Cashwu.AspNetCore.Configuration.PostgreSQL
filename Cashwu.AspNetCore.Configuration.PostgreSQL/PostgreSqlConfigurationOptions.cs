﻿namespace Cashwu.AspNetCore.Configuration.PostgreSQL
{
    public class PostgreSqlConfigurationOptions
    {
        public string ConnectionStringName { get; set; }
        
        public int PollingInterval { get; set; }
    }
}