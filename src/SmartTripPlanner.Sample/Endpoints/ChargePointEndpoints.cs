using Microsoft.AspNetCore.Mvc;
using SmartTripPlanner.ChargePoints.Graphs;
using SmartTripPlanner.ChargePoints.Models;
using SmartTripPlanner.Sample.Repositories;

namespace SmartTripPlanner.Sample.Endpoints;

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

    private static async ValueTask<IReadOnlyDictionary<ChargePointBarcode, List<Way>>> GetGraph(
        [FromServices] IChargePointGraph graph)
        => await graph.GetAdjacencyDictAsync();
}
