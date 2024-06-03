using System;
using System.Collections.Generic;
using System.Text;
using SmartTripPlanner.ChargePoints.Models;
using SmartTripPlanner.Core.Routing.Models;
using SmartTripPlanner.Core.TripPlanner.Interfaces;

namespace SmartTripPlanner.ChargePoints.TripPlanner.Interfaces;
public interface IChargePointTripPlanner : ITripPlanner<ChargePoint, ChargePointBarcode, Way>
{
    Task<RoutesWithIntermediateWayPoints> PlanTripAsync(
        double initialBatteryPercentage,
        double batteryConsumptionPer100Km,
        double minBatteryThreshold,
        LatLng origin,
        LatLng destination,
        CancellationToken cancellationToken);
}
