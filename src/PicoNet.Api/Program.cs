using PicoNet.Application.Extensions;
using PicoNet.Infrastructure.Extensions;
using PicoNet.ServiceDefaults;
using Scalar.AspNetCore;
using Wolverine;
using Wolverine.FluentValidation;
using Wolverine.Http;

var builder = WebApplication.CreateBuilder(args);

InfrastructureExtensions.AddInfrastructure(builder.Services, builder.Configuration);

// Add Wolverine
builder.Services.AddWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(ApplicationExtension).Assembly);
    opts.Durability.Mode = DurabilityMode.MediatorOnly;
    opts.UseFluentValidation(cfg => cfg.RegistrationBehavior = RegistrationBehavior.ExplicitRegistration);
}).AddWolverineHttp();


// Add Aspire service defaults
builder.AddServiceDefaults();

// Add Garnet distributed caching
builder.AddRedisDistributedCache(connectionName: "piconet-cache");

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    // using (var scope = app.Services.CreateScope())
    // {
    //     var db = scope.ServiceProvider.GetRequiredService<PicoNetDbContext>();
    //     await db.Database.EnsureCreatedAsync();
    // }
}

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();