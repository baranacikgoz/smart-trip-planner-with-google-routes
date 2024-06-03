using Microsoft.AspNetCore.Mvc;
using SmartTripPlanner.Core.Routing.Interfaces;
using SmartTripPlanner.Core.Routing.Models;

namespace SmartTripPlanner.Sample.Endpoints;

public static class GoogleRoutesEndpoints
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var googleRoutesApiGroup = endpoints.MapGroup("google-routes");

        googleRoutesApiGroup.MapPost("routes", GetRoutesAsync);
        googleRoutesApiGroup.MapPost("routes-with-intermediate-waypoints", GetRoutesWithIntermediateWaypointsAsync);
    }

    public sealed record RoutesRequest(LatLng Origin, LatLng Destination);
    private static async Task<DecodedRoute> GetRoutesAsync(
        [FromBody] RoutesRequest request,
        [FromServices] IRoutesService routesService
    )
    {
        return await routesService.GetRoutesAsync(request.Origin, request.Destination, TrafficAwareness.TrafficAware);
    }

    public sealed record RoutesWithIntermediateWaypointsRequest(LatLng Origin, LatLng Destination, ICollection<LatLng> IntermediateWaypoints);
    private static async Task<RoutesWithIntermediateWayPoints> GetRoutesWithIntermediateWaypointsAsync(
        [FromBody] RoutesWithIntermediateWaypointsRequest request,
        [FromServices] IRoutesService routesService
    )
    {
        return await routesService.GetRoutesWithIntermediateWaypointsAsync(
            request.Origin,
            request.Destination,
            request.IntermediateWaypoints,
            TrafficAwareness.TrafficAware);
    }
}
