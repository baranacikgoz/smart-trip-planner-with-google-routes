using System.Globalization;
using SmartTripPlanner.ChargePoints.Models;
using SmartTripPlanner.API.Services;

namespace SmartTripPlanner.API.Repositories;

// data source is the chargepoints.csv, not db for simplicity
public class ChargePointCsvRepository(CsvService csvService) : IChargePointRepository
{
    private const string CsvPath = "chargepoints.csv";
    private readonly List<ChargePoint> _chargePoints = csvService.ReadFromCsv(CsvPath, fields => new ChargePoint(
                                                                        Barcode: new ChargePointBarcode(fields[0]),
                                                                        Latitude: double.Parse(fields[1], CultureInfo.InvariantCulture),
                                                                        Longitude: double.Parse(fields[2], CultureInfo.InvariantCulture),
                                                                        IsDc: bool.Parse(fields[3])))
                                                                  .ToList();
    public IReadOnlyList<ChargePoint> GetAll() => _chargePoints.AsReadOnly();
    public IReadOnlyList<ChargePointBarcode> GetAllBarcodes()
        => _chargePoints
             .Select(c => c.Barcode)
             .ToList()
             .AsReadOnly();
    public IReadOnlyList<ChargePoint> GetByBarcodes(ICollection<ChargePointBarcode> barcodes)
        => _chargePoints
            .Where(c => barcodes.Contains(c.Barcode))
            .ToList();

    public ChargePoint? GetByBarcode(ChargePointBarcode barcode)
        => _chargePoints
            .SingleOrDefault(c => c.Barcode == barcode);
}
