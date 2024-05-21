using SmartTripPlanner.ChargePoints.Models;
using SmartTripPlanner.Core.DataSource.Interfaces;
using SmartTripPlanner.Core.Graph;
using SmartTripPlanner.Core.Routes.Interfaces;
using SmartTripPlanner.Core.Routes.Models;

namespace SmartTripPlanner.ChargePoints.Graphs;

public class ChargePointGraph : GraphBase<ChargePointBarcode, Way>, IChargePointGraph
{
    private readonly IVertexDataSource<ChargePoint, ChargePointBarcode> _chargePointDataSource;
    private readonly IRoutesService _routesService;

    public ChargePointGraph(IVertexDataSource<ChargePoint, ChargePointBarcode> chargePointDataSource, IRoutesService routesService)
    {
        _chargePointDataSource = chargePointDataSource;
        _routesService = routesService;
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

        var tasks = new List<Task<(ChargePointBarcode From, Way Edge)>>(chargePoints.Count * chargePoints.Count);
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

                tasks.Add(CalculateRouteAndAddEdgeAsync(from, to));
            }
        }

        var results = await Task.WhenAll(tasks);

        foreach (var result in results)
        {
            await AddEdgeAsync(result.From, result.Edge);
        }
    }

    public override ValueTask ReconstructFrom(Dictionary<ChargePointBarcode, List<Way>> adjacencyDict)
    {
        _adjacencyDict = adjacencyDict;
        return new();
    }

    private async Task<(ChargePointBarcode From, Way Edge)> CalculateRouteAndAddEdgeAsync(ChargePoint from, ChargePoint to)
    {
        var route = await _routesService.GetRoutesAsync(new LatLng(from.Latitude, from.Longitude),
                                                        new LatLng(to.Latitude, to.Longitude));

        return (from.Barcode, new Way(to.Barcode, route.Duration, route.DistanceMeters));
    }
}
