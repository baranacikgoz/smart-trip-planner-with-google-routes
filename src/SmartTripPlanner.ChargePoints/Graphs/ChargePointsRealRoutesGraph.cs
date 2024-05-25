using System;
using System.Collections.Generic;
using System.Text;
using SmartTripPlanner.ChargePoints.Models;
using SmartTripPlanner.Core.Graph;
using SmartTripPlanner.Core.Routes.Interfaces;

namespace SmartTripPlanner.ChargePoints.Graphs;
public class ChargePointsRealRoutesGraph : IChargePointGraph
{
    private readonly IChargePointGraph _decoree;
    private readonly IRoutesService _routesService;
    public ChargePointsRealRoutesGraph(IChargePointGraph decoree, IRoutesService routesService)
    {
        _decoree = decoree;
        _routesService = routesService;
    }

    public ValueTask<Dictionary<ChargePoint, List<Way>>> GetAdjacencyDictAsync() => _decoree.GetAdjacencyDictAsync();
    public ValueTask ReconstructFrom(Dictionary<ChargePoint, List<Way>> adjacencyDict) => _decoree.ReconstructFrom(adjacencyDict);
    public Task AddEdgeAsync(ChargePoint from, Way edge) => _decoree.AddEdgeAsync(from, edge);
    public Task AddNodeAsync(ChargePoint node) => _decoree.AddNodeAsync(node);

    public async Task EnsureInitializedAsync()
    {
        await _decoree.EnsureInitializedAsync();

        var oldGraph = await _decoree.GetAdjacencyDictAsync();

        Dictionary<ChargePoint, List<Way>> newGraphWithRealRoutes = [];

        var calculateNewEdgeTasks = new List<Task<(ChargePoint From, Way Way)>>();
        foreach (var (chargePoint, edges) in oldGraph)
        {
            newGraphWithRealRoutes.Add(chargePoint, []);

            foreach (var edge in edges)
            {
                calculateNewEdgeTasks.Add(CalculateNewEdgeWithRealRoute(chargePoint, edge.To));
            }
        }

        var newEdges = await Task.WhenAll(calculateNewEdgeTasks);
        foreach (var (chargePoint, edge) in newEdges)
        {
            newGraphWithRealRoutes[chargePoint].Add(edge);
        }

        await _decoree.ReconstructFrom(newGraphWithRealRoutes);
    }

    private async Task<(ChargePoint From, Way Edge)> CalculateNewEdgeWithRealRoute(ChargePoint from, ChargePoint to)
    {
        var (duration, distanceMeters) = await _routesService.GetDurationAndDistanceOnlyAsync(
                                                                    new(from.Latitude, from.Longitude),
                                                                    new(to.Latitude, to.Longitude));

        return (from, new Way(to, duration, distanceMeters));
    }
}
