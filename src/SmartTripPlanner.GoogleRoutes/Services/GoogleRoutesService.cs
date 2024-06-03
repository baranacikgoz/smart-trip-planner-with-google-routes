using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using SmartTripPlanner.Core.Routing.Responses;
using SmartTripPlanner.GoogleRoutes.Constants;
using SmartTripPlanner.GoogleRoutes.Exceptions;
using System.Text.Json;
using SmartTripPlanner.GoogleRoutes.Extensions;
using SmartTripPlanner.Core.Routing.Models;
using SmartTripPlanner.Core.Routing.Interfaces;

namespace SmartTripPlanner.GoogleRoutes.Services;
public class GoogleRoutesService(
    IRoutesApiService _apiService,
    IPolyLineDecoder _polylineDecoder
    ) : IRoutesService
{
    public async Task<DecodedRoute> GetRoutesAsync(
        LatLng origin,
        LatLng destination,
        TrafficAwareness trafficAwareness,
        CancellationToken cancellationToken = default)
    {
        var response = await _apiService.GetComputeRoutesResponseAsync(origin, destination, trafficAwareness, cancellationToken);
        var route = response.Routes.Single();

        return new DecodedRoute(
            DistanceMeters: route.DistanceMeters,
            Duration: route.Duration.ToTimeSpan(),
            Locations: _polylineDecoder.Decode(route.Polyline.EncodedPolyline)
            );
    }

    public async Task<(TimeSpan Duration, double DistanceMeters)> GetDurationAndDistanceOnlyAsync(
        LatLng origin,
        LatLng destination,
        TrafficAwareness trafficAwareness,
        CancellationToken cancellationToken = default)
    {
        var response = await _apiService.GetComputeDurationAndDistanceOnlyResponseAsync(origin, destination, trafficAwareness, cancellationToken);
        var durationAndDistanceOnlyRoute = response.Routes.Single();

        return (durationAndDistanceOnlyRoute.Duration.ToTimeSpan(), durationAndDistanceOnlyRoute.DistanceMeters);
    }

    public async Task<RoutesWithIntermediateWayPoints> GetRoutesWithIntermediateWaypointsAsync(
        LatLng origin,
        LatLng destination,
        ICollection<LatLng> intermediateWaypoints,
        TrafficAwareness trafficAwareness,
        CancellationToken cancellationToken = default)
    {

        var response = await _apiService.GetRoutesWithIntermediateWaypointsResponseAsync(origin, destination, intermediateWaypoints, trafficAwareness, cancellationToken);

        var responseRoutes = response.Routes.Single();

        return new RoutesWithIntermediateWayPoints(
            new DecodedRoutes(
                DistanceMeters: responseRoutes.DistanceMeters,
                Duration: responseRoutes.Duration.ToTimeSpan(),
                StaticDuration: responseRoutes.StaticDuration.ToTimeSpan(),
                Polyline: responseRoutes.Polyline,
                Legs: responseRoutes
                        .Legs
                        .Select(l => new DecodedLeg(
                            l.Duration.ToTimeSpan(),
                            l.StaticDuration.ToTimeSpan(),
                            l.StartLocation,
                            l.EndLocation,
                            l.Polyline,
                            l.Steps.Select(s => new DecodedStep(
                                                    s.StaticDuration.ToTimeSpan(),
                                                    s.Polyline,
                                                    _polylineDecoder.Decode(s.Polyline.EncodedPolyline),
                                                    s.StartLocation,
                                                    s.EndLocation)))),
                 Locations: _polylineDecoder.Decode(responseRoutes.Polyline.EncodedPolyline)));
    }
}
