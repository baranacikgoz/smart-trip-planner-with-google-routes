namespace SmartTripPlanner.Core.Routes.Models;

public sealed record Leg(
    double DistanceMeters,
    string Duration,
    string StaticDuration,
    Location StartLocation,
    Location EndLocation,
    ICollection<Step> Steps
    );
