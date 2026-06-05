using Microsoft.EntityFrameworkCore;
using PicoNet.Application.Extensions;
using PicoNet.Infrastructure.Cache;
using PicoNet.Infrastructure.Data;
using PicoNet.Infrastructure.Extensions;
using PicoNet.ServiceDefaults;
using Scalar.AspNetCore;
using Wolverine;
using Wolverine.FluentValidation;
using Wolverine.Http;

var builder = WebApplication.CreateBuilder(args);

//builder.AddServiceDefaults();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);

// Add Wolverine
builder.Services.AddWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(ApplicationExtension).Assembly);
    opts.Durability.Mode = DurabilityMode.MediatorOnly;
    opts.UseFluentValidation(cfg => cfg.RegistrationBehavior = RegistrationBehavior.ExplicitRegistration);
}).AddWolverineHttp();

// Add Redis distributed caching
builder.AddRedisDistributedCache(connectionName: "piconet-cache");
builder.Services.AddTransient<IRedirectCacheService,RedirectCacheService>();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<PicoNetDbContext>();
    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
}
app.MapScalarApiReference();
app.MapOpenApi();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();