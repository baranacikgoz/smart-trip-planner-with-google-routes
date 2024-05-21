using System;
using System.Globalization;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SmartTripPlanner.Core.Routes.Interfaces;
using SmartTripPlanner.Core.Routes.Models;
using SmartTripPlanner.GoogleRoutes.Constants;
using SmartTripPlanner.GoogleRoutes.Exceptions;
using SmartTripPlanner.GoogleRoutes.Extensions;
using SmartTripPlanner.GoogleRoutes.Responses;

namespace SmartTripPlanner.GoogleRoutes;

public class GoogleRoutesService(
    HttpClient _httpClient,
    IPolyLineDecoder _polylineDecoder,
    ILogger<GoogleRoutesService> logger
    ) : IRoutesService
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public async Task<DecodedRoute> GetRoutesAsync(
        LatLng origin,
        LatLng destination,
        CancellationToken cancellationToken = default)
    {
        var requestJsonString = RequestToJsonString(
            new Location(origin),
            new Location(destination)
            );

        var computeRoutesResponse = await GetResponseAsync<ComputeRoutesResponse>(requestJsonString, "*", cancellationToken);
        var route = computeRoutesResponse.Routes.Single();

        return new DecodedRoute(
            DistanceMeters: route.DistanceMeters,
            Duration: route.Duration.ToTimeSpan(),
            Polylines: _polylineDecoder
                            .Decode(route.Polyline.EncodedPolyline)
                            .Select(latlng => new Location(latlng))
            );
    }

    public async Task<RoutesWithIntermediateWayPoints> GetRoutesWithIntermediateWaypointsAsync(
        LatLng origin,
        LatLng destination,
        ICollection<LatLng> intermediateWaypoints,
        CancellationToken cancellationToken = default)
    {
        var requestJsonString = RequestToJsonString(
            new Location(origin),
            new Location(destination),
            intermediateWaypoints: intermediateWaypoints.Select(latLng => new Location(latLng)).ToList());

        var routes = await GetResponseAsync<ComputeRoutesWithIntermediateWaypointsResponse>(requestJsonString, "*", cancellationToken);

        return new RoutesWithIntermediateWayPoints(
            routes
                .Legs
                .Select(l => new DecodedLeg(
                    l.DistanceMeters,
                    l.Duration.ToTimeSpan(),
                    l.StaticDuration.ToTimeSpan(),
                    l.StartLocation,
                    l.EndLocation,
                    l.Steps))
                .ToList());
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
            await GoogleRoutesException.ThrowIfNotSuccess(response, requestJsonString);
        }
        catch (GoogleRoutesException ex)
        {
            logger.LogGoogleRoutesAPIRequestFailed(ex);
            throw;
        }

        return await response.Content.ReadFromJsonAsync<T>(_jsonSerializerOptions, cancellationToken: cancellationToken)
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

internal static partial class LoggerExtensions
{
    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Sending request to Google Routes API: {RequestJsonString}")]
    public static partial void LogSendingRequestToGoogleRoutesAPI(this ILogger<GoogleRoutesService> logger, string requestJsonString);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Google Routes API request failed.")]
    public static partial void LogGoogleRoutesAPIRequestFailed(this ILogger<GoogleRoutesService> logger, GoogleRoutesException ex);
}