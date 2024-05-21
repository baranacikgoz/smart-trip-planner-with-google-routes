namespace SmartTripPlanner.Core.Routes.Models;

public sealed record DecodedLeg(
    double DistanceMeters,
    TimeSpan Duration,
    TimeSpan StaticDuration,
    Location StartLocation,
    Location EndLocation,
    IEnumerable<DecodedStep> Steps);
