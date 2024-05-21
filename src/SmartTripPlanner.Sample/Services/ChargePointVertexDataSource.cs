using SmartTripPlanner.ChargePoints.Models;
using SmartTripPlanner.Sample.Repositories;

namespace SmartTripPlanner.Sample.Services;

public class ChargePointVertexDataSource(
    IChargePointRepository _repository
    ) : IChargePointVertexDataSource
{
    public async Task<IReadOnlyList<ChargePoint>> GetAllAsync() => await Task.Run(() => _repository.GetAll());
}
