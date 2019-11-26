using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;

namespace Config.Logic
{
    public class EnumService
    {
        public static DataRow GetEnumDefRow(string enumKey)
        {

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            DataTable dt = sqlHelper.ExecuteDataTable(string.Format("select * from S_M_EnumDef where Code='{0}'", enumKey));

            return dt.Rows[0];
        }

        public static DataTable GetEnumTable(string enumKey, string category = "", string subCategory = "")
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("id");
            dt.Columns.Add("value");
            dt.Columns.Add("text");
            if (enumKey.ToLower() == "connenum")
            {
                foreach (ConnectionStringSettings item in ConfigurationManager.ConnectionStrings)
                {
                    if (item.Name == "LocalSqlServer")
                        continue;
                    dt.Rows.Add(item.Name, item.Name, item.Name);
                }
                return dt;
            }
            else if (enumKey.ToLower() == "yearenum")
            {
                int c = 5;
                if (!string.IsNullOrEmpty(category))
                    c = int.Parse(category);
                for (int i = DateTime.Now.Year - 5; i <= DateTime.Now.Year + 5; i++)
                    dt.Rows.Add(i, i, i.ToString() + "年");
                return dt;
            }
            else if (enumKey.ToLower() == "quarterenum")
            {
                dt.Rows.Add("1", "1", "一季度");
                dt.Rows.Add("2", "2", "二季度");
                dt.Rows.Add("3", "3", "三季度");
                dt.Rows.Add("4", "4", "四季度");
                return dt;
            }
            else if (enumKey.ToLower() == "monthenum")
            {
                dt.Rows.Add("1", "1", "一月份");
                dt.Rows.Add("2", "2", "二月份");
                dt.Rows.Add("3", "3", "三月份");
                dt.Rows.Add("4", "4", "四月份");
                dt.Rows.Add("5", "5", "五月份");
                dt.Rows.Add("6", "6", "六月份");
                dt.Rows.Add("7", "7", "七月份");
                dt.Rows.Add("8", "8", "八月份");
                dt.Rows.Add("9", "9", "九月份");
                dt.Rows.Add("10", "10", "十月份");
                dt.Rows.Add("11", "11", "十一月");
                dt.Rows.Add("12", "12", "十二月");
                return dt;
            }

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            DataRow enumDef = GetEnumDefRow(enumKey);

            if (enumDef["Type"].ToString() == "Normal")
            {

                string sql = string.Format("select Code as id,Code as value,Name as text,Category,SubCategory from S_M_EnumItem where EnumDefID='{0}' {1} {2} order by SortIndex ", enumDef["ID"].ToString(), string.IsNullOrEmpty(category) ? "" : string.Format(" and Category='{0}'", category)
                    , string.IsNullOrEmpty(subCategory) ? "" : string.Format(" and SubCategory='{0}'", subCategory));
                dt = sqlHelper.ExecuteDataTable(sql);
            }
            else
            {
                sqlHelper = SQLHelper.CreateSqlHelper(enumDef["ConnName"].ToString());

                string sql = enumDef["Sql"].ToString();

                sql = string.Format("select * from ({0}) table1 where 1=1 {1} {2}"
                    , sql
                    , string.IsNullOrEmpty(category) ? "" : string.Format(" and Category='{0}'", category)
                    , string.IsNullOrEmpty(subCategory) ? "" : string.Format(" and SubCategory='{0}'", subCategory)
                    );

                sql = sql + " " + enumDef["Orderby"].ToString();

                dt = sqlHelper.ExecuteDataTable(sql);
            }


            return dt;
        }

        public static IList<DicItem> GetEnumDataSource(string enumKey, string category = "", string subCategory = "")
        {
            DataTable dt = GetEnumTable(enumKey, category, subCategory);
            IList<DicItem> list = new List<DicItem>();
            foreach (DataRow row in dt.Rows)
            {
                var dicItem = new DicItem
                {
                    Text = row["text"].ToString(),
                    Value = row["value"].ToString()
                };
                if (dt.Columns.Contains("category"))
                    dicItem.Category = row["category"].ToString();
                if (dt.Columns.Contains("subCategory"))
                    dicItem.SubCategory = row["subCategory"].ToString();

                list.Add(dicItem);
            }
            return list;
        }

        public static string GetEnumText(string enumKey, string value)
        {
            var selectedItem = GetEnumDataSource(enumKey).Where(item => item.Value == value).FirstOrDefault();
            if (selectedItem != null)
            {
                return selectedItem.Text;
            }
            else
            {
                return value;
            }
        }

        public static string GetEnumValue(string enumKey, string text)
        {
            var selectedItem = GetEnumDataSource(enumKey).Where(item => item.Text == text).FirstOrDefault();
            if (selectedItem != null)
            {
                return selectedItem.Value;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
