using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Config;
using System.Xml;
using Formula;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace MvcConfig
{
    public class RdlcHelper
    {
        public RdlcHelper(string reportCode, HtmlTable table)
        {
            this.reportCode = reportCode;
            this.table = table;
        }

        private string reportCode;
        private HtmlTable table;
        SQLHelper baseSQLHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);

        #region TransferEnum

        //翻译DataTable中的枚举
        private void TransferEnum(DataTable dt, DataTable dtFields)
        {
            var enumService = FormulaHelper.GetService<IEnumService>();

            foreach (DataRow field in dtFields.Rows)
            {
                string code = field["Code"].ToString();
                string enumKey = field["EnumKey"].ToString();
                if (enumKey == "")
                    continue;

                dt.Columns[code].ColumnName = code + "_";
                dt.Columns.Add(code);

                foreach (DataRow row in dt.Rows)
                {
                    string enumValue = row[code + "_"].ToString();
                    row[code] = enumService.GetEnumText(enumKey, enumValue);
                }
                dt.Columns.Remove(code + "_");
            }
        }

        #endregion

        #region reportDefineRow

        private DataRow _reportDefineRow = null;
        public DataRow reportDefineRow
        {
            get
            {
                if (_reportDefineRow == null)
                {
                    _reportDefineRow = baseSQLHelper.ExecuteDataTable(string.Format("select * from S_R_Define where Code='{0}'", reportCode)).Rows[0];
                }

                return _reportDefineRow;
            }
        }

        #endregion

        #region dataSetDefine

        private DataTable _dataSetDefine = null;
        public DataTable dataSetDefine
        {
            get
            {
                if (_dataSetDefine == null)
                {
                    _dataSetDefine = baseSQLHelper.ExecuteDataTable(string.Format("select * from S_R_DataSet where DefineID='{0}'", reportDefineRow["ID"]));
                }

                return _dataSetDefine;
            }
        }

        #endregion

        #region dicDataSetField

        private Dictionary<string, DataTable> _dicDataSetField = null;
        public Dictionary<string, DataTable> dicDataSetField
        {
            get
            {
                if (_dicDataSetField == null)
                {
                    _dicDataSetField = new Dictionary<string, DataTable>();

                    foreach (DataRow row in dataSetDefine.Rows)
                    {
                        string sql = row["Sql"].ToString();

                        DataTable dtFields = baseSQLHelper.ExecuteDataTable(string.Format("select * from S_R_Field where DataSetID='{0}'", row["ID"]));

                        _dicDataSetField.Add(row["Name"].ToString(), dtFields);

                    }
                }
                return _dicDataSetField;
            }
        }

        #endregion

        #region dicDataSets

        private Dictionary<string, DataTable> _dicDataSets = null;
        public Dictionary<string, DataTable> dicDataSet
        {
            get
            {
                if (_dicDataSets == null)
                {
                    _dicDataSets = new Dictionary<string, DataTable>();

                    foreach (DataRow row in dataSetDefine.Rows)
                    {
                        SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(row["ConnName"].ToString());

                        string sql = row["Sql"].ToString();
                        //替换sql中的变量
                        Base.Logic.BusinessFacade.UIFO uiFO = new Base.Logic.BusinessFacade.UIFO();
                        sql = uiFO.ReplaceString(sql);

                        List<SqlParameter> pList = new List<SqlParameter>();
                        int start = 0;
                        if (paramList.Count > 0 && paramList[0].ValidValues.Count > 0)
                        {
                            start = 1;
                        }
                        for (int i = paramList.Count - 1; i >= start; i--)
                        {
                            var item = paramList[i];
                            if (item.Prompt.StartsWith("@"))
                            {
                                TextBox txt = table.FindControl("txt" + (i - start + 1).ToString()) as TextBox;
                                if (string.IsNullOrEmpty(txt.Text) && item.Nullable == "true")
                                {
                                    pList.Add(new SqlParameter(item.Prompt, null));
                                }
                                else
                                {
                                    pList.Add(new SqlParameter(item.Prompt, txt.Text));
                                }
                            }
                        }

                        DataTable dt = null;
                        if (!sql.Trim().StartsWith("select", StringComparison.CurrentCultureIgnoreCase))
                            dt = sqlHelper.ExecuteDataTable(sql, pList.ToArray(), CommandType.StoredProcedure);
                        else
                            dt = sqlHelper.ExecuteDataTable(sql, pList.ToArray(), CommandType.Text);

                        string name = row["Name"].ToString();

                        TransferEnum(dt, dicDataSetField[name]);

                        _dicDataSets.Add(name, dt);

                    }

                }
                return _dicDataSets;
            }
        }

        #endregion

        #region sourceDoc

        private XmlDocument _sourceDoc = null;
        private XmlDocument sourceDoc
        {
            get
            {
                if (_sourceDoc == null)
                {
                    string path = HttpContext.Current.Server.MapPath("/") + "AutoReport/" + reportCode + ".rdl";
                    _sourceDoc = new XmlDocument();
                    _sourceDoc.Load(path);
                }
                return _sourceDoc;
            }
        }

        #endregion

        #region paramList

        private List<ParamInfo> _paramList = null;
        public List<ParamInfo> paramList
        {
            get
            {
                if (_paramList == null)
                {
                    _paramList = new List<ParamInfo>();

                    XmlNamespaceManager xmlNM = new XmlNamespaceManager(sourceDoc.NameTable);


                    var parameters = sourceDoc.GetElementsByTagName("ReportParameter");

                    foreach (XmlNode node in parameters)
                    {
                        xmlNM.AddNamespace("rd", node.NamespaceURI);

                        if (node.Attributes["Name"].Value == "RootPath")
                        {
                            this.hasRootPath = true;
                            continue;
                        }

                        ParamInfo pInfo = new ParamInfo();
                        pInfo.ValidValues = new List<ParamValue>();
                        _paramList.Add(pInfo);

                        pInfo.Name = node.Attributes["Name"].Value;
                        pInfo.DataType = node.SelectSingleNode("rd:DataType", xmlNM).InnerText;
                        pInfo.Prompt = pInfo.Name;
                        if (node.SelectSingleNode("rd:Prompt", xmlNM) != null)
                            pInfo.Prompt = node.SelectSingleNode("rd:Prompt", xmlNM).InnerText;
                        pInfo.AllowBlank = "false";
                        if (node.SelectSingleNode("rd:AllowBlank", xmlNM) != null)
                            pInfo.AllowBlank = "true";

                        pInfo.Nullable = "false";
                        if (node.SelectSingleNode("rd:Nullable", xmlNM) != null)
                            pInfo.Nullable = "true";

                        pInfo.MultiValue = "false";
                        if (node.SelectSingleNode("rd:MultiValue", xmlNM) != null)
                            pInfo.MultiValue = "true";

                        #region 可用值

                        var ValidValues = node.SelectSingleNode("rd:ValidValues", xmlNM);

                        if (ValidValues != null)
                        {
                            var ParameterValues = ValidValues.ChildNodes[0].SelectNodes("rd:ParameterValue", xmlNM);
                            if (ParameterValues.Count > 0)
                            {
                                foreach (XmlNode item in ParameterValues)
                                {
                                    pInfo.ValidValues.Add(new ParamValue { value = item.FirstChild.InnerText, text = item.LastChild.InnerText });
                                }
                            }
                            else
                            {
                                var DataSetReference = ValidValues.SelectSingleNode("rd:DataSetReference", xmlNM);
                                string DataSetName = DataSetReference.SelectSingleNode("rd:DataSetName", xmlNM).InnerText;
                                string ValueField = DataSetReference.SelectSingleNode("rd:ValueField", xmlNM).InnerText;
                                string LabelField = DataSetReference.SelectSingleNode("rd:LabelField", xmlNM).InnerText;
                                ValueField = dicDataSetField[DataSetName].AsEnumerable().Where(c => c["Name"].ToString() == ValueField).SingleOrDefault()["Code"].ToString();
                                LabelField = dicDataSetField[DataSetName].AsEnumerable().Where(c => c["Name"].ToString() == LabelField).SingleOrDefault()["Code"].ToString();
                                //如果为枚举字段
                                string enumKey = dicDataSetField[DataSetName].AsEnumerable().Where(c => c["Code"].ToString() == ValueField).SingleOrDefault()["EnumKey"].ToString();
                                if (enumKey == "")
                                {
                                    foreach (DataRow row in dicDataSet[DataSetName].Rows)
                                    {
                                        pInfo.ValidValues.Add(new ParamValue { value = row[ValueField].ToString(), text = row[LabelField].ToString() });
                                    }
                                }
                                else
                                {
                                    IEnumService enumService = FormulaHelper.GetService<IEnumService>();
                                    DataTable dtEnum = enumService.GetEnumTable(enumKey);
                                    foreach (DataRow row in dtEnum.Rows)
                                    {
                                        pInfo.ValidValues.Add(new ParamValue { value = row["value"].ToString(), text = row["text"].ToString() });
                                    }
                                }

                            }
                        }
                        #endregion

                        #region 默认值

                        XmlNode DefaultValue = node.SelectSingleNode("rd:DefaultValue", xmlNM);
                        if (DefaultValue != null)
                        {
                            XmlNode values = DefaultValue.SelectSingleNode("rd:Values", xmlNM);
                            if (values != null)
                            {
                                foreach (XmlNode item in values.ChildNodes)
                                {
                                    pInfo.DefaultValue += "," + item.InnerText;
                                }
                                pInfo.DefaultValue = pInfo.DefaultValue.Trim(',');
                            }
                            else
                            {
                                var DataSetReference = DefaultValue.SelectSingleNode("rd:DataSetReference", xmlNM);
                                string dataSetName = DataSetReference.SelectSingleNode("rd:DataSetName", xmlNM).InnerText;
                                string valueField = DataSetReference.SelectSingleNode("rd:ValueField", xmlNM).InnerText;
                                valueField = dicDataSetField[dataSetName].AsEnumerable().Where(c => c["Name"].ToString() == valueField).FirstOrDefault()["Code"].ToString();

                                DataTable table = dicDataSet[dataSetName];
                                string value = "";
                                if (pInfo.MultiValue == "true")
                                {
                                    foreach (DataRow row in table.Rows)
                                    {
                                        value = row[valueField].ToString();
                                        pInfo.DefaultValue += "," + value;
                                    }
                                    pInfo.DefaultValue = pInfo.DefaultValue.Trim(',');
                                }
                                else if (table.Rows.Count > 0)
                                {
                                    value = table.Rows[0][valueField].ToString();
                                    pInfo.DefaultValue = value;
                                }

                            }
                        }

                        #endregion

                    }

                }
                return _paramList;
            }
        }

        #endregion

        public bool hasRootPath = false;


    }

    #region 私有类

    public class ParamInfo
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 参数类型
        /// </summary>
        public string DataType { get; set; }
        /// <summary>
        /// 参数标题
        /// </summary>
        public string Prompt { get; set; }
        /// <summary>
        /// 可用值
        /// </summary>
        public List<ParamValue> ValidValues { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// 是否允许空
        /// </summary>
        public string AllowBlank { get; set; }

        public string Nullable { get; set; }

        public string MultiValue { get; set; }
    }

    public class ParamValue
    {
        public string value { get; set; }
        public string text { get; set; }
    }

    #endregion

}