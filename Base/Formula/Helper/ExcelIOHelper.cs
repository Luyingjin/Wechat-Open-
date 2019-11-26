using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Web;
using System.Xml.Linq;
using System.Reflection;


namespace Formula.Helper
{
    public class ExcelDataSet
    {
        public DataTable ExcelTable { get; set; }
        public Dictionary<string, string> ExcelItemDic { get; set; }
    }

    public class CheckResultItem
    {
        public int Col { get; set; }
        public int Row { get; set; }
        public string Msg { get; set; }
    }

    /// <summary>
    /// 帮助扩展类
    /// </summary>
    public static class HelperExtensions
    {
        /// <summary>
        /// 添加错误信息
        /// </summary>
        /// <param name="errorList"></param>
        /// <param name="msg">错误信息</param>
        /// <returns></returns>
        public static List<CheckResultItem> AddMsg(this List<CheckResultItem> errorList, string msg)
        {
            if (errorList == null)
                throw new ArgumentNullException("items");
            if (string.IsNullOrEmpty(msg))
                throw new ArgumentNullException("msg");

            if (errorList.Where(item => item.Msg == msg).Count() == 0)
                errorList.Add(new CheckResultItem { Msg = msg });

            return errorList;
        }
    }

    //public interface ICheckExcel
    //{
    //    CheckResult CheckData(DataTable dt);
    //}

    //public interface IExcelIntoDB
    //{
    //    bool SaveData(DataTable dt);
    //}

    /// <summary>
    /// 数据用途枚举
    /// </summary>
    public enum DataUsage
    {
        /// <summary>
        /// 用于显示
        /// </summary>
        display,
        /// <summary>
        /// 用于导入
        /// </summary>
        import,
    }


    public class ExcelIOHelper
    {
        #region 私有属性

        string tmplRootPath = System.Configuration.ConfigurationManager.AppSettings["ServerMapPath"] + "/ExcelImportTmpl/";
        string excelRootPath = System.Configuration.ConfigurationManager.AppSettings["TemparatoryDirectory"] + "/";

        XElement xmlConfig;
        DataTable dtExcel;
        int intStartRowIndex=0;
        int intEndRowIndex=0;

        private ExcelDataSet _excelDataSet = null;
        public ExcelDataSet excelDataSet
        {
            get
            {
                if (_excelDataSet == null)
                {
                    _excelDataSet = new ExcelDataSet();
                    _excelDataSet.ExcelTable = GetExcelDataTable(DataUsage.import);
                    _excelDataSet.ExcelItemDic = GetExcelDataItemDic(DataUsage.import);
                }
                return _excelDataSet;
            }
        }

        #endregion

        #region 公共属性

        private IList<CheckResultItem> _listTmplCheck = new List<CheckResultItem>();
        /// <summary>
        /// 模板检测是否通过
        /// </summary>
        public IList<CheckResultItem> ListTmplCheck
        {
            get { return _listTmplCheck; }
        }

        #endregion

        #region 构造方法

        public ExcelIOHelper(string tmplCode, string excelFileName)
        {
            XElement xEle = XElement.Load(System.Configuration.ConfigurationManager.AppSettings["ServerMapPath"] + "/ExcelTmplConfig.xml");
            xmlConfig = (from item in xEle.Elements("ExcelTmpl")
                         where item.Attribute("Code").Value == tmplCode
                         select item).First();


            string excelFilePath = excelRootPath + excelFileName;
            string sheetName = xmlConfig.Attribute("SheetName").Value;
            try
            {
                string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + excelFilePath + "';User ID=Admin;Mode=Share Deny None;Extended Properties='Excel 12.0 xml;HDR=No;IMEX=1'";
                // 林飞 2010-10-26 修改连接Excel的字符串，根据判断不同后缀名来使用不同的连接字符串
                if (excelFilePath.ToLower().EndsWith("xls"))
                {
                    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + excelFilePath + "';Extended Properties='Excel 8.0; HDR=yes; IMEX=1;'";
                }

                OleDbConnection OleConn = new OleDbConnection(strConn);
                OleConn.Open();
                DataTable table = OleConn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, null);

                #region 判断Sheet页是否存在
                bool existSheet = false;
                foreach (DataRow row in table.Rows)
                {
                    string tableName = row["Table_Name"].ToString().Trim('$');
                    if (tableName == sheetName)
                    {
                        existSheet = true;
                        break;
                    }
                }
                if (existSheet == false)
                {
                    CheckResultItem item = new CheckResultItem();
                    item.Msg = "sheet名称错误，可能是模板不对。";
                    _listTmplCheck.Add(item);
                    dtExcel = new DataTable();
                    return;
                }
                #endregion



                String sql = string.Format("SELECT * FROM  [{0}$]", sheetName);
                //可是更改Sheet名称，比如sheet2，等等    
                OleDbDataAdapter OleDaExcel = new OleDbDataAdapter(sql, OleConn);
                DataSet OleDsExcle = new DataSet();
                OleDaExcel.Fill(OleDsExcle, "aa");
                OleConn.Close();
                dtExcel = OleDsExcle.Tables[0];
                string firstRow= xmlConfig.Element("Columns").FirstAttribute.Value;
                // 计算开始行
                int firstColumnIndex = GetCol(xmlConfig.Element("Columns").Elements("Column").First().Attribute("Col").Value);
                string firstColumnName = xmlConfig.Element("Columns").Elements("Column").First().Attribute("Name").Value;
                if (firstRow != "1")//处理两行标题的情况（为往Excel中写数据，第一行为标题行，第二个标题行下边为数据行。）
                {
                    for (int j = 1; j < dtExcel.Rows.Count; j++)
                    {
                        // 如果该行的第一个Column的文本与XML配置中的列名一致就为标题行。
                        if (dtExcel.Rows[j][firstColumnIndex].ToString() == firstColumnName)
                        {
                            intStartRowIndex =j+ 1;
                            break;
                        }
                    }
                }
                else
                {
                    intStartRowIndex =  1;
                }
                // 计算结束行
                string endRowFlag = xmlConfig.Attribute("EndRowFlag").Value;
                bool flag = false;
                for (int i = dtExcel.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dataRow = dtExcel.Rows[i];
                    for (int j = 0; j < dtExcel.Columns.Count; j++)
                    {
                        if (dataRow[j].ToString().Trim().StartsWith(endRowFlag.Trim()))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        intEndRowIndex = i;
                        break;
                    }
                }

                if (intEndRowIndex == 0)
                {
                    CheckResultItem item = new CheckResultItem();
                    item.Msg = "Excel表中没有“注：结束 ”标记";
                    _listTmplCheck.Add(item);
                }
                //检测模板
                CheckTmpl();

            }
            catch (Exception err)
            {
                throw err;
            }
        }

        #endregion

        #region 公共方法

        #region GetExcelTmplUrl

        public string GetExcelTmplUrl()
        {
            string url = HttpContext.Current.Request.Url.Scheme + "://"
                + HttpContext.Current.Request.Url.Host + ":"
                + HttpContext.Current.Request.Url.Port + "/ExcelImportTmpl/" + xmlConfig.Attribute("Name").Value;
            return url;
        }

        #endregion

        #region GetExcelDataTable

        public DataTable GetExcelDataTable(DataUsage usage)
        {
            DataTable dt = dtExcel.Copy();

            FixDataTable(dt, usage);

            return dt;
        }

        #endregion

        #region GetExcelDataItemDic

        public Dictionary<string, string> GetExcelDataItemDic(DataUsage usage)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            var quary = xmlConfig.Element("Items").Elements("Item");
            foreach (var item in quary)
            {
                string code = item.Attribute("Code").Value;
                string name = item.Attribute("Name").Value;
                string row = item.Attribute("Row").Value;
                string col = item.Attribute("Col").Value;
                string type = item.Attribute("Type").Value;
                string value = "";

                //如果要求的条目在列表的上方则直接取值，否则要求表尾行+偏移行
                if (type.ToLower().Trim() == "up")
                    value = GetExcelData(row, col);
                else
                {
                    int aboseluteRow = intEndRowIndex + int.Parse(row);
                    int aboseluteCol = GetCol(col);
                    value = GetExcelData(aboseluteRow, aboseluteCol);
                }


                if (usage == DataUsage.display)
                    dic.Add(name, value);
                else
                    dic.Add(code, value);
            }

            return dic;
        }

        #endregion

        #region CheckData

        /// <summary>
        /// 验证Excel表格的数据
        /// </summary>
        /// <returns>错误信息列表</returns>
        public IList<CheckResultItem> CheckData()
        {
            IList<CheckResultItem> list = new List<CheckResultItem>();

            #region 验证非空字段

            // 收集所有需要验证非空的列名
            var columns = new Dictionary<string, string>();
            var query = xmlConfig.Element("Columns").Elements("Column");
            foreach (var item in query)
            {
                if (item.Attribute("NotAllowEmpty") != null
                    && item.Attribute("NotAllowEmpty").Value.ToLower() == "true")
                {
                    string code = item.Attribute("Code").Value;
                    string name = item.Attribute("Name").Value;
                    string col = item.Attribute("Col").Value;

                    columns.Add(code, name.Trim('＊') + "不能为空（<b>单元格" + col + "{0}</b>）");
                }
            }

            // 循环验证非空字段
            for (int i = 0; i < excelDataSet.ExcelTable.Rows.Count; i++)
            {
                DataRow row = excelDataSet.ExcelTable.Rows[i];
                foreach (var column in columns)
                {
                    if (string.IsNullOrEmpty(row[column.Key].ToString().Trim()))
                    {
                        list.Add(new CheckResultItem
                        {
                            Msg = string.Format(column.Value, i + intStartRowIndex + 1)
                        });
                    }
                }
            }
            #endregion

            #region 调用自定义的验证逻辑来验证数据的正确性
            string validateMethodName = xmlConfig.Element("ValidateMethod").Attribute("MethodName").Value;
            string assemblyName = xmlConfig.Element("ValidateMethod").Attribute("AssemblyName").Value;
            string className = xmlConfig.Element("ValidateMethod").Attribute("ClassName").Value;

            //调用数据检测方法
            object obj = CreateObjectInstance(className, assemblyName);
            if (obj == null)
                throw new Exception(string.Format("无法加载程序集{0},或者程序集中没有{1}类！", assemblyName, className));
            MethodInfo validateMethod = obj.GetType().GetMethod(validateMethodName);
            if (validateMethodName == null)
                throw new Exception(string.Format("在类{0}中没有找到{1}方法", className, validateMethodName));

            object validateResutl = validateMethod.Invoke(obj, new object[] { excelDataSet });
            #endregion

            var result = validateResutl as IList<CheckResultItem>;
            if (result != null)
            {
                foreach (var item in result)
                {
                    list.Add(item);
                }
            }
            else
            {
                throw new Exception(string.Format("类{0}的{1}方法返回值非IList<CheckResultItem>类型", className, validateMethodName));
            }

            return list;
        }

        #endregion

        #region SaveData

        public bool SaveData()
        {
            string saveMethodName = xmlConfig.Element("SaveMethod").Attribute("MethodName").Value;
            string assemblyName = xmlConfig.Element("SaveMethod").Attribute("AssemblyName").Value;
            string className = xmlConfig.Element("SaveMethod").Attribute("ClassName").Value;


            //调用数据检测方法
            object obj = CreateObjectInstance(className, assemblyName);
            if (obj == null)
                throw new Exception(string.Format("无法加载程序集{0},或者程序集中没有{1}类！", assemblyName, className));
            MethodInfo saveMethod = obj.GetType().GetMethod(saveMethodName);
            if (saveMethod == null)
                throw new Exception(string.Format("在类{0}中没有找到{1}方法", className, saveMethodName));

            object result = saveMethod.Invoke(obj, new object[] { excelDataSet });

            return Convert.ToBoolean(result);
        }

        #endregion

        #endregion

        #region 静态方法

        public static string GetExcelTmplUrl(string tmplCode)
        {
            XElement xEle = XElement.Load(HttpContext.Current.Server.MapPath("/") + "/ExcelTmplConfig.xml");

            var query = from item in xEle.Elements("ExcelTmpl")
                        where item.Attribute("Code").Value == tmplCode
                        select item;

            string url = HttpContext.Current.Request.Url.Scheme + "://"
                + HttpContext.Current.Request.Url.Host + ":"
                + HttpContext.Current.Request.Url.Port + "/ExcelImportTmpl/" + query.First().Attribute("Name").Value;

            return url;
        }

        #endregion

        #region 私有方法

        #region FixDataTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="attributeName">配置文件的属性名。Code或Name</param>
        private void FixDataTable(DataTable dt, DataUsage usage)
        {
            int firstColumnIndex = GetCol(xmlConfig.Element("Columns").Elements("Column").First().Attribute("Col").Value);
            string firstColumnName = xmlConfig.Element("Columns").Elements("Column").First().Attribute("Name").Value;

            //移除结束标志以后的行
            while (dt.Rows.Count - 1 >= intEndRowIndex)
            {
                dt.Rows.RemoveAt(dt.Rows.Count - 1);
            }

            int j = intStartRowIndex;
            //移除标题行以前的行
            while (j > 0)
            {
                //if (dt.Rows[intStartRowIndex-1][firstColumnIndex].ToString() == firstColumnName)
                //{
                //    dt.Rows.RemoveAt(0);
                //    break;
                //}
                dt.Rows.RemoveAt(0);
                j--;
            }


            //移除多余的列,并设置列名
            for (int i = dt.Columns.Count - 1; i >= 0; i--)
            {
                var query = xmlConfig.Element("Columns").Elements("Column").Where(c => GetCol(c.Attribute("Col").Value) == i);
                if (query.Count() == 0)
                {
                    dt.Columns.RemoveAt(i);
                }
                else
                {
                    if (usage == DataUsage.display)
                        dt.Columns[i].ColumnName = query.Single().Attribute("Name").Value;
                    else
                        dt.Columns[i].ColumnName = query.Single().Attribute("Code").Value;
                }
            }

        }

        #endregion

        #region CheckTmpl

        private void CheckTmpl()
        {

            //验证Columns
            string row = xmlConfig.Element("Columns").Attribute("Row").Value;
            var query = xmlConfig.Element("Columns").Elements("Column");
            foreach (var item in query)
            {

                string name = item.Attribute("Name").Value;
                string col = item.Attribute("Col").Value;

                string colName = "";
                while (colName == "")
                {
                    colName = GetExcelData(row, col);
                    if (colName == "Error")
                    {
                        CheckResultItem checkItem = new CheckResultItem();
                        checkItem.Msg = "配置文件的列数与模板文件不一致，请检查！";
                        _listTmplCheck.Add(checkItem);
                        return;
                    }
                    int x = int.Parse(row);
                    if (x <= 1)
                        break;
                    x--;
                    row = x.ToString();
                }

                row = xmlConfig.Element("Columns").Attribute("Row").Value;
                if (name != colName)
                {
                    // return false;
                    CheckResultItem checkItem = new CheckResultItem();
                    //checkItem.Col = int.Parse(col);
                    //checkItem.Row = int.Parse(row);
                    checkItem.Msg = string.Format("{0}{1}为：{2},配置文件为：{3}", row, col, colName, name);
                    _listTmplCheck.Add(checkItem);
                }

            }


            //验证Items            
            //query = xmlConfig.Element("Items").Elements("Item");
            //foreach (var item in query)
            //{
            //    row = item.Attribute("Row").Value;
            //    string col = item.Attribute("Col").Value;
            //    GetExcelData(row, col);
            //}

            //return true;
        }

        #endregion

        #region GetExcelData

        /// <summary>
        ///  说明，row col为excel中的行列
        /// </summary>
        /// <param name="strRow"></param>
        /// <param name="strCol"></param>
        /// <returns></returns>
        private string GetExcelData(string strRow, string strCol)
        {
            //Excel Row Col 转化
            int row = GetRow(strRow);
            int col = GetCol(strCol);

            return GetExcelData(row, col);
        }

        /// <summary>
        /// 说明：row col为DataTable中的行列
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private string GetExcelData(int row, int col)
        {
            if (row >= dtExcel.Rows.Count || col >= dtExcel.Columns.Count)
            {
                return "Error";
            }
            //throw new Exception(string.Format("配置文件设置的行列超出excel文件的行数或列数，row:{0},col{1}", row, col));

            return dtExcel.Rows[row][col].ToString();
        }

        #endregion

        #region Excel行列转换为程序行列

        /// <summary>
        /// Excel行转化为程序行
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private int GetRow(string row)
        {
            return int.Parse(row) - 1;
        }

        /// <summary>
        /// Excel列转化为程序列
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        private int GetCol(string col)
        {
            col = col.Trim();

            int colNumber = 0;
            for (int i = 0; i < col.Length; i++)
            {
                colNumber += Convert.ToInt32(Math.Pow(26, col.Length - i - 1)) * GetCol(col[i]);
            }


            return colNumber - 1;
        }

        private int GetCol(char c)
        {
            return c - 'A' + 1;
        }
        #endregion



        #endregion

        #region CreateObjectInstance

        public object CreateObjectInstance(string typeName, string assemblyName, params object[] avgs)
        {
            Assembly assembly = Assembly.Load(assemblyName);
            Type type = assembly.GetType(typeName, true, false);


            if (avgs != null)
            {
                return Activator.CreateInstance(type, avgs);
            }
            else
            {
                return Activator.CreateInstance(type);
            }
        }

        #endregion

    }
}
