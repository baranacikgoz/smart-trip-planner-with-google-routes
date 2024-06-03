using System.ComponentModel.DataAnnotations;

namespace SmartTripPlanner.GoogleRoutes.Setup;

public class GoogleRoutesOptions
{
    [Required(AllowEmptyStrings = false)]
    public string ApiKey { get; set; } = default!;

    public Uri BaseUrl { get; set; } = new("https://routes.googleapis.com");
}
