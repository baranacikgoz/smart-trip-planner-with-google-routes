namespace SmartTripPlanner.Core.Routes.Models;

public sealed record Step(
    double DistanceMeters,
    double StaticDuration,
    Polyline Polyline,
    Location StartLocation,
    Location EndLocation
    );
