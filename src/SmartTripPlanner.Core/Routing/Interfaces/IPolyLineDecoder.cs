using System;
using System.Collections.Generic;
using System.Text;
using SmartTripPlanner.Core.Routing.Models;

namespace SmartTripPlanner.Core.Routing.Interfaces;

public interface IPolyLineDecoder
{
    IEnumerable<LatLng> Decode(string encodedPolyline);
}
