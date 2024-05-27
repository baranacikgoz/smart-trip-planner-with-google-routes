using System.Text.Json;
using System.Xml.Linq;
using SmartTripPlanner.Core.Cache;
using SmartTripPlanner.Core.JsonConverters;

namespace SmartTripPlanner.Core.Graph;

public class CachedGraph<TVertex, TVertexId, TEdge> : IGraph<TVertex, TVertexId, TEdge>
    where TVertex : IVertex<TVertexId>
    where TVertexId : StronglyTypedVertexId, new()
    where TEdge : Edge<TVertex, TVertexId>
{
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true,
    };

    private readonly IGraph<TVertex, TVertexId, TEdge> _decoree;
    private readonly IGraphCache<TVertex, TVertexId, TEdge> _cache;
    private readonly string _cacheKey;

    public CachedGraph(
        IGraph<TVertex, TVertexId, TEdge> decoree,
        IGraphCache<TVertex, TVertexId, TEdge> cache,
        string cacheKey)
    {
        _decoree = decoree;
        _cache = cache;
        _cacheKey = cacheKey;
    }

    public async Task<Dictionary<TVertex, List<TEdge>>> GetAdjacencyDictAsync()
        => await _cache.GetAsync(
                            _cacheKey,
                             serializerOptions: _serializerOptions) ?? throw new InvalidOperationException("Not found in cache.");

    public async Task EnsureInitializedAsync()
    {
        if (await _cache.GetAsync(_cacheKey, serializerOptions: _serializerOptions) is null)
        {
            await _decoree.EnsureInitializedAsync();
            var decoreeGraph = await _decoree.GetAdjacencyDictAsync();
            await _cache.SetAsync(_cacheKey, decoreeGraph);
        }
    }

    public async Task ReconstructFrom(Dictionary<TVertex, List<TEdge>> adjacencyDict)
    {
        await _cache.SetAsync(_cacheKey, adjacencyDict);
    }

    public async Task AddNodeAsync(TVertex node)
    {
        var graph = await GetAdjacencyDictAsync();
        graph.TryAdd(node, []);
        await _cache.SetAsync(_cacheKey, graph);
    }
    public async Task AddEdgeAsync(TVertex from, TEdge edge)
    {
        var graph = await GetAdjacencyDictAsync();
        graph.TryAdd(from, []);
        graph[from].Add(edge);

        await _cache.SetAsync(_cacheKey, graph);
    }
}

