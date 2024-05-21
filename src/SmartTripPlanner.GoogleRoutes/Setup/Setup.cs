using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SmartTripPlanner.Core.Cache;
using SmartTripPlanner.Core.Routes.Interfaces;
using SmartTripPlanner.Core.Routes.Services;
using SmartTripPlanner.Core.Setup;
using SmartTripPlanner.GoogleRoutes.Constants;

namespace SmartTripPlanner.GoogleRoutes.Setup;
public static class Setup
{
    public static ISmartTripPlannerConfigBuilder WithGoogleRoutes(
        this ISmartTripPlannerConfigBuilder builder, IConfiguration config)
    {
        builder.Services
                .AddOptions<GoogleRoutesOptions>()
                .Bind(config.GetSection(nameof(GoogleRoutesOptions)))
                .ValidateDataAnnotations()
                .ValidateOnStart();

        builder.Services.AddTransient<GoogleRoutesService>();
        builder.Services.AddTransient<IRoutesService, GoogleRoutesService>();
        builder.Services.AddHttpClient<IRoutesService, GoogleRoutesService>((sp, httpClient) =>
        {
            var googleRoutesOptions = sp.GetRequiredService<IOptions<GoogleRoutesOptions>>().Value;

            httpClient.BaseAddress = googleRoutesOptions.BaseUrl;
            httpClient.DefaultRequestHeaders.Add(GoogleRoutesConstants.Headers.ApiKey, googleRoutesOptions.ApiKey);
        });

        builder.Services.AddSingleton<IPolyLineDecoder, GoogleRoutesPolylineDecoder>();
        builder.Services.Decorate<IRoutesService>((decoree, sp) => new CachedRoutesService(decoree, sp.GetRequiredService<ICache>()));

        return builder;
    }
}
