using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Formula.DynConditionObject
{
    internal class BetweenTransformProvider : ITransFormProvider
    {
        public bool Match(ConditionItem item, Type type)
        {
            return item.Method == QueryMethod.Between;
        }

        public IEnumerable<ConditionItem> Transform(ConditionItem item, Type type)
        {
            object[] objs = item.Value as object[];

            return new[]
                       {
                           new ConditionItem(item.Field, QueryMethod.GreaterThanOrEqual, objs[0]),
                           new ConditionItem(item.Field, QueryMethod.LessThan, objs[1])
                       };
        }
    }
}
