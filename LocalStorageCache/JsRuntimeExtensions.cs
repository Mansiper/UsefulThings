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

    public static async Task<string> LocalStorageGetItem(this IJSRuntime jsRuntime, string key) =>
        await jsRuntime.InvokeAsync<string>(GetItemMethod, key);

    public static async Task LocalStorageSetItem(this IJSRuntime jsRuntime, string key, string value) =>
        await jsRuntime.InvokeVoidAsync(SetItemMethod, key, value);

    public static async Task LocalStorageRemoveItem(this IJSRuntime jsRuntime, string key) =>
        await jsRuntime.InvokeVoidAsync(RemoveItemMethod, key);

    public static async Task<int> LocalStorageLength(this IJSRuntime jsRuntime) =>
        await jsRuntime.InvokeAsync<int>("eval", LengthMethod);

    public static async Task<string> LocalStorageGetKey(this IJSRuntime jsRuntime, int index) =>
        await jsRuntime.InvokeAsync<string>(KeyMethod, index);
}