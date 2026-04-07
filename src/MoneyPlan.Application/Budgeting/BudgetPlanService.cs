using Microsoft.EntityFrameworkCore;
using Savings.DAO.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoneyPlan.Model;
using MoneyPlan.Application.Abstractions.Budgeting;

namespace MoneyPlan.Application.Budgeting
{
    // TODO: Quando si lavora nell'Application dobbiamo fare uscire solo modelli del dominio Application.
    /// <summary>
    /// Given a context, it works against it, to produce anything related to a Budget Plan
    /// </summary>
    internal class BudgetPlanService : IBudgetPlanService
    {
        private readonly SavingsContext dbContext;

        public BudgetPlanService(SavingsContext dbContext)
        {
            this.dbContext = dbContext;
        }
        
        // TODO: Questo endpoint deve essere trasformato per accettare il BudgetPlanId. In quanto
        //       delle Rules 'intese come anagrafica' non hanno alcun senso di esistere, se non che potremmo riciclarle
        //       tra un BudgetPlan e l'altro. Ma piuttosto preferisco creare dei duplicati, anche perche' se cambio la Rule
        //       questo si riflette automaticamente sugli altri BudgetPlan (e non e' corretto!, si pensi ad un Rule.Filter che contiene una stringa).
        public IEnumerable<BudgetPlanRule> GetBudgetPlanRules()
        {
            // TODO: Filtrare per BudgetPlanId; al momento e' uno solo ma ne potrei avere piu' di uno.
            // Order by rules, to be sure they properly catch the items they tends to.
            var orderedRules = dbContext.BudgetPlanRules
                .OrderBy(x => x.CategoryId)
                //.ThenByDescending(x => x.CategoryFilter)
                .ToList();
            return orderedRules;
        }

        // TODO: Valutare se mettere un filtro addizionale 'includeDescendants' al fine di ottenere tutte le Rules che sono presenti
        //       nei figli (oltre che al padre ovviamente).
        //       Puo' tornare comodo sia quando rappresento un Tree, sia quando voglio poter filtrare nel IBudgetPlanCalculator.
        public IEnumerable<BudgetPlanRule> GetRulesForCategory(int budgetPlanId, int categoryId)
        {
            var rules = dbContext.BudgetPlans.Where(x => x.Id == budgetPlanId)
                .Include(x => x.Rules).ThenInclude(x => x.Category)
                .Select(x => x.Rules.Where(z => z.CategoryId == categoryId))
                .SelectMany(x => x.Select(z => z))
                .ToList();


            // TODO: Provare ad 'ottimizzare' mettendo la Where-Expression per CategoryId ma con tutti gli ID correlati.
            //       Per fare cio', mi serve il Service per le Catetories.
            var test = dbContext.BudgetPlans.Where(x => x.Id == budgetPlanId)
                .Include(x => x.Rules).ThenInclude(x => x.Category)
                .SelectMany(x => x.Rules.Select(z => z))
                .Where(x => x.CategoryId == categoryId)
                .ToList();
                

            return test;
        }
    }
}
