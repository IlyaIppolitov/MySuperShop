using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MySuperShop;
using MudBlazor.Services;
using MySuperShop.HttpApiClient;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();
builder.Services.AddSingleton<AppState>();
builder.Services.AddSingleton<IMyShopClient>(new MyShopClient(host: "localhost:7161"));

builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
