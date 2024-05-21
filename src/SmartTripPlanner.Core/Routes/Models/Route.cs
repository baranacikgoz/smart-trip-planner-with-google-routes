namespace SmartTripPlanner.Core.Routes.Models;

public sealed record Route(
    double DistanceMeters,
    string Duration,
    Polyline Polyline
    );
