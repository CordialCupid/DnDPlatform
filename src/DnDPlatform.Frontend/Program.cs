using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Http;
using DnDPlatform.Frontend;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri("http://localhost:5024/api") 
});
builder.Services.AddHttpClient("CharactersApi" , c => c.BaseAddress = new Uri(builder.Configuration["ApiSettings:CharactersApi"]));
builder.Services.AddHttpClient("TemplatesApi" , c => c.BaseAddress = new Uri(builder.Configuration["ApiSettings:TemplatesApi"]));

await builder.Build().RunAsync();
    