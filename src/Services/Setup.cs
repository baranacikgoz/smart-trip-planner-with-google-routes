using Microsoft.Extensions.Options;
using SmartTripPlanner.WebAPI.Constants;
using SmartTripPlanner.WebAPI.Options;

namespace SmartTripPlanner.WebAPI.Services;

public static class Setup
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        => services
            .AddGoogleRoutesService(configuration)
            .AddPolylineDecoderService()
            .AddTripPlannerService();

    private static IServiceCollection AddGoogleRoutesService(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<GoogleRoutesOptions>()
            .Bind(configuration.GetSection(nameof(GoogleRoutesOptions)))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddTransient<GoogleRoutesService>();

        services.AddHttpClient<GoogleRoutesService>((sp, httpClient) =>
        {
            var options = sp.GetRequiredService<IOptions<GoogleRoutesOptions>>().Value;

            httpClient.BaseAddress = options.Url;

            httpClient.DefaultRequestHeaders.Add(GoogleRoutesConstants.Headers.ApiKey, options.ApiKey);
            httpClient.DefaultRequestHeaders.Add(GoogleRoutesConstants.Headers.FieldMask, options.FieldMask);
        });

        services.AddSingleton<PolylineDecoderService>();

        return services;
    }

    private static IServiceCollection AddPolylineDecoderService(this IServiceCollection services)
        => services.AddSingleton<PolylineDecoderService>();

    private static IServiceCollection AddTripPlannerService(this IServiceCollection services)
        => services.AddTransient<TripPlannerService>();
}
