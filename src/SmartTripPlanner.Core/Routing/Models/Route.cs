namespace SmartTripPlanner.Core.Routing.Models;

public sealed record Route(
    double DistanceMeters,
    string Duration,
    Polyline Polyline
    );
