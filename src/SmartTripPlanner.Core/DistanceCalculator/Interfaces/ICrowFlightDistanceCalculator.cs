using System;
using System.Collections.Generic;
using System.Text;
using SmartTripPlanner.Core.Routing.Models;

namespace SmartTripPlanner.Core.DistanceCalculator.Interfaces;

/// <summary>
/// Used to calculate the distance between two points on the Earth as if it were a straight line.
/// Use this to calculations you don't want use Google Maps API for (hence reducing costs).
/// </summary>
public interface ICrowFlightDistanceCalculator
{
    double CalculateDistanceMeters(LatLng origin, LatLng destination);
}
