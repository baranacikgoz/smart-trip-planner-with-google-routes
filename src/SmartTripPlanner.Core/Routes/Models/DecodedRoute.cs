namespace SmartTripPlanner.Core.Routes.Models;

public sealed record DecodedRoute(
    double DistanceMeters,
    TimeSpan Duration,
    IEnumerable<LatLng> Locations);
