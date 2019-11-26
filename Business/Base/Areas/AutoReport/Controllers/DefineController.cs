using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using MvcAdapter;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using Config;
using System.Data;
using System.Text;
using Formula;
using Formula.Exceptions;

namespace Base.Areas.AutoReport.Controllers
{
    public class DefineController : BaseController<S_R_Define>
    {

        #region 树和列表数据获取

        public override JsonResult GetTree()
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            DataTable dt = sqlHelper.ExecuteDataTable(string.Format("select ID,ParentID,FullID,Code,Name from S_M_Category where FullID like '{0}%'", "0"));
            return Json(dt, JsonRequestBehavior.AllowGet);
        }

        #endregion


        public JsonResult GetRelationList(QueryBuilder qb)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);

            var sql = " select ID,Code,Name,ModifyUserID,ModifyUserName,modifyTime,0 as HasReport from S_R_Define ";
            if (Config.Constant.IsOracleDb)
            {
                sql = "SELECT ID,CODE as \"Code\",NAME as \"Name\",MODIFYUSERID as \"ModifyUserID\",MODIFYUSERNAME as \"ModifyUserName\",MODIFYTIME as \"modifyTime\",0 AS \"HasReport\" FROM S_R_DEFINE ";
            }

            string categoryID = Request["NodeFullID"];
            if (!string.IsNullOrEmpty(categoryID))
            {
                categoryID = categoryID.Split('.').Last();
                sql += string.Format(" where CATEGORYID = '{0}'", categoryID);
            }


            DataTable dt = sqlHelper.ExecuteDataTable(sql, qb);
            foreach (DataRow row in dt.Rows)
            {
                string path = HttpContext.Server.MapPath("/") + "AutoReport/" + row["Code"].ToString() + ".rdl";
                if (System.IO.File.Exists(path))
                    row["HasReport"] = "1";
            }
            GridData data = new GridData(dt);
            data.total = qb.TotolCount;
            return Json(data);
        }

        public JsonResult GetDataSetList(string defineID)
        {
            return Json(entities.Set<S_R_DataSet>().Where(c => c.DefineID == defineID));
        }

        public JsonResult GetFieldList(string dataSetID)
        {
            return Json(entities.Set<S_R_Field>().Where(c => c.DataSetID == dataSetID));
        }

        public override JsonResult Save()
        {
            base.UpdateEntity<S_R_Define>();
            return base.JsonSaveList<S_R_DataSet>(Request["dataSetList"]);
        }

        public JsonResult SaveField()
        {
            base.UpdateEntity<S_R_Define>();
            base.UpdateList<S_R_DataSet>(Request["dataSetList"]);
            return base.JsonSaveList<S_R_Field>(Request["fieldList"]);
        }

        public JsonResult DeleteDataList()
        {
            return base.JsonDelete<S_R_DataSet>(Request["ListIDs"]);
        }

        public JsonResult DeleteFieldList()
        {
            return base.JsonDelete<S_R_Field>(Request["ListIDs"]);
        }

        public JsonResult UploadRdlc()
        {
            if (Request.Files.Count > 0)
            {
                string fileFullName = HttpContext.Server.MapPath("/") + "AutoReport/" + Request.Files[0].FileName;
                Request.Files[0].SaveAs(fileFullName);

                string code = Request.Files[0].FileName.Split('.').First();

                var define = entities.Set<S_R_Define>().Where(c => c.Code == code).SingleOrDefault();
                var user = FormulaHelper.GetUserInfo();
                define.ModifyUserID = user.UserID;
                define.ModifyUserName = user.UserName;
                define.ModifyTime = DateTime.Now;
                entities.SaveChanges();
            }
            return Json("");
        }

        public JsonResult ImportField(string dataSetID)
        {
            //先保存
            Save();



            //去除Sql中没有涉及的字段
            var dataSet = entities.Set<S_R_DataSet>().Where(c => c.ID == dataSetID).SingleOrDefault();
            var define = entities.Set<S_R_Define>().Where(c => c.ID == dataSet.DefineID).SingleOrDefault();


            string tableNames = dataSet.TableNames;

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(dataSet.ConnName);

            DataTable dtField = null;
            if (dataSet.Sql.Trim().StartsWith("select", StringComparison.CurrentCultureIgnoreCase) == false)
            {
                dtField = sqlHelper.ExecuteDataTable(dataSet.Sql, CommandType.StoredProcedure);
            }
            else
            {
                dtField = sqlHelper.ExecuteDataTable(string.Format("select * from ({0}) table1 where 1=2", dataSet.Sql));
            }

            if (!string.IsNullOrEmpty(tableNames))
            {
                #region 从数据字典导入

                sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);

                string sql = string.Format("SELECT S_M_FIELD.* FROM S_M_FIELD JOIN S_M_TABLE ON S_M_TABLE.ID = TABLEID WHERE S_M_TABLE.CODE IN('{0}') order by SortIndex", tableNames.Replace(",", "','"));

                DataTable dt = sqlHelper.ExecuteDataTable(sql);

                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                {
                    if (!dtField.Columns.Contains(dt.Rows[i]["Code"].ToString()))
                    {
                        dt.Rows.RemoveAt(i);
                    }
                }

                var dtDest = sqlHelper.ExecuteDataTable(string.Format("select * from S_R_Field where DataSetID='{0}'", dataSetID)).AsEnumerable();

                StringBuilder sb = new StringBuilder();
                int index = 0;
                foreach (DataRow row in dt.Rows)
                {
                    if (dtDest.Where(c => c["Code"].ToString().ToLower() == row["Code"].ToString().ToLower()).Count() > 0)
                        continue;
                    sb.AppendFormat(" INSERT INTO S_R_FIELD(ID,DATASETID,CODE,NAME,TYPE) VALUES('{0}','{1}','{2}','{3}','{4}');"
                        , FormulaHelper.CreateGuid(index++), dataSetID, row["Code"], row["Name"].ToString() == "" ? row["Code"].ToString() : row["Name"].ToString(), "System.String");
                }
                if (sb.Length > 0)
                {
                    if (Config.Constant.IsOracleDb)
                    {
                        sqlHelper.ExecuteNonQuery("BEGIN " + sb.ToString() + " END;");
                    }
                    else
                    {
                        sqlHelper.ExecuteNonQuery(sb.ToString());
                    }
                }

                #endregion
            }
            else
            {
                #region 直接导入

                sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                var dtDest = sqlHelper.ExecuteDataTable(string.Format("select * from S_R_Field where DataSetID='{0}'", dataSetID)).AsEnumerable();

                StringBuilder sb = new StringBuilder();
                int index = 0;
                foreach (DataColumn col in dtField.Columns)
                {
                    if (dtDest.Where(c => c["Code"].ToString().ToLower() == col.ColumnName.ToLower()).Count() > 0)
                        continue;
                    sb.AppendFormat(" INSERT INTO S_R_FIELD(ID,DATASETID,CODE,NAME,TYPE) VALUES('{0}','{1}','{2}','{3}','{4}');"
                        , FormulaHelper.CreateGuid(index), dataSetID, col.ColumnName, col.ColumnName, "System.String");
                    index++;
                }
                if (sb.Length > 0)
                {
                    if (Config.Constant.IsOracleDb)
                    {
                        sqlHelper.ExecuteNonQuery("BEGIN " + sb.ToString() + " END;");
                    }
                    else
                    {
                        sqlHelper.ExecuteNonQuery(sb.ToString());
                    }
                }

                #endregion
            }
            return Json("");
        }

        public FileResult RdlFile(string rdlCode)
        {
            var define = entities.Set<S_R_Define>().Where(c => c.Code == rdlCode).SingleOrDefault();
            string filePath = HttpContext.Server.MapPath("/") + "AutoReport/" + rdlCode + ".rdl";
            if (!System.IO.File.Exists(filePath))
            {
                return TmplFile(define.ID);
            }

            XmlDocument sourceDoc = new XmlDocument();
            sourceDoc.Load(filePath);

            var dataSources = sourceDoc.GetElementsByTagName("DataSources")[0];
            dataSources.RemoveAll();
            var dataSets = sourceDoc.GetElementsByTagName("DataSets")[0];

            //保留数据集中的Filters
            Dictionary<string, XmlNode> dicFilterNode = new Dictionary<string, XmlNode>();
            foreach (XmlElement item in dataSets.ChildNodes)
            {
                var c = item.GetElementsByTagName("Filters");
                if (c.Count > 0)
                    dicFilterNode.Add(item.Attributes["Name"].Value, c[0]);

            }
            //清空数据集
            dataSets.RemoveAll();
            foreach (var item in define.S_R_DataSet)
            {
                XmlNode dataSetNode = GetDataSet(item.Name, item.ConnName, item.Sql, item.S_R_Field.ToList());

                //加入刚才保留的的Filters
                if (dicFilterNode.ContainsKey(item.Name))
                {
                    dataSetNode.AppendChild(dataSetNode.OwnerDocument.ImportNode(dicFilterNode[item.Name], true));
                    //dataSetNode.AppendChild(dicFilterNode[item.Name]);
                }

                XmlNode dataSourceNode = GetDataSource(item.Name);

                var newDataSource = sourceDoc.ImportNode(dataSourceNode, true);
                dataSources.AppendChild(newDataSource);

                var newDataSet = sourceDoc.ImportNode(dataSetNode, true);
                dataSets.AppendChild(newDataSet);
            }

            MemoryStream ms = new MemoryStream();
            XmlSerializer serializer = new XmlSerializer(typeof(XmlDocument));
            serializer.Serialize(ms, sourceDoc);
            ms.Position = 0;

            return File(ms, "application/octet-stream ; Charset=UTF8", define.Code + ".rdl");

        }

        private XmlNode GetDataSet(string reportCode, string connName, string sql, List<S_R_Field> fieldList = null)
        {
            StringBuilder sbCommendText = new StringBuilder();
            StringBuilder sbData = new StringBuilder();
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(connName);
            DataTable dt = sqlHelper.ExecuteDataTable(sql, 0, 10, CommandType.Text);

            TransferEnum(dt, fieldList);

            List<Dictionary<string, string>> dicFieldList = new List<Dictionary<string, string>>();
            if (fieldList != null)
            {
                foreach (var field in fieldList)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("Code", field.Code);
                    dic.Add("Name", field.Name);
                    dic.Add("Type", field.Type);
                    dicFieldList.Add(dic);
                }
            }
            else
            {
                foreach (DataColumn col in dt.Columns)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();

                    dic.Add("Code", col.ColumnName);
                    dic.Add("Name", col.ColumnName);
                    dic.Add("Type", col.DataType.FullName);
                    dicFieldList.Add(dic);
                }
            }

            //xml数据格式
            string fieldFormat = "";


            foreach (var dic in dicFieldList)
            {
                string reportFieldType = "String";
                string fieldType = dic["Type"].Replace("System.", "");
                if (fieldType.StartsWith("Int"))
                    reportFieldType = "Integer";
                else if (fieldType == "Double")
                    reportFieldType = "Decimal";
                fieldFormat += string.Format("@{0}({1}),", dic["Code"], reportFieldType);
            }

            fieldFormat = fieldFormat.Trim(',');




            foreach (DataRow row in dt.Rows)
            {
                sbData.Append("<Row");
                foreach (DataColumn col in dt.Columns)
                {
                    sbData.AppendFormat(" {0}=\"{1}\"", col.ColumnName, row[col].ToString().Replace('\"', '\''));
                }
                sbData.Append("></Row>");
            }
            sbCommendText.Append("<Query>\n");
            sbCommendText.Append("<ElementPath>Root/Row{" + fieldFormat + "}</ElementPath>");
            sbCommendText.Append("<XmlData>");
            sbCommendText.Append("<Root>");
            sbCommendText.Append(sbData.ToString());
            sbCommendText.Append("</Root>");
            sbCommendText.Append("</XmlData>");
            sbCommendText.Append("</Query>");


            string path = HttpContext.Server.MapPath("/") + "AutoReport/Tmpl/dataSet.xml";
            XmlDocument sourceDoc = new XmlDocument();
            sourceDoc.Load(path);

            //数据源名称
            var nodeDataSourceName = sourceDoc.GetElementsByTagName("DataSourceName")[0];
            nodeDataSourceName.InnerXml = reportCode;

            //数据集
            var nodeDataSet = sourceDoc.GetElementsByTagName("DataSet")[0];
            nodeDataSet.Attributes["Name"].Value = reportCode;

            //查询语句
            var nodeCommentText = sourceDoc.GetElementsByTagName("CommandText")[0];
            nodeCommentText.InnerXml = HttpContext.Server.HtmlEncode(sbCommendText.ToString());

            //设置字段集
            var list = sourceDoc.GetElementsByTagName("Fields");
            var fields = list[0];
            var node = fields.FirstChild;
            fields.RemoveAll();

            foreach (var dic in dicFieldList)
            {
                var newNode = node.Clone();
                newNode.Attributes["Name"].Value = dic["Name"];
                newNode.FirstChild.InnerXml = dic["Code"];
                newNode.LastChild.InnerXml = dic["Type"];
                fields.AppendChild(newNode);
            }

            return sourceDoc.GetElementsByTagName("DataSet")[0].Clone();
        }

        private XmlNode GetDataSource(string reportCode)
        {
            string path = HttpContext.Server.MapPath("/") + "AutoReport/Tmpl/dataSet.xml";
            XmlDocument sourceDoc = new XmlDocument();
            sourceDoc.Load(path);
            //数据源名称
            var nodeDataSource = sourceDoc.GetElementsByTagName("DataSource")[0];
            nodeDataSource.Attributes["Name"].Value = reportCode;
            var nodeDataSourceID = sourceDoc.GetElementsByTagName("rd:DataSourceID")[0];
            nodeDataSourceID.InnerXml = Guid.NewGuid().ToString();

            return sourceDoc.GetElementsByTagName("DataSource")[0].Clone();
        }

        //翻译DataTable中的枚举
        private void TransferEnum(DataTable dt, List<S_R_Field> dtFields)
        {
            if (dtFields == null)
                return;

            var enumService = FormulaHelper.GetService<IEnumService>();

            foreach (var field in dtFields)
            {
                string code = field.Code;
                string enumKey = field.EnumKey;
                if (string.IsNullOrEmpty(enumKey))
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

        public FileResult TmplFile(string defineID)
        {
            var define = entities.Set<S_R_Define>().Where(c => c.ID == defineID).SingleOrDefault();

            string path = HttpContext.Server.MapPath("/") + "AutoReport/Tmpl/tmpl.rdl";
            XmlDocument sourceDoc = new XmlDocument();
            sourceDoc.Load(path);

            //RootPath的默认值       
            var reportParameter = sourceDoc.GetElementsByTagName("ReportParameter")[0];
            reportParameter.ChildNodes[1].ChildNodes[0].ChildNodes[0].InnerXml = Request.Url.Scheme + "://" + Request.Url.Host + ":" + Request.Url.Port;

            var dataSources = sourceDoc.GetElementsByTagName("DataSources")[0];
            dataSources.RemoveAll();
            var dataSets = sourceDoc.GetElementsByTagName("DataSets")[0];
            dataSets.RemoveAll();

            foreach (var item in define.S_R_DataSet)
            {
                XmlNode dataSetNode = GetDataSet(item.Name, item.ConnName, item.Sql, item.S_R_Field.ToList());
                XmlNode dataSourceNode = GetDataSource(item.Name);

                var newDataSource = sourceDoc.ImportNode(dataSourceNode, true);
                dataSources.AppendChild(newDataSource);

                var newDataSet = sourceDoc.ImportNode(dataSetNode, true);
                dataSets.AppendChild(newDataSet);
            }

            MemoryStream ms = new MemoryStream();
            XmlSerializer serializer = new XmlSerializer(typeof(XmlDocument));
            serializer.Serialize(ms, sourceDoc);
            ms.Position = 0;

            return File(ms, "application/octet-stream ; Charset=UTF8", define.Code + ".rdl");
        }


        #region 导出

        public FileResult ExportSql(string defID, string fileCode)
        {
            string sql = exportSql(defID);
            MemoryStream ms = new MemoryStream(System.Text.Encoding.Default.GetBytes(sql));
            ms.Position = 0;
            return File(ms, "application/octet-stream ; Charset=UTF8", fileCode + ".sql");
        }

        private string exportSql(string defID)
        {
            StringBuilder sb = new StringBuilder();
            string sql = string.Format("select * from S_R_Define where ID='{0}'", defID);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dtDefReport = sqlHelper.ExecuteDataTable(sql);
            sql = string.Format("select * from S_R_DataSet where DefineID='{0}'", defID);
            DataTable dtDataSet = sqlHelper.ExecuteDataTable(sql);
            sql = string.Format("select S_R_Field.* from S_R_Field join S_R_DataSet on S_R_DataSet.ID=DataSetID where DefineID='{0}'", defID);
            DataTable dtField = sqlHelper.ExecuteDataTable(sql);


            sb.AppendLine(string.Format("delete from S_R_Define where ID='{0}'", defID));
            sb.AppendLine(SQLHelper.CreateInsertSql("S_R_Define", dtDefReport));
            sb.AppendLine(SQLHelper.CreateInsertSql("S_R_DataSet", dtDataSet));
            sb.AppendLine(SQLHelper.CreateInsertSql("S_R_Field", dtField));

            return sb.ToString();
        }

        #endregion

        #region 创建菜单入口

        public JsonResult CreateMenu(string ReportID)
        {
            var report = entities.Set<S_R_Define>().SingleOrDefault(c => c.ID == ReportID);

            string url = "/MvcConfig/RdlView.aspx?ReportCode=" + report.Code;

            if (entities.Set<S_A_Res>().Count(c => c.Url.StartsWith(url)) > 0)
            {
                throw new BusinessException("菜单入口已存在！");
            }

            var category = entities.Set<S_M_Category>().SingleOrDefault(c => c.ID == report.CategoryID);
            var pMenu = entities.Set<S_A_Res>().SingleOrDefault(c => c.Name == category.Name);
            if (pMenu == null)
                pMenu = entities.Set<S_A_Res>().SingleOrDefault(c => c.ID == Config.Constant.MenuRooID);
            var menu = new S_A_Res();
            menu.ID = FormulaHelper.CreateGuid();
            menu.Name = report.Name;
            menu.Url = url;
            menu.ParentID = pMenu.ID;
            menu.FullID = pMenu.FullID + "." + menu.ID;
            menu.SortIndex = 9999;
            menu.Type = "Menu";
            entities.Set<S_A_Res>().Add(menu);
            S_A__OrgRes orgRes = new S_A__OrgRes();
            orgRes.OrgID = Config.Constant.OrgRootID;
            orgRes.ResID = menu.ID;
            entities.Set<S_A__OrgRes>().Add(orgRes);
            entities.SaveChanges();
            return Json(new { ID = menu.ID });
        }

        #endregion


    }
}
