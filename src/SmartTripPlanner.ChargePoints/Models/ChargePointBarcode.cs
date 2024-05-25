using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using SmartTripPlanner.Core.Graph;
using SmartTripPlanner.Core.JsonConverters;

namespace SmartTripPlanner.ChargePoints.Models;

[JsonConverter(typeof(StronglyTypedVertexIdJsonConverter<ChargePointBarcode>))]
public record ChargePointBarcode : StronglyTypedVertexId
{
    public ChargePointBarcode(string value)
        : base(value)
    {
    }

    public ChargePointBarcode()
        : base(string.Empty)
    {

    }
}
