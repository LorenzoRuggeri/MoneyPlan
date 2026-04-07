using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyPlan.Builder.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUnitTestBuilder(this IServiceCollection services)
        {
            services.AddScoped<DaoBuilder>();
            return services;
        }
    }
}
