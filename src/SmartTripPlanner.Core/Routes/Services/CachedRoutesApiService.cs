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
        TrafficAwareness trafficAwareness,
        CancellationToken cancellationToken = default)
        => await _cache
                    .GetOrSetAsync(
                        key: GenerateGetComputeRoutesResponseAsyncCacheKey(origin, destination, trafficAwareness),
                        async () => await _decoree.GetComputeRoutesResponseAsync(origin, destination, trafficAwareness),
                        cancellationToken: cancellationToken);

    public async Task<ComputeDurationAndDistanceOnlyResponse> GetComputeDurationAndDistanceOnlyResponseAsync(
        LatLng origin,
        LatLng destination,
        TrafficAwareness trafficAwareness,
        CancellationToken cancellationToken = default)
        => await _cache
                    .GetOrSetAsync(
                        key: GenerateGetComputeDurationAndDistanceOnlyResponseAsyncCacheKey(origin, destination, trafficAwareness),
                        async () => await _decoree.GetComputeDurationAndDistanceOnlyResponseAsync(origin, destination, trafficAwareness),
                        cancellationToken: cancellationToken);

    public async Task<ComputeRoutesWithIntermediateWaypointsResponse> GetRoutesWithIntermediateWaypointsResponseAsync(
        LatLng origin,
        LatLng destination,
        ICollection<LatLng> intermediateWaypoints,
        TrafficAwareness trafficAwareness,
        CancellationToken cancellationToken = default)
        => await _cache
                    .GetOrSetAsync(
                        key: GenerateGetRoutesWithIntermediateWaypointsResponseAsyncCacheKey(origin, destination, intermediateWaypoints, trafficAwareness),
                        async () => await _decoree.GetRoutesWithIntermediateWaypointsResponseAsync(origin, destination, intermediateWaypoints, trafficAwareness, cancellationToken: cancellationToken),
                        cancellationToken: cancellationToken);

    private static string GenerateGetComputeRoutesResponseAsyncCacheKey(LatLng origin, LatLng destination, TrafficAwareness trafficAwareness)
        => $"{TrafficeAwarenessInCacheKey(trafficAwareness)}.({origin})-({destination})";

    private static string GenerateGetComputeDurationAndDistanceOnlyResponseAsyncCacheKey(LatLng origin, LatLng destination, TrafficAwareness trafficAwareness)
        => $"{TrafficeAwarenessInCacheKey(trafficAwareness)}.DDOnly.({origin})-({destination})";

    private static string GenerateGetRoutesWithIntermediateWaypointsResponseAsyncCacheKey(LatLng origin, LatLng destination, ICollection<LatLng> intermediateWaypoints, TrafficAwareness trafficAwareness)
        => $"{TrafficeAwarenessInCacheKey(trafficAwareness)}" +
           $".({origin})" +
           $"->{intermediateWaypoints.Aggregate(
                                         new StringBuilder(),
                                         (acc, current) => acc.Append('(')
                                                              .Append(current)
                                                              .Append(')'))}" +
           $"->({destination})";

    private static string TrafficeAwarenessInCacheKey(TrafficAwareness trafficAwareness)
        => trafficAwareness switch
        {
            TrafficAwareness.NonTrafficAware => "T:0",
            TrafficAwareness.TrafficAware => "T:1",
            _ => throw new NotImplementedException($"Unknown {nameof(TrafficAwareness)}: ({trafficAwareness}).")
        };
}
