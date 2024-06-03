using SmartTripPlanner.Core.Routing.Models;

namespace SmartTripPlanner.Core.Routing.Responses;
public sealed record ComputeRoutesWithIntermediateWaypointsResponse(IEnumerable<Routes> Routes);
