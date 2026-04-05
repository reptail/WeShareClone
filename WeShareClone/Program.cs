using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Fido2NetLib;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using WeShareClone.Components;
using WeShareClone.DataAccess.Repositories;
using WeShareClone.Domain.Repositories;
using WeShareClone.Domain.Services;
using WeShareClone.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Prevent ASP.NET Core from remapping standard JWT claim names (e.g. "sub")
// to WS-Federation URIs. Claims arrive as-is from the token.
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    string[] xmlFiles =
    [
        $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml",
        $"{typeof(WeShareClone.Dto.Settlements.SettlementDto).Assembly.GetName().Name}.xml",
    ];

    foreach (string xmlFile in xmlFiles)
    {
        string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
            options.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Disable ASP.NET Core's default claim type mapping, which would rename
        // "sub" → ClaimTypes.NameIdentifier and other standard JWT claims to their
        // long WS-Federation URN equivalents.
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!)
            ),
        };
    });

builder.Services.AddScoped<Func<SqlConnection>>(
    sp => () => new SqlConnection(builder.Configuration.GetConnectionString("WeShareClone"))
);
builder.Services.AddScoped<ISettlementRepository, SettlementRepository>();
builder.Services.AddScoped<IEntryRepository, EntryRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVerificationCodeRepository, VerificationCodeRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IPendingSignupRepository, PendingSignupRepository>();
builder.Services.AddScoped<IPasskeyCredentialRepository, PasskeyCredentialRepository>();
builder.Services.AddScoped<IPasskeyChallengeRepository, PasskeyChallengeRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasskeyService, PasskeyService>();

// Fido2NetLib's Fido2 constructor requires Fido2Configuration directly, not IOptions<T>.
Fido2Configuration fido2Config = builder.Configuration.GetSection("Fido2").Get<Fido2Configuration>() ?? new Fido2Configuration();
builder.Services.AddSingleton(fido2Config);
builder.Services.AddScoped<Fido2>();

// Blazor Web App: server renders HTML shell, client pages run as WebAssembly.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options => options.EnableTryItOutByDefault());

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapControllers();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(WeShareClone.Web._Imports).Assembly);

app.Run();