using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using System.Text.RegularExpressions;
using System.Web;

namespace Formula.Helper
{
    public class SerialNumberHelper
    {
        [Flags]
        public enum SerialNumberResetRule
        {
            Code = 1,
            YearCode = 2,
            MonthCode = 4,
            DayCode = 8,
            CategoryCode = 16,
            SubCategoryCode = 32,
            OrderNumCode = 64,
            PrjCode = 128,
            OrgCode = 256,
            UserCode = 512,
        }

        public static int GetSerialNumber(SerialNumberParam param, SerialNumberResetRule rule = SerialNumberResetRule.YearCode|SerialNumberResetRule.MonthCode, bool applySerialNumber = false)
        {

            #region 转换重复规则

            var mode = new Dictionary<string, string>();
            if (param == null)
                mode.Add("Code", "");
            else
                mode.Add("Code", param.Code);

            if (SerialNumberResetRule.YearCode == (rule | SerialNumberResetRule.YearCode))
                mode.Add("YearCode", DateTime.Now.Year.ToString());
            if (SerialNumberResetRule.MonthCode == (rule | SerialNumberResetRule.MonthCode))
                mode.Add("MonthCode", DateTime.Now.Month.ToString());
            if (SerialNumberResetRule.DayCode == (rule | SerialNumberResetRule.DayCode))
                mode.Add("DayCode", DateTime.Now.Day.ToString());
            if (SerialNumberResetRule.CategoryCode == (rule | SerialNumberResetRule.CategoryCode))
                mode.Add("CategoryCode", param.CategoryCode);
            if (SerialNumberResetRule.SubCategoryCode == (rule | SerialNumberResetRule.SubCategoryCode))
                mode.Add("SubCategoryCode", param.SubCategoryCode);
            if (SerialNumberResetRule.OrderNumCode == (rule | SerialNumberResetRule.OrderNumCode))
                mode.Add("OrderNumCode", param.OrderNumCode);
            if (SerialNumberResetRule.PrjCode == (rule | SerialNumberResetRule.PrjCode))
                mode.Add("PrjCode", param.PrjCode);
            if (SerialNumberResetRule.OrgCode == (rule | SerialNumberResetRule.OrgCode))
                mode.Add("OrgCode", param.OrgCode);
            if (SerialNumberResetRule.UserCode == (rule | SerialNumberResetRule.UserCode))
                mode.Add("UserCode", param.UserCode);

            #endregion

            int number = 0;

            #region 查询流水号
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = "select Number from S_UI_SerialNumber where 1=1";
            foreach (var item in mode)
                sql += string.Format(" and {0}='{1}'", item.Key, item.Value);
            object obj = sqlHelper.ExecuteScalar(sql);
            if (obj != null)
                number = Convert.ToInt32(obj);
            number++;
            #endregion

            #region 应用流水号
            if (applySerialNumber)
            {
                if (obj != null)
                {
                    sql = string.Format("update S_UI_SerialNumber set Number='{0}' where 1=1 ", number);
                    foreach (var item in mode)
                        sql += string.Format(" and {0}='{1}'", item.Key, item.Value);
                }
                else
                {
                    string fields = "";
                    string values = "";
                    foreach (var item in mode)
                    {
                        fields += "," + item.Key;
                        values += ",'" + item.Value + "'";
                    }

                    sql = string.Format("insert into S_UI_SerialNumber (ID,Number{2}) VALUES('{0}','{1}'{3})"
                        , FormulaHelper.CreateGuid()
                        , number
                        , fields
                        , values);
                }
                sqlHelper.ExecuteNonQuery(sql);
            }
            #endregion

            return number;
        }

        public static string GetSerialNumberString(string tmpl, SerialNumberParam param, string serialNumberResetRule, bool applySerialNumber = false)
        {
            SerialNumberResetRule rule = SerialNumberResetRule.Code;
            foreach (var item in serialNumberResetRule.Split(','))
            {
                if (item == "") continue;
                var e = (SerialNumberResetRule)Enum.Parse(typeof(SerialNumberResetRule), item);
                rule = rule | e;
            }

            return GetSerialNumberString(tmpl, param, rule, applySerialNumber);
        }

        public static string GetSerialNumberString(string tmpl, SerialNumberParam param, SerialNumberResetRule rule = SerialNumberResetRule.YearCode|SerialNumberResetRule.MonthCode, bool applySerialNumber = false)
        {
            int number = GetSerialNumber(param, rule, applySerialNumber);

            Regex reg = new Regex("\\{[0-9a-zA-Z_\u4e00-\u9faf]*\\}");
            string result = reg.Replace(tmpl, (Match m) =>
            {
                string value = m.Value.Trim('{', '}');

                if (value.Replace('N', ' ').Trim() == "") //顺序号
                    return number.ToString("D" + value.Length);

                switch (value)
                {
                    case "yyyy":
                    case "YYYY":
                        return DateTime.Now.ToString("yyyy");
                    case "yy":
                    case "YY":
                        return DateTime.Now.ToString("yy");
                    case "mm":
                    case "MM":
                        return DateTime.Now.ToString("MM");
                    case "dd":
                    case "DD":
                        return DateTime.Now.ToString("dd");
                    case "PrjCode":
                        return param.PrjCode;
                    case "OrgCode":
                        return param.OrgCode;
                    case "UserCode":
                        return param.UserCode;
                    case "CategoryCode":
                        return param.CategoryCode;
                    case "SubCategoryCode":
                        return param.SubCategoryCode;
                    case "OrderNumCode":
                        return param.OrderNumCode;
                }

                if (!string.IsNullOrEmpty(HttpContext.Current.Request[value]))
                    return HttpContext.Current.Request[value];
                return m.Value;
            });


            return result;
        }
    }

    public class SerialNumberParam
    {
        public string Code { get; set; }
        public string PrjCode { get; set; }
        public string OrgCode { get; set; }
        public string UserCode { get; set; }
        public string CategoryCode { get; set; }
        public string SubCategoryCode { get; set; }
        public string OrderNumCode { get; set; }
    }
}
