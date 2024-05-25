using Microsoft.Extensions.DependencyInjection;
using SmartTripPlanner.Core.Graph;

namespace SmartTripPlanner.Core.Setup.GraphSetup;

public class GraphDecoratorConfigBuilder<TGraphInterface, TVertex, TVertexId, TEdge>(IServiceCollection services)
    : IGraphDecoratorConfigBuilder<TGraphInterface, TVertex, TVertexId, TEdge>
    where TVertex : IVertex<TVertexId>
    where TVertexId : StronglyTypedVertexId
    where TEdge : Edge<TVertex, TVertexId>
    where TGraphInterface : class, IGraph<TVertex, TVertexId, TEdge>
{
    public IServiceCollection Services { get; } = services;
}
