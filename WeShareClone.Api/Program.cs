using Microsoft.Data.SqlClient;
using WeShareClone.Api.DataAccess.Repositories;
using WeShareClone.Api.Domain.Repositories;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    string[] xmlFiles =
    [
        $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml",
        $"{typeof(WeShareClone.Api.Dto.Settlements.SettlementDto).Assembly.GetName().Name}.xml",
    ];

    foreach (string xmlFile in xmlFiles)
    {
        string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
            options.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddScoped<Func<SqlConnection>>(
    sp => () => new SqlConnection(builder.Configuration.GetConnectionString("WeShareClone"))
);
builder.Services.AddScoped<ISettlementRepository, SettlementRepository>();
builder.Services.AddScoped<IEntryRepository, EntryRepository>();

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options => options.EnableTryItOutByDefault());

app.UseAuthorization();
app.MapControllers();

app.Run();