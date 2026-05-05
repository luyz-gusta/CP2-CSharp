using BancoDigital.API.Data;
using BancoDigital.API.Services;
using BancoDigital.API.Workers;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        opt.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Projeto Banco — API",
        Version = "v1",
        Description = "API do banco digital — FIAP 3ESR. Cadastra clientes (PF/PJ), agências e contratações de produtos com processamento assíncrono."
    });
});

var connectionString = builder.Configuration.GetConnectionString("Oracle")
    ?? throw new InvalidOperationException("Connection string 'Oracle' não configurada.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(connectionString));

builder.Services.AddSingleton<IContratacaoQueue, ContratacaoQueue>();
builder.Services.AddScoped<AnaliseCreditoService>();
builder.Services.AddHostedService<ContratacaoWorker>();

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        if (Environment.GetEnvironmentVariable("RESET_DB") == "true")
        {
            logger.LogWarning("RESET_DB=true — dropando tabelas gerenciadas antes de aplicar migrations.");
            await OracleSchemaReset.DropTabelasGerenciadasAsync(db);
        }

        await db.Database.MigrateAsync();
        await DbSeeder.SeedAsync(db);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Falha ao aplicar migrations / seed.");
    }
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Projeto Banco v1");
    c.RoutePrefix = string.Empty;
});

app.MapControllers();

app.Run();

public partial class Program { }
