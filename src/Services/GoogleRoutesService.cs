using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SmartTripPlanner.WebAPI.Constants;
using SmartTripPlanner.WebAPI.Exceptions;
using SmartTripPlanner.WebAPI.Models;
using SmartTripPlanner.WebAPI.Responses;

namespace SmartTripPlanner.WebAPI.Services;

public class GoogleRoutesService(
    HttpClient httpClient,
    PolylineDecoderService polylineDecoderService,
    ILogger<GoogleRoutesService> logger
    )
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public async Task<DecodedRoute> GetRoutesAsync(
        LatLng origin,
        LatLng destination
    )
    {
        var requestJsonString = RequestToJsonString(
            new Location(origin),
            new Location(destination)
            );

        var computeRoutesResponse = await GetResponseAsync<ComputeRoutesResponse>(requestJsonString);
        var route = computeRoutesResponse.Routes.Single();

        return new DecodedRoute(
            DistanceMeters: route.DistanceMeters,
            Duration: route.Duration,
            Polylines: polylineDecoderService.Decode(route.Polyline.EncodedPolyline).Select(ecp => new Location(ecp))
            );
    }

    public async Task<ComputeRoutesWithIntermediateWaypointsResponse> GetRoutesWithIntermediateWaypointsAsync(
        LatLng origin,
        LatLng destination,
        ICollection<LatLng> intermediateWaypoints
    )
    {
        var requestJsonString = RequestToJsonString(
            new Location(origin),
            new Location(destination),
            intermediateWaypoints: intermediateWaypoints.Select(latLng => new Location(latLng)).ToList()
            );

        return await GetResponseAsync<ComputeRoutesWithIntermediateWaypointsResponse>(requestJsonString);
    }

    private async Task<T> GetResponseAsync<T>(string requestJsonString)
        where T : notnull
    {
        using var stringContent = new StringContent(requestJsonString, Encoding.UTF8, "application/json");

        logger.LogDebug("Sending request to Google Routes API: {RequestJsonString}", requestJsonString);

        var response = await httpClient.PostAsync(
            GoogleRoutesConstants.Endpoints.ComputeRoutes,
            stringContent
            );

        try
        {
            await GoogleRoutesException.ThrowIfNotSuccess(response, requestJsonString);
        }
        catch (GoogleRoutesException ex)
        {
            logger.LogError(ex, "Google Routes API request failed. Sent body: {SentBody}. Received body: {ReceivedBody}", ex.SentBody, ex.ReceivedBody);
            throw;
        }

        return await response.Content.ReadFromJsonAsync<T>(_jsonSerializerOptions)
            ?? throw new GoogleRoutesException("Google Routes API response was empty or invalid.", requestJsonString, await response.Content.ReadAsStringAsync());
    }

    private static string RequestToJsonString(
        Location origin,
        Location destination,
        string travelMode = GoogleRoutesConstants.TravelModes.Driving,
        string routingPreference = GoogleRoutesConstants.RoutingPreferences.TrafficAware,
        ICollection<Location>? intermediateWaypoints = null)
    {

        if (intermediateWaypoints?.Count > 0)
        {
            return JsonSerializer.Serialize(new
            {
                origin = new { location = origin },
                destination = new { location = destination },
                travelMode,
                routingPreference,
                intermediates = intermediateWaypoints
            },
            _jsonSerializerOptions);
        }

        return JsonSerializer.Serialize(new
        {
            origin = new { location = origin },
            destination = new { location = destination },
            travelMode,
            routingPreference,
        },
        _jsonSerializerOptions);
    }
}
