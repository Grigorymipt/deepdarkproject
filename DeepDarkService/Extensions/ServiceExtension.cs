using Hangfire;
using Hangfire.MemoryStorage;

namespace DeepDarkService.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Example: Add Hangfire configuration
        services.AddHangfire(x => x.UseMemoryStorage()); //.UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));
        services.AddHangfireServer();
        services.AddSignalR();
        // Add MVC support 
        services.AddControllersWithViews();
        
        return services;
    }
}