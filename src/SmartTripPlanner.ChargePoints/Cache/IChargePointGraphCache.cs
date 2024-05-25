using System;
using System.Collections.Generic;
using System.Text;
using SmartTripPlanner.ChargePoints.Models;
using SmartTripPlanner.Core.Cache;

namespace SmartTripPlanner.ChargePoints.Cache;
public interface IChargePointGraphCache : IGraphCache<ChargePoint, ChargePointBarcode, Way>
{
}
