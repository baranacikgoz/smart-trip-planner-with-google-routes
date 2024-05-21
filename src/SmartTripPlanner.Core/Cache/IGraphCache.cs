using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using SmartTripPlanner.Core.Graph;

namespace SmartTripPlanner.Core.Cache;
public interface IGraphCache<TVertexId, TEdge> : ICache<Dictionary<TVertexId, List<TEdge>>>
    where TVertexId : StronglyTypedVertexId
    where TEdge : Edge
{
}
