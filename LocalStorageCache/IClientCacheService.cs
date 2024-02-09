namespace LocalStorageCache;

public interface IClientCacheService
{
	Task<T?> GetOrCreate<T>(CacheKey key, Func<Task<T>> factory);
	Task<T?> GetOrCreate<T>(CacheKey key, string? additional, Func<Task<T>> factory);
	Task Remove(CacheKey key);
	Task Remove(CacheBigKey type);
}