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
public class GraphImplementationConfigBuilder<TGraphInterface, TVertex, TVertexId, TEdge>(IServiceCollection services)
    : IGraphImplementationConfigBuilder<TGraphInterface, TVertex, TVertexId, TEdge>
    where TVertex : IVertex<TVertexId>
    where TVertexId : StronglyTypedVertexId
    where TEdge : Edge<TVertex, TVertexId>
    where TGraphInterface : class, IGraph<TVertex, TVertexId, TEdge>
{
    public IServiceCollection Services { get; } = services;
}
