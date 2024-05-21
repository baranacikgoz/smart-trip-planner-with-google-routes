using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTripPlanner.Core.Graph;
public interface IVertex<out TVertexId>
    where TVertexId : StronglyTypedVertexId
{
    TVertexId VertexId { get; }
}
