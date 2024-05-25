using System;
using System.Collections.Generic;
using System.Text;
using SmartTripPlanner.ChargePoints.Cache;
using SmartTripPlanner.ChargePoints.Models;
using SmartTripPlanner.Core.Cache;
using SmartTripPlanner.Core.Graph;

namespace SmartTripPlanner.ChargePoints.Graphs;
public class CachedChargePointGraph : CachedGraph<ChargePoint, ChargePointBarcode, Way>, IChargePointGraph
{
    public CachedChargePointGraph(IGraph<ChargePoint, ChargePointBarcode, Way> decoree, IChargePointGraphCache cache, string cacheKey)
        : base(decoree, cache, cacheKey)
    {
    }
}
