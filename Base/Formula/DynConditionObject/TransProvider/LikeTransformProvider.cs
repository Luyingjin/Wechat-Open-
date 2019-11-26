using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Formula.DynConditionObject
{
    /// <summary>
    /// Like运算符解析引擎
    /// </summary>
    internal class LikeTransformProvider : ITransFormProvider
    {
        /// <summary>
        /// 匹配运算符是否于查询单元中的运算符一致
        /// </summary>
        /// <param name="item">查询匹配单元</param>
        /// <param name="type">类型对象</param>
        /// <returns>是否匹配</returns>
        public bool Match(ConditionItem item, Type type)
        {
            return item.Method == QueryMethod.Like;
        }

        /// <summary>
        /// 转换翻译指定对象类型的ConditionItem实体
        /// </summary>
        /// <param name="item">查询匹配单元</param>
        /// <param name="type">类型对象</param>
        /// <returns>查询单元集合体</returns>
        public IEnumerable<ConditionItem> Transform(ConditionItem item, Type type)
        {
            var str = item.Value.ToString();
            var keyWords = str.Split('*');
            if (keyWords.Length == 1)
            {
                return new[] { new ConditionItem(item.Field, QueryMethod.Contains, item.Value) };
            }
            var list = new List<ConditionItem>();
            if (!string.IsNullOrEmpty(keyWords.First()))
                list.Add(new ConditionItem(item.Field, QueryMethod.StartsWith, keyWords.First()));
            if (!string.IsNullOrEmpty(keyWords.Last()))
                list.Add(new ConditionItem(item.Field, QueryMethod.EndsWith, keyWords.Last()));
            for (int i = 1; i < keyWords.Length - 1; i++)
            {
                if (!string.IsNullOrEmpty(keyWords[i]))
                    list.Add(new ConditionItem(item.Field, QueryMethod.Contains, keyWords[i]));
            }
            return list;
        }
    }
}
