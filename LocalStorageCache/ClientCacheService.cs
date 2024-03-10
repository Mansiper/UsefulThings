using System.Text.Json;
using Microsoft.JSInterop;
using System.Text;

namespace LocalStorageCache;

public class ClientCacheService(IJSRuntime jsRuntime, ILogger<ClientCacheService> logger) : IClientCacheService
{
	private const int ChunkSize = 25 * 1024;	//SignalR has restrictions. It works fine with 25 KB per JS invokation
	
    public async Task<T?> GetOrCreate<T>(CacheKey key, Func<Task<T>> factory,
		CancellationToken? ct = null) where T : class
	{
		var (baseKey, expTime) = GetBaseKey(key);
		return await GetOrCreateBase(baseKey, expTime, factory, ct ?? CancellationToken.None);
	}

	public async Task<T?> GetOrCreate<T>(CacheKey key, string? additionalKey, Func<Task<T>> factory,
		CancellationToken? ct = null) where T : class
	{
		var (baseKey, expTime) = GetBaseData(key);
		baseKey += additional?.ToLower();
		return await GetOrCreateBase(baseKey, expTime, factory, ct ?? CancellationToken.None);
	}

	public async Task<T?> GetOrCreate<T>(CacheKey key, string[] additionalKeys, Func<Task<T>> factory,
		CancellationToken? ct = null) where T : class
	{
		var (baseKey, expTime) = GetBaseData(key);
		baseKey += string.Join("-", additionalKeys).ToLower();
		return await GetOrCreateBase(baseKey, expTime, factory, ct ?? CancellationToken.None);
	}

	private async Task<T?> GetOrCreateBase<T>(string baseKey, int expTime, Func<Task<T>> factory, CancellationToken ct)
	{
		var dataKey = GetDataKey(baseKey);
		var expKey = GetExpKey(baseKey);

		var jsonValue = await GetItem(dataKey, expKey, ct);
		if (!string.IsNullOrWhiteSpace(jsonValue))
            try
            {
                return JsonSerializer.Deserialize<T>(jsonValue!);
            }
            catch
            {
                // ignored
            }
		
		var value = await factory();

		//if (!AllowAddToCache(value)) return value;

		try
		{
			var serialized = JsonSerializer.Serialize(value);
			if (Encoding.UTF8.GetByteCount(serialized) > ChunkSize)
			{
				var chunks = SplitIntoChunks(serialized, ChunkSize).ToList();
				for (var i = 0; i < chunks.Count; i++)
				{
					var chunkKey = GetChunkKey(dataKey, i);
					await jsRuntime.LocalStorageSetItem(chunkKey, chunks[i]);
				}
			}
			else
				await jsRuntime.LocalStorageSetItem(dataKey, serialized);
		
			var expirationTime = DateTime.UtcNow.AddSeconds(expTime);
			await jsRuntime.LocalStorageSetItem(expKey, expirationTime.ToString("O"));
		}
		catch (Exception e)
		{
			logger.LogError(e, "Cannot set value to client cache");
			await Remove(baseKey);
		}
		
		return value;
	}

	public async Task Remove(CacheKey key)
	{
		var (baseKey, _) = GetBaseData(key);
		await Remove(baseKey);
	}

	private async Task Remove(string baseKey)
	{
		var dataKey = GetDataKey(baseKey);
		var expKey = GetExpKey(baseKey);

		var keysToDelete = await GetAllKeys()
			.Where(e => e.StartsWith(dataKey) || e.StartsWith(expKey))
			.ToListAsync();

		foreach (var k in keysToDelete)
			await jsRuntime.LocalStorageRemoveItem(k);
	}

	public async Task Remove(CacheBigKey key)
	{
		switch (key)
		{
			case CacheBigKey.Value1:
				await Remove(CacheKey.Value11);
				await Remove(CacheKey.Value12);
				break;
			case CacheBigKey.Value2:
				await Remove(CacheKey.Value21);
				await Remove(CacheKey.Value22);
				break;
			case CacheBigKey.Unknown:
				await Remove(CacheKey.Unknown);
				break;
		}
	}
	private async Task<string?> GetItem(string dataKey, string expKey, CancellationToken ct)
	{
		var expValue = await jsRuntime.LocalStorageGetItem(expKey, ct);
		if (IsExpired(expValue))
			return null;
		
		var value = await jsRuntime.LocalStorageGetItem(dataKey, ct);
		if (!string.IsNullOrWhiteSpace(value))
			return value;
		
		var result = new StringBuilder();
		for (var i = 0; ; i++)
		{
			var chunkKey = GetChunkKey(dataKey, i);
			var chunk = await jsRuntime.LocalStorageGetItem(chunkKey, ct);
			if (string.IsNullOrWhiteSpace(chunk))
				break;
			result.Append(chunk);
		}
		
		return result.ToString();
	}
	private static bool IsExpired(string expValue) =>
		string.IsNullOrWhiteSpace(expValue) ||
		!DateTime.TryParse(expValue, out var expirationTime) ||
		DateTime.UtcNow > expirationTime.ToUniversalTime();

	private static IEnumerable<string> SplitIntoChunks(string str, int chunkSize)
	{
		for (var i = 0; i < str.Length; i += chunkSize)
		{
			if (i + chunkSize > str.Length)
				chunkSize = str.Length - i;
			yield return str.Substring(i, chunkSize);
		}
	}

    private async IAsyncEnumerable<string> GetAllKeys()
    {
        var length = await jsRuntime.LocalStorageLength();
        for (var i = 0; i < length; ++i)
        {
			var key = await jsRuntime.LocalStorageGetKey(i);
			if (string.IsNullOrWhiteSpace(key))
				yield break;
			yield return key;
		}
    }

    private static (string key, int seconds) GetBaseKey(CacheKey key) =>
		key switch
		{
			CacheKey.Value11 => ("value_11-", 30),	// is for additional values after
			CacheKey.Value12 => ("value_12", 30),	//_ is for using in names

			CacheKey.Value21 => ("value_list", 60),
			CacheKey.Value22 => ("value_one-", 100),	//add Id in an additional key

			_ => ("unknown-", 60),
		};

	private static string GetDataKey(string baseKey) =>
		$"data_{baseKey}";

	private static string GetExpKey(string baseKey) =>
		$"exp_{baseKey}";

	private static string GetChunkKey(string dataKey, int index) =>
		$"{dataKey}_chunk{index}";
}