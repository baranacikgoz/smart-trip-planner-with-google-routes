using SmartTripPlanner.ChargePoints.Models;

namespace SmartTripPlanner.ChargePoints.Graphs;

/// <summary>
/// This class represents the processed version of the <see cref="ChargePointGraph"/>.
/// We reduce edge counts of nodes by reconstructing the graph with edging only neighbour ChargePoints in terms of locations.
/// </summary>
public class NeighbourChargePointsGraph : IChargePointGraph
{
    private readonly IChargePointGraph _decoree;
    private readonly int _numberOfNeighboursPerChargePoint;
    public NeighbourChargePointsGraph(IChargePointGraph decoree, int numberOfNeighboursPerChargePoint)
    {
        _decoree = decoree;
        _numberOfNeighboursPerChargePoint = numberOfNeighboursPerChargePoint;
    }

    public ValueTask<Dictionary<ChargePointBarcode, List<Way>>> GetAdjacencyDictAsync()
        => _decoree.GetAdjacencyDictAsync();
    public ValueTask ReconstructFrom(Dictionary<ChargePointBarcode, List<Way>> adjacencyDict)
        => _decoree.ReconstructFrom(adjacencyDict);

    public Task AddEdgeAsync(ChargePointBarcode from, Way edge) => _decoree.AddEdgeAsync(from, edge);

    public Task AddNodeAsync(ChargePointBarcode node) => _decoree.AddNodeAsync(node);

    public async Task EnsureInitializedAsync()
    {
        await _decoree.EnsureInitializedAsync();

        await ReconstructWithNeighbourChargePointsOnly();
    }

    // Initial graph might have all possible edge combination with all ChargePoints which is highly inefficient.
    // We will reconstruct the graph so that only neighbour ChargePoints will be connected together.
    private async ValueTask ReconstructWithNeighbourChargePointsOnly()
    {
        Console.WriteLine(_numberOfNeighboursPerChargePoint);

        var reconstructed = new Dictionary<ChargePointBarcode, List<Way>>();

        await _decoree.ReconstructFrom(reconstructed);
    }
}
