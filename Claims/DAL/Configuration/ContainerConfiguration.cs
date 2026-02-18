using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Runtime.InteropServices;
using Testcontainers.MongoDb;
using Testcontainers.MsSql;
using Microsoft.EntityFrameworkCore;
using DAL.Data2;

namespace DAL.Configuration
{
    public static class ContainerConfiguration
    {
        public async static Task ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Start Testcontainers for SQL Server and MongoDB
            var sqlContainer = (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                    ? new MsSqlBuilder()
                        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                    : new()

                ).Build();

            var mongoContainer = new MongoDbBuilder()
                .WithImage("mongo:latest")
                .Build();

            await sqlContainer.StartAsync();
            await mongoContainer.StartAsync();

            services.AddDbContext<AuditContext>(options => options.UseSqlServer(sqlContainer.GetConnectionString()));

            services.AddDbContext<ClaimsContext>(options =>
            {
                var client = new MongoClient(mongoContainer.GetConnectionString());
                var database = client.GetDatabase(configuration["MongoDb:DatabaseName"]); // Use a default/test database name
                options.UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName);
            });
        }
    }
}
