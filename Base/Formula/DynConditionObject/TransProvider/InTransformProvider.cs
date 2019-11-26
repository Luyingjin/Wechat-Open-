using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Formula.DynConditionObject
{
    /// <summary>
    /// IN运算符解析引擎
    /// </summary>
    internal class InTransformProvider : ITransFormProvider
    {
        /// <summary>
        /// 匹配运算符是否于查询单元中的运算符一致
        /// </summary>
        /// <param name="item">查询匹配单元</param>
        /// <param name="type">类型对象</param>
        /// <returns>是否匹配</returns>
        public bool Match(ConditionItem item, Type type)
        {
            return item.Method == QueryMethod.In;
        }

        /// <summary>
        /// 转换翻译指定对象类型的ConditionItem实体
        /// </summary>
        /// <param name="item">查询匹配单元</param>
        /// <param name="type">类型对象</param>
        /// <returns>查询单元集合体</returns>
        public IEnumerable<ConditionItem> Transform(ConditionItem item, Type type)
        {
            var arr = (item.Value as Array);
            if (arr == null)
            {
                var arrStr = item.Value.ToString();
                if (!string.IsNullOrEmpty(arrStr))
                {
                    arr = arrStr.Split(',');
                }
            }
            return new[] { new ConditionItem(item.Field, QueryMethod.StdIn, arr) };
        }
    }
}
