using System;
using System.Collections.Generic;
using System.Text;
using SmartTripPlanner.Core.Routes.Models;
using SmartTripPlanner.Core.Routes.Responses;

namespace SmartTripPlanner.Core.Routes.Interfaces;
public interface IRoutesApiService
{
    Task<ComputeRoutesResponse> GetComputeRoutesResponseAsync(
        LatLng origin,
        LatLng destination,
        CancellationToken cancellationToken = default);

    Task<ComputeRoutesWithIntermediateWaypointsResponse> GetRoutesWithIntermediateWaypointsResponseAsync(
        LatLng origin,
        LatLng destination,
        ICollection<LatLng> intermediateWaypoints,
        CancellationToken cancellationToken = default);
}
