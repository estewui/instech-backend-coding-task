using Auditing.Infrastructure.Interfaces;
using System;
using Microsoft.Extensions.DependencyInjection;
using Auditing.Services;

namespace Auditing.Configuration
{
    public static class ContainerConfiguration
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IAuditService, AuditService>();
        }
    }
}
