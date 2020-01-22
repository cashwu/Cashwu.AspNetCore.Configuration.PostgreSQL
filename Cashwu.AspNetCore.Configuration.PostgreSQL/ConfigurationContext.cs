using Microsoft.EntityFrameworkCore;

namespace Cashwu.AspNetCore.Configuration.PostgreSQL
{
    public class ConfigurationContext : DbContext
    {
        public ConfigurationContext(DbContextOptions<ConfigurationContext> options)
            : base(options)
        {
        }

        public DbSet<ConfigurationValue> ConfigurationValue { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ConfigurationValue>(builder =>
            {
                builder.ToTable(nameof(ConfigurationValue).ToLower(), "kv");

                builder.HasKey(x => x.Key);
                builder.HasIndex(x => x.LastUpdated);

                builder.Property(x => x.Key)
                       .HasColumnName("key")
                       .HasMaxLength(64)
                       .IsRequired();

                builder.Property(x => x.Value)
                       .HasColumnName("value")
                       .IsRequired();

                builder.Property(x => x.LastUpdated)
                       .HasColumnName("lastupdated")
                       .HasDefaultValueSql("CURRENT_TIMESTAMP")
                       .HasField("_lastUpdated")
                       .IsRequired();
            });
        }
    }
}