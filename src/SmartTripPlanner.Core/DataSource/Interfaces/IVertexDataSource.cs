using System;
using System.Collections.Generic;
using System.Text;
using SmartTripPlanner.Core.Graph;

namespace SmartTripPlanner.Core.DataSource.Interfaces;
public interface IVertexDataSource<TVertex, TVertexId>
    where TVertexId : StronglyTypedVertexId
    where TVertex : IVertex<TVertexId>
{
    Task<IReadOnlyList<TVertex>> GetAllAsync();
}
