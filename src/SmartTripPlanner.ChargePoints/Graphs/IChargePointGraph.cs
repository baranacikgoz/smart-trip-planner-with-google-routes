using SmartTripPlanner.ChargePoints.Models;
using SmartTripPlanner.Core.Graph;

namespace SmartTripPlanner.ChargePoints.Graphs;

public interface IChargePointGraph : IGraph<ChargePointBarcode, Way>
{
}
