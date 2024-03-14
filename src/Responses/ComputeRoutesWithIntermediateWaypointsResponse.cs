using SmartTripPlanner.WebAPI.Models;

namespace SmartTripPlanner.WebAPI.Responses;

public sealed record ComputeRoutesWithIntermediateWaypointsResponse(ICollection<Leg> Legs);
