using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula;
using Formula.Exceptions;
using Formula.Helper;
using Config;
using System.Data.Entity;
using Base.Logic.Domain;

namespace Base.Logic
{
    /// <summary>
    /// 自动生成编号
    /// </summary>
    public class GenerateCode
    {
        /// <summary>
        /// 根据规则Key生成自动编号，不保存最大编号
        /// </summary>
        /// <param name="KeyCode"></param>
        /// <returns></returns>
        public string GetCodeNotSave(string KeyCode)
        {
            return GetCode(KeyCode,false);
        }
        /// <summary>
        /// 根据规则Key生成自动编号，并保存新的最大编号
        /// </summary>
        /// <param name="KeyCode"></param>
        /// <returns></returns>
        public string GetCodeSave(string KeyCode)
        {
            return GetCode(KeyCode, true);
        }

        private string GetCode(string KeyCode,bool isSave=true)
        {
            string strCode = "";
            DbContext entities = FormulaHelper.GetEntities<BaseEntities>();
            var rule = entities.Set<S_RC_RuleCode>().FirstOrDefault(t => t.Code == KeyCode);
            if (rule != null)
            {
                var maxData = entities.Set<S_RC_RuleCodeData>().Where(t => t.Code == KeyCode).FirstOrDefault();
                string last = "";
                long newNumber=0;
                //前缀+分隔符+年月+分隔符
                string first = rule.Prefix + rule.Seperative + DateTime.Now.Year.ToString() + rule.Seperative;
                if (maxData == null || maxData.AutoNumber == null)
                {
                    //如果规则数据表中没有最大的数据，则重新生成，尾数不足的补0
                    for (int i = 1; i <= rule.Digit - 1; i++)
                    {
                        last += "0";
                    }
                    newNumber = Convert.ToInt64(rule.StartNumber);
                    if (isSave)
                    {
                        S_RC_RuleCodeData data = new S_RC_RuleCodeData();
                        data.ID = FormulaHelper.CreateGuid();
                        data.Code = KeyCode;
                        data.Year = DateTime.Now.Year;
                        data.AutoNumber = newNumber;
                        entities.Set<S_RC_RuleCodeData>().Add(data);
                    }
                }
                else
                {
                    newNumber= Convert.ToInt64(maxData.AutoNumber) + 1;
                    if (newNumber.ToString().Length < rule.Digit)
                    {
                        for (int i = 0; i < rule.Digit - newNumber.ToString().Length; i++)
                        {
                            last += "0";
                        }
                    }
                }
                last += newNumber.ToString();
                //存在后缀，则加上后缀，不存在就不加
                if (!string.IsNullOrWhiteSpace(rule.PostFix))
                {
                    last += rule.Seperative + rule.PostFix;
                }
                strCode = first + last;
                if (isSave)
                {
                    entities.Set<S_RC_RuleCodeData>().Where(t => t.Code == KeyCode).Update(t => t.AutoNumber = newNumber);
                    entities.SaveChanges();
                }
            }
            return strCode;
        }


        private string GetCodeNew(string KeyCode, bool isSave = true)
        {
            string strSql = "";

            return "";
        }
    }

    public class GenerateFormCode
    {
        /// <summary>
        /// 生成编号
        /// </summary>
        /// <param name="sqlhelper">连接字符串</param>
        /// <param name="tableName">表名</param>
        /// <param name="column">列名</param>
        /// <param name="serialNumber">流水号位数</param>
        /// <param name="s">分隔符</param>
        /// <param name="prefix">前缀</param>
        /// <param name="suffix">后缀</param>
        /// <param name="condition">查询补充条件</param>
        /// <returns></returns>
        public static string CreateCode(SQLHelper sqlhelper, string tableName, string column, int serialNumber, string s = "", string prefix = "", string suffix = "", string condition = "")
        {
            string sql = "select Max(" + column + ") From " + tableName + " where " + column + " like '" + prefix + "%' " + condition;
            string maxNo = sqlhelper.ExecuteScalar(sql).ToString();
            string temp;
            string maxNumber;
            string number = "";
            for (int i = 0; i < serialNumber; i++)
            {
                number += "0";
            }
            if (String.IsNullOrWhiteSpace(maxNo))
            {
                if (prefix != "" && suffix == "")
                {
                    temp = prefix + s;
                }
                else if (suffix != "" && prefix == "")
                {
                    temp = suffix + s;
                }
                else if (suffix != "" && prefix != "")
                {
                    temp = prefix + s + suffix + s;
                }
                else
                {
                    temp = "";
                }

                maxNumber = number;
            }
            else
            {
                temp = maxNo.Substring(0, maxNo.Length - serialNumber);
                maxNumber = maxNo.Substring(temp.Length, serialNumber);
            }

            string currentNo = (Convert.ToInt64(maxNumber) + 1).ToString(number);

            return temp + currentNo;
        }

        /// <summary>
        /// 生成编号
        /// </summary>
        /// <param name="sqlhelper">连接字符串</param>
        /// <param name="tableName">表名</param>
        /// <param name="column">列名</param>
        /// <param name="serialNumber">流水号位数</param>
        /// <param name="c">分隔符</param>
        /// <param name="noPrefix">前缀</param>
        /// <param name="noSuffix">后缀</param>
        /// <param name="querycolumn">查询字段</param>
        /// <param name="queryconditions">查询条件</param>
        /// <returns></returns>
        public static string GetFormNumber(SQLHelper sqlhelper, string tableName, string column, int serialNumber, string querycolumn = "", string queryconditions = "", string c = "", string noPrefix = "", string noSuffix = "")
        {
            string sql = "";
            if (querycolumn != "")
            {
                sql = "select Max(" + column + ") From " + tableName + " where " + querycolumn + " like '" + queryconditions + "%'";
            }
            else
            {
                sql = "select Max(" + column + ") From " + tableName + "";
            }
            string maxNo = sqlhelper.ExecuteScalar(sql).ToString();
            string currentNo;

            string number = "";
            for (int i = 0; i < serialNumber; i++)
            {
                number += "0";
            }
            if (string.IsNullOrEmpty(maxNo))
            {
                if (noPrefix != "" && noSuffix == "")
                {
                    currentNo = noPrefix + c + number;
                }
                else if (noSuffix != "" && noPrefix == "")
                {
                    currentNo = noSuffix + c + number;
                }
                else if (noSuffix != "" && noPrefix != "")
                {
                    currentNo = noPrefix + c + noSuffix + c + number;
                }
                else
                {
                    currentNo = number;
                }
            }
            else
            {
                currentNo = maxNo;
            }
            if (c != "")
            {
                string[] strs = currentNo.Split(Convert.ToChar(c));

                if (noPrefix != "" && noSuffix == "")
                {
                    currentNo = noPrefix + c + (Convert.ToInt32(strs[strs.Length - 1]) + 1).ToString(number);
                }
                else if (noSuffix != "" && noPrefix == "")
                {
                    currentNo = noSuffix + c + (Convert.ToInt32(strs[strs.Length - 1]) + 1).ToString(number);
                }
                else if (noSuffix != "" && noPrefix != "")
                {
                    currentNo = noPrefix + c + noSuffix + c + (Convert.ToInt32(strs[strs.Length - 1]) + 1).ToString(number);
                }
                else
                {
                    currentNo = (Convert.ToInt32(strs[strs.Length - 1]) + 1).ToString(number);
                }

            }
            else
            {
                currentNo = (Convert.ToInt64(currentNo) + 1).ToString(number);
            }


            return currentNo;
        }

        /// <summary>
        /// 生成编号
        /// </summary>
        /// <param name="sqlhelper">连接字符串</param>
        /// <param name="tableName">表名</param>
        /// <param name="column">列名</param>
        /// <param name="c">分隔符</param>
        /// <param name="noPrefix">前缀</param>
        /// <param name="noSuffix">后缀</param>
        /// <param name="serialNumber">流水号位数</param>
        /// <param name="querycolumn">查询字段</param>
        /// <param name="queryconditions">查询条件</param>
        /// <param name="preFix"></param>
        /// <returns></returns>
        public static string GetFormNumberNew(SQLHelper sqlhelper, string tableName, string column, int serialNumber, string querycolumn = "", string queryconditions = "", string c = "", string noPrefix = "", string noSuffix = "", string preFix = "")
        {
            string sql = "";
            if (querycolumn != "")
            {
                if (preFix != "")
                {
                    sql = "select Max(" + column + ") From " + tableName + " where " + querycolumn + " like '%" + queryconditions + "%'";
                }
                else
                {
                    sql = "select Max(" + column + ") From " + tableName + " where " + querycolumn + " like '" + queryconditions + "%'";
                }
            }
            else
            {
                sql = "select Max(" + column + ") From " + tableName + "";
            }
            string MaxNo = sqlhelper.ExecuteScalar(sql).ToString();
            string CurrentNo = "";

            string Number = "";
            for (int i = 0; i < serialNumber; i++)
            {
                Number += "0";
            }
            if (MaxNo == "" || MaxNo == null)
            {
                if (noPrefix != "" && noSuffix == "")
                {
                    CurrentNo = noPrefix + c + Number;
                }
                else if (noSuffix != "" && noPrefix == "")
                {
                    CurrentNo = noSuffix + c + Number;
                }
                else if (noSuffix != "" && noPrefix != "")
                {
                    CurrentNo = noPrefix + c + noSuffix + c + Number;
                }
                else
                {
                    CurrentNo = Number;
                }
            }
            else
            {
                CurrentNo = MaxNo;
            }
            if (c != "")
            {
                if (preFix != "")
                {
                    CurrentNo = CurrentNo.Replace(preFix, "");
                }
                string[] strs = CurrentNo.Split(Convert.ToChar(c));

                if (noPrefix != "" && noSuffix == "")
                {
                    CurrentNo = noPrefix + c + (Convert.ToInt64(strs[strs.Length - 1]) + 1).ToString(Number);
                }
                else if (noSuffix != "" && noPrefix == "")
                {
                    CurrentNo = noSuffix + c + (Convert.ToInt64(strs[strs.Length - 1]) + 1).ToString(Number);
                }
                else if (noSuffix != "" && noPrefix != "")
                {
                    CurrentNo = noPrefix + c + noSuffix + c + (Convert.ToInt64(strs[strs.Length - 1]) + 1).ToString(Number);
                }
                else
                {
                    CurrentNo = (Convert.ToInt64(strs[strs.Length - 1]) + 1).ToString(Number);
                }

            }
            else if (preFix != "")
            {
                CurrentNo = CurrentNo.Replace(preFix, "");
                CurrentNo = (Convert.ToInt64(CurrentNo) + 1).ToString(Number);
            }
            else
            {
                CurrentNo = (Convert.ToInt64(CurrentNo) + 1).ToString(Number);
            }


            return preFix + CurrentNo;
        }
    }
}
