using SmartTripPlanner.Core.Routing.Models;

namespace SmartTripPlanner.Core.Routing.Responses;

public sealed record ComputeRoutesResponse(IEnumerable<Route> Routes);
