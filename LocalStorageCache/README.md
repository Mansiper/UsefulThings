# LocalStorageCache
Cache on a client side for Blazor using Local Storage

You can use it however you want. Just copy in your code and change it according to your task.

Use AddOrCreate for any Get request.  
Use additionalKey for collecting all parameters in your Get request.  
Use Remove(CacheBigType) after any successful Insert, Update, or Delete request.

CacheKey helps to avoid strings.  
CacheBigKey is very convenient in deletion.

script.js contains one method which cleans expired cache every 30 seconds.