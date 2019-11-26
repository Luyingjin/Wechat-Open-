using System;
using System.Collections.Generic;
using System.Text;
using Formula;
using System.Collections;

namespace System.Linq
{
    public class SearchCondition
    {
        public string Fields = "*";

        public SearchCondition()
        {
            Items = new List<ConditionItem>();
        }

        public SearchCondition Add(string field, QueryMethod method, object val, string orGroup = "Group1", string baseOrGroup = "BaseGroup1")
        {
            //处理日期型数据
            if (method == QueryMethod.LessThan || method == QueryMethod.LessThanOrEqual)
            {
                if (val.GetType() == typeof(DateTime))
                {
                    DateTime t = (DateTime)val;
                    val = t.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                }
            }

            ConditionItem item = new ConditionItem(field, method, val);
            item.OrGroup = orGroup;
            item.BaseOrGroup = baseOrGroup;
            Items.Add(item);
            return this;
        }

        public SearchCondition AddBetweenCondition(string field, object beginVal, object endVal, string orGroup = "Group1", string baseOrGroup = "BaseGroup1")
        {
            //处理日期型数据
            if (endVal.GetType() == typeof(DateTime))
            {
                DateTime t = (DateTime)endVal;
                endVal = t.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            ConditionItem item = new ConditionItem();
            item.Field = field;
            item.Method = QueryMethod.Between;
            item.Value = new[] { beginVal, endVal };
            item.OrGroup = orGroup;
            item.BaseOrGroup = baseOrGroup;
            Items.Add(item);

            return this;
        }


        /// <summary>
        /// 请不要直接使用Items的Add方法，设计失误，应该为私有属性，但项目上已经使用，不修改
        /// </summary>
        public List<ConditionItem> Items { get; set; }

        /// <summary>
        /// 查询条件间是否为Or关系
        /// </summary>
        public bool IsOrRelateion { get; set; }

        /// <summary>
        /// 获取where条件，如果没有条件则返回空字符串
        /// </summary>
        /// <param name="hasWhere"></param>
        /// <returns></returns>
        public string GetWhereString(bool hasWhere = true)
        {
            if (Items.Count == 0)
                return "";

            string strWhere = "";

            foreach (string baseGroup in Items.Select(c => c.BaseOrGroup).Distinct())
            {
                var _items = Items.Where(c => c.BaseOrGroup == baseGroup).ToList();

                string str1 = "";
                foreach (string group in _items.Select(c => c.OrGroup).Distinct())
                {
                    var arr = _items.Where(c => c.OrGroup == group).ToList();
                    str1 += string.Format(" and ({0})", GetGourpWhereString(arr));
                }

                strWhere += string.Format("  or ({0})", str1.Substring(4));
            }

            if (hasWhere)
                strWhere = " where " + strWhere.Substring(4);
            else
                strWhere = " and " + strWhere.Substring(4);

            return strWhere;
        }

        private string GetGourpWhereString(List<ConditionItem> list)
        {

            if (list.Count == 0)
                return "";

            string strWhere = "";
            for (int i = 0; i < list.Count(); i++)
            {
                var item = list[i];
                string str = GetWhereString(item);

                if (IsOrRelateion)
                {
                    strWhere += " or  " + str;
                }
                else
                {
                    strWhere += " and " + str;
                }
            }

            strWhere = strWhere.Substring(4);

            return strWhere;
        }

        #region 私有方法

        private string GetWhereString(ConditionItem item)
        {
            string value = "'" + item.Value + "'";

            if (Config.Constant.IsOracleDb)
            {
                DateTime oracleTime = DateTime.Now;
                if (DateTime.TryParse(item.Value.ToString(), out oracleTime))
                    value = string.Format("to_timestamp('{0}','YYYY-MM_DD HH24:MI:SS')", oracleTime.ToString("yyyy-MM-dd HH:mm:ss"));
            }


            string str = "";

            switch (item.Method)
            {
                case QueryMethod.Contains:
                case QueryMethod.StdIn:
                case QueryMethod.DateTimeLessThanOrEqual:
                    break;
                case QueryMethod.Equal:
                    str = string.Format("{0}={1}", item.Field, value);
                    break;
                case QueryMethod.LessThan:
                    str = string.Format("{0}<{1}", item.Field, value);
                    break;
                case QueryMethod.GreaterThan:
                    str = string.Format("{0}>{1}", item.Field, value);
                    break;
                case QueryMethod.LessThanOrEqual:
                    str = string.Format("{0}<={1}", item.Field, value);
                    break;
                case QueryMethod.GreaterThanOrEqual:
                    str = string.Format("{0}>={1}", item.Field, value);
                    break;
                case QueryMethod.Like:
                    str = string.Format("{0} like '%{1}%'", item.Field, item.Value);
                    break;
                case QueryMethod.In:
                    string strInValue = "";
                    if (item.Value is ICollection)
                    {
                        ICollection<string> collection = item.Value as ICollection<string>;
                        strInValue = string.Join("','", collection.ToArray<string>());
                    }
                    else
                    {
                        strInValue = item.Value.ToString().Replace(",", "','");
                    }
                    str = string.Format("{0} in ('{1}')", item.Field, strInValue);
                    break;
                case QueryMethod.InLike:
                    string[] arr = null;
                    if (item.Value is ICollection)
                    {
                        ICollection<string> collection = item.Value as ICollection<string>;
                        arr = collection.ToArray<string>();
                    }
                    else
                    {
                        arr = item.Value.ToString().Split(',', '，');
                    }
                    foreach (string s in arr)
                    {
                        str += string.Format("or {0} like '%{1}%'", item.Field, s);
                    }
                    str = "(" + str.Substring(2) + ")";
                    break;
                case QueryMethod.DateBlock:
                    DateTime dt = DateTime.Now;
                    if (!DateTime.TryParse(item.Value.ToString(), out dt))
                    {
                        throw new Exception("查询条件不能转化为日期时间");
                    }
                    string start = dt.Date.ToString("yyyy-MM-dd");
                    string end = dt.Date.AddDays(1).ToString("yyyy-MM-dd");
                    str = string.Format("{0} between '{1}' and '{2}'", item.Field, start, end);
                    if (Config.Constant.IsOracleDb)
                        str = string.Format("{0} between date'{1}' and date'{2}' ", item.Field, start, end);

                    break;
                case QueryMethod.NotEqual:
                    str = string.Format("{0}<>'{1}'", item.Field, item.Value);
                    break;
                case QueryMethod.StartsWith:
                    str = string.Format("{0} like '{1}%'", item.Field, item.Value);
                    break;
                case QueryMethod.EndsWith:
                    str = string.Format("{0} like '%{1}'", item.Field, item.Value);
                    break;
                case QueryMethod.Between:
                    object[] objs = item.Value as object[];
                    str = string.Format("{0} between '{1}' and '{2}'", item.Field, objs[0], objs[1]);
                    if (Config.Constant.IsOracleDb)
                    {
                        DateTime s = DateTime.Now;
                        DateTime e = DateTime.Now;
                        if (DateTime.TryParse(objs[0].ToString(), out s) && DateTime.TryParse(objs[1].ToString(), out e))
                        {
                            str = string.Format("{0} between to_timestamp('{1}','YYYY-MM-DD HH24-MI-SS') and to_timestamp('{2}','YYYY-MM-DD HH24-MI-SS')", item.Field
                                , s.ToString("yyyy-MM-dd hh:mm:ss")
                                , e.ToString("yyyy-MM-dd hh:mm:ss"));
                        }
                    }

                    break;

            }


            return str;
        }

        #endregion

    }
}
