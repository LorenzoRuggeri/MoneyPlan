using Microsoft.Extensions.DependencyInjection;
using MoneyPlan.Business.Importer;
using Savings.Import;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyPlan.Import.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddImporters(this IServiceCollection self)
        {
            self.AddScoped<IExcelImporter, IntesaSanPaoloImportService>();
            self.AddScoped<IExcelImporter, FinecoImportService>();
        }
    }
}
