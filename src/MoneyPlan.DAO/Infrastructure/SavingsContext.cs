using Microsoft.EntityFrameworkCore;
using MoneyPlan.DAO.Mapping;
using Savings.DAO.Mapping;
using Savings.Model;
using System;

namespace Savings.DAO.Infrastructure
{
    public class SavingsContext : DbContext
    {
        public SavingsContext(DbContextOptions<SavingsContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

            modelBuilder.Entity<Configuration>().HasData(
                new Configuration { ID = 1, EndPeriodRecurrencyInterval = 1, EndPeriodRecurrencyType = RecurrencyType.Month }
            );

            modelBuilder.Entity<MoneyCategory>().HasData(
               new MoneyCategory { ID = 1, Description = "Family" },
                    new MoneyCategory { ID = 16, Description = "Food & Groceries", ParentId = 1 },

               new MoneyCategory { ID = 2, Description = "Home" },
                    new MoneyCategory { ID = 17, Description = "Mortgage", ParentId = 2 },

               new MoneyCategory { ID = 3, Description = "Leisure Time" },
                    new MoneyCategory { ID = 18, Description = "Restaurant", ParentId = 3 },
                    new MoneyCategory { ID = 19, Description = "Tobacco shop", ParentId = 3 },
                    new MoneyCategory { ID = 20, Description = "Other", ParentId = 3 },
                    new MoneyCategory { ID = 21, Description = "Shows, Concerts & Museums", ParentId = 3 },
                    // NOTE: puo' essere l'abbonamento alla palestra, cosi' come l'abbonamento a PrimeVideo o Spotify.
                    new MoneyCategory { ID = 22, Description = "Subscriptions", ParentId = 3 },

               new MoneyCategory { ID = 4, Description = "Transports" },
                    new MoneyCategory { ID = 23, Description = "Public Transport", ParentId = 4 },
                    // NOTE: ma nella macchina ci faccio convogliare tutto? Anche l'assicurazione.
                    new MoneyCategory { ID = 24, Description = "Car", ParentId = 4 },
                    // NOTE: se accendo un mutuo su una bici o su una macchina, userò questo.
                    new MoneyCategory { ID = 25, Description = "Loan", ParentId = 4 },
                    new MoneyCategory { ID = 26, Description = "Fuel", ParentId = 4 },

               new MoneyCategory { ID = 5, Description = "Financial Trading" },
                    new MoneyCategory { ID = 27, Description = "Compravendita titoli", ParentId = 5 },
                    new MoneyCategory { ID = 28, Description = "Subscriptions", ParentId = 5 },

               new MoneyCategory { ID = 6, Description = "Tech & Information Technologies" },
                    new MoneyCategory { ID = 29, Description = "Subscriptions", ParentId = 6 },

               new MoneyCategory { ID = 7, Description = "Other" },
                    // NOTE: Assicurazioni sulla casa, sulla vita
                    new MoneyCategory { ID = 30, Description = "Insurances & Policies", ParentId = 7 },
                    // NOTE: Usato per imposte di bollo, commissioni bancarie, etc
                    new MoneyCategory { ID = 31, Description = "Duties", ParentId = 7 },

               // NOTE: Forse questa e' l'unica entrata delle voci sopra. Oltre a Gift...
               new MoneyCategory { ID = 8, Description = "Salary" }

           );

            modelBuilder.Entity<MaterializedMoneyItem>().HasData(
               new MaterializedMoneyItem { ID = 1, Date = DateTime.Now.Date.AddDays(-DateTime.Now.Date.Day), Amount = 0, Type = MoneyType.InstallmentPayment, Projection = 0, EndPeriod = true, Cash = false }
            );
        }

        public DbSet<MaterializedMoneyItem> MaterializedMoneyItems { get; set; }
        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<MoneyCategory> MoneyCategories { get; set; }
        public DbSet<RecurrentMoneyItem> RecurrentMoneyItems { get; set; }
        public DbSet<FixedMoneyItem> FixedMoneyItems { get; set; }
        public DbSet<Configuration> Configuration { get; set; }

        public DbSet<MoneyAccount> MoneyAccounts { get; set; }

        public DbSet<BudgetPlan> BudgetPlans { get; set; }

        public DbSet<BudgetPlanRule> BudgetPlanRules { get; set; }

        public DbSet<BudgetPlanBudgetRules> RelationshipBudgetPlanToRules { get; set; }

    }
}
