using SmartTripPlanner.WebAPI.Models;

namespace SmartTripPlanner.WebAPI.Services;

public class PolylineDecoderService
{
    public ICollection<LatLng> Decode(string encodedPolyline)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(encodedPolyline);

        var polylineChars = encodedPolyline.ToCharArray();
        var index = 0;
        var currentLat = 0;
        var currentLng = 0;
        var coordinates = new List<LatLng>();

        while (index < polylineChars.Length)
        {
            // Decode latitude
            var sum = 0;
            var shifter = 0;
            int next5Bits;
            do
            {
                next5Bits = polylineChars[index++] - 63;
                sum |= (next5Bits & 31) << shifter;
                shifter += 5;
            } while (next5Bits >= 32 && index < polylineChars.Length);

            if (index >= polylineChars.Length)
            {
                break;
            }

            currentLat += (sum & 1) == 1 ? ~(sum >> 1) : sum >> 1;

            // Decode longitude
            sum = 0;
            shifter = 0;
            do
            {
                next5Bits = polylineChars[index++] - 63;
                sum |= (next5Bits & 31) << shifter;
                shifter += 5;
            } while (next5Bits >= 32 && index < polylineChars.Length);

            currentLng += (sum & 1) == 1 ? ~(sum >> 1) : sum >> 1;

            coordinates.Add(new LatLng(currentLat / 1E5, currentLng / 1E5));
        }

        return coordinates;
    }
}
