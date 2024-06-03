using System;
using System.Globalization;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SmartTripPlanner.Core.Routing.Interfaces;
using SmartTripPlanner.Core.Routing.Models;
using SmartTripPlanner.Core.Routing.Responses;
using SmartTripPlanner.GoogleRoutes.Constants;
using SmartTripPlanner.GoogleRoutes.Exceptions;
using SmartTripPlanner.GoogleRoutes.Extensions;

namespace SmartTripPlanner.GoogleRoutes;

public class GoogleRoutesApiService(
    HttpClient _httpClient,
    ILogger<GoogleRoutesApiService> logger
    ) : IRoutesApiService
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public async Task<ComputeRoutesResponse> GetComputeRoutesResponseAsync(
        LatLng origin,
        LatLng destination,
        TrafficAwareness trafficAwareness,
        CancellationToken cancellationToken = default)
    {
        var requestJsonString = RequestToJsonString(
            new Location(origin),
            new Location(destination),
            trafficAwareness
            );

        return await GetResponseAsync<ComputeRoutesResponse>(requestJsonString, "*", cancellationToken);
    }

    public async Task<ComputeDurationAndDistanceOnlyResponse> GetComputeDurationAndDistanceOnlyResponseAsync(
        LatLng origin,
        LatLng destination,
        TrafficAwareness trafficAwareness,
        CancellationToken cancellationToken = default)
    {
        var requestJsonString = RequestToJsonString(
            new Location(origin),
            new Location(destination),
            trafficAwareness
            );

        return await GetResponseAsync<ComputeDurationAndDistanceOnlyResponse>(requestJsonString, "routes.duration,routes.distanceMeters", cancellationToken);
    }

    public async Task<ComputeRoutesWithIntermediateWaypointsResponse> GetRoutesWithIntermediateWaypointsResponseAsync(
        LatLng origin,
        LatLng destination,
        ICollection<LatLng> intermediateWaypoints,
        TrafficAwareness trafficAwareness,
        CancellationToken cancellationToken = default)
    {
        var requestJsonString = RequestToJsonString(
            new Location(origin),
            new Location(destination),
            trafficAwareness,
            intermediateWaypoints: intermediateWaypoints.Select(latLng => new Location(latLng)).ToList());

        return await GetResponseAsync<ComputeRoutesWithIntermediateWaypointsResponse>(requestJsonString, "*", cancellationToken);
    }

    private async Task<T> GetResponseAsync<T>(string requestJsonString, string fieldMask, CancellationToken cancellationToken = default)
        where T : notnull
    {
        HttpResponseMessage? response = null;
        using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, GoogleRoutesConstants.Endpoints.ComputeRoutes))
        {
            using var content = new StringContent(requestJsonString, Encoding.UTF8, "application/json");
            requestMessage.Content = content;

            // Add fieldMask to the HttpRequestMessage headers
            requestMessage.Headers.Add(GoogleRoutesConstants.Headers.FieldMask, fieldMask);

            logger.LogSendingRequestToGoogleRoutesAPI(requestJsonString);

            // Send the request using HttpRequestMessage
            response = await _httpClient.SendAsync(requestMessage, cancellationToken);
        }

        try
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new GoogleRoutesException("Google Routes API request failed.", requestJsonString, await response.Content.ReadAsStringAsync(cancellationToken));
            }
        }
        catch (GoogleRoutesException ex)
        {
            logger.LogGoogleRoutesAPIRequestFailed(ex);
            throw;
        }

        return await JsonSerializer.DeserializeAsync<T>(
                                        await response.Content.ReadAsStreamAsync(cancellationToken),
                                        _jsonSerializerOptions,
                                        cancellationToken)
               ?? throw new GoogleRoutesException( "Google Routes API response was empty or invalid.", requestJsonString, await response.Content.ReadAsStringAsync(cancellationToken));
    }

    private static string RequestToJsonString(
        Location origin,
        Location destination,
        TrafficAwareness trafficAwareness,
        ICollection<Location>? intermediateWaypoints = null)
    {
        var routingPreference = trafficAwareness switch
        {
            TrafficAwareness.NonTrafficAware => GoogleRoutesConstants.RoutingPreferences.NonTrafficAware,
            TrafficAwareness.TrafficAware => GoogleRoutesConstants.RoutingPreferences.TrafficAware,
            _ => throw new NotImplementedException($"Unknown {nameof(TrafficAwareness)}: ({trafficAwareness}).")
        };

        return RequestToJsonString(origin, destination, routingPreference, GoogleRoutesConstants.TravelModes.Driving, intermediateWaypoints);
    }

    // !!! DO NOT change parameter names, they are mapped to anonymous object with that param names.
    private static string RequestToJsonString(
        Location origin,
        Location destination,
        string routingPreference,
        string travelMode,
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
                intermediates = intermediateWaypoints.Select(iw => new { location = iw }).ToList()
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

internal static partial class LoggerExtensions
{
    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Sending request to Google Routes API: {RequestJsonString}")]
    public static partial void LogSendingRequestToGoogleRoutesAPI(this ILogger<GoogleRoutesApiService> logger, string requestJsonString);

#pragma warning disable SYSLIB1006 // Multiple logging methods cannot use the same event id within a class
    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Google Routes API request failed.")]
#pragma warning restore SYSLIB1006 // Multiple logging methods cannot use the same event id within a class
    public static partial void LogGoogleRoutesAPIRequestFailed(this ILogger<GoogleRoutesApiService> logger, GoogleRoutesException ex);
}
