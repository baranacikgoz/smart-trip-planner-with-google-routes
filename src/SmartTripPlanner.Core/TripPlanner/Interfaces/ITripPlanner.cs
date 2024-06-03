using SmartTripPlanner.Core.Graph;

namespace SmartTripPlanner.Core.TripPlanner.Interfaces;
#pragma warning disable S2326 // Unused type parameters should be removed
public interface ITripPlanner<TVertex, TVertexId, TEdge>
#pragma warning restore S2326 // Unused type parameters should be removed
    where TVertex : IVertex<TVertexId>
    where TVertexId : StronglyTypedVertexId
    where TEdge : Edge<TVertex, TVertexId>
{
}
