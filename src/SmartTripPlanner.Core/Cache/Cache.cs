using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace SmartTripPlanner.Core.Cache;

public class Cache<T>(IDistributedCache _cache) : ICache<T>
{
    public async Task<T?> GetAsync(string key, JsonSerializerOptions? serializerOptions = null, CancellationToken cancellationToken = default)
    {
        var value = await _cache.GetStringAsync(key, cancellationToken);

        if (value is null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(value, serializerOptions);
    }

    public async Task<T> GetOrSetAsync(
        string key,
        Func<Task<T>> func,
        TimeSpan? absoluteExpirationRelativeToNow = null,
        TimeSpan? slidingExpiration = null,
        JsonSerializerOptions? serializerOptions = null,
        CancellationToken cancellationToken = default)
    {
        var value = await _cache.GetStringAsync(key, cancellationToken);

        if (value is not null)
        {
            return JsonSerializer.Deserialize<T>(value, serializerOptions)!;
        }

        var result = await func();

        if (result is null)
        {
            return default!;
        }

        await SetAsync(key, result, absoluteExpirationRelativeToNow, slidingExpiration, serializerOptions, cancellationToken);

        return result;
    }

    public async Task<T> GetOrSetAsync(
        string key,
        Func<T> func,
        TimeSpan? absoluteExpirationRelativeToNow = null,
        TimeSpan? slidingExpiration = null,
        JsonSerializerOptions? serializerOptions = null,
        CancellationToken cancellationToken = default)
    {
        var value = await _cache.GetStringAsync(key, cancellationToken);

        if (value is not null)
        {
            return JsonSerializer.Deserialize<T>(value, serializerOptions)!;
        }

        var result = func();

        if (result is null)
        {
            return default!;
        }

        await SetAsync(key, result, absoluteExpirationRelativeToNow, slidingExpiration, serializerOptions, cancellationToken);

        return result;
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        => await _cache.RemoveAsync(key, cancellationToken);

    public Task SetAsync(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null, TimeSpan? slidingExpiration = null, JsonSerializerOptions? serializerOptions = null, CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions();

        if (absoluteExpirationRelativeToNow is not null)
        {
            options.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
        }

        if (slidingExpiration is not null)
        {
            options.SlidingExpiration = slidingExpiration;
        }

        return _cache.SetStringAsync(key, JsonSerializer.Serialize(value, serializerOptions), options, token: cancellationToken);
    }

    public async Task SetAsync(string cacheKey, Func<Task<T>> func, TimeSpan? absoluteExpirationRelativeToNow = null, TimeSpan? slidingExpiration = null, JsonSerializerOptions? serializerOptions = null, CancellationToken cancellationToken = default)
    {
        var value = await func();

        await SetAsync(cacheKey, value, absoluteExpirationRelativeToNow, slidingExpiration, serializerOptions, cancellationToken);
    }
}

public class Cache(IDistributedCache _cache) : ICache
{
    public async Task<T?> GetAsync<T>(string key, JsonSerializerOptions? serializerOptions = null, CancellationToken cancellationToken = default)
    {
        var value = await _cache.GetStringAsync(key, cancellationToken);

        if (value is null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(value, serializerOptions);
    }

    public async Task<T> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> func,
        TimeSpan? absoluteExpirationRelativeToNow = null,
        TimeSpan? slidingExpiration = null,
        JsonSerializerOptions? serializerOptions = null,
        CancellationToken cancellationToken = default)
    {
        var value = await _cache.GetStringAsync(key, cancellationToken);

        if (value is not null)
        {
            return JsonSerializer.Deserialize<T>(value, serializerOptions)!;
        }

        var result = await func();

        if (result is null)
        {
            return default!;
        }

        await SetAsync(key, result, absoluteExpirationRelativeToNow, slidingExpiration, serializerOptions, cancellationToken);

        return result;
    }

    public async Task<T> GetOrSetAsync<T>(
        string key,
        Func<T> func,
        TimeSpan? absoluteExpirationRelativeToNow = null,
        TimeSpan? slidingExpiration = null,
        JsonSerializerOptions? serializerOptions = null,
        CancellationToken cancellationToken = default)
    {
        var value = await _cache.GetStringAsync(key, cancellationToken);

        if (value is not null)
        {
            return JsonSerializer.Deserialize<T>(value, serializerOptions)!;
        }

        var result = func();

        if (result is null)
        {
            return default!;
        }

        await SetAsync(key, result, absoluteExpirationRelativeToNow, slidingExpiration, serializerOptions, cancellationToken);

        return result;
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        => await _cache.RemoveAsync(key, cancellationToken);

    public Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null, TimeSpan? slidingExpiration = null, JsonSerializerOptions? serializerOptions = null, CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions();

        if (absoluteExpirationRelativeToNow is not null)
        {
            options.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
        }

        if (slidingExpiration is not null)
        {
            options.SlidingExpiration = slidingExpiration;
        }

        return _cache.SetStringAsync(key, JsonSerializer.Serialize(value, serializerOptions), options, token: cancellationToken);
    }

    public async Task SetAsync<T>(string cacheKey, Func<Task<T>> func, TimeSpan? absoluteExpirationRelativeToNow = null, TimeSpan? slidingExpiration = null, JsonSerializerOptions? serializerOptions = null, CancellationToken cancellationToken = default)
    {
        var value = await func();

        await SetAsync(cacheKey, value, absoluteExpirationRelativeToNow, slidingExpiration, serializerOptions, cancellationToken);
    }
}
