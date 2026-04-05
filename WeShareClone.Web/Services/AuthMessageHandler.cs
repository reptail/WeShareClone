namespace WeShareClone.Web.Services;

/// <summary>
/// DelegatingHandler that injects the Bearer token from <see cref="AuthStateService"/>
/// into every outgoing HTTP request to the API.
/// </summary>
public class AuthMessageHandler(AuthStateService authState) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (authState.AccessToken is not null)
        {
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authState.AccessToken);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
