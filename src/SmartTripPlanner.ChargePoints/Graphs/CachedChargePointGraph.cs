using System;
using System.Collections.Generic;
using System.Text;
using SmartTripPlanner.ChargePoints.Cache;
using SmartTripPlanner.ChargePoints.Models;
using SmartTripPlanner.Core.Cache;
using SmartTripPlanner.Core.Graph;

namespace SmartTripPlanner.ChargePoints.Graphs;
public class CachedChargePointGraph : CachedGraph<ChargePointBarcode, Way>, IChargePointGraph
{
    public CachedChargePointGraph(IGraph<ChargePointBarcode, Way> decoree, IChargePointGraphCache cache, string cacheKey)
        : base(decoree, cache, cacheKey)
    {
    }
}
