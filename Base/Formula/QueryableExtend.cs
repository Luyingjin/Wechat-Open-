using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Reflection;
using Formula;
using Formula.DynConditionObject;
using System.Collections;

namespace System.Linq
{
    public static class BaseQueryableExtend
    {
        public static void Delete<TEntity>(this IDbSet<TEntity> dbSet, Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            foreach (var item in dbSet.Where(predicate).ToArray())
            {
                dbSet.Remove(item);
            }
        }

        public static void RemoveWhere<TEntity>(this ICollection<TEntity> collection, Expression<Func<TEntity, bool>> predicate)
        {
            foreach (var item in collection.AsQueryable().Where(predicate).ToArray())
            {
                collection.Remove(item);
            }
        }

        public static void Update<TEntity>(this IQueryable<TEntity> query, Action<TEntity> updateAction)
        {
            foreach (var item in query.ToArray())
            {
                updateAction(item);
            }
        }

        public static void Update<TEntity>(this ICollection<TEntity> collection, Action<TEntity> updateAction)
        {
            collection.AsQueryable().Update(updateAction);
        }

        #region 按照QueryBuilder查询

        public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> source, SearchCondition cnd)
        {
            var query = source;

            Specifications specification = new Specifications();

            #region 权限过滤

            SearchCondition authCnd = FormulaHelper.CreateAuthDataFilter();
            foreach (var item in authCnd.Items)
            {
                specification.Or(item.Field, item.Value, item.Method, "Group1");
            }

            if (specification != null && specification.ConItemsCount > 0)
                query = query.Where(specification.GetExpression<TEntity>());

            #endregion

            specification.Clear();
            foreach (var item in cnd.Items)
            {
                #region 处理inLike值
                string[] arr = null;
                if (item.Method == QueryMethod.InLike)
                {
                    if (item.Value is ICollection)
                    {
                        ICollection<string> collection = item.Value as ICollection<string>;
                        arr = collection.ToArray<string>();
                    }
                    else
                    {
                        arr = item.Value.ToString().Split(',', '，');
                    }
                }
                #endregion

                if (!cnd.IsOrRelateion)
                {
                    if (item.Method != QueryMethod.InLike)
                    {
                        specification.AndAlso(item.Field, item.Value, item.Method);
                    }
                    else
                    {
                        foreach (string s in arr)
                        {
                            specification.Or(item.Field, s, QueryMethod.Like, string.IsNullOrEmpty(item.OrGroup) ? item.Field : item.OrGroup); //"Group1");
                        }
                    }
                }
                else
                {
                    if (item.Method != QueryMethod.InLike)
                    {
                        specification.Or(item.Field, item.Value, item.Method, string.IsNullOrEmpty(item.OrGroup) ? "Group0" : item.OrGroup);//"Group1");
                    }
                    else
                    {

                        foreach (string s in arr)
                        {
                            specification.Or(item.Field, s, QueryMethod.Like, string.IsNullOrEmpty(item.OrGroup) ? "Group0" : item.OrGroup); //"Group1");
                        }
                    }
                }
            }

            if (specification != null && specification.ConItemsCount > 0)
                query = query.Where(specification.GetExpression<TEntity>());

            return query;
        }

        #endregion

        #region 按周BaseQueryBuilder查询

        public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> source, BaseQueryBuilder qb)
        {
            var query = source.Where<TEntity>((SearchCondition)qb);

            if (qb.DefaultSort && typeof(TEntity).GetProperty("SortIndex") != null)
            {
                qb.SortField = "SortIndex";
                qb.SortOrder = "asc";
            }

            qb.TotolCount = query.Count();

            if (!string.IsNullOrEmpty(qb.SortField))
            {
                string[] fields = qb.SortField.Split(',');
                string[] orders = qb.SortOrder.Split(',');
                for (int i = 0; i < fields.Length; i++)
                {
                    bool isThenBy = true;
                    if (i == 0)
                        isThenBy = false;
                    query = query.OrderBy<TEntity>(fields[i], string.Equals(orders[i], Formula.SortMode.Asc.ToString(), StringComparison.CurrentCultureIgnoreCase), isThenBy);

                }                
            }
            else
            {
                query = query.OrderBy<TEntity>("ID", false);
            }

            if (qb.PageSize == 0)
                return query;

            query = query.Skip(qb.PageSize * qb.PageIndex).Take(qb.PageSize);

            return query;
        }

        #endregion

        #region 动态OrderBy

        public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> query, string property, bool isAscending, bool isThenBy = false)
        {
            ParameterExpression param = System.Linq.Expressions.Expression.Parameter(typeof(TEntity), "it");
            System.Linq.Expressions.Expression body = param;
            if (Nullable.GetUnderlyingType(body.Type) != null)
                body = System.Linq.Expressions.Expression.Property(body, "Value");

            PropertyInfo sortProperty = typeof(TEntity).GetProperty(property, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (sortProperty == null)
                throw new Exception("对象上不存在" + property + "的字段");
            body = System.Linq.Expressions.Expression.MakeMemberAccess(body, sortProperty);
            LambdaExpression keySelectorLambda = System.Linq.Expressions.Expression.Lambda(body, param);
            string queryMethod = isAscending ? "OrderBy" : "OrderByDescending";
            if (isThenBy == true)
                queryMethod = isAscending ? "ThenBy" : "ThenByDescending";
            query = query.Provider.CreateQuery<TEntity>(System.Linq.Expressions.Expression.Call(typeof(Queryable), queryMethod,
                                                      new Type[] { typeof(TEntity), body.Type },
                                                      query.Expression,
                                                       System.Linq.Expressions.Expression.Quote(keySelectorLambda)));
            return query;
        }

        #endregion
    }
}
