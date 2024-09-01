using Microsoft.AspNetCore.Mvc;
using MoneyPlan.Business.Importer;
using MoneyPlan.Model.API;
using Savings.DAO.Infrastructure;
using Savings.Import;

namespace Savings.API.Controllers
{
    [Route("api/Import")]
    [ApiController]
    public class ImportController : ControllerBase
    {
        private readonly SavingsContext _context;
        private readonly IEnumerable<IExcelImporter> importers;

        public ImportController(SavingsContext context, IEnumerable<IExcelImporter> importers)
        {
            this._context = context;
            this.importers = importers;
        }

        [HttpPost("ImportFromFile")]
        public async Task<ActionResult> ImportFromfile(ImportFileRequest request)
        {
            var importService = importers.FirstOrDefault(x => x.Name == request.Importer);
            if (importService == null)
            {
                throw new Exception("Importer service has not been found");
            }    

            var result = importService.LoadFromExcel(request.Content);
            importService.Import(request.Account, result);

            await Task.CompletedTask;
            return Ok();
        }

        [HttpGet("Importers")]
        public async Task<IActionResult> GetImporters()
        {
            var result = importers.Select(x => x.Name).ToList();

            await Task.CompletedTask;
            return Ok(result);
        }
    }
}
