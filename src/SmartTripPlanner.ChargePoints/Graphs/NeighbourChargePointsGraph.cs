using SmartTripPlanner.ChargePoints.Models;
using SmartTripPlanner.Core.DataSource.Interfaces;

namespace SmartTripPlanner.ChargePoints.Graphs;

/// <summary>
/// This class represents the processed version of the <see cref="ChargePointsGraph"/>.
/// We reduce edge counts of nodes by reconstructing the graph with edging only neighbour ChargePoints in terms of locations.
/// </summary>
public class NeighbourChargePointsGraph : IChargePointGraph
{
    private readonly IChargePointGraph _decoree;
    private readonly int _numberOfNeighboursPerChargePoint;
    private readonly IVertexDataSource<ChargePoint, ChargePointBarcode> _chargePointDataSource;
    public NeighbourChargePointsGraph(
        IChargePointGraph decoree,
        int numberOfNeighboursPerChargePoint,
        IVertexDataSource<ChargePoint, ChargePointBarcode> chargePointDataSource)
    {
        _decoree = decoree;
        _numberOfNeighboursPerChargePoint = numberOfNeighboursPerChargePoint;
        _chargePointDataSource = chargePointDataSource;
    }

    public ValueTask<Dictionary<ChargePoint, List<Way>>> GetAdjacencyDictAsync()
        => _decoree.GetAdjacencyDictAsync();
    public ValueTask ReconstructFrom(Dictionary<ChargePoint, List<Way>> adjacencyDict)
        => _decoree.ReconstructFrom(adjacencyDict);

    public Task AddEdgeAsync(ChargePoint from, Way edge) => _decoree.AddEdgeAsync(from, edge);

    public Task AddNodeAsync(ChargePoint node) => _decoree.AddNodeAsync(node);

    public async Task EnsureInitializedAsync()
    {
        await _decoree.EnsureInitializedAsync();

        await ReconstructWithNeighbourChargePointsOnly();
    }

    // Initial graph might have all possible edge combination with all ChargePoints which is highly inefficient.
    // We will reconstruct the graph so that only neighbour ChargePoints will be connected together.
    private async ValueTask ReconstructWithNeighbourChargePointsOnly()
    {
        var graph = await _decoree.GetAdjacencyDictAsync();
        var barcodes = graph.Keys.Select(cp => cp.Barcode).ToList();
        var chargePoints = await _chargePointDataSource.GetByIds(barcodes);

        // Clear existing graph
        var newAdjacencyDict = new Dictionary<ChargePoint, List<Way>>();

        foreach (var from in chargePoints)
        {
            var distances = new List<(ChargePoint to, double distance)>();

            foreach (var to in chargePoints)
            {
                if (from.Equals(to))
                {
                    continue;
                }

                var distance = CalculateDistance(from.Latitude, from.Longitude, to.Latitude, to.Longitude);
                distances.Add((to, distance));
            }

            // Sort by distance and take the top N nearest neighbors
            var nearestNeighbors = distances.OrderBy(d => d.distance)
                                            .Take(_numberOfNeighboursPerChargePoint)
                                            .ToList();

            // Didn't use calculated distance to construct Way on purpose.
            // These are initial lat,long distances, not google route calculated actual pyhsichal way's distances.
            // We just initally select neighbours, then calculate actual routes with another decorator class.
            newAdjacencyDict[from] = nearestNeighbors
                                                .Select(nn => new Way(nn.to, TimeSpan.MaxValue, double.MaxValue))
                                                .ToList();
        }

        await _decoree.ReconstructFrom(newAdjacencyDict);
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
