using BdsAdmin.API.Services.Interfaces;
using BdsAdmin.API.Services.Implementations;
using BdsAdmin.API.Repositories.Interfaces;
using BdsAdmin.API.Repositories.Implementations;
using BdsAdmin.API.Mappers.Interfaces;
using BdsAdmin.API.Mappers.Implementations;
using BdsAdmin.API.Entities;
using BdsAdmin.API.Constants;
using BdsAdmin.API.Data;
using BdsAdmin.API.Hubs;
using BdsAdmin.API.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

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
        services.AddScoped<ISellerProfileService, SellerProfileService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<ILeadService, LeadService>();
        services.AddScoped<IConversationService, ConversationService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IObjectStorageService, MinioObjectStorageService>();

        services.AddScoped<ISellerProfileRepository, SellerProfileRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<ILeadRepository, LeadRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();
    }

    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        services.Configure<MinioOptions>(configuration.GetSection(MinioOptions.SectionName));
        services.AddSignalR();
        services.AddCors(options =>
        {
            options.AddPolicy("AngularCors", policy => policy
                .WithOrigins(configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["http://localhost:4200"])
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
        });
    }

    public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            var key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is required.");
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                NameClaimType = ClaimTypes.Name,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });
    }

    public static void AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthPolicies.AdminOnly, p => p.RequireRole(AppRoles.Admin));
            options.AddPolicy(AuthPolicies.SellerOnly, p => p.RequireRole(AppRoles.Seller));
            options.AddPolicy(AuthPolicies.ConsultantOnly, p => p.RequireRole(AppRoles.Consultant));
            options.AddPolicy(AuthPolicies.UserOrSeller, p => p.RequireRole(AppRoles.User, AppRoles.Seller));
            options.AddPolicy(AuthPolicies.AuthenticatedUser, p => p.RequireAuthenticatedUser());
        });
    }

    public static void AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Real Estate Platform API", Version = "v1" });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                [new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                }] = []
            });
        });
    }

    public static void MapRealtimeHubs(this WebApplication app)
    {
        app.MapHub<NotificationHub>("/hubs/notification");
        app.MapHub<ChatHub>("/hubs/chat");
    }
}
