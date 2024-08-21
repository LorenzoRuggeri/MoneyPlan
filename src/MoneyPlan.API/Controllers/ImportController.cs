using Microsoft.AspNetCore.Mvc;
using Savings.DAO.Infrastructure;
using Savings.Import;

namespace Savings.API.Controllers
{
    [Route("api/Import")]
    [ApiController]
    public class ImportController : ControllerBase
    {
        private readonly SavingsContext _context;
        private readonly IntesaSanPaoloImportService importService;

        public ImportController(SavingsContext context, IntesaSanPaoloImportService importService)
        {
            this._context = context;
            this.importService = importService;
        }

        /*
        [HttpPut("excel/{stream}")]
        public IActionResult Index(Stream stream)
        {
            return View();
        }
        */

        [HttpPut("ImportFromFile")]
        public async Task<ActionResult> ImportFromfile()
        {
            importService.Import("Data\\lista_completa_20082024.xlsx");

            await Task.CompletedTask;
            return Ok();
        }

    }
}
