using System.Text.Json.Serialization;
using SmartTripPlanner.ChargePoints.JsonConverters;
using SmartTripPlanner.Core.Graph;

namespace SmartTripPlanner.ChargePoints.Models;

[JsonConverter(typeof(ChargePointJsonConverter))]
public sealed record ChargePoint(ChargePointBarcode Barcode, double Latitude, double Longitude, bool IsDc) : IVertex<ChargePointBarcode>
{
    public ChargePointBarcode VertexId { get; } = Barcode;
    public double Latitude { get; } = Math.Round(Latitude, 4);
    public double Longitude { get; } = Math.Round(Longitude, 4);
}
