using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTripPlanner.Core.Graph;
public record StronglyTypedVertexId
{
    public string Value { get; init; }

    protected StronglyTypedVertexId(string value)
    {
        Value = value;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public StronglyTypedVertexId() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
