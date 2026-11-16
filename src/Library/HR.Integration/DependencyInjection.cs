using HR.Integration.Reporting;
using Microsoft.Extensions.DependencyInjection;

namespace HR.Integration
{
    // Services that talk to the outside world (files, export formats, later e-mail / CV parsing).
    public static class DependencyInjection
    {
        public static IServiceCollection AddHrIntegration(this IServiceCollection services)
        {
            services.AddSingleton<ICsvWriter, CsvWriter>();
            return services;
        }
    }
}
