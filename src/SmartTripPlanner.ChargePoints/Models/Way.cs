using SmartTripPlanner.Core.Graph;

namespace SmartTripPlanner.ChargePoints.Models;
public sealed record Way(ChargePointBarcode To, TimeSpan Duration, double DistanceInMeters) : Edge(To, Duration, DistanceInMeters);
