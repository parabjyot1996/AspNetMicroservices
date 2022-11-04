using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Ordering.API.Extensions;
public static class HostExtensions
{
    public static IServiceCollection MigrateDatabase<TContext>(this IServiceCollection services,
                                            Action<TContext, IServiceProvider> seeder,
                                            int? retry = 0) where TContext : DbContext
    {
        int retryForAvailability = retry.Value;

        var serviceProvider = services.BuildServiceProvider();

        using (var scope = serviceProvider.CreateScope())
        {
            var logger = serviceProvider.GetRequiredService<ILogger<TContext>>();
            var context = serviceProvider.GetService<TContext>();

            try
            {
                logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);

                InvokeSeeder(seeder, context, serviceProvider);

                logger.LogInformation("Migrated database associated with context {DbContextName}", typeof(TContext).Name);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);

                if (retryForAvailability < 50)
                {
                    retryForAvailability++;
                    System.Threading.Thread.Sleep(2000);
                    MigrateDatabase<TContext>(services, seeder, retryForAvailability);
                }
            }
        }
        return services;
    }

    private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder,
                                                    TContext context,
                                                    IServiceProvider services)
                                                    where TContext : DbContext
    {
        context.Database.Migrate();
        seeder(context, services);
    }
}