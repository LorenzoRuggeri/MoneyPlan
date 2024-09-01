using MoneyPlan.Business.Importer;
using Savings.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyPlan.Import
{
    public class FinecoImportService : IExcelImporter
    {
        public string Name { get; } = "Fineco";

        public void Import(int account, IEnumerable<FixedMoneyItem> operations)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FixedMoneyItem> LoadFromExcel(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
