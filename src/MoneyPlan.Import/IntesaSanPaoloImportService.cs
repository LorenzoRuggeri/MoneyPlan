using MoneyPlan.Business.Importer;
using OfficeOpenXml;
using Savings.DAO.Infrastructure;
using Savings.Model;

namespace Savings.Import
{
    /// <summary>
    /// Service that import Excel from Intesa San Paolo into our Application.
    /// </summary>
    public class IntesaSanPaoloImportService : IExcelImporter
    {
        private readonly SavingsContext context;

        public string Name { get; } = "Intesa San Paolo";

        public IntesaSanPaoloImportService(SavingsContext context)
        {
            this.context = context;
        }

        public IEnumerable<FixedMoneyItem> LoadFromExcel(byte[] bytes)
        {
            List<FixedMoneyItem> operations = new List<FixedMoneyItem>();

            using (MemoryStream ms = new MemoryStream(bytes))
            using (ExcelPackage excelPackage = new ExcelPackage(ms))
            {
                ExcelWorkbook workBook = excelPackage.Workbook;
                ExcelWorksheet firstWorksheet = workBook.Worksheets[0];

                var rowsToSkip = 3;
                var rowsToProcess = firstWorksheet.Rows.Skip(rowsToSkip);

                int colCount = firstWorksheet.Dimension.End.Column;
                int rowCount = firstWorksheet.Dimension.End.Row;

                int actualRow = rowsToSkip + 1;

                for (int x = actualRow; x < rowCount; x++)
                {
                    var dateCell = firstWorksheet.Cells[x, 1];
                    var noteOperationCell = firstWorksheet.Cells[x, 2];
                    var noteDetailsCell = firstWorksheet.Cells[x, 3];
                    var categoryCell = firstWorksheet.Cells[x, 6];
                    var amountCell = firstWorksheet.Cells[x, 8];

                    // Discard invalid rows
                    if (dateCell.Value == null || amountCell.Value == null ||
                        dateCell.Value?.GetType() != typeof(DateTime))
                    {                        
                        continue;
                    }

                    // Transform output values
                    var category = MapCategory(categoryCell.GetValue<string>());
                    string notes;
                    if (category == null)
                    {
                        // We give some hints to understand which is the source.
                        notes = categoryCell.GetValue<string>() + " || " + noteOperationCell.GetValue<string>() + " || " + noteDetailsCell.GetValue<string>();
                    }
                    else
                    {
                        notes = noteOperationCell.GetValue<string>() + " || " + noteDetailsCell.GetValue<string>();
                    }

                    var itemtoAdd = new FixedMoneyItem()
                    {
                        Cash = false,
                        Date = dateCell.GetValue<DateTime>(),
                        Category = category,
                        Amount = amountCell.GetValue<decimal>(),
                        Note = notes
                    };

                    operations.Add(itemtoAdd);
                }
            }

            return operations;
        }

        public void Import(int accountId, IEnumerable<FixedMoneyItem> operations)
        {
            if (context.MoneyAccounts.FirstOrDefault(x => x.ID == accountId) == null)
            {
                throw new Exception($"Money account '{accountId}' has not been found");
            }

            operations = operations.Select(x => { x.AccountID = accountId; return x; });

            foreach (var operation in operations)
            {
                // NOTE: Controllo sui doppioni che pero' lascia molto a desiderare.... in quanto potrei avere nello stesso giorno
                //       due operazioni con lo stesso importo.
                //if (!context.FixedMoneyItems.Any(x => x.Date == operation.Date && x.Amount == operation.Amount))
                //{
                context.FixedMoneyItems.Add(operation);
                //}
            }

            context.SaveChanges();
        }


        private MoneyCategory? MapCategory(string category)
        {
            MoneyCategory? found;
            switch (category?.Trim())
            {
                case "Imposte, bolli e commissioni":
                    found = context.MoneyCategories.FirstOrDefault(x => x.Parent.Description == "Other" && x.Description == "Imposte, bolli e commissioni");
                    break;
                case "Polizze":
                    found = context.MoneyCategories.FirstOrDefault(x => x.Parent.Description == "Other" && x.Description == "Insurances & Policies");
                    break;
                case "Generi alimentari e supermercato":
                    found = context.MoneyCategories.FirstOrDefault(x => x.Parent.Description == "Family" && x.Description == "Food & Groceries");
                    break;
                case "Rate Mutuo e Finanziamento":
                    found = context.MoneyCategories.FirstOrDefault(x => x.Parent.Description == "Home" && x.Description == "Mortgage");
                    break;
                case "Cura della persona":
                    found = context.MoneyCategories.FirstOrDefault(x => x.Parent.Description == "Health and Wellness" && x.Description == "Personal care");
                    break;
                case "Ristoranti e bar":
                    found = context.MoneyCategories.FirstOrDefault(x => x.Parent.Description == "Leisure Time" && x.Description == "Restaurant & Bar");
                    break;
                case "Carburanti":
                    found = context.MoneyCategories.FirstOrDefault(x => x.Description == "Fuel" && x.Parent.Description == "Transports");
                    break;
                case "Tabaccai e simili":
                    found = context.MoneyCategories.FirstOrDefault(x => x.Parent.Description == "Leisure Time" && x.Description == "Tobacco shop");
                    break;
                case "Viaggi e vacanze":
                    found = context.MoneyCategories.FirstOrDefault(x => x.Parent.Description == "Leisure Time" && x.Description == "Viaggi e vacanze");
                    break;
                case "Spettacoli e musei":
                    found = context.MoneyCategories.FirstOrDefault(x => x.Parent.Description == "Leisure Time" && x.Description == "Shows, Concerts & Museums");
                    break;
                case "Tempo libero varie":
                    found = context.MoneyCategories.FirstOrDefault(x => x.Parent.Description == "Leisure Time" && x.Description == "Other");
                    break;
                default:
                    // TODO: Far tirare un'eccezione di modo da averli tutti mappati fin da subito.
                    found = null;
                    break;
            }

            return found;
        }


    }
}
