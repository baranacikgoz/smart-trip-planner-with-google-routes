using System.Text.Json;
using SmartTripPlanner.Core.Cache;
using SmartTripPlanner.Core.JsonConverters;

namespace SmartTripPlanner.Core.Graph;

public class CachedGraph<TVertexId, TEdge> : IGraph<TVertexId, TEdge>
    where TVertexId : StronglyTypedVertexId, new()
    where TEdge : Edge
{
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true,
        Converters = { new StronglyTypedVertexIdJsonConverter<TVertexId>() }
    };

    private readonly IGraph<TVertexId, TEdge> _decoree;
    private readonly IGraphCache<TVertexId, TEdge> _cache;
    private readonly string _cacheKey;

    public CachedGraph(
        IGraph<TVertexId, TEdge> decoree,
        IGraphCache<TVertexId, TEdge> cache,
        string cacheKey)
    {
        _decoree = decoree;
        _cache = cache;
        _cacheKey = cacheKey;
    }

    public async ValueTask<Dictionary<TVertexId, List<TEdge>>> GetAdjacencyDictAsync()
    => await _cache.GetOrSetAsync(
            _cacheKey,
            async () => await _decoree.GetAdjacencyDictAsync(),
            serializerOptions: _serializerOptions);

    public async ValueTask ReconstructFrom(Dictionary<TVertexId, List<TEdge>> adjacencyDict)
    {
        await _decoree.ReconstructFrom(adjacencyDict);
        await RefreshCacheAsync();
    }

    public async Task EnsureInitializedAsync()
    {
        if (await _cache.GetAsync(_cacheKey) is null)
        {
            await _decoree.EnsureInitializedAsync();
            await RefreshCacheAsync();
        }
    }

    public async Task AddNodeAsync(TVertexId node)
    {
        await _decoree.AddNodeAsync(node);
        await RefreshCacheAsync();
    }
    public async Task AddEdgeAsync(TVertexId from, TEdge edge)
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

