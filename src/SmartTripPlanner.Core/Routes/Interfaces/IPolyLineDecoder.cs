using System;
using System.Collections.Generic;
using System.Text;
using SmartTripPlanner.Core.Routes.Models;

namespace SmartTripPlanner.Core.Routes.Interfaces;

public interface IPolyLineDecoder
{
    IEnumerable<LatLng> Decode(string encodedPolyline);
}
