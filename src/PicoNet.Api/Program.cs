using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PicoNet.Api.ModelBinding;
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
    //opts.Durability.Mode = DurabilityMode.Balanced;
    opts.UseFluentValidation(cfg => cfg.RegistrationBehavior = RegistrationBehavior.ExplicitRegistration);
}).AddWolverineHttp();

// Add Redis distributed caching
builder.AddRedisDistributedCache(connectionName: "piconet-cache");
builder.Services.AddSingleton<IRedirectCacheService,RedirectCacheService>();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();

var jwtSection = builder.Configuration.GetSection("Jwt");
var signingKey = Encoding.UTF8.GetBytes(jwtSection["Key"]!);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = JwtRegisteredClaimNames.Sub,
            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(signingKey),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.MapDefaultEndpoints();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PicoNetDbContext>();

    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
}
app.MapScalarApiReference();
app.MapOpenApi();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();