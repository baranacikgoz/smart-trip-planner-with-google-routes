using SmartTripPlanner.Core.Graph;

namespace SmartTripPlanner.ChargePoints.Models;
public sealed record Way(ChargePoint To, TimeSpan Duration, double DistanceInMeters)
    : Edge<ChargePoint, ChargePointBarcode>(To, Duration, DistanceInMeters);
