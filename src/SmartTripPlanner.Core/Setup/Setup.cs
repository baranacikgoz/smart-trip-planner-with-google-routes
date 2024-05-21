using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartTripPlanner.Core.Graph;
using SmartTripPlanner.Core.Setup.GraphSetup;

namespace SmartTripPlanner.Core.Setup;
public static class Setup
{
    public static IServiceCollection AddSmartTripPlanner(
        this IServiceCollection services,
        Action<SmartTripPlannerConfig> configAction)
    {
        var config = new SmartTripPlannerConfig(services);

        configAction(config);

        config.RegisterServicesInternal();

        return services;
    }
}
