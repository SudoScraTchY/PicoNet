using BitzArt.Blazor.Auth.Server;
using Microsoft.AspNetCore.Authentication.Cookies;
using PicoNet.Contracts.Extensions;
using PicoNet.UI.Components;
using PicoNet.UI.Extensions;
using PicoNet.UI.Services;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddServiceDiscovery();

    builder.Services.ConfigureHttpClientDefaults(http =>
    {
        http.AddServiceDiscovery();
    });
}

// Add Garnet distributed caching
var rawRedisUrl = builder.Configuration.GetConnectionString("piconet-cache");
var redisConnection = RedisConnectionParser.ParseRenderRedisUrl(rawRedisUrl);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnection;
});

builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<ICircuitTokenStore, CircuitTokenStore>();
builder.Services.AddScoped<PendingValidationState>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.AddBlazorAuth<PicoNetAuthenticationService>();

builder.Services.AddApiClients(builder.Configuration);
builder.Services.AddAdminServices(builder.Configuration);

// Auth
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "PicoNet.Auth";
        options.LoginPath = "/login";
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState(); // makes auth state available to all components

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapAuthEndpoints();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();