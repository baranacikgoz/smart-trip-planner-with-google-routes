using Microsoft.AspNetCore.Mvc;
using SmartTripPlanner.ChargePoints.Graphs;
using SmartTripPlanner.ChargePoints.Models;
using SmartTripPlanner.API.Repositories;

namespace SmartTripPlanner.API.Endpoints;

public static class ChargePointEndpoints
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var chargePointsApiGroup = endpoints.MapGroup("chargepoints");

        chargePointsApiGroup.MapGet("all", GetAll);
        chargePointsApiGroup.MapGet("graph", GetGraph);
    }
    private static IReadOnlyCollection<ChargePoint> GetAll(
        [FromServices] ChargePointCsvRepository chargePointRepository
    )
    {
        return chargePointRepository.GetAll();
    }

    private static async Task<IReadOnlyDictionary<ChargePoint, List<Way>>> GetGraph(
        [FromServices] IChargePointGraph graph)
        => await graph.GetAdjacencyDictAsync();
}
