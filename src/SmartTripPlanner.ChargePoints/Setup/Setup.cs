using Microsoft.Extensions.DependencyInjection;
using SmartTripPlanner.ChargePoints.Cache;
using SmartTripPlanner.ChargePoints.Graphs;
using SmartTripPlanner.ChargePoints.Models;
using SmartTripPlanner.Core.Cache;
using SmartTripPlanner.Core.DataSource.Interfaces;
using SmartTripPlanner.Core.Graph;
using SmartTripPlanner.Core.Routes.Interfaces;
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

        builder.For<IChargePointGraph, ChargePointsGraph, ChargePoint, ChargePointBarcode, Way>()
               .DecorateWith((decoree, sp) => new CachedChargePointGraph(
                                                decoree,
                                                sp.GetRequiredService<IChargePointGraphCache>(),
                                                cacheKey: nameof(ChargePointsGraph)))
               .DecorateWith((decoree, sp) => new NeighbourChargePointsGraph(
                                                decoree,
                                                numberOfNeighboursPerChargePoint: 3,
                                                sp.GetRequiredService<IVertexDataSource<ChargePoint, ChargePointBarcode>>()))
               .DecorateWith((decoree, sp) => new CachedChargePointGraph(
                                                decoree,
                                                sp.GetRequiredService<IChargePointGraphCache>(),
                                                cacheKey: nameof(NeighbourChargePointsGraph)))
               .DecorateWith((decoree, sp) => new ChargePointsRealRoutesGraph(
                                                decoree,
                                                sp.GetRequiredService<IRoutesService>()))
               .DecorateWith((decoree, sp) => new CachedChargePointGraph(
                                                decoree,
                                                sp.GetRequiredService<IChargePointGraphCache>(),
                                                cacheKey: nameof(ChargePointsRealRoutesGraph)));

        return builder;
    }
}
