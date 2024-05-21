using System;
using System.Collections.Generic;
using System.Text;
using SmartTripPlanner.Core.Cache;
using SmartTripPlanner.Core.Routes.Interfaces;
using SmartTripPlanner.Core.Routes.Models;
using SmartTripPlanner.Core.Routes.Responses;

namespace SmartTripPlanner.Core.Routes.Services;
public class CachedRoutesApiService(IRoutesApiService _decoree, ICache _cache) : IRoutesApiService
{
    public async Task<ComputeRoutesResponse> GetComputeRoutesResponseAsync(
        LatLng origin,
        LatLng destination,
        CancellationToken cancellationToken = default)
        => await _cache
                    .GetOrSetAsync(
                        key: GenerateCacheKey(origin, destination),
                        async () => await _decoree.GetComputeRoutesResponseAsync(origin, destination),
                        cancellationToken: cancellationToken);

    public async Task<ComputeRoutesWithIntermediateWaypointsResponse> GetRoutesWithIntermediateWaypointsResponseAsync(
        LatLng origin,
        LatLng destination,
        ICollection<LatLng> intermediateWaypoints,
        CancellationToken cancellationToken = default)
        => await _cache
                    .GetOrSetAsync(
                        key: GenerateCacheKey(origin, destination, intermediateWaypoints),
                        async () => await _decoree.GetRoutesWithIntermediateWaypointsResponseAsync(origin, destination, intermediateWaypoints, cancellationToken: cancellationToken),
                        cancellationToken: cancellationToken);

    private static string GenerateCacheKey(LatLng origin, LatLng destination)
        => $"{nameof(ComputeRoutesResponse)}.({origin})-({destination})";

    private static string GenerateCacheKey(LatLng origin, LatLng destination, ICollection<LatLng> intermediateWaypoints)
        => $"{nameof(ComputeRoutesWithIntermediateWaypointsResponse)}" +
           $".({origin})-({destination})" +
           $".Intermediates->{intermediateWaypoints.Aggregate(
                                                        new StringBuilder(),
                                                        (acc, current) => acc.Append('(')
                                                                             .Append(current)
                                                                             .Append(')'))}";
}
