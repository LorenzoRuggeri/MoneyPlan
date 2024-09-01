using Savings.Model;

namespace MoneyPlan.Business.Importer
{
    public interface IExcelImporter
    {
        /// <summary>
        /// Identify the Excel Importer
        /// </summary>
        string Name { get; }

        void Import(int account, IEnumerable<FixedMoneyItem> operations);

        IEnumerable<FixedMoneyItem> LoadFromExcel(byte[] bytes);
    }
}