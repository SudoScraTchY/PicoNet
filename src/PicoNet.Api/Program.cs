using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using PicoNet.Application.Extensions;
using PicoNet.Application.Features.Auth.Handler;
using PicoNet.Application.Features.Redirect.Handler;
using PicoNet.Infrastructure.Cache;
using PicoNet.Infrastructure.Data;
using PicoNet.Infrastructure.Extensions;
using PicoNet.Infrastructure.Identity.Entities;
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
    opts.UseFluentValidation(cfg => cfg.RegistrationBehavior = RegistrationBehavior.ExplicitRegistration);
}).AddWolverineHttp();

// Add Redis distributed caching
builder.AddRedisDistributedCache(connectionName: "piconet-cache");
builder.Services.AddSingleton<IRedirectCacheService,RedirectCacheService>();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();

builder.Services.AddScoped<RegisterHandler>();
builder.Services.AddScoped<LoginHandler>();
builder.Services.AddScoped<ValidateRegistrationHandler>();
builder.Services.AddScoped<RefreshHandler>();
builder.Services.AddScoped<ChangeEmailHandler>();
builder.Services.AddScoped<RedirectHandler>();

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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
});

var app = builder.Build();

app.MapDefaultEndpoints();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PicoNetDbContext>();
    await db.Database.MigrateAsync();

    // Ensure default roles exist
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    foreach (var roleName in new[] { "User", "Admin" })
    {
        if (!await roleManager.RoleExistsAsync(roleName))
            await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
    }
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    if(await userManager.FindByNameAsync("administrator") == null)
    {
        var adminUser = new ApplicationUser
        {
            UserName = "administrator",
            Email = "administrator@example.com",
            EmailConfirmed = true,
            LockoutEnabled = false,
            TwoFactorEnabled = false,
            PhoneNumberConfirmed = false,
        };
        await userManager.CreateAsync(adminUser, "PicoNet@1234");
    }
}

app.MapScalarApiReference();
app.MapOpenApi();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();