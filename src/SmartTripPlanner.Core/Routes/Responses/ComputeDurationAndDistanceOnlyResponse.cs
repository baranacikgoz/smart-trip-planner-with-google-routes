using SmartTripPlanner.Core.Routes.Models;

namespace SmartTripPlanner.Core.Routes.Responses;

public sealed record ComputeDurationAndDistanceOnlyResponse(ICollection<DistanceAndDurationOnlyRoute> Routes);
