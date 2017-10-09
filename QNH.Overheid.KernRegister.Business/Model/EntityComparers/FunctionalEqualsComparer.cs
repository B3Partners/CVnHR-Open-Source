using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QNH.Overheid.KernRegister.Business.Model.Entities;

namespace QNH.Overheid.KernRegister.Business.Model.EntityComparers
{
    public class FunctionalEqualsComparer<T> : IEqualityComparer<T> where T : IFunctionalEquals<T>
    {
        public bool Equals(T x, T y)
        {
            return x.FunctionalEquals(y);
        }

        public int GetHashCode(T obj)
        {
            return 0; // obj.GetHashCode();
        }
    }

    public static class FunctionalCompare
    {
        public static bool FunctionalEquals<T>(this IEnumerable<T> items, IEnumerable<T> other) where T : IFunctionalEquals<T>
        {
            return !items.Except(other, new FunctionalEqualsComparer<T>()).Any()
                    && !other.Except(items, new FunctionalEqualsComparer<T>()).Any();
        }
    }
}
