using SmartTripPlanner.Sample.Repositories;

namespace SmartTripPlanner.Sample.Services;

public static class Setup
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        => services
            .AddCsvService()
            .AddTripPlannerService()
            .AddChargePointRepository()
            .AddRedisCacahe(configuration);

    private static IServiceCollection AddTripPlannerService(this IServiceCollection services)
        => services.AddTransient<TripPlannerService>();

    private static IServiceCollection AddChargePointRepository(this IServiceCollection services)
       => services.AddSingleton<ChargePointCsvRepository>();
    private static IServiceCollection AddCsvService(this IServiceCollection services)
        => services.AddSingleton<CsvService>();

    private static IServiceCollection AddRedisCacahe(this IServiceCollection services, IConfiguration configuration)
        => services
            .AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetValue<string>("RedisOptions:Host");
                options.InstanceName = configuration.GetValue<string>("RedisOptions:InstanceName");
            });
}
