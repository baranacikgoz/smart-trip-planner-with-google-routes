namespace SmartTripPlanner.Core.Graph;

public interface IGraph { }
public interface IGraph<TVertex, TVertexId, TEdge> : IGraph
    where TVertex : IVertex<TVertexId>
    where TVertexId : StronglyTypedVertexId
    where TEdge : Edge<TVertex, TVertexId>
{
    Task EnsureInitializedAsync();
    ValueTask<Dictionary<TVertex, List<TEdge>>> GetAdjacencyDictAsync();
    Task AddNodeAsync(TVertex node);
    Task AddEdgeAsync(TVertex from, TEdge edge);
    ValueTask ReconstructFrom(Dictionary<TVertex, List<TEdge>> adjacencyDict);
}
