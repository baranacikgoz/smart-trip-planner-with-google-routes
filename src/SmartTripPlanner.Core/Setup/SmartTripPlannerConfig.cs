using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SmartTripPlanner.Core.Cache;
using SmartTripPlanner.Core.DistanceCalculator.Interfaces;

namespace SmartTripPlanner.Core.Setup;
public class SmartTripPlannerConfig(IServiceCollection Services) : ISmartTripPlannerConfigBuilder
{
    public IServiceCollection Services { get; } = Services;

    internal void RegisterServicesInternal()
    {
        // End user is free to use any kind of IDistributedCache implementation such as StackExchangeRedis.
        // But if there is not implementation at all, we add DistributedMemoryCache as fallback.
        if (!Services.Any(serviceDescriptor => serviceDescriptor.ServiceType == typeof(IDistributedCache)))
        {
            // No IDistributedCache registered, so add default In-Memory Cache
            Services.AddDistributedMemoryCache();
        }

        Services.AddSingleton(typeof(IGraphCache<,,>), typeof(GraphCache<,,>));
        Services.AddSingleton(typeof(ICache<>), typeof(Cache<>));
        Services.AddSingleton<ICache, Cache.Cache>();

        Services.AddSingleton<ICrowFlightDistanceCalculator, DistanceCalculator.Services.CrowFlightDistanceCalculator>();
    }
}
