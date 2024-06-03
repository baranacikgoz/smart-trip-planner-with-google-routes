using Microsoft.AspNetCore.Mvc;
using SmartTripPlanner.ChargePoints.TripPlanner.Interfaces;
using SmartTripPlanner.Core.Routing.Models;

namespace SmartTripPlanner.Sample.Endpoints;

public static class TripPlannerEndpoints
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var googleRoutesApiGroup = endpoints.MapGroup("trip-planner");

        googleRoutesApiGroup.MapPost("plan-trip", PlanTripAsync);
    }

    public sealed record PlanTripRequest(double InitialBatteryPercentage, double BatteryPercentagePer100Km, LatLng Origin, LatLng Destination);
    private static async Task<RoutesWithIntermediateWayPoints> PlanTripAsync(
        [FromBody] PlanTripRequest request,
        [FromServices] IChargePointTripPlanner tripPlanner,
        CancellationToken cancellationToken
    )
    {
        return await tripPlanner.PlanTripAsync(
                                    request.InitialBatteryPercentage,
                                    request.BatteryPercentagePer100Km,
                                    minBatteryThreshold: 5,
                                    request.Origin,
                                    request.Destination,
                                    cancellationToken);
    }
}
