namespace SmartTripPlanner.Core.Graph;

public interface IGraph { }
public interface IGraph<TVertexId, TEdge> : IGraph
    where TVertexId : notnull
    where TEdge : Edge
{
    Task EnsureInitializedAsync();
    ValueTask<Dictionary<TVertexId, List<TEdge>>> GetAdjacencyDictAsync();
    Task AddNodeAsync(TVertexId node);
    Task AddEdgeAsync(TVertexId from, TEdge edge);
    ValueTask ReconstructFrom(Dictionary<TVertexId, List<TEdge>> adjacencyDict);
}
