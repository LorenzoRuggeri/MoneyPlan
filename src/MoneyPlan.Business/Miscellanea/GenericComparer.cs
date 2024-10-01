using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyPlan.Business
{
    public sealed class GenericComparer<T, TProperty> : IEqualityComparer<T>
    {
        public static IEqualityComparer<T> Comparer(Func<T, TProperty> selector)
        {
            return new GenericComparer<T, TProperty>(selector);
        }

        private readonly Func<T, TProperty> selector;

        public GenericComparer(Func<T, TProperty> selector)
        {
            this.selector = selector;
        }

        public bool Equals(T? x, T? y)
        {
            if (x == null || y == null) return false;

            return Equals(selector(x), selector(y));
        }

        public int GetHashCode([DisallowNull] T obj)
        {
            object? value = selector(obj);

            if (value == null) return obj.GetHashCode();

            return value.GetHashCode();
        }
    }
}
