using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Savings.DAO.Infrastructure;
using Savings.Model;

namespace MoneyPlan.API.Controllers
{
    [Route("api/BudgetPlan")]
    [ApiController]
    public class BudgetPlanController : ControllerBase
    {
        private readonly SavingsContext _context;

        public BudgetPlanController(SavingsContext context)
        {
            _context = context;
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
                    await _context.RelationshipBudgetPlanToRules.Where(x => x.BudgetPlanId == id).ExecuteDeleteAsync();
                    await _context.SaveChangesAsync();
                    foreach(var relation in model.Rules)
                    {
                        _context.RelationshipBudgetPlanToRules.Add(relation);
                        await _context.SaveChangesAsync();
                    }

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

                var ids = await _context.RelationshipBudgetPlanToRules
                    .Where(x => x.BudgetPlanId == planId)
                    .Select(x => x.BudgetPlanRule.Id)
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

            var inUseEntities = _context.BudgetPlans.Include(x => x.Rules)
                .Where(x => x.Rules.Any(x => x.BudgetPlanRuleId == id));
            if (inUseEntities.Any())
            {
                return this.Conflict($"The Budget Plan Rule is in use on the following Budget Plans {string.Join("; ", inUseEntities.Select(x => x.Name))}");
            }

            await _context.BudgetPlanRules.Where(x => x.Id == id).ExecuteDeleteAsync();

            return Ok();
        }
    }
}
