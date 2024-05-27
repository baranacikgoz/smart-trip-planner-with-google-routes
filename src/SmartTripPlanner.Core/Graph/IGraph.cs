namespace SmartTripPlanner.Core.Graph;

public interface IGraph { }
public interface IGraph<TVertex, TVertexId, TEdge> : IGraph
    where TVertex : IVertex<TVertexId>
    where TVertexId : StronglyTypedVertexId
    where TEdge : Edge<TVertex, TVertexId>
{
    Task EnsureInitializedAsync();
    Task<Dictionary<TVertex, List<TEdge>>> GetAdjacencyDictAsync();
    Task AddNodeAsync(TVertex node);
    Task AddEdgeAsync(TVertex from, TEdge edge);
    Task ReconstructFrom(Dictionary<TVertex, List<TEdge>> adjacencyDict);
}
