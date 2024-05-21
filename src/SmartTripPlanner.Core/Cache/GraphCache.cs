using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using SmartTripPlanner.Core.Graph;

namespace SmartTripPlanner.Core.Cache;

public class GraphCache<TVertexId, TEdge>(IDistributedCache _cache)
    : Cache<Dictionary<TVertexId, List<TEdge>>>(_cache), IGraphCache<TVertexId, TEdge>
    where TVertexId : StronglyTypedVertexId
    where TEdge : Edge
{
}
