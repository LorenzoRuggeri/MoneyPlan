using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components;
using Radzen;
using Refit;
using Savings.Model;
using MoneyPlan.SPA;
using Savings.SPA.Authorization;
using MoneyPlan.SPA.Services;
using MoneyPlan.SPA.Handlers;

public class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        #region Radzen
        builder.Services.AddRadzenComponents();
        builder.Services.AddRadzenQueryStringThemeService();
        builder.Services.AddScoped<DialogService>();
        builder.Services.AddScoped<NotificationService>();
        #endregion

        builder.Services.AddScoped<CustomAuthorizationMessageHandler>();
        builder.Services.AddBlazoredLocalStorage();

        var configuredAuthentication = builder.Configuration["AuthenticationToUse"];

        if (configuredAuthentication == AuthenticationToUse.AzureAD)
        {
            builder.Services.AddMsalAuthentication(options =>
            {
                options.ProviderOptions.Cache.CacheLocation = "localStorage";
                options.ProviderOptions.LoginMode = "redirect";
                builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
                options.ProviderOptions.DefaultAccessTokenScopes.Add(builder.Configuration["AzureAd:DefaultScope"]);
            });
        }

        builder.Services.AddSingleton<HttpErrorBus>();
        builder.Services.AddScoped<HttpErrorHandler>();
        var httpClientBuilder = builder.Services.AddRefitClient<ISavingsApi>(new RefitSettings()
        {
            // Non lancia mai eccezione, cosi da non dover gestire i try/catch
            ExceptionFactory = httpResponse => Task.FromResult<Exception>(null)
        }).ConfigureHttpClient((sp, c) =>
        {
            c.BaseAddress = new Uri(builder.Configuration["ApiServiceUrl"]);
            if (configuredAuthentication == AuthenticationToUse.ApiKey)
            {
                c.DefaultRequestHeaders.Add("X-API-Key", builder.Configuration["ApiKey"]);
            }
        }).AddHttpMessageHandler<HttpErrorHandler>();

        if (configuredAuthentication == AuthenticationToUse.AzureAD)
        {
            httpClientBuilder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        }

        await builder.Build().RunAsync();
    }
}
