using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Formula.Helper;
using System.Data;
using System.Collections;
using System.Linq.Expressions;
using System.Collections.Specialized;
using MvcAdapter;
using Config;
using System.ComponentModel;
using System.Reflection;
using System.Data.Entity;

namespace MvcAdapter
{
    public static class TabBuilder
    {
        private static string QUERYDATA = "querydata";

        /// <summary>
        /// 获取多个标签枚举信息
        /// </summary>
        /// <param name="enumKeys">枚举Key与对应查询列的字典</param>
        /// <returns>标签枚举集合</returns>
        public static ArrayList GetEnumsData(Dictionary<string, string> alEnumColumns)
        {
            ArrayList al = new ArrayList();
            foreach (KeyValuePair<string, string> kv in alEnumColumns)
            {
                Type type = GetEnumType(kv.Value);
                if (type == null)
                {
                    EnumDefInfo enumInfo = EnumBaseHelper.GetEnumDef(kv.Value);
                    Hashtable ht = new Hashtable();
                    ht["enumkey"] = enumInfo.Code;
                    ht["title"] = enumInfo.Name;
                    ht["queryfield"] = Convert.ToString(kv.Key);
                    ArrayList alItems = new ArrayList();
                    //系统枚举：周期
                    if (enumInfo.Code.ToLower() == "system.interval")
                    {
                        NameValueCollection nvc = GetNVCFromEnumValue(typeof(SystemEnumInterval));
                        foreach (string key in nvc)
                        {
                            Hashtable htItem = new Hashtable();
                            htItem["value"] = nvc[key];
                            htItem["text"] = key;
                            htItem["radio"] = "T";
                            alItems.Add(htItem);
                        }
                    }
                    //系统枚举：本人本部门
                    else if (enumInfo.Code.ToLower() == "system.ownerowndept")
                    {
                        NameValueCollection nvc = GetNVCFromEnumValue(typeof(SystemEnumUser));
                        foreach (string key in nvc)
                        {
                            Hashtable htItem = new Hashtable();
                            htItem["value"] = nvc[key];
                            htItem["text"] = key;
                            htItem["radio"] = "T";
                            alItems.Add(htItem);
                        }
                    }
                    else
                    {
                        ICollection<EnumItemInfo> enumItems = enumInfo.EnumItem;
                        foreach (EnumItemInfo item in enumItems)
                        {
                            Hashtable htItem = new Hashtable();
                            htItem["value"] = item.Code;
                            htItem["text"] = item.Name;
                            htItem["radio"] = "F";
                            alItems.Add(htItem);
                        }
                    }
                    ht["menus"] = alItems;
                    al.Add(ht);
                }
                else
                {
                    EnumDefInfo enumInfo = EnumBaseHelper.GetEnumDef(type);

                    Hashtable ht = new Hashtable();
                    ht["enumkey"] = enumInfo.Code;
                    ht["title"] = enumInfo.Description;
                    ht["queryfield"] = kv.Key;
                    ArrayList alItems = new ArrayList();

                    foreach (EnumItemInfo item in enumInfo.EnumItem)
                    {
                        Hashtable htItem = new Hashtable();
                        htItem["value"] = item.Code;
                        htItem["text"] = item.Name;
                        htItem["radio"] = "F";
                        alItems.Add(htItem);
                    }
                    ht["menus"] = alItems;
                    al.Add(ht);
                }
            }
            return al;
        }

        public static string GetEnumData(Type enumType,string radio="F")
        {
            Hashtable ht = new Hashtable();
            var enumDef =  EnumBaseHelper.GetEnumDef(enumType);
            var enumItems = EnumBaseHelper.GetEnumDef(enumType).EnumItem;
            ht["title"] = enumDef.Description;
            ht["enumkey"] = enumDef.Code;
            ArrayList alItems = new ArrayList();
            foreach (var item in enumItems)
            {
                var itemHs = new Hashtable();
                itemHs["value"] = item.Code;
                itemHs["text"] = item.Name; ;
                itemHs["radio"] = radio;
                alItems.Add(itemHs);
            }
            ht["menus"] = alItems;
            return JsonHelper.ToJson(ht);
        }

        /// <summary>
        /// 获取单个枚举信息
        /// </summary>
        /// <param name="enumCode"></param>
        /// <returns></returns>
        public static String GetEnumData(string enumCode)
        {
            Hashtable ht = new Hashtable();
            Type type = GetEnumType(enumCode);
            if (type == null)
            {
                EnumDefInfo enumInfo = EnumBaseHelper.GetEnumDef(enumCode);
                ht["enumkey"] = enumInfo.Code;
                ht["title"] = enumInfo.Name;
                ArrayList alItems = new ArrayList();
                //系统枚举：周期
                if (enumInfo.Code.ToLower() == "system.interval")
                {
                    NameValueCollection nvc = GetNVCFromEnumValue(typeof(SystemEnumInterval));
                    foreach (string key in nvc)
                    {
                        Hashtable htItem = new Hashtable();
                        htItem["value"] = nvc[key];
                        htItem["text"] = key;
                        htItem["radio"] = "T";
                        alItems.Add(htItem);
                    }
                }
                //系统枚举：本人本部门
                else if (enumInfo.Code.ToLower() == "system.ownerowndept")
                {
                    NameValueCollection nvc = GetNVCFromEnumValue(typeof(SystemEnumUser));
                    foreach (string key in nvc)
                    {
                        Hashtable htItem = new Hashtable();
                        htItem["value"] = nvc[key];
                        htItem["text"] = key;
                        htItem["radio"] = "T";
                        alItems.Add(htItem);
                    }
                }
                else
                {
                    ICollection<EnumItemInfo> enumItems = enumInfo.EnumItem;
                    foreach (EnumItemInfo item in enumItems)
                    {
                        Hashtable htItem = new Hashtable();
                        htItem["value"] = item.Code;
                        htItem["text"] = item.Name;
                        htItem["radio"] = "F";
                        alItems.Add(htItem);
                    }
                }
                ht["menus"] = alItems;
            }
            else
            {
                EnumDefInfo enumInfo = EnumBaseHelper.GetEnumDef(type);

                ht["enumkey"] = enumInfo.Code;
                ht["title"] = enumInfo.Description;
                ArrayList alItems = new ArrayList();

                foreach (EnumItemInfo item in enumInfo.EnumItem)
                {
                    Hashtable htItem = new Hashtable();
                    htItem["value"] = item.Code;
                    htItem["text"] = item.Name;
                    htItem["radio"] = "F";
                    alItems.Add(htItem);
                }
                ht["menus"] = alItems;
            }
            return JsonHelper.ToJson(ht);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source"></param>
        /// <param name="qb"></param>
        /// <param name="requestParams"></param>
        /// <param name="userId">当前用户Id</param>
        /// <returns>查询结果</returns>
        public static IList<TEntity> GetQueryResult<TEntity>(IQueryable<TEntity> source, QueryBuilder qb, NameValueCollection requestParams, string userId)
        {
            string queryData = requestParams[QUERYDATA];
            QueryBuilder qbTab = null;
            if (!string.IsNullOrEmpty(queryData))
            {
                Dictionary<string, List<Dictionary<string, string>>> ht = JsonHelper.ToObject<Dictionary<string, List<Dictionary<string, string>>>>(queryData);
                SearchCondition scTab = GetTabSearch(ht, userId);
                qbTab = new QueryBuilder();
                qbTab.PageSize = 0;
                qbTab.IsOrRelateion = scTab.IsOrRelateion;
                qbTab.Items = scTab.Items;

                SearchCondition sckey = GetKeySearch(ht);
                qb.Items = sckey.Items;
                qb.IsOrRelateion = sckey.IsOrRelateion;
            }
            qb.PageSize = 0;
            var list = source.Where(qb);
            int totalCount = qb.TotolCount;
            if (qbTab != null)
            {
                list = list.Where(qbTab);
            }
            //JSON
            return list.ToList();
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="sqlHelper"></param>
        /// <param name="sql"></param>
        /// <param name="qb"></param>
        /// <param name="requestParams"></param>
        /// <returns></returns>
        public static DataTable GetQueryResult(SQLHelper sqlHelper, string sql, QueryBuilder qb, NameValueCollection requestParams, string userId)
        {
            string queryData = requestParams[QUERYDATA];
            //#if demoData
            //            if (!string.IsNullOrEmpty(queryData))
            //                queryData = queryData.Replace("Industry", "CustomerBalance.Industry");
            //#else
            //#endif
            if (!string.IsNullOrEmpty(queryData))
            {

                Dictionary<string, List<Dictionary<string, string>>> ht = JsonHelper.ToObject<Dictionary<string, List<Dictionary<string, string>>>>(queryData);
                SearchCondition scTab = GetTabSearch(ht, userId);
                sql += scTab.GetWhereString();

                SearchCondition sckey = GetKeySearch(ht);
                qb.Items = sckey.Items;
                qb.IsOrRelateion = sckey.IsOrRelateion;
            }

            qb.PageSize = 0;
            DataTable result = sqlHelper.ExecuteDataTable(sql, qb); //SQLHelperExtend.ExecuteDataTable(sqlHelper, sql, qb);
            //JSON
            return result;
        }



        private static SearchCondition GetKeySearch(Dictionary<string, List<Dictionary<string, string>>> queryData)
        {
            SearchCondition condition = new SearchCondition();
            condition.IsOrRelateion = true;
            if (queryData != null)
            {
                List<Dictionary<string, string>> alKeyData = queryData["keydata"];
                foreach (Dictionary<string, string> ht in alKeyData)
                {
                    string queryField = ht["queryfield"];
                    string value = ht["value"];
                    condition.Add(queryField, Formula.QueryMethod.Like, value);
                }
            }
            return condition;
        }


        private static SearchCondition GetTabSearch(Dictionary<string, List<Dictionary<string, string>>> queryData, string userId)
        {
            SearchCondition condition = new SearchCondition();
            condition.IsOrRelateion = false;
            if (queryData != null)
            {
                List<Dictionary<string, string>> tabDatas = queryData["tabdata"];
                foreach (Dictionary<string, string> tab in tabDatas)
                {
                    string queryField = tab["queryfield"];
                    string enumKey = tab["enumkey"];
                    string[] values = tab["value"].Split(',');

                    if (values != null)
                    {
                        if (enumKey.ToLower() == "system.interval")
                        {
                            AddIntervalSearch(ref condition, queryField, values);
                        }
                        else if (enumKey.ToLower() == "system.ownerowndept")
                        {
                            AddUserSearch(ref condition, queryField, values, userId);
                        }
                        else
                        {
                            string strValue = string.Empty;
                            foreach (object obj in values)
                            {
                                strValue += Convert.ToString(obj) + ",";
                            }
                            condition.Add(queryField, Formula.QueryMethod.In, strValue.TrimEnd(','));
                        }
                    }
                }
            }
            return condition;
        }

        private static void AddIntervalSearch(ref SearchCondition condition, string queryField, string[] intervals)
        {
            foreach (string interval in intervals)
            {
                DateTime? t0 = null;
                DateTime? t1 = null;
                if (SystemEnumInterval.YearBefore.ToString() == interval)
                {
                    t1 = DateTime.Now.Date.AddDays(1).AddYears(-1);
                    condition.Add(queryField, Formula.QueryMethod.LessThan, (DateTime)t1);
                }
                else if (SystemEnumInterval.Year.ToString() == interval)
                {
                    t1 = DateTime.Now.Date.AddDays(1);
                    t0 = ((DateTime)t1).AddYears(-1);
                }
                else if (SystemEnumInterval.HalfYear.ToString() == interval)
                {
                    t1 = DateTime.Now.Date.AddDays(1);
                    t0 = ((DateTime)t1).AddMonths(-6);
                }
                else if (SystemEnumInterval.Month.ToString() == interval)
                {
                    t1 = DateTime.Now.Date.AddDays(1);
                    t0 = ((DateTime)t1).AddMonths(-1);
                }
                else if (SystemEnumInterval.Week.ToString() == interval)
                {
                    t1 = DateTime.Now.Date.AddDays(1);
                    t0 = ((DateTime)t1).AddDays(-7);
                }
                if (t0 != null && t1 != null)
                    condition.AddBetweenCondition(queryField, (DateTime)t0, (DateTime)t1);
                else
                {
                    if (t0 != null)
                        condition.Add(queryField, Formula.QueryMethod.GreaterThanOrEqual, (DateTime)t0);
                    else if (t1 != null)
                        condition.Add(queryField, Formula.QueryMethod.LessThan, (DateTime)t1);
                }
            }
        }

        private static void AddUserSearch(ref SearchCondition condition, string queryField, string[] users, string userId)
        {
            if (users.Contains(SystemEnumUser.OwnDept.ToString()))
            {
                string userOrgId = Formula.FormulaHelper.GetService<Formula.IUserService>().GetUserInfoByID(userId).UserOrgID;
                string userIds = Formula.FormulaHelper.GetService<Formula.IOrgService>().GetUserIDsInOrgs(userOrgId);
                condition.Add(queryField, Formula.QueryMethod.In, userIds);
            }
            else if (users.Contains(SystemEnumUser.Owner.ToString()))
            {
                condition.Add(queryField, Formula.QueryMethod.Equal, userId);
            }
        }

        /// <summary>
        ///DataTable2ArrayList
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static ArrayList DataTable2ArrayList(DataTable data)
        {
            ArrayList array = new ArrayList();
            for (int i = 0; i < data.Rows.Count; i++)
            {
                DataRow row = data.Rows[i];

                Hashtable record = new Hashtable();
                for (int j = 0; j < data.Columns.Count; j++)
                {
                    object cellValue = row[j];
                    if (cellValue.GetType() == typeof(DBNull))
                    {
                        cellValue = null;
                    }
                    record[data.Columns[j].ColumnName] = cellValue;
                }
                array.Add(record);
            }
            return array;
        }

        private static DataTable CopyToDataTable<T>(this IEnumerable<T> varlist)
        {
            DataTable dtReturn = new DataTable();
            // column names
            PropertyInfo[] oProps = null;
            // Could add a check to verify that there is an element 0
            foreach (T rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others will follow
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType; if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }
                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }
                DataRow dr = dtReturn.NewRow(); foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue(rec, null);
                }
                dtReturn.Rows.Add(dr);
            }
            return (dtReturn);
        }

        /// <summary>
        /// 周期
        /// </summary>
        public enum SystemEnumInterval
        {
            [Description("最近一周")]
            Week = 0,
            [Description("最近一月")]
            Month = 1,
            [Description("最近半年")]
            HalfYear = 2,
            [Description("最近一年")]
            Year = 3,
            [Description("一年以前")]
            YearBefore = 4
        }

        /// <summary>
        /// 本人本部门
        /// </summary>
        public enum SystemEnumUser
        {
            [Description("本人")]
            Owner,
            [Description("本部门")]
            OwnDept,
        }

        /// <summary>
        /// 从枚举类型和它的特性读出并返回一个键值对
        /// </summary>
        /// <param name="enumType">Type,该参数的格式为typeof(需要读的枚举类型)</param>
        /// <returns>键值对</returns>
        public static NameValueCollection GetNVCFromEnumValue(Type enumType)
        {
            NameValueCollection nvc = new NameValueCollection();
            Type typeDescription = typeof(DescriptionAttribute);
            System.Reflection.FieldInfo[] fields = enumType.GetFields();
            string strText = string.Empty;
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    object[] arr = field.GetCustomAttributes(typeDescription, true);
                    if (arr.Length > 0)
                    {
                        DescriptionAttribute aa = (DescriptionAttribute)arr[0];
                        strText = aa.Description;
                    }
                    else
                    {
                        strText = field.Name;
                    }
                    nvc.Add(strText, field.Name);
                }
            }
            return nvc;
        }

        private static Type GetEnumType(string fullName)
        {
            if (fullName.Contains('.'))
            {
                var asmName = fullName.Substring(0, fullName.LastIndexOf('.'));
                return Type.GetType(fullName + "," + asmName, false, true);
            }
            else
            {
                return null;
            }
        }
    }
}