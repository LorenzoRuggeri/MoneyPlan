using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MoneyPlan.Application.Abstractions.Budgeting;
using MoneyPlan.Application.Budgeting;
using MoneyPlan.Application.Extensions;
using MoneyPlan.Builder;
using MoneyPlan.Builder.Extensions;
using Savings.DAO.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MoneyPlan.Application.Tests
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public abstract class UnitTestBase
    {
        private static object locker = new object();

        private ServiceProvider container;
        private static IServiceCollection services = new ServiceCollection();

        static UnitTestBase()
        {

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
            var sp = GetContainer();
            var scope = sp.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<SavingsContext>();
            dbContext.Database.EnsureCreated();

            SeedData(dbContext);

            return new UnitTestContext(scope, dbContext);
        }

        private void SeedData(SavingsContext dbContext)
        {
            // Delete the presets
            dbContext.MoneyCategories.ExecuteDelete();
            dbContext.SaveChanges();
        }

        /// <summary>
        /// Build <b>ONCE</b> the Container.
        /// </summary>
        /// <returns></returns>
        private ServiceProvider GetContainer()
        {
            lock (locker)
            {
                if (container != null)
                    return container;

                var services = RegisterServices();
                /*
                if (oneTimeRegistrations != null)
                {
                    var overrideDeps = new RegistarDependency(services);
                    oneTimeRegistrations(overrideDeps);
                }
                */
                return container = services.BuildServiceProvider();
            }
        }

        private IServiceCollection RegisterServices()
        {
            var services = new ServiceCollection();

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

            services.AddApplicationServices();
            services.AddUnitTestBuilder();

            return services;
        }

    }

    public class UnitTestContext : IDisposable
    {
        private readonly IServiceScope serviceScope;
        private readonly IServiceProvider serviceProvider;
        private readonly SavingsContext dbContext;

        public SavingsContext DbContext => dbContext;
        public DaoBuilder Builder => serviceProvider.GetRequiredService<DaoBuilder>();
        public DbSetup Setup => new DbSetup(Builder);
        public IBudgetPlanCalculator ProjectionCalculator => serviceProvider.GetRequiredService<IBudgetPlanCalculator>();
        public IBudgetPlanService BudgetPlanService => serviceProvider.GetRequiredService<IBudgetPlanService>();

        public UnitTestContext(IServiceScope scope, SavingsContext dbContext)
        {
            this.serviceScope = scope;
            this.serviceProvider = scope.ServiceProvider;
            this.dbContext = dbContext;
        }

        public T Resolve<T>()
            where T : class
        {
            return serviceScope.ServiceProvider.GetRequiredService<T>();
        }

        public void Dispose()
        {
            try
            {
                dbContext.Database.EnsureDeleted();
                serviceScope.Dispose();
            }
            catch { }
        }
    }
}
