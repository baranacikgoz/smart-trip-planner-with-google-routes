using SmartTripPlanner.ChargePoints.Graphs;
using SmartTripPlanner.ChargePoints.Models;
using SmartTripPlanner.ChargePoints.TripPlanner.Interfaces;
using SmartTripPlanner.Core.DistanceCalculator.Interfaces;
using SmartTripPlanner.Core.Routing.Interfaces;
using SmartTripPlanner.Core.Routing.Models;

namespace SmartTripPlanner.ChargePoints.TripPlanner.Services;
public class ChargePointTripPlanner(
    IChargePointGraph _graph,
    ICrowFlightDistanceCalculator _crowFlighDistanceCalculator,
    IRoutesService _routesService
    ) : IChargePointTripPlanner
{
    public async Task<RoutesWithIntermediateWayPoints> PlanTripAsync(
        double initialBatteryPercentage,
        double batteryConsumptionPer100Km,
        double minBatteryThreshold,
        LatLng origin,
        LatLng destination,
        CancellationToken cancellationToken)
    {
        // Copy graph to avoid modifying the original graph.
        var graph = await _graph.GetAdjacencyDictAsync();

        // Calculate and select the nearest node to origin, lets say node's name is: NearestToOrigin
        // Calculate and select the nearest node to destination, lets say node's name is: NearestToDest
        ChargePoint? nearestToOrigin = null;
        var minDistanceToOrigin = double.MaxValue;

        ChargePoint? nearestToDest = null;
        var minDistanceToDest = double.MaxValue;

        foreach (var chargePoint in graph.Keys)
        {
            var distanceToOrigin = _crowFlighDistanceCalculator.CalculateDistanceMeters(origin, new LatLng(chargePoint.Latitude, chargePoint.Longitude));
            if (distanceToOrigin < minDistanceToOrigin)
            {
                minDistanceToOrigin = distanceToOrigin;
                nearestToOrigin = chargePoint;
            }

            var distanceToDest = _crowFlighDistanceCalculator.CalculateDistanceMeters(destination, new LatLng(chargePoint.Latitude, chargePoint.Longitude));
            if (distanceToDest < minDistanceToDest)
            {
                minDistanceToDest = distanceToDest;
                nearestToDest = chargePoint;
            }
        }

        // Add these two new node to the graph with edges to origin and destination respectively.
        var temporaryOriginVertex = new ChargePoint(new("TemporaryOriginVertex"), origin.Latitude, origin.Longitude, IsDc: false);

        // Calculate the actual google routes way between origin and NearestToOrigin
        var (fromOriginToNearestToOriginDuration, fromOriginToNearestToOriginDistance) = await _routesService.GetDurationAndDistanceOnlyAsync(
                                                                                origin: new(temporaryOriginVertex.Latitude, temporaryOriginVertex.Longitude),
                                                                                destination: new(nearestToOrigin!.Latitude, nearestToOrigin.Longitude),
                                                                                TrafficAwareness.NonTrafficAware,
                                                                                cancellationToken);
        graph.Add(temporaryOriginVertex, [new Way(nearestToOrigin, fromOriginToNearestToOriginDuration, fromOriginToNearestToOriginDistance)]);

        // Now add the bidirection
        var (fromNearestToOriginToOriginDuration, fromNearestToOriginToOriginDistance) = await _routesService.GetDurationAndDistanceOnlyAsync(
                                                                                            origin: new(nearestToOrigin.Latitude, nearestToOrigin.Longitude),
                                                                                            destination: new(temporaryOriginVertex.Latitude, temporaryOriginVertex.Longitude),
                                                                                            TrafficAwareness.NonTrafficAware,
                                                                                            cancellationToken);
        graph[nearestToOrigin].Add(new Way(temporaryOriginVertex, fromNearestToOriginToOriginDuration, fromNearestToOriginToOriginDistance));

        // Now to the same for destination
        var temporaryDestVertex = new ChargePoint(new("TemporaryDestVertex"), destination.Latitude, destination.Longitude, IsDc: false);

        // Calculate the actual google routes way between destination and NearestToDest
        var (fromDestToNearestToDestDuration, fromDestToNearestToDestDistance) = await _routesService.GetDurationAndDistanceOnlyAsync(
                                                                                    origin: new(temporaryDestVertex.Latitude, temporaryDestVertex.Longitude),
                                                                                    destination: new(nearestToDest!.Latitude, nearestToDest.Longitude),
                                                                                    TrafficAwareness.NonTrafficAware,
                                                                                    cancellationToken);
        graph.Add(temporaryDestVertex, [new Way(nearestToDest, fromDestToNearestToDestDuration, fromDestToNearestToDestDistance)]);

        // Now add the bidirection
        var (fromNearestToDestToDestDuration, fromNearestToDestToDestDistance) = await _routesService.GetDurationAndDistanceOnlyAsync(
                                                                                    origin: new(nearestToDest.Latitude, nearestToDest.Longitude),
                                                                                    destination: new(temporaryDestVertex.Latitude, temporaryDestVertex.Longitude),
                                                                                    TrafficAwareness.NonTrafficAware,
                                                                                    cancellationToken);
        graph[nearestToDest].Add(new Way(temporaryDestVertex, fromNearestToDestToDestDuration, fromNearestToDestToDestDistance));

        // Write an algorithm so that, find the optimum route that have minimum distance without going out of battery.
        // The algorithm should return the charge points those vehicle should stop by.

        var shortestPath = FindShortestPathWithBatteryConstraints(
                                graph,
                                temporaryOriginVertex,
                                temporaryDestVertex,
                                initialBatteryPercentage,
                                batteryConsumptionPer100Km,
                                minBatteryThreshold);

        // After finding the stop locations, calculate the actual google routes way between origin to destination with
        // the stop locations being intermediate way points.
        return await _routesService.GetRoutesWithIntermediateWaypointsAsync(
            origin: origin,
            destination: destination,
            intermediateWaypoints: shortestPath.Select(cp => new LatLng(cp.Latitude, cp.Longitude)).ToList(),
            TrafficAwareness.NonTrafficAware,
            cancellationToken);

    }

    private static List<ChargePoint> FindShortestPathWithBatteryConstraints(
        Dictionary<ChargePoint, List<Way>> graph,
        ChargePoint start,
        ChargePoint end,
        double initialBatteryPercentage,
        double batteryConsumptionPer100Km,
        double minBatteryThreshold // The value that we don't want to go below under any circumstances
        )
    {
        var priorityQueue = new PriorityQueue<(ChargePoint Node, double DistanceMeters, double Battery), double>();
        var shortestPaths = new Dictionary<ChargePoint, (ChargePoint? PreviousNode, double Cost, double Battery)>();
        var visited = new HashSet<ChargePoint>();

        priorityQueue.Enqueue((start, 0, initialBatteryPercentage), 0);
        shortestPaths[start] = (null, 0, initialBatteryPercentage);

        while (priorityQueue.Count > 0)
        {
            var (currentNode, currentDistanceMeters, currentBattery) = priorityQueue.Dequeue();
            if (visited.Contains(currentNode))
            {
                continue;
            }

            visited.Add(currentNode);

            if (currentNode.Equals(end))
            {
                break;
            }

            foreach (var way in graph[currentNode])
            {
                var nextNode = way.To;

                if (visited.Contains(nextNode))
                {
                    continue;
                }

                var distanceMeters = way.DistanceInMeters;
                var energyConsumption = (distanceMeters / 100) * batteryConsumptionPer100Km;

                var remainingBattery = currentBattery - energyConsumption;
                if (remainingBattery < minBatteryThreshold)
                {
                    // Need to recharge at the current node
                    remainingBattery = 100 - energyConsumption; // Recharge to full (100%) then continue
                }

                var newDistanceMeters = currentDistanceMeters + distanceMeters;
                if (!shortestPaths.TryGetValue(nextNode, out var value) || newDistanceMeters < value.Cost)
                {
                    value = (currentNode, newDistanceMeters, remainingBattery);
                    shortestPaths[nextNode] = value;
                    priorityQueue.Enqueue((nextNode, newDistanceMeters, remainingBattery), newDistanceMeters);
                }
            }
        }

        // Reconstruct the path
        var path = new List<ChargePoint>();
        var current = end;
        while (current != null)
        {
            path.Insert(0, current);
            current = shortestPaths[current].PreviousNode;
        }

        return path;
    }
}
