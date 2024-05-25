using SmartTripPlanner.ChargePoints.Models;
using SmartTripPlanner.Sample.Repositories;

namespace SmartTripPlanner.Sample.Services;

public class ChargePointVertexDataSource(
    IChargePointRepository _repository
    ) : IChargePointVertexDataSource
{
    public async Task<IReadOnlyList<ChargePoint>> GetAllAsync()
        => await Task.Run(() => _repository.GetAll());

    public async Task<ChargePoint?> GetById(ChargePointBarcode id)
        => await Task.Run(() => _repository.GetByBarcode(id));

    public async Task<IReadOnlyList<ChargePoint>> GetByIds(ICollection<ChargePointBarcode> ids)
        => await Task.Run(() => _repository.GetByBarcodes(ids));
}
