namespace SmartTripPlanner.Core.Routing.Models;

public sealed record Step(
    string StaticDuration,
    Polyline Polyline,
    Location StartLocation,
    Location EndLocation
    );
