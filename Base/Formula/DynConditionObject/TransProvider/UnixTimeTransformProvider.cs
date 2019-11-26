using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Formula.DynConditionObject
{
    /// <summary>
    /// 时间段查询解析引擎
    /// </summary>
    internal class UnixTimeTransformProvider : ITransFormProvider
    {
        /// <summary>
        /// 匹配运算符是否于查询单元中的运算符一致
        /// </summary>
        /// <param name="item">查询匹配单元</param>
        /// <param name="type">类型对象</param>
        /// <returns>是否匹配</returns>
        public bool Match(ConditionItem item, Type type)
        {
            var elementType = TypeUtil.GetUnNullableType(type);
            return ((elementType == typeof(int) && !(item.Value is int))
                    || (elementType == typeof(long) && !(item.Value is long))
                    || (elementType == typeof(DateTime) && !(item.Value is DateTime))
                   )
                   && item.Value.ToString().Contains("-");
        }

        /// <summary>
        /// 转换翻译指定对象类型的ConditionItem实体
        /// </summary>
        /// <param name="item">查询匹配单元</param>
        /// <param name="type">类型对象</param>
        /// <returns>查询单元集合体</returns>
        public IEnumerable<ConditionItem> Transform(ConditionItem item, Type type)
        {
            DateTime willTime;
            Type instanceType = TypeUtil.GetUnNullableType(type);
            if (DateTime.TryParse(item.Value.ToString(), out willTime))
            {
                var method = item.Method;

                if (method == QueryMethod.LessThan || method == QueryMethod.LessThanOrEqual)
                {
                    method = QueryMethod.DateTimeLessThanOrEqual;
                    if (willTime.Hour == 0 && willTime.Minute == 0 && willTime.Second == 0)
                    {
                        willTime = willTime.AddDays(1).AddMilliseconds(-1);
                    }
                }
                object value = null;
                if (instanceType == typeof(DateTime))
                {
                    value = willTime;
                }
                else if (instanceType == typeof(int))
                {
                    value = (int)UnixTime.FromDateTime(willTime);
                }
                else if (instanceType == typeof(long))
                {
                    value = UnixTime.FromDateTime(willTime);
                }
                return new[] { new ConditionItem(item.Field, method, value) };
            }

            return new[] { new ConditionItem(item.Field, item.Method, Convert.ChangeType(item.Value, type)) };
        }
    }
}
