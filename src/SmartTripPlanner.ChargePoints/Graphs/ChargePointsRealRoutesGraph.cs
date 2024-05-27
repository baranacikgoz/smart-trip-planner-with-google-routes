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

    private Dictionary<ChargePoint, List<Way>>? _adjDict;
    public Task<Dictionary<ChargePoint, List<Way>>> GetAdjacencyDictAsync() => Task.FromResult(_adjDict ?? throw new InvalidOperationException("Not initialized yet."));
    public Task ReconstructFrom(Dictionary<ChargePoint, List<Way>> adjacencyDict)
    {
        _adjDict = adjacencyDict;
        return Task.CompletedTask;
    }

    public Task AddEdgeAsync(ChargePoint from, Way edge) => _decoree.AddEdgeAsync(from, edge);
    public Task AddNodeAsync(ChargePoint node) => _decoree.AddNodeAsync(node);

    public async Task EnsureInitializedAsync()
    {
        await _decoree.EnsureInitializedAsync();
        _adjDict = await _decoree.GetAdjacencyDictAsync();
        await ReconstructWithRealRoutesAsync();
    }

    private async Task ReconstructWithRealRoutesAsync()
    {
        var oldGraph = await GetAdjacencyDictAsync();
        var newGraphWithRealRoutes = new Dictionary<ChargePoint, List<Way>>();

        using var throttler = new SemaphoreSlim(initialCount: 8); // Limit concurrency otherwise both redis and httpclient throws exceptions
        var tasks = new List<Task>();

        foreach (var (chargePoint, edges) in oldGraph)
        {
            newGraphWithRealRoutes[chargePoint] = new List<Way>();

            foreach (var edge in edges)
            {
                await throttler.WaitAsync();
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var newEdge = await CalculateNewEdgeWithRealRoute(chargePoint, edge.To);
                        lock (newGraphWithRealRoutes)
                        {
                            newGraphWithRealRoutes[chargePoint].Add(newEdge.Edge);
                        }
                    }
                    finally
                    {
                        throttler.Release();
                    }
                }));
            }
        }

        await Task.WhenAll(tasks);
        await ReconstructFrom(newGraphWithRealRoutes);
    }

    private async Task<(ChargePoint From, Way Edge)> CalculateNewEdgeWithRealRoute(ChargePoint from, ChargePoint to)
    {
        var (duration, distanceMeters) = await _routesService.GetDurationAndDistanceOnlyAsync(
                                                                    new(from.Latitude, from.Longitude),
                                                                    new(to.Latitude, to.Longitude),
                                                                    TrafficAwareness.NonTrafficAware // We are constructing a graph, not doing a route finding at this time.
                                                                    );

        return (from, new Way(to, duration, distanceMeters));
    }
}
