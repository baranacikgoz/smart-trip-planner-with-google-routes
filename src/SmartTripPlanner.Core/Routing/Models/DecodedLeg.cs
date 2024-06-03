namespace SmartTripPlanner.Core.Routing.Models;

public sealed record DecodedLeg(
    TimeSpan Duration,
    TimeSpan StaticDuration,
    Location StartLocation,
    Location EndLocation,
    Polyline Polyline,
    IEnumerable<DecodedStep> Steps);
