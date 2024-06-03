namespace SmartTripPlanner.Core.Routing.Models;

public sealed record Leg(
    string Duration,
    string StaticDuration,
    Polyline Polyline,
    Location StartLocation,
    Location EndLocation,
    IEnumerable<Step> Steps
    );
