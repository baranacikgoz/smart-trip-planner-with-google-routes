using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SmartTripPlanner.Core.Cache;
using SmartTripPlanner.Core.Graph;

namespace SmartTripPlanner.Core.Setup.GraphSetup;
public class GraphImplementationConfigBuilder<TGraphInterface, TVertexId, TEdge>(IServiceCollection services)
    : IGraphImplementationConfigBuilder<TGraphInterface, TVertexId, TEdge>
    where TVertexId : StronglyTypedVertexId
    where TEdge : Edge
    where TGraphInterface : class, IGraph<TVertexId, TEdge>
{
    public IServiceCollection Services { get; } = services;
}
