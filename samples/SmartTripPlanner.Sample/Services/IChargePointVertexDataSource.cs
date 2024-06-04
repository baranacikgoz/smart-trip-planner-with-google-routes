using SmartTripPlanner.ChargePoints.Models;
using SmartTripPlanner.Core.DataSource.Interfaces;

namespace SmartTripPlanner.API.Services;

public interface IChargePointVertexDataSource : IVertexDataSource<ChargePoint, ChargePointBarcode>
{
}
