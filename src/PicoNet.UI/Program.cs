using BitzArt.Blazor.Auth.Server;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using PicoNet.UI.ApiClients.Implementations;
using PicoNet.UI.ApiClients.Interfaces;
using PicoNet.UI.Components;
using PicoNet.UI.Extensions;
using PicoNet.UI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceDiscovery();
builder.Services.ConfigureHttpClientDefaults(http =>
{
    http.AddServiceDiscovery();
});

// Add Garnet distributed caching
builder.AddRedisDistributedCache(connectionName: "piconet-cache");

builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<ICircuitTokenStore, CircuitTokenStore>();
builder.Services.AddScoped<PendingValidationState>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.AddBlazorAuth<PicoNetAuthenticationService>();

builder.Services.AddApiClients(builder.Configuration);

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