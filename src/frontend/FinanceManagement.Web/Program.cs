using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FinanceManagement.Web;
using FinanceManagement.Web.Services.Auth;
using FinanceManagement.Web.Services.Http;
using FinanceManagement.Web.Services.Operations;
using FinanceManagement.Web.Services.OperationTypes;
using FinanceManagement.Web.Services.Reports;
using FinanceManagement.Web.Services.Users;
using FinanceManagement.Web.Services.Wallets;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
    ?? throw new InvalidOperationException("ApiBaseUrl not configured");

builder.Services.AddSingleton<AuthStateService>();
builder.Services.AddScoped<AuthDelegatingHandler>();
builder.Services.AddScoped<AppAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    sp => sp.GetRequiredService<AppAuthenticationStateProvider>());

builder.Services.AddAuthorizationCore();

builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
    })
    .AddHttpMessageHandler<AuthDelegatingHandler>();

builder.Services.AddHttpClient("Api", client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
    })
    .AddHttpMessageHandler<AuthDelegatingHandler>();

builder.Services.AddHttpClient("Local", client =>
    {
        client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    });

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));

builder.Services.AddMudServices();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IOperationTypeService, OperationTypeService>();
builder.Services.AddScoped<IOperationService, OperationService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProfileService, ProfileService>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
    try
    {
        await authService.TryRestoreSessionAsync();
    }
    catch
    {
        // ignored
    }
}

await host.RunAsync();
