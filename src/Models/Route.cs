namespace SmartTripPlanner.WebAPI.Models;

public sealed record Route(
    double DistanceMeters,
    string Duration,
    Polyline Polyline
    );
