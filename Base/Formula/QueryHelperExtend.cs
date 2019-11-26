using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Formula;
using System.Data.SqlClient;

namespace Config
{
    public static class BaseSQLHelperExtend
    {
        #region SQLHelper支持SearchCondition

        public static DataTable ExecuteDataTable(this SQLHelper sqlHelper, string sql, SearchCondition cnd)
        {
            SearchCondition authCnd = FormulaHelper.CreateAuthDataFilter();

            #region 处理@参数
            List<SqlParameter> pList = new List<SqlParameter>();
            for (int i = authCnd.Items.Count - 1; i >= 0; i--)
            {
                var item = authCnd.Items[i];
                if (item.Field.StartsWith("@"))
                {
                    authCnd.Items.RemoveAt(i);
                    pList.Add(new SqlParameter(item.Field, item.Value));
                }
            }
            for (int i = cnd.Items.Count - 1; i >= 0; i--)
            {
                var item = cnd.Items[i];
                if (item.Field.StartsWith("@"))
                {
                    cnd.Items.RemoveAt(i);
                    pList.Add(new SqlParameter(item.Field, item.Value));
                }
            }
            #endregion

            string orderby = "";
            int index = sql.LastIndexOf("order by", StringComparison.CurrentCultureIgnoreCase);
            if (index > 0)
            {
                orderby = sql.Substring(index);
                sql = sql.Substring(0, index);
            }

            if (authCnd.Items.Count > 0)
                sql = string.Format("select * from ({0}) sourceTable1 {1}", sql, authCnd.GetWhereString());

            sql = string.Format("select * from ({0}) sourceTable {1} {2}", sql, cnd.GetWhereString(), orderby);

            DataTable dt = sqlHelper.ExecuteDataTable(sql, pList.ToArray(), CommandType.Text);
            return dt;
        }

        #endregion

        #region SQLHelper支持BaseQueryBuilder

        public static DataTable ExecuteDataTable(this SQLHelper sqlHelper, string sql, BaseQueryBuilder qb, bool dealOrderby = true)
        {
            string orderby = "";

            if (dealOrderby)
            {
                int index = sql.LastIndexOf(" order by", StringComparison.CurrentCultureIgnoreCase);
                if (index > 0)
                {
                    orderby = sql.Substring(index + " order by".Length);
                    sql = sql.Substring(0, index);
                }
            }

            SearchCondition authCnd = FormulaHelper.CreateAuthDataFilter();

            #region 处理@参数
            List<SqlParameter> pList = new List<SqlParameter>();
            for (int i = authCnd.Items.Count - 1; i >= 0; i--)
            {
                var item = authCnd.Items[i];
                if (item.Field.StartsWith("@"))
                {
                    authCnd.Items.RemoveAt(i);
                    pList.Add(new SqlParameter(item.Field, item.Value));
                }
            }
            for (int i = qb.Items.Count - 1; i >= 0; i--)
            {
                var item = qb.Items[i];
                if (item.Field.StartsWith("@"))
                {
                    qb.Items.RemoveAt(i);
                    pList.Add(new SqlParameter(item.Field, item.Value));
                }
            }
            #endregion

            if (authCnd.Items.Count > 0)
                sql = string.Format("select * from ({0}) sourceTable1 {1}", sql, authCnd.GetWhereString());

            sql = string.Format("select {2} from ({0}) sourceTable {1}", sql, qb.GetWhereString(), qb.Fields);

            string[] qbSortFields = qb.SortField.Split(',');
            string[] qbSortOrders = qb.SortOrder.Split(',');
            for (int i = 0; i < qbSortFields.Length; i++)
            {
                qbSortFields[i] += " " + qbSortOrders[i];
            }
            string qbOrderBy = string.Join(",", qbSortFields);
            if (orderby == "" || !qb.DefaultSort)
                orderby = qbOrderBy;

            if (qb.PageSize == 0)
            {
                DataTable dt = sqlHelper.ExecuteDataTable(sql + " order by " + orderby, pList.ToArray(), CommandType.Text);
                qb.TotolCount = dt.Rows.Count;
                return dt;
            }
            else
            {
                object totalCount = sqlHelper.ExecuteScalar(string.Format("select count(1) from ({0}) tableCount", sql), pList.ToArray(), CommandType.Text);
                qb.TotolCount = Convert.ToInt32(totalCount);


                int start = qb.PageIndex * qb.PageSize + 1;
                int end = start + qb.PageSize - 1;

                sql = string.Format(@"select * from (select tempTable1.*, Row_number() over(order by {1}) as RowNumber from ({0}) tempTable1) tmpTable2 where RowNumber between {2} and {3}", sql, orderby, start, end);

                return sqlHelper.ExecuteDataTable(sql, pList.ToArray(), CommandType.Text);
            }

        }

        #endregion
    }
}
