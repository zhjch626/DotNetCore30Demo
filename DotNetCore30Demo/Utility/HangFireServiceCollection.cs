using Hangfire;
using Hangfire.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetCore30Demo.Utility
{
    public static class HangFireServiceCollection
    {
        public static void AddHangFile(this IServiceCollection services, IConfiguration configuration)
        {
            //是否启动Hangfire
            var enabled = configuration["AppSettings:HangFire:Enabled"];
            if (!string.IsNullOrWhiteSpace(enabled) && bool.Parse(enabled))
            {
                var conn = configuration["AppSettings:HangFire:RedisConnectionString"];
                var prefix = configuration["AppConfig:HangFire:RedisPrefixName"];
                var defaultDb = configuration["AppConfig:HangFire:RedisDefaultDatabase"];
                var option = new RedisStorageOptions
                {
                    Db = 0,
                    Prefix = prefix
                };
                if (int.TryParse(defaultDb, out int db))
                {
                    if (db >= 0 && db <= 15)
                    {
                        option.Db = db;
                    }
                }
                services.AddHangfire(config => { config.UseRedisStorage(conn, option); });
            }
        }
    }
}