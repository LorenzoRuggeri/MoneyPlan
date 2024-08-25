using System;
using System.Collections.Generic;
using System.Text;

namespace MoneyPlan.Model
{
    public interface ICloneable<T>
    {
        T Clone();
    }
}
