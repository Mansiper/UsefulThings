namespace LocalStorageCache;

public interface IClientCacheService
{
	Task<T?> GetOrCreate<T>(CacheKey key, Func<Task<T>> factory, CancellationToken? ct = null);
	Task<T?> GetOrCreate<T>(CacheKey key, string? additional, Func<Task<T>> factory, CancellationToken? ct = null);
	Task Remove(CacheKey key, CancellationToken? ct = null);
	Task Remove(CacheBigKey type, CancellationToken? ct = null);
}