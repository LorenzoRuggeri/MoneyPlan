using OfficeOpenXml;
using Savings.DAO.Infrastructure;
using Savings.Model;

namespace Savings.Import
{
    /// <summary>
    /// Service that import Excel from Intesa San Paolo into our Application.
    /// </summary>
    public class IntesaSanPaoloImportService
    {
        private readonly SavingsContext context;

        public IntesaSanPaoloImportService(SavingsContext context)
        {
            this.context = context;
        }

        public void Import(string fileName)
        {
            FileInfo fi = new FileInfo(fileName);
            
            if (!fi.Exists)
            {
                throw new FileNotFoundException("File doesn't exsist");
            }

            List<FixedMoneyItem> operations = new List<FixedMoneyItem>();

            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                ExcelWorkbook workBook = excelPackage.Workbook;
                ExcelWorksheet firstWorksheet = workBook.Worksheets[0];

                var rowsToSkip = 20;
                var rowsToProcess = firstWorksheet.Rows.Skip(rowsToSkip);

                int colCount = firstWorksheet.Dimension.End.Column;
                int rowCount = firstWorksheet.Dimension.End.Row;

                int actualRow = rowsToSkip + 1;
                int actualCol = 1;

                // TODO: completare l'import.
                //List<MaterializedMoneySubitems>
                for (int x = actualRow; x < rowCount; x++)
                {
                    var dateValue = firstWorksheet.Cells[x, 1];
                    var noteOperationValue = firstWorksheet.Cells[x, 2];
                    var noteDetailsValue = firstWorksheet.Cells[x, 3];
                    var categoryValue = firstWorksheet.Cells[x, 6];
                    var amountValue = firstWorksheet.Cells[x, 8];

                    // TODO: Nel caso una Category non riesca ad essere mappata allora vogliamo avere nelle Note
                    //       il categoryValue, almeno cerchiamo di risalire a posteriori durante la fase di editing.
                    var itemtoAdd = new FixedMoneyItem()
                    {
                        Cash = false,
                        AccumulateForBudget = false,
                        Date = dateValue.GetValue<DateTime>(),
                        Category = MapCategory(categoryValue.GetValue<string>()),
                        Amount = amountValue.GetValue<decimal>(),
                        Note = noteOperationValue.GetValue<string>() + " || " + noteDetailsValue.GetValue<string>()
                    };

                    operations.Add(itemtoAdd);
                }

                foreach(var operation in operations)
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
