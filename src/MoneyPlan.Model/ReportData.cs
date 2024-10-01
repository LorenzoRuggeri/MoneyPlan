using System;
using System.Collections.Generic;
using System.Linq;

namespace Savings.Model
{
    public class ReportCategory
    {
        public long? CategoryID { get; set; }
        public string Category { get; set; }
        public string CategoryIcon { get; set; }
        public ReportPeriodAmount[] Data { get; set; }
    }

    // TODO: Vedere se riesco a metterlo a fattor comune. Mi pare un buon candidato.
    public class ReportPeriodAmount
    {
        public string Period { get; set; }
        public double Amount { get; set; }
    }

    public class ReportDetail
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }

        /// <summary>
        ///     <para>Description of whatever you need to be represented.</para>
        /// </summary>
        public string Description { get; set; }
    }

    public class ReportFullDetail
    {
        public string Type { get; set; }

        public long ID { get; set; }

        public string Period { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }

        public long? CategoryID { get; set; }

        public decimal Amount { get; set; }
    }


    public class GroupCategory : Category
    {
        public List<Category> Related { get; set; } = new List<Category>();

        /// <summary>
        /// Does it belong to any <see cref="Category"/> contained in this group?
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public bool HasCategory(long? category)
        {
            return this.ID == category || this.Related.Any(y => y.ID == category);
        }
    }

    public class Category
    {
        public long? ID { get; set; }

        public string Description { get; set; }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Category)) return false;
            return (obj as Category).ID == ID;
        }
    }
}
