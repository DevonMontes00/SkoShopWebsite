using SKOShopifyWebsite.Components;
using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using SKOShopifyWebsite.Services;

var builder = WebApplication.CreateBuilder(args);

var shopifyConfig = builder.Configuration.GetSection("Shopify");
var shopifyToken = shopifyConfig["AccessToken"];
var shopifyBaseUrl = shopifyConfig["BaseUrl"];

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped(sp =>
{
    var client = new HttpClient
    {
        BaseAddress = new Uri(shopifyBaseUrl!)
    };
    client.DefaultRequestHeaders.Add(
        "X-Shopify-Storefront-Access-Token",
        shopifyToken!);
    return client;
});

builder.Services.AddScoped<ShopifyService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
