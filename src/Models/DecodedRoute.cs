namespace SmartTripPlanner.WebAPI.Models;

public sealed record DecodedRoute(
    double DistanceMeters,
    string Duration,
    IEnumerable<Location> Polylines
    );
