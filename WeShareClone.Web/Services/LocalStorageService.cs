using Microsoft.JSInterop;

namespace WeShareClone.Web.Services;

/// <summary>Thin wrapper around browser localStorage via JS interop.</summary>
public class LocalStorageService(IJSRuntime js)
{
    /// <summary>Returns the value stored under <paramref name="key"/>, or <c>null</c> if not set.</summary>
    public async Task<string?> GetItemAsync(string key)
        => await js.InvokeAsync<string?>("localStorage.getItem", key);

    /// <summary>Persists <paramref name="value"/> under <paramref name="key"/>.</summary>
    public async Task SetItemAsync(string key, string value)
        => await js.InvokeVoidAsync("localStorage.setItem", key, value);

    /// <summary>Removes the entry for <paramref name="key"/>.</summary>
    public async Task RemoveItemAsync(string key)
        => await js.InvokeVoidAsync("localStorage.removeItem", key);
}
