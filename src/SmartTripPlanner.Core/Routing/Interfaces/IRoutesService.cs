using System;
using System.Collections.Generic;
using System.Text;
using SmartTripPlanner.Core.Routing.Models;

namespace SmartTripPlanner.Core.Routing.Interfaces;
public interface IRoutesService
{
    Task<DecodedRoute> GetRoutesAsync(
        LatLng origin,
        LatLng destination,
        TrafficAwareness trafficAwareness,
        CancellationToken cancellationToken = default);

    Task<(TimeSpan Duration, double DistanceMeters)> GetDurationAndDistanceOnlyAsync(
        LatLng origin,
        LatLng destination,
        TrafficAwareness trafficAwareness,
        CancellationToken cancellationToken = default);

    Task<RoutesWithIntermediateWayPoints> GetRoutesWithIntermediateWaypointsAsync(
        LatLng origin,
        LatLng destination,
        ICollection<LatLng> intermediateWaypoints,
        TrafficAwareness trafficAwareness,
        CancellationToken cancellationToken = default);
}
