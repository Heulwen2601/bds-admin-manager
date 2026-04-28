using BdsAdmin.API.Services.Interfaces;
using BdsAdmin.API.Services.Implementations;
using BdsAdmin.API.Repositories.Interfaces;
using BdsAdmin.API.Repositories.Implementations;
using BdsAdmin.API.Mappers.Interfaces;
using BdsAdmin.API.Mappers.Implementations;
using BdsAdmin.API.Entities;
using Microsoft.AspNetCore.Identity;

namespace BdsAdmin.API.Extensions;

public static class ServiceExtensions
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserMapper, UserMapper>();
        services.AddScoped<ICategoryMapper, CategoryMapper>();
        services.AddScoped<IPropertyMapper, PropertyMapper>();

        // Auth
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAdminSeedService, AdminSeedService>();
        services.AddScoped<ILegacyPasswordMigrationService, LegacyPasswordMigrationService>();

        // Repository
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        // Service
        services.AddScoped<IPropertyService, PropertyService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICategoryService, CategoryService>();
    }
}
