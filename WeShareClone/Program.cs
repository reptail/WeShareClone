using System.Text;
using Fido2NetLib;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using WeShareClone.DataAccess.Repositories;
using WeShareClone.Domain.Repositories;
using WeShareClone.Domain.Services;
using WeShareClone.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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

builder.Services.Configure<Fido2Configuration>(builder.Configuration.GetSection("Fido2"));
builder.Services.AddScoped<Fido2>();

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options => options.EnableTryItOutByDefault());

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();