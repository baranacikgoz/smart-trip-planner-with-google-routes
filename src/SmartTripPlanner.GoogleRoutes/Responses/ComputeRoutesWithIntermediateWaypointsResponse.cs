using SmartTripPlanner.Core.Routes.Models;

namespace SmartTripPlanner.GoogleRoutes.Responses;

public sealed record ComputeRoutesWithIntermediateWaypointsResponse(ICollection<Leg> Legs);
