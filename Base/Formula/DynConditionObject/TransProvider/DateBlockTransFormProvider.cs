using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Formula.DynConditionObject
{
    /// <summary>
    /// 时间段查询解析引擎
    /// </summary>
    internal class DateBlockTransFormProvider : ITransFormProvider
    {
        /// <summary>
        /// 匹配运算符是否于查询单元中的运算符一致
        /// </summary>
        /// <param name="item">查询匹配单元</param>
        /// <param name="type">类型对象</param>
        /// <returns>是否匹配</returns>
        public bool Match(ConditionItem item, Type type)
        {
            return item.Method == QueryMethod.DateBlock;
        }

        /// <summary>
        /// 转换翻译指定对象类型的ConditionItem实体
        /// </summary>
        /// <param name="item">查询匹配单元</param>
        /// <param name="type">类型对象</param>
        /// <returns>查询单元集合体</returns>
        public IEnumerable<ConditionItem> Transform(ConditionItem item, Type type)
        {
            return new[]
                       {
                           new ConditionItem(item.Field, QueryMethod.GreaterThanOrEqual, item.Value),
                           new ConditionItem(item.Field, QueryMethod.LessThan, item.Value)
                       };
        }
    }
}
