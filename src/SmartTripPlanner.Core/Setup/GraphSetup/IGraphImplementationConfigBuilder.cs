using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using SmartTripPlanner.Core.Graph;

namespace SmartTripPlanner.Core.Setup.GraphSetup;
#pragma warning disable S2326 // Unused type parameters should be removed
public interface IGraphImplementationConfigBuilder<TGraphInterface, TVertex, TVertexId, TEdge> : ISmartTripPlannerConfigBuilder
#pragma warning restore S2326 // Unused type parameters should be removed
    where TVertex : IVertex<TVertexId>
    where TVertexId : StronglyTypedVertexId
    where TEdge : Edge<TVertex, TVertexId>
    where TGraphInterface : class, IGraph<TVertex, TVertexId, TEdge>
{
}
