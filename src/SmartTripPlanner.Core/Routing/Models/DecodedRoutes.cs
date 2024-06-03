namespace SmartTripPlanner.Core.Routing.Models;

public record DecodedRoutes(
    double DistanceMeters,
    TimeSpan Duration,
    TimeSpan StaticDuration,
    Polyline Polyline,
    IEnumerable<DecodedLeg> Legs,
    IEnumerable<LatLng> Locations);
