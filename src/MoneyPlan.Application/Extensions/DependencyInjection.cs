using Microsoft.Extensions.DependencyInjection;
using MoneyPlan.Application.Abstractions.Budgeting;
using MoneyPlan.Application.Budgeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyPlan.Application.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IBudgetPlanCalculator, BudgetPlanCalculator>();
            services.AddScoped<IBudgetPlanService, BudgetPlanService>();
            return services;
        }
    }
}
