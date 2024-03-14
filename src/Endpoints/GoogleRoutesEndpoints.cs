using Microsoft.AspNetCore.Mvc;
using SmartTripPlanner.WebAPI.Models;
using SmartTripPlanner.WebAPI.Responses;
using SmartTripPlanner.WebAPI.Services;

namespace SmartTripPlanner.WebAPI.Endpoints;

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
        [FromServices] GoogleRoutesService googleRoutesService
    )
    {
        return await googleRoutesService.GetRoutesAsync(request.Origin, request.Destination);
    }

    public sealed record RoutesWithIntermediateWaypointsRequest(LatLng Origin, LatLng Destination, ICollection<LatLng> IntermediateWaypoints);
    private static async Task<ComputeRoutesWithIntermediateWaypointsResponse> GetRoutesWithIntermediateWaypointsAsync(
        [FromBody] RoutesWithIntermediateWaypointsRequest request,
        [FromServices] GoogleRoutesService googleRoutesService
    )
    {
        return await googleRoutesService.GetRoutesWithIntermediateWaypointsAsync(
            request.Origin,
            request.Destination,
            request.IntermediateWaypoints);
    }
}
