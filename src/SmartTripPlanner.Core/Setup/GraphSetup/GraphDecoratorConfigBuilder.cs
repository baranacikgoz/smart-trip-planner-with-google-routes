using Microsoft.Extensions.DependencyInjection;
using SmartTripPlanner.Core.Graph;

namespace SmartTripPlanner.Core.Setup.GraphSetup;

public class GraphDecoratorConfigBuilder<TGraphInterface, TVertexId, TEdge>(IServiceCollection services)
    : IGraphDecoratorConfigBuilder<TGraphInterface, TVertexId, TEdge>
    where TVertexId : StronglyTypedVertexId
    where TEdge : Edge
    where TGraphInterface : class, IGraph<TVertexId, TEdge>
{
    public IServiceCollection Services { get; } = services;
}
