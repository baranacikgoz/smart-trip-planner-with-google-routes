namespace SmartTripPlanner.Core.Routing.Models;

public sealed record DecodedRoute(
    double DistanceMeters,
    TimeSpan Duration,
    IEnumerable<LatLng> Locations);
