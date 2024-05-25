namespace SmartTripPlanner.Core.Graph;

public abstract record Edge<TVertex, TVertexId>(TVertex Destination, TimeSpan Duration, double DistanceInMeters)
    where TVertex : IVertex<TVertexId>
    where TVertexId : StronglyTypedVertexId;
