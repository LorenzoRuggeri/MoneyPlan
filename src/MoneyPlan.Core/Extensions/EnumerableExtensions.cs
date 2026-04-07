using MoneyPlan.Core.Miscellanea;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyPlan.Core.Extensions
{
    public static class EnumerableExtensions
    {
        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static IEnumerable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer,
                                                                                           IEnumerable<TInner> inner,
                                                                                           Func<TOuter, TKey> outerKeySelector,
                                                                                           Func<TInner, TKey> innerKeySelector,
                                                                                           Func<TOuter, TInner?, TResult> resultSelector
                                                                                           //IEqualityComparer<TKey>? comparer
                                                                                           )
        {
            var qry = outer.GroupJoin<TOuter, TInner, TKey, (TOuter, IEnumerable<TInner>)>(
                              inner,
                              foo => outerKeySelector(foo),
                              bar => innerKeySelector(bar),
                              (f, bs) => (f, bs))
                          .SelectMany(
                              fooBars => fooBars.Item2.DefaultIfEmpty(),
                              (x, y) => new { Foo = x.Item1, Bar = y })
                          .Select(z => resultSelector(z.Foo, z.Bar));
            return qry;
        }

        public static IEnumerable<T> Expand<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> elementSelector)
        {
            var stack = new Stack<IEnumerator<T>>();
            var e = source.GetEnumerator();
            try
            {
                while (true)
                {
                    while (e.MoveNext())
                    {
                        var item = e.Current;
                        yield return item;
                        var elements = elementSelector(item);
                        if (elements == null) continue;
                        stack.Push(e);
                        e = elements.GetEnumerator();
                    }
                    if (stack.Count == 0) break;
                    e.Dispose();
                    e = stack.Pop();
                }
            }
            finally
            {
                e.Dispose();
                while (stack.Count != 0) stack.Pop().Dispose();
            }
        }

        public static IEnumerable<TSource> ExceptByProperty<TSource, TProperty>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TProperty> keySelector)
        {
            return first.ExceptBy(second, x => x, GenericComparer<TSource, TProperty>.Comparer(keySelector));
        }
    }
}
