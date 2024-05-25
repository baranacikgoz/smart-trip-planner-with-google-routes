using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using SmartTripPlanner.Core.Graph;

namespace SmartTripPlanner.Core.Cache;
public interface IGraphCache<TVertex, TVertexId, TEdge> : ICache<Dictionary<TVertex, List<TEdge>>>
    where TVertex : IVertex<TVertexId>
    where TVertexId : StronglyTypedVertexId
    where TEdge : Edge<TVertex, TVertexId>
{
}
