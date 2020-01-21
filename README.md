# Asp.Net Core configuration from PostgreSQL

[![actions](https://github.com/cashwu/Cashwu.AspNetCore.Configuration.PostgreSQL/workflows/.NET%20Core/badge.svg?branch=master)](https://github.com/cashwu/Cashwu.AspNetCore.Configuration.PostgreSQL/actions)

---

[![Nuget](https://img.shields.io/badge/Nuget-Cashwu.AspNetCore.Configuration.PostgreSQL-blue.svg)](https://www.nuget.org/packages/Cashwu.AspNetCore.Configuration.PostgreSQL)

---

## Database 初始化

先在 Database 裡面執行 [init.sql](https://github.com/cashwu/Cashwu.AspNetCore.Configuration.PostgreSQL/blob/dev/script/init.sql)

## 註冊

在 `Program` 的 `CreateHostBuilder` 註冊新的 Configuration，記得寫在 `ConfigureWebHostDefaults` 之前。可以傳入，下面兩個參數 

  - ConnectionStringName：在 `appsettings` 裡面的 Database connection string 名稱，預設為 `DefaultConnection`
  - PollingInterval：多久檢查一次 Database 看有沒有異動的資料，預設為 `1000 ms`

```csharp
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((context, builder) =>
        {
            builder.AddPostgreSqlEntityFrameworkValues(options =>
            {
                options.ConnectionStringName = "DefaultConnection";
                options.PollingInterval = 5000;
            });
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
```

## DBContext

如果需要使用 Database 存取 Configuration 的資料，可以註冊 `ConfigurationContext`
來使用，Entity 為 `ConfigurationValue`

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<ConfigurationContext>(builder =>
    {
        builder.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
    });
}
```





