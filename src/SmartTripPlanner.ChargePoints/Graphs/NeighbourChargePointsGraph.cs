using Accord.MachineLearning;
using Accord.Math.Distances;
using SmartTripPlanner.ChargePoints.Models;
using SmartTripPlanner.Core.DataSource.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartTripPlanner.ChargePoints.Graphs;

public class NeighbourChargePointsGraph : IChargePointGraph
{
    private readonly IChargePointGraph _decoree;
    private readonly int _numberOfNeighboursPerChargePoint;

    public NeighbourChargePointsGraph(
        IChargePointGraph decoree,
        int numberOfNeighboursPerChargePoint)
    {
        _decoree = decoree;
        _numberOfNeighboursPerChargePoint = numberOfNeighboursPerChargePoint;
    }

    public Task<Dictionary<ChargePoint, List<Way>>> GetAdjacencyDictAsync()
        => _decoree.GetAdjacencyDictAsync();

    public Task ReconstructFrom(Dictionary<ChargePoint, List<Way>> adjacencyDict)
        => _decoree.ReconstructFrom(adjacencyDict);

    public Task AddEdgeAsync(ChargePoint from, Way edge) => _decoree.AddEdgeAsync(from, edge);

    public Task AddNodeAsync(ChargePoint node) => _decoree.AddNodeAsync(node);

    public async Task EnsureInitializedAsync()
    {
        await _decoree.EnsureInitializedAsync();
        await ReconstructWithNeighbourChargePointsOnly();
    }

    private async Task ReconstructWithNeighbourChargePointsOnly()
    {
        var graph = await _decoree.GetAdjacencyDictAsync();
        var chargePoints = graph.Keys.ToList();

        var numChargePoints = chargePoints.Count;
        var initialNumClusters = Math.Max(1, numChargePoints / _numberOfNeighboursPerChargePoint);

        // Cluster the charge points initially
        var clusteredChargePoints = PerformClustering(chargePoints, graph, initialNumClusters);

        var newAdjacencyDict = new Dictionary<ChargePoint, List<Way>>();

        // Connect within clusters
        foreach (var cluster in clusteredChargePoints)
        {
            ConnectWithinCluster(cluster, newAdjacencyDict);
        }

        // Perform hierarchical clustering until a single cluster remains
        while (clusteredChargePoints.Count > 1)
        {
            var clusterCenters = clusteredChargePoints
                                .Select(cluster => GetRepresentativeChargePoint(cluster))
                                .ToList();

            clusteredChargePoints = PerformClustering(
                                        clusterCenters,
                                        graph,
                                        Math.Max(1, clusteredChargePoints.Count / _numberOfNeighboursPerChargePoint));

            foreach (var cluster in clusteredChargePoints)
            {
                ConnectWithinCluster(cluster, newAdjacencyDict);
            }
        }

        await _decoree.ReconstructFrom(newAdjacencyDict);
    }

    private static List<List<ChargePoint>> PerformClustering(List<ChargePoint> chargePoints, Dictionary<ChargePoint, List<Way>> graph, int numClusters)
    {
        var distanceMatrix = chargePoints
                                .Select(cp => graph[cp]
                                                .Select(e => e.DistanceInMeters)
                                                .ToArray())
                                .ToArray();

        var kmeans = new KMeans(numClusters)
        {
            Distance = new SquareEuclidean()
        };

        var clusters = kmeans.Learn(distanceMatrix);

        var clusteredChargePoints = clusters.Decide(distanceMatrix)
                                            .Select((clusterIndex, pointIndex) => new { chargePoint = chargePoints[pointIndex], clusterIndex })
                                            .GroupBy(item => item.clusterIndex)
                                            .Select(group => group.Select(item => item.chargePoint).ToList())
                                            .ToList();

        return clusteredChargePoints;
    }

    private static void ConnectWithinCluster(List<ChargePoint> cluster, Dictionary<ChargePoint, List<Way>> newAdjacencyDict)
    {
        foreach (var from in cluster)
        {
            foreach (var to in cluster)
            {
                if (from.Equals(to))
                {
                    continue;
                }

                var edge = newAdjacencyDict.ContainsKey(from) ? newAdjacencyDict[from].Find(e => e.To.Equals(to)) : null;
                var distance = edge != null ? edge.DistanceInMeters : double.MaxValue;
                var duration = edge != null ? edge.Duration : TimeSpan.MaxValue;

                if (!newAdjacencyDict.ContainsKey(from))
                {
                    newAdjacencyDict[from] = new List<Way>();
                }

                if (!newAdjacencyDict.ContainsKey(to))
                {
                    newAdjacencyDict[to] = new List<Way>();
                }

                newAdjacencyDict[from].Add(new Way(to, duration, distance));
                newAdjacencyDict[to].Add(new Way(from, duration, distance));
            }
        }
    }

    private static ChargePoint GetRepresentativeChargePoint(List<ChargePoint> cluster)
    {
        // Get the centroid of the cluster
        var centroid = GetClusterCentroid(cluster);

        // Find the charge point closest to the centroid
        ChargePoint? representative = null;
        var minDistance = double.MaxValue;

        foreach (var chargePoint in cluster)
        {
            var distance = CalculateDistance(centroid.Latitude, centroid.Longitude, chargePoint.Latitude, chargePoint.Longitude);
            if (distance < minDistance)
            {
                minDistance = distance;
                representative = chargePoint;
            }
        }

        return representative ?? throw new InvalidOperationException("Representative ChargePoint was null.");
    }

    private static ChargePoint GetClusterCentroid(List<ChargePoint> cluster)
    {
        var avgLatitude = cluster.Average(cp => cp.Latitude);
        var avgLongitude = cluster.Average(cp => cp.Longitude);
        return new ChargePoint(new ChargePointBarcode("center"), avgLatitude, avgLongitude, false);
    }

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var R = 6371e3; // Earth's radius in meters
        var φ1 = lat1 * Math.PI / 180;
        var φ2 = lat2 * Math.PI / 180;
        var Δφ = (lat2 - lat1) * Math.PI / 180;
        var Δλ = (lon2 - lon1) * Math.PI / 180;

        var a = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) +
                Math.Cos(φ1) * Math.Cos(φ2) *
                Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        var distance = R * c; // in meters

        return distance;
    }
}
