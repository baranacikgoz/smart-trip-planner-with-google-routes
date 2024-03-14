using System.ComponentModel.DataAnnotations;

namespace SmartTripPlanner.WebAPI.Options;

public class GoogleRoutesOptions
{
    [Required(AllowEmptyStrings = false)]
    public Uri Url { get; set; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string ApiKey { get; set; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string FieldMask { get; set; } = null!;
}
