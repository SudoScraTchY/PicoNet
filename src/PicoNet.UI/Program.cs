using PicoNet.ServiceDefaults;
using PicoNet.UI.ApiClients.Implementations;
using PicoNet.UI.ApiClients.Interfaces;
using PicoNet.UI.Components;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add Garnet distributed caching
builder.AddRedisDistributedCache(connectionName: "piconet-cache");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient<IUrlApiClient, UrlApiClient>(client =>
{
    // "api" matches the resource name in AppHost: .AddProject<Projects.PicoNet_Api>("api")
    client.BaseAddress = new Uri("https+http://api");
});

var app = builder.Build();

// PicoNet.UI/Program.cs

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
