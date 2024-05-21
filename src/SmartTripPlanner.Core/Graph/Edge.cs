namespace SmartTripPlanner.Core.Graph;

public abstract record Edge(StronglyTypedVertexId Destination, TimeSpan Duration, double DistanceInMeters);
