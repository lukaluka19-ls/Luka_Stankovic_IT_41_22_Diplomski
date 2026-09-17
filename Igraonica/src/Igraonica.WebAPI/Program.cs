using Igraonica.Application;
using Igraonica.Infrastructure;
using Igraonica.WebAPI.Extensions;
using Igraonica.WebAPI.Middleware;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

// Registracija po slojevima - svaki sloj izlaze sopstvenu metodu prosirenja,
// cime ulazna tacka programa ostaje kratka i pregledna.
builder.Services.AddApplicationLayer();
builder.Services.AddInfrastructureLayer(builder.Configuration);
builder.Services.AddWebApiLayer(builder.Configuration);

builder.Services.AddOpenApi();
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Default")!);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging();
app.UseCors("ClientPolicy");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApi();
app.MapScalarApiReference();
app.MapControllers();
app.MapHealthChecks("/health");

// Migracije se automatski primenjuju samo u razvojnom okruzenju.
if (app.Environment.IsDevelopment())
    await app.Services.ApplyMigrationsAndSeedAsync();

app.Run();

public partial class Program; // omogucava referencu iz Igraonica.IntegrationTests
