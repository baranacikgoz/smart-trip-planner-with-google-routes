namespace SmartTripPlanner.Core.Routes.Models;

public sealed record LatLng(double Latitude, double Longitude)
{
    public override string ToString() => $"{Latitude}, {Longitude}";
}
