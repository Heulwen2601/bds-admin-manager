using BdsAdmin.API.Extensions;
using BdsAdmin.API.Options;
using BdsAdmin.API.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddOptions<AdminSeedOptions>()
    .Bind(builder.Configuration.GetSection(AdminSeedOptions.SectionName))
    .ValidateDataAnnotations();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorizationPolicies();

var app = builder.Build();

app.UseExceptionHandling();

using (var scope = app.Services.CreateScope())
{
    var adminSeedService = scope.ServiceProvider.GetRequiredService<IAdminSeedService>();
    await adminSeedService.SeedAsync();

    var passwordMigrationService = scope.ServiceProvider.GetRequiredService<ILegacyPasswordMigrationService>();
    await passwordMigrationService.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseCors("AngularCors");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapRealtimeHubs();
app.Run();
