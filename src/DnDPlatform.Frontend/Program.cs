using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Http;
using DnDPlatform.Frontend;
using DnDPlatform.Frontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri("http://localhost:5024/api") 
});
builder.Services.AddSingleton<LoginSingletonService>();

await builder.Build().RunAsync();
    