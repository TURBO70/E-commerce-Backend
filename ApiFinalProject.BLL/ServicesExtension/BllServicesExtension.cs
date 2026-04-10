using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using ApiFinalProject.BLL.Managers;
using ApiFinalProject.BLL.MappingProfiles;

namespace ApiFinalProject.BLL.ServicesExtension;

public static class BllServicesExtension
{
    public static IServiceCollection AddBllServices(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(cfg => {
            cfg.AddProfile<MappingProfile>();
        });

        // FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Managers
        services.AddScoped<IAuthManager, AuthManager>();
        services.AddScoped<ICategoryManager, CategoryManager>();
        services.AddScoped<IProductManager, ProductManager>();
        services.AddScoped<ICartManager, CartManager>();
        services.AddScoped<IOrderManager, OrderManager>();

        return services;
    }
}
