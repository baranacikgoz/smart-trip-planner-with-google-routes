namespace SmartTripPlanner.GoogleRoutes.Constants;

public static class GoogleRoutesConstants
{
    public static class RoutingPreferences
    {
        public const string TrafficAware = "TRAFFIC_AWARE";
        public const string NonTrafficAware = "TRAFFIC_UNAWARE";
    }

    public static class TravelModes
    {
        public const string Driving = "DRIVE";
    }

    public static class Headers
    {
        public const string FieldMask = "X-Goog-FieldMask";
        public const string ApiKey = "X-Goog-Api-Key";
    }

    public static class Endpoints
    {
        public const string ComputeRoutes = "/directions/v2:computeRoutes";
    }
}
