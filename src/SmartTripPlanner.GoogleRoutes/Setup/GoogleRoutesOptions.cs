using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using SmartTripPlanner.Core.Routes.Interfaces;
using SmartTripPlanner.Core.Setup;
using System.Net.Http;
using SmartTripPlanner.GoogleRoutes.Constants;
using System.ComponentModel.DataAnnotations;

namespace SmartTripPlanner.GoogleRoutes.Setup;

public class GoogleRoutesOptions
{
    [Required(AllowEmptyStrings = false)]
    public string ApiKey { get; set; } = default!;

    public Uri BaseUrl { get; set; } = new("https://routes.googleapis.com");
}
