using SmartTripPlanner.ChargePoints.Models;

namespace SmartTripPlanner.API.Repositories;

public interface IChargePointRepository
{
    IReadOnlyList<ChargePoint> GetAll();
    IReadOnlyList<ChargePoint> GetByBarcodes(ICollection<ChargePointBarcode> barcodes);
    ChargePoint? GetByBarcode(ChargePointBarcode barcode);
}
