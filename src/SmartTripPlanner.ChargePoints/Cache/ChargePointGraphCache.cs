using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using SmartTripPlanner.ChargePoints.Models;
using SmartTripPlanner.Core.Cache;

namespace SmartTripPlanner.ChargePoints.Cache;
public class ChargePointGraphCache : GraphCache<ChargePointBarcode, Way>, IChargePointGraphCache
{
    public ChargePointGraphCache(IDistributedCache _cache) : base(_cache)
    {
    }
}
