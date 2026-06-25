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

builder.Services.AddScoped<IUserTokenProvider, UserTokenProvider>();
builder.Services.AddScoped<ITokenStorage,ProtectedTokenStorage>();
builder.Services.AddScoped<PendingValidationState>();
builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthStateProvider>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "BlazorAuth";
        options.LoginPath = "/login";
    });

// Program.cs
builder.Services.AddScoped<TokenForwardingHandler>();

// Also register a named client for internal proxy if needed
builder.Services.AddHttpClient<IBlazorInternalApi,BlazorInternalApi>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7271/");
}).AddHttpMessageHandler<TokenForwardingHandler>();

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState(); // makes auth state available to all components
builder.Services.AddApiClients(builder.Configuration);

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
app.AddPicoNetAuthInternalHandler();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();