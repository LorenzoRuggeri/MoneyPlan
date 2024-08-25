using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Savings.DAO.Infrastructure;
using Savings.Model;

namespace MoneyPlan.API.Controllers
{
    [Route("api/MoneyAccounts")]
    [ApiController]
    public class MoneyAccountsController : ControllerBase
    {
        private readonly SavingsContext _context;

        public MoneyAccountsController(SavingsContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MoneyAccount>>> GetMoneyAccounts()
        {
            var res = await _context.MoneyAccounts
                .OrderBy(x => x.ID)
                .ToListAsync();

            return res;
        }
    }
}
