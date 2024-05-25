namespace SmartTripPlanner.Core.Graph;

public abstract class GraphBase<TVertex, TVertexId, TEdge> : IGraph<TVertex, TVertexId, TEdge>
    where TVertexId : StronglyTypedVertexId
    where TVertex : IVertex<TVertexId>
    where TEdge : Edge<TVertex, TVertexId>
{

    public abstract ValueTask<Dictionary<TVertex, List<TEdge>>> GetAdjacencyDictAsync();

    public abstract Task EnsureInitializedAsync();
    public async Task AddNodeAsync(TVertex node)
    {
        (await GetAdjacencyDictAsync()).TryAdd(node, []);
    }

    public async Task AddEdgeAsync(TVertex from, TEdge edge)
    {
        await AddNodeAsync(from); // Ensure the charge point exists in the graph.

        (await GetAdjacencyDictAsync())[from].Add(edge);
    }

    public abstract ValueTask ReconstructFrom(Dictionary<TVertex, List<TEdge>> adjacencyDict);
}
