using SmartTripPlanner.Core.Routes.Models;

namespace SmartTripPlanner.GoogleRoutes.Responses;

public sealed record ComputeRoutesResponse(ICollection<Route> Routes);
