using Microsoft.Extensions.DependencyInjection;
using SmartTripPlanner.ChargePoints.Cache;
using SmartTripPlanner.ChargePoints.Graphs;
using SmartTripPlanner.ChargePoints.Models;
using SmartTripPlanner.ChargePoints.TripPlanner.Interfaces;
using SmartTripPlanner.ChargePoints.TripPlanner.Services;
using SmartTripPlanner.Core.Routing.Interfaces;
using SmartTripPlanner.Core.Setup;
using SmartTripPlanner.Core.Setup.GraphSetup;
using SmartTripPlanner.Core.TripPlanner.Interfaces;

namespace SmartTripPlanner.ChargePoints.Setup;

public static class Setup
{
    public static ISmartTripPlannerConfigBuilder ForChargePoints(
        this ISmartTripPlannerConfigBuilder builder)
    {
        builder
            .Services
                .AddSingleton<IChargePointGraphCache, ChargePointGraphCache>();

        builder.For<IChargePointGraph, ChargePointsGraph, ChargePoint, ChargePointBarcode, Way>()
               .DecorateWith((decoree, sp) => new CachedChargePointGraph(
                                                decoree,
                                                sp.GetRequiredService<IChargePointGraphCache>(),
                                                cacheKey: nameof(ChargePointsGraph)))
               .DecorateWith((decoree, sp) => new ChargePointsRealRoutesGraph(
                                                decoree,
                                                sp.GetRequiredService<IRoutesService>()))
               .DecorateWith((decoree, sp) => new CachedChargePointGraph(
                                                decoree,
                                                sp.GetRequiredService<IChargePointGraphCache>(),
                                                cacheKey: nameof(ChargePointsRealRoutesGraph)));

        builder
            .Services
                .AddSingleton<ITripPlanner<ChargePoint, ChargePointBarcode, Way>, ChargePointTripPlanner>()
                .AddSingleton<IChargePointTripPlanner, ChargePointTripPlanner>();

        return builder;
    }
}
