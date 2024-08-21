using System;
using System.Collections.Generic;
using System.Text;

namespace Savings.Model
{
    /// <summary>
    /// It could be anything that serve to display a list of movements.<br/>
    /// Usually it refers to 'cash' or a 'bank account'.
    /// </summary>
    public class MoneyAccount
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
