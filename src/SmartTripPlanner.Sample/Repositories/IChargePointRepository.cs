using SmartTripPlanner.ChargePoints.Models;

namespace SmartTripPlanner.Sample.Repositories;

public interface IChargePointRepository
{
    IReadOnlyList<ChargePoint> GetAll();
    IReadOnlyList<ChargePointBarcode> GetAllBarcodes();
    IReadOnlyList<ChargePoint> GetByBarcodes(ICollection<ChargePointBarcode> barcodes);
    ChargePoint? GetByBarcode(ChargePointBarcode barcode);
}
