using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using SmartTripPlanner.Core.Graph;

namespace SmartTripPlanner.Core.Cache;
public interface ICache<T>
{
    Task<T?> GetAsync(
        string key,
        JsonSerializerOptions? serializerOptions = null,
        CancellationToken cancellationToken = default);

    Task<T> GetOrSetAsync(
        string key,
        Func<T> func,
        TimeSpan? absoluteExpirationRelativeToNow = null,
        TimeSpan? slidingExpiration = null, JsonSerializerOptions? serializerOptions = null,
        CancellationToken cancellationToken = default);

    Task<T> GetOrSetAsync(
        string key,
        Func<Task<T>> func,
        TimeSpan? absoluteExpirationRelativeToNow = null,
        TimeSpan? slidingExpiration = null, JsonSerializerOptions? serializerOptions = null,
        CancellationToken cancellationToken = default);

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    Task SetAsync(
        string key,
        T value,
        TimeSpan? absoluteExpirationRelativeToNow = null,
        TimeSpan? slidingExpiration = null,
        JsonSerializerOptions? serializerOptions = null,
        CancellationToken cancellationToken = default);

    Task SetAsync(
        string cacheKey,
        Func<Task<T>> func,
        TimeSpan? absoluteExpirationRelativeToNow = null,
        TimeSpan? slidingExpiration = null,
        JsonSerializerOptions? serializerOptions = null,
        CancellationToken cancellationToken = default);
}

public interface ICache
{
    Task<T?> GetAsync<T>(
        string key,
        JsonSerializerOptions? serializerOptions = null,
        CancellationToken cancellationToken = default);

    Task<T> GetOrSetAsync<T>(
        string key,
        Func<T> func,
        TimeSpan? absoluteExpirationRelativeToNow = null,
        TimeSpan? slidingExpiration = null, JsonSerializerOptions? serializerOptions = null,
        CancellationToken cancellationToken = default);

    Task<T> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> func,
        TimeSpan? absoluteExpirationRelativeToNow = null,
        TimeSpan? slidingExpiration = null, JsonSerializerOptions? serializerOptions = null,
        CancellationToken cancellationToken = default);

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? absoluteExpirationRelativeToNow = null,
        TimeSpan? slidingExpiration = null,
        JsonSerializerOptions? serializerOptions = null,
        CancellationToken cancellationToken = default);

    Task SetAsync<T>(
        string cacheKey,
        Func<Task<T>> func,
        TimeSpan? absoluteExpirationRelativeToNow = null,
        TimeSpan? slidingExpiration = null,
        JsonSerializerOptions? serializerOptions = null,
        CancellationToken cancellationToken = default);
}
