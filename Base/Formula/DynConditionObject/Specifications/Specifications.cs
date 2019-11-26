using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Formula.DynConditionObject
{
    public class Specifications : ISpecifications
    {
        List<ConditionItem> _conItems;

        /// <summary>
        /// 获取查询对象实体的集合
        /// </summary>
        ///<remarks>只读属性</remarks>
        public int ConItemsCount
        {
            get { return _conItems.Count; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Specifications()
        {
            _conItems = new List<ConditionItem>();
        }

        /// <summary>
        /// 与逻辑关联规约
        /// </summary>
        /// <param name="prop">实体属性名称</param>
        /// <param name="val">值</param>
        /// <param name="method">运算符</param>
        public void AndAlso(string prop, object val, QueryMethod method)
        {
            _conItems.Add(new ConditionItem(prop, method, val));
        }

        /// <summary>
        /// 默认与逻辑关联规约
        /// </summary>
        /// <param name="prop">实体属性名称</param>
        /// <param name="val">值</param>
        public void DefaultAndAlso(string prop, object val)
        {
            _conItems.Add(new ConditionItem(prop, QueryMethod.Like, val));
        }

        /// <summary>
        /// 非逻辑关联规约
        /// </summary>
        /// <param name="prop">实体属性名称</param>
        /// <param name="val">值</param>
        /// <param name="method">运算符</param>
        public void Or(string prop, object val, QueryMethod method, string Group)
        {
            ConditionItem conItem = new ConditionItem(prop, method, val);
            conItem.OrGroup = Group;
            _conItems.Add(conItem);
        }

        /// <summary>
        /// 默认非逻辑关联规约
        /// </summary>
        /// <param name="prop">实体属性名称</param>
        /// <param name="val">值</param>
        public void DefaultOr(string prop, object val)
        {
            ConditionItem conItem = new ConditionItem(prop, QueryMethod.Like, val);
            conItem.OrGroup = "OrGroup_1";
            _conItems.Add(conItem);
        }

        /// <summary>
        /// 清空所有的数据规约
        /// </summary>
        public void Clear()
        {
            this._conItems.Clear();
            _SortField = string.Empty;
        }


        /// <summary>
        /// 获取当前规约的Lambada表达式
        /// </summary>
        /// <returns>当前的表达式</returns>
        public System.Linq.Expressions.Expression<Func<TEntity, bool>> GetExpression<TEntity>()
        {
            #region 兼容Oracle
            if (Config.Constant.IsOracleDb)
            {
                foreach (var item in this._conItems)
                {
                    if (item.Field != item.Field.ToUpper())
                    {
                        if (typeof(TEntity).GetProperty(item.Field) == null)
                            item.Field = item.Field.ToUpper();
                    }
                }
            }
            #endregion
            return LambdaExpressionGenrator.GenerateExpression<TEntity>(this._conItems);
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        string _SortField = string.Empty;
        /// <summary>
        /// 排序字段名称
        /// </summary>
        public string SortField
        {
            get { return _SortField; }
            set { _SortField = value; }
        }

        SortMode _Dir = SortMode.Desc;
        /// <summary>
        /// 升序或降序
        /// </summary>
        public SortMode Dir
        {
            get { return _Dir; }
            set { _Dir = value; }
        }
    }
}
