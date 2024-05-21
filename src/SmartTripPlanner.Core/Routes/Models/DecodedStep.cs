namespace SmartTripPlanner.Core.Routes.Models;

public sealed record DecodedStep(
    double DistanceMeters,
    double StaticDuration,
    IEnumerable<LatLng> Locations,
    Location StartLocation,
    Location EndLocation
    );
