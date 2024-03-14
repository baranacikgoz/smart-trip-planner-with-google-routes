using Route = SmartTripPlanner.WebAPI.Models.Route;

namespace SmartTripPlanner.WebAPI.Responses;

public sealed record ComputeRoutesResponse(ICollection<Route> Routes);
