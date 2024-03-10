namespace LocalStorageCache;

public interface IClientCacheService
{
	Task<T?> GetOrCreate<T>(CacheKey key, Func<Task<T>> factory, CancellationToken? ct = null) where T : class;
	Task<T?> GetOrCreate<T>(CacheKey key, string? additionalKey, Func<Task<T>> factory, CancellationToken? ct = null) where T : class;
	Task<T?> GetOrCreate<T>(CacheKey key, string[] additionalKeys, Func<Task<T>> factory, CancellationToken? ct = null) where T : class;
	Task Remove(CacheKey key);
	Task Remove(CacheBigKey type);
}