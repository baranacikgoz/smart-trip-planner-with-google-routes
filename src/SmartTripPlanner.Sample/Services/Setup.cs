using SmartTripPlanner.Sample.Repositories;
using StackExchange.Redis;

namespace SmartTripPlanner.Sample.Services;

public static class Setup
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        => services
            .AddCsvService()
            .AddTripPlannerService()
            .AddChargePointRepository()
            .AddRedisCache(configuration);

    private static IServiceCollection AddTripPlannerService(this IServiceCollection services)
        => services.AddTransient<TripPlannerService>();

    private static IServiceCollection AddChargePointRepository(this IServiceCollection services)
       => services.AddSingleton<ChargePointCsvRepository>();
    private static IServiceCollection AddCsvService(this IServiceCollection services)
        => services.AddSingleton<CsvService>();

    private static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        var redisHost = configuration.GetValue<string>("RedisOptions:Host");
        var instanceName = configuration.GetValue<string>("RedisOptions:InstanceName");

        var options = ConfigurationOptions.Parse(redisHost!);
        options.SyncTimeout = 10000; // 10 seconds
        options.AsyncTimeout = 10000; // 10 seconds
        options.ConnectTimeout = 10000; // 10 seconds
        options.AbortOnConnectFail = false;
        options.ConnectRetry = 5;

        return services.AddStackExchangeRedisCache(opt =>
        {
            opt.ConfigurationOptions = options;
            opt.InstanceName = instanceName;
        });
    }
}
