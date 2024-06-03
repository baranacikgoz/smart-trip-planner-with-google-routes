namespace SmartTripPlanner.Core.Routing.Models;

public sealed record DecodedStep(
    TimeSpan StaticDuration,
    Polyline Polyline,
    IEnumerable<LatLng> Locations,
    Location StartLocation,
    Location EndLocation
    );
