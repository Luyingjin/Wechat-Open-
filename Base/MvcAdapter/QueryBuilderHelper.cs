using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Formula;
using Formula.Helper;

namespace MvcAdapter
{
    public class QueryBuilderHelper
    {
        public static QueryBuilder BindModel(QueryBuilder qb, System.Collections.Specialized.NameValueCollection dict)
        {
            Dictionary<string, string> queryDic = new Dictionary<string, string>();
            string queryFormData = dict["queryFormData"];
            if (!string.IsNullOrEmpty(queryFormData))
                queryDic = JsonHelper.ToObject<Dictionary<string, string>>(queryFormData);

            foreach (var key in queryDic.Keys)
            {
                if (!key.StartsWith("$")) continue;
                var val = queryDic[key];
                //处理无值的情况
                if (string.IsNullOrEmpty(val)) continue;
                AddSearchItem(qb, key, val);
            }


            int pageIndex = 0;
            int pageSize = 20;
            string sortField = string.IsNullOrEmpty(dict["sortField"]) ? "ID" : dict["sortField"];
            string sortOrder = string.IsNullOrEmpty(dict["sortOrder"]) ? "desc" : dict["sortOrder"];


            int.TryParse(dict["pageIndex"], out pageIndex);
            if (!string.IsNullOrEmpty(dict["pageSize"]))
                int.TryParse(dict["pageSize"], out pageSize);

            qb.PageIndex = pageIndex;
            qb.PageSize = pageSize;
            qb.SortField = sortField;
            qb.SortOrder = sortOrder;
            qb.DefaultSort = string.IsNullOrEmpty(dict["sortField"]);

            if (queryDic.ContainsKey("IsOrRelation") && queryDic["IsOrRelation"] == "True")
                qb.IsOrRelateion = true;
            else
                qb.IsOrRelateion = false;


            return qb;
        }

        /// <summary>
        /// 将一组key=value添加入QueryModel.Items
        /// </summary>
        /// <param name="qb">QueryModel</param>
        /// <param name="key">当前项的HtmlName</param>
        /// <param name="val">当前项的值</param>
        private static void AddSearchItem(QueryBuilder qb, string key, string val)
        {
            val = val.Replace("'", "");//去掉查询条件的单引号

            string field = "", orGroup = "", method = "";
            var keywords = key.Split("$', ')', '}".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            method = SearchMethodAdapter(keywords[0]);
            field = keywords[1];

            if (method == "LessThan" || method == "LessThanOrEqual")
            {
                DateTime time;
                float f;
                if (DateTime.TryParse(val, out time) && float.TryParse(val, out f) == false)  //例如3.1会被认为是3月1号
                {
                    val = time.Date.AddHours(23).AddMinutes(59).AddSeconds(59).ToString();
                }
            }

            if (string.IsNullOrEmpty(method)) return;
            if (!string.IsNullOrEmpty(field))
            {
                var item = new ConditionItem
                {
                    Field = field,
                    Value = val.Trim(),
                    OrGroup = orGroup,
                    Method = (QueryMethod)Enum.Parse(typeof(QueryMethod), method)
                };
                qb.Items.Add(item);
            }
        }
        /// <summary>
        /// 查询匹配模式适配器
        /// </summary>
        /// <param name="method">简化匹配模式</param>
        /// <returns>全称匹配模式</returns>
        private static string SearchMethodAdapter(string method)
        {
            string match = "";
            switch (method.ToUpper())
            {
                case "EQ"://等于
                    match = "Equal";
                    break;
                case "UE"://不等于
                    match = "NotEqual";
                    break;
                case "GT"://大于
                    match = "GreaterThan";
                    break;
                case "LT"://小于
                    match = "LessThan";
                    break;
                case "IN"://in
                    match = "In";
                    break;
                case "FR"://大于等于
                    match = "GreaterThanOrEqual";
                    break;
                case "TO"://小于等于
                    match = "LessThanOrEqual";
                    break;
                case "LK"://like
                    match = "Like";
                    break;
                case "IL":
                    match = "InLike";
                    break;
                case "SW"://以....开始
                    match = "StartsWith";
                    break;
                case "EW"://以....结束
                    match = "EndsWith";
                    break;
                case "IGNORE":
                    match = "";
                    break;
            }
            return match;
        }
    }
}
