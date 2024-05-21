using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using SmartTripPlanner.Core.Routes.Interfaces;
using SmartTripPlanner.Core.Routes.Models;

namespace SmartTripPlanner.GoogleRoutes;

public class GoogleRoutesPolylineDecoder(ILogger<GoogleRoutesPolylineDecoder> _logger) : IPolyLineDecoder
{
    public IEnumerable<LatLng> Decode(string encodedPolyline)
    {
        if (string.IsNullOrWhiteSpace(encodedPolyline))
        {
            _logger.LogEncodedPolylineIsNullOrEmpty();
            yield break;
        }

        var polylineChars = encodedPolyline.ToCharArray();
        var index = 0;
        var currentLat = 0;
        var currentLng = 0;

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

            yield return new LatLng(currentLat / 1E5, currentLng / 1E5);
        }
    }
}

internal static partial class LoggerExtensions
{
    [LoggerMessage(
    Level = LogLevel.Warning,
    Message = "EncodedPolyline is null or empty. Returning early...")]
    public static partial void LogEncodedPolylineIsNullOrEmpty(this ILogger<GoogleRoutesPolylineDecoder> logger);
}
