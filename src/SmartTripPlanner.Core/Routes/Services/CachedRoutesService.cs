using System;
using System.Collections.Generic;
using System.Text;
using SmartTripPlanner.Core.Cache;
using SmartTripPlanner.Core.Routes.Interfaces;
using SmartTripPlanner.Core.Routes.Models;

namespace SmartTripPlanner.Core.Routes.Services;
public class CachedRoutesService(IRoutesService _decoree, ICache _cache) : IRoutesService
{
    public async Task<DecodedRoute> GetRoutesAsync(
        LatLng origin,
        LatLng destination,
        CancellationToken cancellationToken = default)
        => await _cache
                    .GetOrSetAsync(
                        key: GenerateCacheKey(origin, destination),
                        async () => await _decoree.GetRoutesAsync(origin, destination),
                        cancellationToken: cancellationToken);

    public async Task<RoutesWithIntermediateWayPoints> GetRoutesWithIntermediateWaypointsAsync(
        LatLng origin,
        LatLng destination,
        ICollection<LatLng> intermediateWaypoints,
        CancellationToken cancellationToken = default)
        => await _cache
                    .GetOrSetAsync(
                        key: GenerateCacheKey(origin, destination, intermediateWaypoints),
                        async () => await _decoree.GetRoutesWithIntermediateWaypointsAsync(origin, destination, intermediateWaypoints, cancellationToken: cancellationToken),
                        cancellationToken: cancellationToken);

    private static string GenerateCacheKey(LatLng origin, LatLng destination)
        => $"{nameof(GetRoutesAsync)}.({origin})-({destination})";

    private static string GenerateCacheKey(LatLng origin, LatLng destination, ICollection<LatLng> intermediateWaypoints)
        => $"{nameof(GetRoutesWithIntermediateWaypointsAsync)}" +
           $".({origin})-({destination})" +
           $".Intermediates->{intermediateWaypoints.Aggregate(
                                                        new StringBuilder(),
                                                        (acc, current) => acc.Append('(')
                                                                             .Append(current)
                                                                             .Append(')'))}";
}
