using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SimpleIdempotency.Internal;
using SimpleIdempotency.Persistence;
using SimpleIdempotency.Persistence.Repository;
using SimpleIdempotency.Services;

namespace SimpleIdempotency
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIdempotency(this IServiceCollection services)
        {
            services.AddScoped<IIdempotencyCache, IdempotencyCache>();
            services.AddScoped<IIdempotencyCacheCleaner, IdempotencyCacheCleaner>();
            services.AddHostedService<CleanupJob>();
            return services;
        }

        public static IServiceCollection AddInvoices(this IServiceCollection services)
        {
            services.AddScoped<IInvoiceService, InvoiceService>();
            return services;
        }

        public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
        {
            services.AddApplicationDbContext(connectionString);
            services.TryAddTransient<DbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
            services.TryAddScoped<IUnitOfWork, UnitOfWork>();
            services.TryAddScoped<IIdempotencyKeyRepository, IdempotencyKeyRepository>();
            services.TryAddScoped<IInvoiceRepository, InvoiceRepository>();
            return services;
        }

        private static void AddApplicationDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options
                    .UseSqlServer(
                        connectionString,
                        cfg => cfg.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
            });
        }
    }
}