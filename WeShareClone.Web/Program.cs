using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WeShareClone.Web.Services;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);

// Use an explicit ApiBaseUrl from config, or fall back to the hosting environment's
// base address so same-origin API calls resolve correctly when server-hosted.
string apiBase = builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress;

// Services are registered as singletons so state is shared across all components.
builder.Services.AddSingleton<LocalStorageService>();
builder.Services.AddSingleton<AuthStateService>();
builder.Services.AddSingleton<AuthMessageHandler>();

// AuthMessageHandler injects the JWT Bearer token into every API request.
builder.Services.AddScoped(sp =>
{
    AuthMessageHandler handler = sp.GetRequiredService<AuthMessageHandler>();
    handler.InnerHandler = new HttpClientHandler();
    return new HttpClient(handler) { BaseAddress = new Uri(apiBase) };
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<SettlementService>();
builder.Services.AddScoped<EntryService>();
builder.Services.AddScoped<ExchangeRateService>();

WebAssemblyHost host = builder.Build();

// Restore auth state from localStorage before the app renders.
AuthStateService authState = host.Services.GetRequiredService<AuthStateService>();
await authState.InitializeAsync();

await host.RunAsync();
