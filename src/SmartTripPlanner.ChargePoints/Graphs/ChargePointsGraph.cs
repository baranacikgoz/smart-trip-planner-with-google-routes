using SmartTripPlanner.ChargePoints.Models;
using SmartTripPlanner.Core.DataSource.Interfaces;
using SmartTripPlanner.Core.Graph;
using SmartTripPlanner.Core.Routes.Interfaces;
using SmartTripPlanner.Core.Routes.Models;

namespace SmartTripPlanner.ChargePoints.Graphs;

public class ChargePointsGraph : GraphBase<ChargePointBarcode, Way>, IChargePointGraph
{
    private readonly IVertexDataSource<ChargePoint, ChargePointBarcode> _chargePointDataSource;

    public ChargePointsGraph(IVertexDataSource<ChargePoint, ChargePointBarcode> chargePointDataSource)
    {
        _chargePointDataSource = chargePointDataSource;
    }

    private Dictionary<ChargePointBarcode, List<Way>> _adjacencyDict = [];

    public override ValueTask<Dictionary<ChargePointBarcode, List<Way>>> GetAdjacencyDictAsync()
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
                await AddEdgeAsync(from.Barcode, new Way(to.Barcode, TimeSpan.MaxValue, double.MaxValue));
            }
        }
    }

    public override ValueTask ReconstructFrom(Dictionary<ChargePointBarcode, List<Way>> adjacencyDict)
    {
        _adjacencyDict = adjacencyDict;
        return new();
    }

    //private async Task<(ChargePointBarcode From, Way Edge)> CalculateRouteAndAddEdgeAsync(ChargePoint from, ChargePoint to)
    //{
    //    var route = await _routesService.GetRoutesAsync(new LatLng(from.Latitude, from.Longitude),
    //                                                    new LatLng(to.Latitude, to.Longitude));

    //    return (from.Barcode, new Way(to.Barcode, route.Duration, route.DistanceMeters));
    //}
}
