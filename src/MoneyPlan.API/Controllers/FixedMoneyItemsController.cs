using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Savings.API.Services;
using Savings.DAO.Infrastructure;
using Savings.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Savings.API.Controllers
{
    [Route("api/fixedmoneyitems")]
    [ApiController]
    public class FixedMoneyItemsController : ControllerBase
    {
        private readonly SavingsContext _context;
        private readonly CategoriesService categoriesService;

        public FixedMoneyItemsController(SavingsContext context, CategoriesService categoriesService)
        {
            _context = context;
            this.categoriesService = categoriesService;
        }

        // GET: api/FixedMoneyItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FixedMoneyItem>>> GetFixedMoneyItems(DateTime? from, DateTime? to, bool excludeWithdrawal, long? filterCategory)
        {
            var withdrawalID = _context.Configuration.FirstOrDefault()?.CashWithdrawalCategoryID;
            var result = _context.FixedMoneyItems.Include(x => x.Category).AsQueryable();
            if (from.HasValue) result = result.Where(x => x.Date >= from);
            if (to.HasValue) result = result.Where(x => x.Date <= to);


            // If a Category has children, we want to return items for all Categories belonging to the Category we're filtering for.
            // That means the Category itself and all the descendants.
            if (filterCategory.HasValue)
            {
                var categoriesIDs = categoriesService.GetDescendats(filterCategory.Value)
                    .ToList();

                result = result.Where(x => x.CategoryID == filterCategory.Value ||
                                           x.CategoryID.HasValue && categoriesIDs.Contains(x.CategoryID.Value));
            }

            if (excludeWithdrawal) result = result.Where(x => x.CategoryID != withdrawalID);
            return await result.OrderByDescending(x => x.Date).ToListAsync();
        }

        // GET: api/FixedMoneyItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FixedMoneyItem>> GetFixedMoneyItem(long id)
        {
            var fixedMoneyItem = await _context.FixedMoneyItems.FindAsync(id);

            if (fixedMoneyItem == null)
            {
                return NotFound();
            }

            return fixedMoneyItem;
        }

        // PUT: api/FixedMoneyItems/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFixedMoneyItem(long id, FixedMoneyItem fixedMoneyItem)
        {
            if (id != fixedMoneyItem.ID)
            {
                return BadRequest();
            }

            _context.Entry(fixedMoneyItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FixedMoneyItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/FixedMoneyItems
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<FixedMoneyItem>> PostFixedMoneyItem(FixedMoneyItem fixedMoneyItem)
        {
            _context.FixedMoneyItems.Add(fixedMoneyItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFixedMoneyItem", new { id = fixedMoneyItem.ID }, fixedMoneyItem);
        }

        // DELETE: api/FixedMoneyItems/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<FixedMoneyItem>> DeleteFixedMoneyItem(long id)
        {
            var fixedMoneyItem = await _context.FixedMoneyItems.FindAsync(id);
            if (fixedMoneyItem == null)
            {
                return NotFound();
            }

            // The item is already persisted into Projection; so we need to remove it from Projection before deleting it.
            var persisted = _context.MaterializedMoneyItems.FirstOrDefault(x => x.FixedMoneyItemID == id);
            if (persisted != null)
            {
                return this.Conflict("Item has been persisted into History. Unlock it from History if you want to delete it.");
            }

            _context.FixedMoneyItems.Remove(fixedMoneyItem);
            await _context.SaveChangesAsync();

            return fixedMoneyItem;
        }

        private bool FixedMoneyItemExists(long id)
        {
            return _context.FixedMoneyItems.Any(e => e.ID == id);
        }
    }
}
