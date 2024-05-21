namespace SmartTripPlanner.Core.Graph;

public abstract class GraphBase<TVertexId, TEdge> : IGraph<TVertexId, TEdge>
    where TVertexId : StronglyTypedVertexId
    where TEdge : Edge
{

    public abstract ValueTask<Dictionary<TVertexId, List<TEdge>>> GetAdjacencyDictAsync();

    public abstract Task EnsureInitializedAsync();
    public async Task AddNodeAsync(TVertexId node)
    {
        (await GetAdjacencyDictAsync()).TryAdd(node, []);
    }

    public async Task AddEdgeAsync(TVertexId from, TEdge edge)
    {
        await AddNodeAsync(from); // Ensure the charge point exists in the graph.

        (await GetAdjacencyDictAsync())[from].Add(edge);
    }

    public abstract ValueTask ReconstructFrom(Dictionary<TVertexId, List<TEdge>> adjacencyDict);
}
