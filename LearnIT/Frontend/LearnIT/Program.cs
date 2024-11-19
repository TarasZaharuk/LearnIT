using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace LearnIT
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");
            builder.Services.AddMudServices();
            builder.Services.AddIgniteUIBlazor();
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddScoped<AuthenticatedHttpClient>();
            builder.Services.AddScoped<UserAuthenticationStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
                provider.GetRequiredService<UserAuthenticationStateProvider>());
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7123") });

            await builder.Build().RunAsync();
        }
    }
}
