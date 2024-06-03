using SmartTripPlanner.Core.Routing.Models;

namespace SmartTripPlanner.Core.Routing.Responses;

public sealed record ComputeDurationAndDistanceOnlyResponse(IEnumerable<DistanceAndDurationOnlyRoute> Routes);
