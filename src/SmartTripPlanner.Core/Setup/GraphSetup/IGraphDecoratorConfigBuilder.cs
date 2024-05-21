using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using SmartTripPlanner.Core.Graph;

namespace SmartTripPlanner.Core.Setup.GraphSetup;
#pragma warning disable S2326 // Unused type parameters should be removed
public interface IGraphDecoratorConfigBuilder<TGraphInterface, TVertexId, TEdge> : ISmartTripPlannerConfigBuilder
#pragma warning restore S2326 // Unused type parameters should be removed
    where TVertexId : StronglyTypedVertexId
    where TEdge : Edge
    where TGraphInterface : class, IGraph<TVertexId, TEdge>
{
}
