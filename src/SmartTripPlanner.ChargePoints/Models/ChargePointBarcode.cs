using System;
using System.Collections.Generic;
using System.Text;
using SmartTripPlanner.Core.Graph;

namespace SmartTripPlanner.ChargePoints.Models;
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
