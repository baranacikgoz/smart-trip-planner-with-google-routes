using System.Text.Json;
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
        => await _cache.GetOrSetAsync(
                            _cacheKey,
                            async () => await _decoree.GetAdjacencyDictAsync(),
                            serializerOptions: _serializerOptions);

    public async Task ReconstructFrom(Dictionary<TVertex, List<TEdge>> adjacencyDict)
    {
        await _decoree.ReconstructFrom(adjacencyDict);
        await RefreshCacheAsync();
    }

    public async Task EnsureInitializedAsync()
    {
        if (await _cache.GetAsync(_cacheKey, serializerOptions: _serializerOptions) is null)
        {
            await _decoree.EnsureInitializedAsync();
            await RefreshCacheAsync();
        }
    }

    public async Task AddNodeAsync(TVertex node)
    {
        await _decoree.AddNodeAsync(node);
        await RefreshCacheAsync();
    }
    public async Task AddEdgeAsync(TVertex from, TEdge edge)
    {
        await _decoree.AddEdgeAsync(from, edge);
        await RefreshCacheAsync();
    }

    private async Task RefreshCacheAsync()
        => await _cache.SetAsync(
                            cacheKey: _cacheKey,
                            func: async () => await _decoree.GetAdjacencyDictAsync(),
                            serializerOptions: _serializerOptions);
}

