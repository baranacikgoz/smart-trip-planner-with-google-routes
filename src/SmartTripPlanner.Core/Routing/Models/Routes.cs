namespace SmartTripPlanner.Core.Routing.Models;

public sealed record Routes(
    double DistanceMeters,
    string Duration,
    string StaticDuration,
    Polyline Polyline,
    IEnumerable<Leg> Legs);
