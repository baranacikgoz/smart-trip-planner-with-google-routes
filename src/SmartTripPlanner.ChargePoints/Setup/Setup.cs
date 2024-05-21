using Microsoft.Extensions.DependencyInjection;
using SmartTripPlanner.ChargePoints.Cache;
using SmartTripPlanner.ChargePoints.Graphs;
using SmartTripPlanner.ChargePoints.Models;
using SmartTripPlanner.Core.Cache;
using SmartTripPlanner.Core.Graph;
using SmartTripPlanner.Core.Setup;
using SmartTripPlanner.Core.Setup.GraphSetup;

namespace SmartTripPlanner.ChargePoints.Setup;

public static class Setup
{
    public static ISmartTripPlannerConfigBuilder ForChargePoints(
        this ISmartTripPlannerConfigBuilder builder)
    {
        builder
            .Services
                .AddSingleton<IChargePointGraphCache, ChargePointGraphCache>();

        builder.For<IChargePointGraph, ChargePointGraph, ChargePointBarcode, Way>()
               .DecorateWith((decoree, sp) => new CachedChargePointGraph(
                                                decoree,
                                                sp.GetRequiredService<IChargePointGraphCache>(),
                                                cacheKey: nameof(ChargePointGraph)))
               .DecorateWith((decoree, sp) => new NeighbourChargePointsGraph(
                                                decoree,
                                                numberOfNeighboursPerChargePoint: 3))
               .DecorateWith((decoree, sp) => new CachedChargePointGraph(
                                                decoree,
                                                sp.GetRequiredService<IChargePointGraphCache>(),
                                                cacheKey: nameof(NeighbourChargePointsGraph)));

        return builder;
    }
}
