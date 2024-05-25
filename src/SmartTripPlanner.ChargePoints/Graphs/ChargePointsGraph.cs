using SmartTripPlanner.ChargePoints.Models;
using SmartTripPlanner.Core.DataSource.Interfaces;
using SmartTripPlanner.Core.Graph;
using SmartTripPlanner.Core.Routes.Interfaces;
using SmartTripPlanner.Core.Routes.Models;

namespace SmartTripPlanner.ChargePoints.Graphs;

public class ChargePointsGraph : GraphBase<ChargePoint, ChargePointBarcode, Way>, IChargePointGraph
{
    private readonly IVertexDataSource<ChargePoint, ChargePointBarcode> _chargePointDataSource;

    public ChargePointsGraph(IVertexDataSource<ChargePoint, ChargePointBarcode> chargePointDataSource)
    {
        _chargePointDataSource = chargePointDataSource;
    }

    private Dictionary<ChargePoint, List<Way>> _adjacencyDict = [];

    public override ValueTask<Dictionary<ChargePoint, List<Way>>> GetAdjacencyDictAsync()
        => new(_adjacencyDict);

    public override async Task EnsureInitializedAsync()
    {
        if (_adjacencyDict.Count == 0)
        {
            await InitializeChargePointsGraph();
        }
    }

    private async Task InitializeChargePointsGraph()
    {
        var chargePoints = await _chargePointDataSource.GetAllAsync();

        for (var i = 0; i < chargePoints.Count; i++)
        {
            var from = chargePoints[i];

            for (var j = 0; j < chargePoints.Count; j++)
            {
                var to = chargePoints[j];
                if (from.Equals(to))
                {
                    continue;
                }

                // Edge(Way) is added with placeholder values those will be calculated by google routes later.
                await AddEdgeAsync(from, new Way(to, TimeSpan.MaxValue, double.MaxValue));
            }
        }
    }

    public override ValueTask ReconstructFrom(Dictionary<ChargePoint, List<Way>> adjacencyDict)
    {
        _adjacencyDict = adjacencyDict;
        return new();
    }
}
