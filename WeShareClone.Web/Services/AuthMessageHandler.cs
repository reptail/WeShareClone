using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using WeShareClone.Web.Models;

namespace WeShareClone.Web.Services;

/// <summary>
/// DelegatingHandler that injects the Bearer token and transparently refreshes it
/// when expired, without requiring user interaction.
///
/// Two strategies:
///   1. Proactive — refreshes before sending if the access token is already expired.
///   2. Reactive  — intercepts 401 responses, refreshes, then retries the original request once.
///
/// Note: the refresh call is made directly via base.SendAsync() to avoid the circular
/// dependency that would occur if AuthService (which depends on HttpClient) were injected here.
/// </summary>
public class AuthMessageHandler(AuthStateService authState, NavigationManager navigation) : DelegatingHandler
{
    private readonly SemaphoreSlim _refreshSemaphore = new(1, 1);

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        bool isAuthEndpoint = IsAuthEndpoint(request);

        // Proactively refresh if the token is already expired before wasting a round-trip.
        if (!isAuthEndpoint && authState.IsAuthenticated && IsTokenExpired())
        {
            await TryRefreshAsync(cancellationToken);
        }

        // Inject current Bearer token (may have just been refreshed above).
        SetBearerToken(request);

        // Buffer the request content so the body can be re-read on retry.
        if (request.Content is not null)
            await request.Content.LoadIntoBufferAsync();

        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        // Reactively refresh on 401 from a protected endpoint.
        if (response.StatusCode == HttpStatusCode.Unauthorized && !isAuthEndpoint)
        {
            bool refreshed = await TryRefreshAsync(cancellationToken);
            if (refreshed)
            {
                HttpRequestMessage retry = await CloneRequestAsync(request);
                SetBearerToken(retry);
                return await base.SendAsync(retry, cancellationToken);
            }
        }

        return response;
    }

    private async Task<bool> TryRefreshAsync(CancellationToken cancellationToken)
    {
        await _refreshSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (authState.RefreshToken is null)
            {
                await HandleRefreshFailureAsync();
                return false;
            }

            using HttpRequestMessage refreshRequest = new(HttpMethod.Post, "api/auth/refresh");
            refreshRequest.Content = JsonContent.Create(new { refreshToken = authState.RefreshToken });

            HttpResponseMessage response = await base.SendAsync(refreshRequest, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await HandleRefreshFailureAsync();
                return false;
            }

            AuthTokenModel? token = await response.Content.ReadFromJsonAsync<AuthTokenModel>(cancellationToken);
            if (token is null)
            {
                await HandleRefreshFailureAsync();
                return false;
            }

            await authState.SetTokenAsync(token.AccessToken, token.RefreshToken);
            return true;
        }
        finally
        {
            _refreshSemaphore.Release();
        }
    }

    private async Task HandleRefreshFailureAsync()
    {
        await authState.ClearAsync();
        navigation.NavigateTo("/login");
    }

    private bool IsTokenExpired()
        => authState.AccessTokenExpiresAtUtc is { } expiry && expiry <= DateTime.UtcNow;

    private void SetBearerToken(HttpRequestMessage request)
    {
        if (authState.AccessToken is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authState.AccessToken);
    }

    private static bool IsAuthEndpoint(HttpRequestMessage request)
        => request.RequestUri?.AbsolutePath.Contains("/api/auth/", StringComparison.OrdinalIgnoreCase) == true;

    private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage original)
    {
        HttpRequestMessage clone = new(original.Method, original.RequestUri);

        foreach ((string key, IEnumerable<string> values) in original.Headers)
            clone.Headers.TryAddWithoutValidation(key, values);

        if (original.Content is not null)
        {
            MemoryStream ms = new();
            await original.Content.CopyToAsync(ms);
            ms.Position = 0;
            clone.Content = new StreamContent(ms);

            foreach ((string key, IEnumerable<string> values) in original.Content.Headers)
                clone.Content.Headers.TryAddWithoutValidation(key, values);
        }

        return clone;
    }
}
