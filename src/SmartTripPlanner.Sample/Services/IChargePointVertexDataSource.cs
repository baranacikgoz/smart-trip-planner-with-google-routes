using SmartTripPlanner.ChargePoints.Models;
using SmartTripPlanner.Core.DataSource.Interfaces;

namespace SmartTripPlanner.Sample.Services;

public interface IChargePointVertexDataSource : IVertexDataSource<ChargePoint, ChargePointBarcode>
{
}
