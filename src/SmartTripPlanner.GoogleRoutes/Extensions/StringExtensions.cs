using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using SmartTripPlanner.Core.Routes.Interfaces;
using SmartTripPlanner.GoogleRoutes.Constants;

namespace SmartTripPlanner.GoogleRoutes.Extensions;
internal static class StringExtensions
{
    public static TimeSpan ToTimeSpan(this string duration)
        => duration
            .AsSpan()
            .ToTimeSpan();
    private static TimeSpan ToTimeSpan(this ReadOnlySpan<char> duration)
        => duration[^1] switch
        {
            's' => TimeSpan.FromSeconds(duration.ToDurationDouble()),
            'h' => TimeSpan.FromHours(duration.ToDurationDouble()),
            'd' => TimeSpan.FromDays(duration.ToDurationDouble()),
            _ => throw new NotImplementedException($"Unimplemented duration type: {duration[^1]}")
        };

    private static double ToDurationDouble(this ReadOnlySpan<char> duration)
        => double.Parse(duration[..^1], provider: CultureInfo.InvariantCulture);
}
