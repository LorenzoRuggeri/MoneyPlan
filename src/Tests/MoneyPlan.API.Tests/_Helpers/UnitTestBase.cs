using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Savings.API.Services;
using Savings.API.Services.Abstract;
using Savings.DAO.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MoneyPlan.API.Tests._Helpers
{
    public abstract class UnitTestBase
    {
        private static IServiceCollection services = new ServiceCollection();

        static UnitTestBase()
        {
            services.AddDbContext<SavingsContext>((sp, options) =>
            {
                //var configuration = sp.GetRequiredService<IConfiguration>();
                //var dbName = configuration["DatabasePath"];

                var databaseName = Guid.NewGuid().ToString("D") + ".db";

                options.UseSqlite($"Data Source={databaseName}", sqlOpt =>
                {
                    var migrationAssemlby = Assembly.GetAssembly(typeof(SavingsContext)).FullName;

                    sqlOpt.MigrationsAssembly(migrationAssemlby);
                });
            });

            services.AddScoped<IProjectionCalculator, ProjectionCalculator>();
            services.AddScoped<ReportService>();
        }

        /// <summary>
        /// Register an implementation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Register<T>()
           where T : class
        {
            services.TryAddSingleton<T>();
        }

        public UnitTestContext CreateContext()
        {
            var sp = services.BuildServiceProvider();

            var dbContext = sp.GetRequiredService<SavingsContext>();
            dbContext.Database.EnsureCreated();
            SeedData(dbContext);

            return new UnitTestContext(sp, dbContext);
        }

        private void SeedData(SavingsContext dbContext)
        {
            // Delete the presets
            dbContext.MoneyCategories.ExecuteDelete();
            dbContext.SaveChanges();
        }
    }

    public class UnitTestContext : IDisposable
    {
        private readonly ServiceProvider serviceProvider;
        private readonly SavingsContext dbContext;
        private readonly Lazy<IProjectionCalculator> projectionCalculator;
        private readonly Lazy<ReportService> reportService;

        public SavingsContext DbContext => dbContext;

        public IProjectionCalculator ProjectionCalculator => projectionCalculator.Value;

        public ReportService ReportService => reportService.Value;

        public UnitTestContext(ServiceProvider sp, SavingsContext dbContext)
        {
            this.serviceProvider = sp;
            this.dbContext = dbContext;
            this.projectionCalculator = new Lazy<IProjectionCalculator>(() => sp.GetRequiredService<ProjectionCalculator>());
            this.reportService = new Lazy<ReportService>(() => sp.GetRequiredService<ReportService>());
        }

        public T Resolve<T>()
            where T : class
        {
            return serviceProvider.GetRequiredService<T>();
        }

        public void Dispose()
        {
            try
            {
                dbContext.Database.EnsureDeleted();
            }
            catch { }            
        }
    }
}
