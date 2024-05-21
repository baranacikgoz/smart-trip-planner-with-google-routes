using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SmartTripPlanner.Core.Setup;
public interface ISmartTripPlannerConfigBuilder
{
    IServiceCollection Services { get; }
}
