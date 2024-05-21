namespace SmartTripPlanner.Sample.Repositories;

public static class Setup
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
        => services.AddSingleton<IChargePointRepository, ChargePointCsvRepository>();
}
