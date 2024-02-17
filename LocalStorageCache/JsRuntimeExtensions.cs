using Microsoft.JSInterop;

namespace LocalStorageCache;

public static class JsRuntimeExtensions
{
    private const string StorageName = "localStorage";
    private const string SetItemMethod = StorageName + ".setItem";
    private const string GetItemMethod = StorageName + ".getItem";
    private const string RemoveItemMethod = StorageName + ".removeItem";
    private const string LengthMethod = StorageName + ".length";
    private const string KeyMethod = StorageName + ".key";

    public static async Task<string> LocalStorageGetItem(this IJSRuntime jsRuntime, string key, CancellationToken ct) =>
        await jsRuntime.InvokeAsync<string>(GetItemMethod, ct, key);

    public static async Task LocalStorageSetItem(this IJSRuntime jsRuntime, string key, string value, CancellationToken ct) =>
        await jsRuntime.InvokeVoidAsync(SetItemMethod, ct, key, value);

    public static async Task LocalStorageRemoveItem(this IJSRuntime jsRuntime, string key, CancellationToken ct) =>
        await jsRuntime.InvokeVoidAsync(RemoveItemMethod, ct, key);

    public static async Task<int> LocalStorageLength(this IJSRuntime jsRuntime, CancellationToken ct) =>
        await jsRuntime.InvokeAsync<int>("eval", ct, LengthMethod);

    public static async Task<string> LocalStorageGetKey(this IJSRuntime jsRuntime, int index, CancellationToken ct) =>
        await jsRuntime.InvokeAsync<string>(KeyMethod, ct, index);
}