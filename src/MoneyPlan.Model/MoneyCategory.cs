using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Savings.Model
{
    public class MoneyCategory
    {
        public long ID { get; set; }
        
        public long? ParentId { get; set; }
        public virtual MoneyCategory Parent { get; set; }
        
        public string Description { get; set; }

        public string Icon { get; set; }

        public virtual ICollection<MoneyCategory> Children { get; set; } = new List<MoneyCategory>();

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is MoneyCategory)) return false;
            return (obj as MoneyCategory).ID == ID;
        }
    }
}
