using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyPlan.Application.Abstractions.Budgeting;
using MoneyPlan.Model;
using Savings.DAO.Infrastructure;

namespace MoneyPlan.API.Controllers
{
    [Route("api/BudgetPlan")]
    [ApiController]
    public class BudgetPlanController : ControllerBase
    {
        private readonly SavingsContext _context;
        private readonly IBudgetPlanService budgetPlanService;

        public BudgetPlanController(SavingsContext context, IBudgetPlanService budgetPlanService)
        {
            _context = context;
            this.budgetPlanService = budgetPlanService;
        }

        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<BudgetPlan>>> GetBudgetPlans()
        {
            try
            {
                return await _context.BudgetPlans.ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPut("")]
        public async Task<IActionResult> UpdateBudgetPlan(int id, BudgetPlan model)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    // Save the rules to be applied to the Budget Plan.
                    /*
                    await _context.RelationshipBudgetPlanToRules.Where(x => x.BudgetPlanId == id).ExecuteDeleteAsync();
                    await _context.SaveChangesAsync();
                    foreach(var relation in model.Rules)
                    {
                        _context.RelationshipBudgetPlanToRules.Add(relation);
                        await _context.SaveChangesAsync();
                    }
                    */
                    // Clear the model to avoid reference tracking exception.
                    //model.Rules.Clear();

                    // Empty the materialized entities.
                    _context.BudgetPlans.Update(model);
                    await _context.SaveChangesAsync();

                    transaction.Commit();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpGet("Rules")]
        public async Task<ActionResult<IEnumerable<BudgetPlanRule>>> GetRules()
        {
            return await _context.BudgetPlanRules
                .Include(x => x.Category)
                .ToListAsync();
        }

        [HttpGet("{planId}/Rules")]
        public async Task<ActionResult<IEnumerable<BudgetPlanRule>>> GetRulesForBudgetPlan(int planId)
        {
            try
            {
                // TODO: Questo e' da cambiare!, prima faceva un altro giro, ora basta consultare BudgetPlanRules.
                var ids = await _context.BudgetPlanRules
                    .Where(x => x.BudgetPlanId == planId)
                    .Select(x => x.Id)
                    .ToListAsync();

                return await _context.BudgetPlanRules.Include(x => x.Category)
                    .Where(x => ids.Contains(x.Id))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("{planId}/Rules/Category/{categoryId}")]
        public ActionResult<IEnumerable<BudgetPlanRule>> GetRulesForCategory(int planId, int categoryId)
        {
            return Ok(budgetPlanService.GetRulesForCategory(planId, categoryId));
        }

        [HttpGet("Rules/{id}")]
        public ActionResult<BudgetPlanRule> GetBudgetPlanRule(int id)
        {
            var item = _context.BudgetPlanRules.FirstOrDefault(x => x.Id == id);

            return Ok(item);
        }

        [HttpPost("Rules")]
        public async Task<ActionResult<int>> CreateBudgetPlanRule(BudgetPlanRule item)
        {
            _context.BudgetPlanRules.Add(item);
            await _context.SaveChangesAsync();

            return Ok(item.Id);
        }

        [HttpPut("Rules/{id}")]
        public async Task<ActionResult> UpdateBudgetPlanRule(int id, BudgetPlanRule item)
        {
            if (!_context.BudgetPlanRules.Any(x => x.Id == id))
            {
                return NotFound("Budget Plan Rule has not been found.");
            }

            _context.BudgetPlanRules.Update(item);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("Rules")]
        public async Task<ActionResult> DeleteBudgetPlanRule(int id)
        {
            if (!_context.BudgetPlanRules.Any(x => x.Id == id))
            {
                return NotFound("Budget Plan Rule has not been found.");
            }

            await _context.BudgetPlanRules.Where(x => x.Id == id).ExecuteDeleteAsync();

            return Ok();
        }
    }
}
