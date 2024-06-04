using SmartTripPlanner.ChargePoints.Models;
using SmartTripPlanner.API.Repositories;

namespace SmartTripPlanner.API.Services;

public class ChargePointVertexDataSource(
    IChargePointRepository _repository
    ) : IChargePointVertexDataSource
{
    public async Task<IReadOnlyList<ChargePoint>> GetAllAsync()
        => await Task.Run(() => _repository.GetAll());
}
