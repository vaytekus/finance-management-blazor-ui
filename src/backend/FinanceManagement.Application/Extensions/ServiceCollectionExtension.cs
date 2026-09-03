using FinanceManagement.Application.Common.Behaviors;
using FinanceManagement.Application.Features.Reports.Common;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Application.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceManagement.Application.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(ServiceCollectionExtension).Assembly;
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddScoped<ReportBuilder>();
        services.AddScoped<ITokenIssuer, TokenIssuer>();
        services.AddScoped<IUserProvisioningService, UserProvisioningService>();

        return services;
    }
}
