using DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DAL
{
    public static class DatabaseMigrator
    {
        public static void ApplyMigrations(this IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AuditContext>();
            db.Database.Migrate();
        }
    }
}
