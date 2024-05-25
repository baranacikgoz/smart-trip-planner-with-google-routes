using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using SmartTripPlanner.Core.Graph;

namespace SmartTripPlanner.Core.Cache;

public class GraphCache<TVertex, TVertexId, TEdge>(IDistributedCache _cache)
    : Cache<Dictionary<TVertex, List<TEdge>>>(_cache), IGraphCache<TVertex, TVertexId, TEdge>
    where TVertex : IVertex<TVertexId>
    where TVertexId : StronglyTypedVertexId
    where TEdge : Edge<TVertex, TVertexId>
{
}
