using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Linq.Expressions;

namespace Formula.DynConditionObject
{
    public interface ISpecifications : IEnumerable
    {
        /// <summary>
        /// 获取查询对象实体的集合
        /// </summary>
        /// <remarks>只读属性</remarks>
        int ConItemsCount
        { get; }

        /// <summary>
        /// 与逻辑关联规约
        /// </summary>
        /// <param name="prop">实体属性名称</param>
        /// <param name="val">值</param>
        /// <param name="method">运算符</param>
        void AndAlso(string prop, object val, QueryMethod method);

        /// <summary>
        /// 默认与逻辑关联规约
        /// </summary>
        /// <param name="prop">实体属性名称</param>
        /// <param name="val">值</param>
        void DefaultAndAlso(string prop, object val);

        /// <summary>
        /// 非逻辑关联规约
        /// </summary>
        /// <param name="prop">实体属性名称</param>
        /// <param name="val">值</param>
        /// <param name="method">运算符</param>
        void Or(string prop, object val, QueryMethod method, string Group);

        /// <summary>
        /// 默认非逻辑关联规约
        /// </summary>
        /// <param name="prop">实体属性名称</param>
        /// <param name="val">值</param>
        void DefaultOr(string prop, object val);

        /// <summary>
        /// 清空所有的数据规约
        /// </summary>
        void Clear();

        /// <summary>
        /// 排序字段名称
        /// </summary>
        string SortField
        { get; set; }

        /// <summary>
        /// 升序或降序
        /// </summary>
        SortMode Dir
        { get; set; }

        /// <summary>
        /// 获取当前规约的Lambada表达式
        /// </summary>
        /// <returns>当前的表达式</returns>
        Expression<Func<TEntity, bool>> GetExpression<TEntity>();

      

    }
}
