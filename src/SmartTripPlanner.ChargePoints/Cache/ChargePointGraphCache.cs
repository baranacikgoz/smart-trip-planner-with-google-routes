using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using SmartTripPlanner.ChargePoints.Models;
using SmartTripPlanner.Core.Cache;

namespace SmartTripPlanner.ChargePoints.Cache;
public class ChargePointGraphCache(IDistributedCache _cache)
    : GraphCache<ChargePoint, ChargePointBarcode, Way>(_cache), IChargePointGraphCache
{
}
