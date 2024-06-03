using System;
using System.Collections.Generic;
using System.Text;
using SmartTripPlanner.Core.DistanceCalculator.Interfaces;
using SmartTripPlanner.Core.Routing.Models;

namespace SmartTripPlanner.Core.DistanceCalculator.Services;
public class CrowFlightDistanceCalculator : ICrowFlightDistanceCalculator
{
    public double CalculateDistanceMeters(LatLng origin, LatLng destination)
    {
        var R = 6371e3; // Earth's radius in meters
        var φ1 = origin.Latitude * Math.PI / 180;
        var φ2 = destination.Latitude * Math.PI / 180;
        var Δφ = (destination.Latitude - origin.Latitude) * Math.PI / 180;
        var Δλ = (destination.Longitude - origin.Longitude) * Math.PI / 180;

        var a = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) +
                Math.Cos(φ1) * Math.Cos(φ2) *
                Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        var distance = R * c; // in meters

        return distance;
    }
}
