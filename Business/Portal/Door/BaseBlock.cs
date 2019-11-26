using System;
using System.Data;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Text;
using Config;
using Formula;
using Formula.Helper;
using System.Collections.Generic;

namespace Portal.Door
{
    public class BaseBlock
    {
        SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
        //外部webservice数据源
        public event GetServiceHandler GetServiceData;

        #region 构造函数
        public BaseBlock() { }

        public BaseBlock(string nodeId)
        {
            this.Entity = sqlHelper.ExecuteObject<DoorBlock>(string.Format("select * from S_P_DoorBlock where ID = '{0}'", nodeId));
        }

        #endregion

        #region 属性

        /// <summary>
        /// 保存修改前的基本信息
        /// </summary>
        private DoorBlock Entity = new DoorBlock();

        /// <summary>
        /// 当前用户
        /// </summary>
        public string UserID
        {
            get
            {
                return Formula.FormulaHelper.UserID;
            }
        }

        public string BlockKey
        {
            get
            {
                return this.Entity.BlockKey;
            }
        }

        public string RepeatDataDataSql
        {
            get
            {
                return this.Entity.RepeatDataDataSql;
            }
        }

        public string RepeatItemTemplate
        {
            get
            {
                return this.Entity.RepeatItemTemplate;
            }
        }

        public int? RepeatItemCount
        {
            get
            {
                return this.Entity.RepeatItemCount;
            }
        }
        #endregion

        #region 公有方法

        /// <summary>
        /// 重载更新方法,进行节点数据的校验
        /// </summary>
        /// <returns></returns>
        public void Update()
        {
            //todo 处理块修订逻辑、包含根据ID判断是否新增
        }

        public DataTable GetDataSource()
        {
            string sourceSQL = this.Entity.RepeatDataDataSql;

            if (sourceSQL != null && sourceSQL.StartsWith("["))//自定义数据
            {
                return GetServiceData(this.BlockKey, this.Entity.RepeatItemCount.Value);
            }
            else if (sourceSQL != null)
            {
                sourceSQL = sourceSQL.Replace("[UserID]", this.UserID).Replace("[UserId]", this.UserID);
                sourceSQL = ReplaceLabel(sourceSQL.Trim());
                if (this.Entity.RepeatItemCount == null)
                    return sqlHelper.ExecuteDataTable(sourceSQL);
                else
                    return sqlHelper.ExecuteDataTable(sourceSQL, 0, this.Entity.RepeatItemCount.Value, CommandType.Text);
            }
            else
                return null;

        }
        //替换库名标签{Market}
        private string ReplaceLabel(string sql)
        {
            string[] connNames = typeof(ConnEnum).GetEnumNames();
            foreach (string conn in connNames)
            {
                if (sql.IndexOf("{" + conn + "}") > -1)
                {
                    object objName = CacheHelper.Get("DBName_" + conn);
                    if (objName == null)
                    {
                        SQLHelper sqlH = SQLHelper.CreateSqlHelper(conn);
                        CacheHelper.Set("DBName_" + conn, sqlH.DbName);
                        objName = sqlH.DbName;
                    }
                    sql = sql.Replace("{" + conn + "}", objName.ToString());
                }
            }
            return sql;
        }

        //获取标题头
        public string GetHeadHtml()
        {
            string template = this.Entity.HeadHtml == null ? string.Empty : this.Entity.HeadHtml;
            Regex rg = new Regex("\\[[^][]*\\]", RegexOptions.Multiline);
            MatchCollection mtc = rg.Matches(template);
            string tcs = "";
            foreach (Match mt in mtc)
            {
                if (tcs.IndexOf(mt.Value) > -1)
                    continue;
                string val = mt.Value;
                tcs += val + ",";
                string col = val.Substring(1, mt.Value.Length - 2);

                string propertyValue = Convert.ToString(typeof(DoorBlock).GetProperty(col).GetValue(this.Entity, null));
                if (string.IsNullOrEmpty(propertyValue))
                    continue;
                template = template.Replace(val, propertyValue);
            }
            return template;
        }

        //获取内容
        public string GetContentHtml()
        {
            string template = this.Entity.RepeatItemTemplate == null ? string.Empty : this.Entity.RepeatItemTemplate;
            string html = "";

            DataTable source = this.GetDataSource();
            if (source != null && source.Rows.Count > 0)
            {
                foreach (DataRow dr in source.Rows)
                {
                    string item = template;
                    Regex rg = new Regex("\\[[^][]*\\]", RegexOptions.Multiline);
                    MatchCollection mtc = rg.Matches(template);
                    foreach (Match mt in mtc)
                    {
                        item = item.Replace(mt.Value, dr[mt.Value.Substring(1, mt.Value.Length - 2)].ToString());
                    }
                    html += item;
                }
                return html;
            }
            else
            {
                return "";
            }
        }
        //获取页脚
        public string GetFootHtml()
        {
            string template = this.Entity.FootHtml == null ? string.Empty : this.Entity.FootHtml;
            Regex rg = new Regex("\\[[^][]*\\]", RegexOptions.Multiline);
            MatchCollection mtc = rg.Matches(template);
            foreach (Match mt in mtc)
            {
                string val = mt.Value;
                string col = val.Substring(1, mt.Value.Length - 2);

                string propertyValue = Convert.ToString(typeof(DoorBlock).GetProperty(col).GetValue(this.Entity, null));
                if (string.IsNullOrEmpty(propertyValue))
                    continue;
                template = template.Replace(val, propertyValue);
            }
            return template;
        }
        #endregion

        #region 静态方法

        private static DataTable GetDoorBaseTemplate(SQLHelper sqlHelper, string templateId)
        {
            return sqlHelper.ExecuteDataTable(string.Format("select * from S_P_DoorBaseTemplate where ID='{0}'", templateId));
        }

        //获得模版布局html
        public static string GetBlocks(ref string LayoutType, string blockType, string templateId)
        {
            string html = "", withT = string.Empty, templateString = string.Empty;

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);

            DataTable dt = GetDoorBaseTemplate(sqlHelper, templateId);
            DataRow dtr = dt.Rows[0];
            withT = (dtr["TemplateColWidth"] == null) ? "" : dtr["TemplateColWidth"].ToString();
            templateString = (dtr["TemplateString"] == null) ? "" : dtr["TemplateString"].ToString().Trim();
            if (templateString != "")
            {
                string[] cols = templateString.Split(';');
                LayoutType = cols.Length.ToString();
                string[] widths = withT.Split(',');
                int i = 0;
                foreach (string col in cols)
                {
                    html += GetCols(col, widths[i], ++i);
                }
            }
            return html;
        }

        public static string GetCols(string col, string width, int i)
        {
            StringBuilder blockHtmls = new StringBuilder();
            string colBeginHtml = "<DIV class=\"col_div\" id=\"col_{0}\" style=\"WIDTH: {1}\">";
            string colEndHtml = @"<DIV class='drag_div no_drag' id='col_{0}_hidden_div'>
					<DIV id='col_{0}_hidden_div_h'></DIV>
				</DIV></DIV>";
            string BlockBeginHtml = "<DIV class=\"drag_div\" id=\"drag_{0}\" style=\"BORDER-COLOR: {1}; BACKGROUND: #fff;\">";
            string BlockEndHtml = @"<DIV id='drag_switch_{0}'>
						<DIV class='drag_editor' id='drag_editor_{0}' style='DISPLAY: none'>
							<DIV id='loadeditorid_{0}' style='WIDTH: 100px'><IMG src='/Portal/Door/image/loading.gif'><SPAN id='loadeditortext_{0}' style='COLOR: #333'></SPAN></DIV>
						</DIV>
						<DIV class='drag_content' id='drag_content_{0}'>
							<DIV id='loadcontentid_{0}' style='WIDTH: 100px'><IMG src='/Portal/Door/image/loading.gif'><SPAN id='loadcontenttext_{0}' style='COLOR: #333'></SPAN></DIV>
						</DIV>
						<SCRIPT id='{0}' type='text/javascript'>{1}</SCRIPT>
					</DIV>
				</DIV>";
            blockHtmls.Append(string.Format(colBeginHtml, Convert.ToString(i), width));
            string[] blockIds = col.Split(',');
            //每列的各块
            foreach (string blockId in blockIds)
            {
                if (blockId != "")
                {
                    BaseBlock bl = new BaseBlock(blockId);
                    if (bl.Entity == null) continue;

                    blockHtmls.Append(string.Format(BlockBeginHtml, blockId, bl.Entity.ColorValue));

                    blockHtmls.Append(bl.GetHeadHtml());
                    //是否延时加载
                    if (bl.Entity.DelayLoadSecond.Value > 0)
                        blockHtmls.Append(string.Format(BlockEndHtml, blockId, "window.setTimeout(\"loadDragContent('" + blockId + "','" + bl.Entity.RepeatItemCount + "')\"," + Convert.ToString(bl.Entity.DelayLoadSecond.Value * 1000) + ");\n" + bl.Entity.RelateScript));
                    else
                        blockHtmls.Append(string.Format(BlockEndHtml, blockId, "loadDragContent('" + blockId + "','" + bl.Entity.RepeatItemCount + "');\n" + bl.Entity.RelateScript));
                }
            }
            blockHtmls.Append(string.Format(colEndHtml, Convert.ToString(i)));
            return blockHtmls.ToString();
        }

        //保存门户块拖拽布局
        public static void SaveGetBlocks(string TemplateString, string templateId)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);

            string updateSQL = "update S_P_DoorBaseTemplate set TemplateString='{0}' where ID='{1}'";
            sqlHelper.ExecuteNonQuery(String.Format(updateSQL, TemplateString, templateId));
        }
        //保存模版列布局
        public static void SaveChangeColumns(string columns, string TemplateString, string templateId, params string[] lists)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);

            int cols = int.Parse(columns);
            string widths = "";
            for (int i = 0; i < cols; i++)
            {
                widths += lists[i] + "%,";
            }
            widths = widths.TrimEnd(',');

            string updateSQL = "update S_P_DoorBaseTemplate set TemplateString='{0}',TemplateColWidth='{1}' where ID='{2}'";
            sqlHelper.ExecuteNonQuery(String.Format(updateSQL, TemplateString, widths, templateId));
        }
        //保存列宽度
        public static void ChangeColumnsWidth(string columns, string templateId, params string[] lists)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            int cols = int.Parse(columns);
            string widths = "";
            for (int i = 0; i < cols; i++)
            {
                widths += lists[i] + "%,";
            }
            widths = widths.TrimEnd(',');

            string updateSQL = "update S_P_DoorBaseTemplate set TemplateColWidth='{0}' where ID='{1}'";
            sqlHelper.ExecuteNonQuery(String.Format(updateSQL, widths, templateId));
        }

        //获得BlockBase设置编辑模版，未用
        public string GetUserBlock(string blockId, string blockType, string templateId, string isManage)
        {
            string temp = @"<div class='block_editor_a'>标题：</div>
			<div class='block_editor_b'><input type='text' maxlength='20' style='width:100px' name='blocktitle_{0}' class='block_input'
					id='blocktitle' onchange=changeDragText('{0}') value='{1}' ></div>
			<div class='block_editor_a'>显示条数：</div>
			<div class='block_editor_b'><input type='text' maxlength='2' name='blockrow' style='width:30px' class='block_input'
					id='blockrow_{0}' value='{2}' onkeyup=value=value.replace(/[^0-9.]/g,''); onbeforepaste=value=value.replace(/[^0-9.]/g,'');></div>
			<div class='block_editor_a' style='display:none;'>内容长度：</div>
			<div class='block_editor_b' style='display:none;'><input type='text' maxlength='2' name='subjectlength' style='width:30px' class='block_input'
					id='subjectlength_{0}' value='{3}' onkeyup=value=value.replace(/[^0-9.]/g,''); onbeforepaste=value=value.replace(/[^0-9.]/g,'');></div>
			<div class='block_editor_a'>颜色：</div>
			<div class='block_editor_b'>
				<div>
					<div class='colorblock' style='background:Gray;cursor:hand' onclick=switchTpl('{0}','Default')></div>
					<div class='colorblock' style='background:SkyBlue;cursor:hand' onclick=switchTpl('{0}','Vista')></div>

				</div>
			</div>
			<div class='block_editor_a'></div>
			<div class='block_editor_b'>
				<div>

				</div>
				<input type='hidden' name='blocktpl_{0}' id='blocktpl_{0}' value='{4}' colorvalue='{5}'>
			</div>
			<div style='width:100%;'><input class='block_button' type='button' value='确定' onclick=saveDragEditor('{0}') ID='Button1'
					NAME='Button1'> <input type='button' value='取消' class='block_button' onclick=modifyBlock('{0}') ID='Button2'
					NAME='Button2'></div>";

            //temp = string.Format(temp,item.GetAttr("Id"),item.GetAttr("BlockTitle"),item.GetAttr("RepeatItemCount"),item.GetAttr("RepeatItemLength"),item.GetAttr("Color"),item.GetAttr("ColorValue"));
            return temp;
        }

        //获取所有版块名称
        public static string GetAllBlockNames(string blockType, string templateId)
        {
            string temp = @"<div align='left' class='panelcon'>
					<img src='{2}' align='absmiddle' class='panelicon'>{1} <img src='/Portal/Door/image/add.gif' blockID='{0}' title='添加' align='absmiddle' class='paneladdimg' onclick=addBlock('{0}','{1}');>
				</div>
				";

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            StringBuilder html = new StringBuilder();
            DataTable dt = sqlHelper.ExecuteDataTable(string.Format("select ID,BlockKey,BlockTitle,BlockImage from S_P_DoorBlock where IsHidden='F' and BlockType='{0}' and TemplateId='{1}' order by SortIndex", blockType, templateId));
            DataTable dtCatalog = sqlHelper.ExecuteDataTable("select * from S_I_PublicInformCatalog where IsOnHomePage = '0'");
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dtr in dt.Rows)
                {
                    string key = dtr["BlockKey"].ToString();
                    if (dtCatalog.Select("CatalogKey='" + key + "'").Length == 0)
                        html.Append(string.Format(temp, dtr["ID"].ToString(), dtr["BlockTitle"].ToString().Replace(" ", "&nbsp;"), dtr["BlockImage"].ToString()));
                }
            }
            return html.ToString();
        }

        //获取单个块的html
        public static string GetOneBlockHtmls(string blockId)
        {
            Dictionary<string, object> dicBlock = new Dictionary<string, object>();
            StringBuilder blockHtmls = new StringBuilder();
            string BlockBeginHtml = "<DIV class=\"drag_div\" id=\"drag_{0}\" style=\"BORDER-COLOR: {1}; BACKGROUND: #fff;\">";
            string BlockEndHtml = @"<DIV id='drag_switch_{0}'>
						<DIV class='drag_editor' id='drag_editor_{0}' style='DISPLAY: none'>
							<DIV id='loadeditorid_{0}' style='WIDTH: 100px'><IMG src='/Portal/Door/image/loading.gif'><SPAN id='loadeditortext_{0}' style='COLOR: #333'></SPAN></DIV>
						</DIV>
						<DIV class='drag_content' id='drag_content_{0}'>
							<DIV id='loadcontentid_{0}' style='WIDTH: 100px'><IMG src='/Portal/Door/image/loading.gif'><SPAN id='loadcontenttext_{0}' style='COLOR: #333'></SPAN></DIV>
						</DIV>
					</DIV>
				</DIV>";
            BaseBlock bl = new BaseBlock(blockId);
            if (bl.Entity == null) return "";
            blockHtmls.Append(string.Format(BlockBeginHtml, blockId, bl.Entity.ColorValue));
            blockHtmls.Append(bl.GetHeadHtml());
            blockHtmls.Append(string.Format(BlockEndHtml, blockId));
            dicBlock["Html"] = blockHtmls.ToString();
            dicBlock["DelayLoadSecond"] = bl.Entity.DelayLoadSecond.Value;
            dicBlock["RepeatItemCount"] = bl.Entity.RepeatItemCount;
            dicBlock["RelateScript"] = bl.Entity.RelateScript;
            return JsonHelper.ToJson(dicBlock);
        }

        //添加新模块了,保存主页个人模版
        public static void UpdateAfterAddNewOneBlock(string blockId, string templateId)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            if (Config.Constant.IsOracleDb)
            {
                var TemplateString = sqlHelper.ExecuteScalar(String.Format("SELECT TEMPLATESTRING FROM S_P_DOORBASETEMPLATE  WHERE ID='{0}'", templateId));
                if (TemplateString != null)
                {
                    var updateSQL = "UPDATE S_P_DOORBASETEMPLATE SET TEMPLATESTRING='{0}' WHERE ID='{1}'";
                    if (TemplateString.ToString() == "")
                    {
                        updateSQL = String.Format(updateSQL, blockId, templateId);
                    }
                    else
                    {
                        updateSQL = String.Format(updateSQL, TemplateString + "," + blockId, templateId);
                    }
                    sqlHelper.ExecuteNonQuery(String.Format(updateSQL, blockId, templateId));
                }
            }
            else
            {
                string updateSQL = "update S_P_DoorBaseTemplate set TemplateString=CASE TemplateString WHEN '' THEN '{0}' ELSE TemplateString+',{0}' END where ID='{1}'";
                sqlHelper.ExecuteNonQuery(String.Format(updateSQL, blockId, templateId));
            }

        }
        //删除模版中的块
        public static void DeleteBlockFromBaseTemplate(string blockId, string templateId)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);

            string updateSQL = "update S_P_DoorBaseTemplate set TemplateString=replace(replace(replace(TemplateString,'{0},',''),',{0}',''),'{0}','') where ID='{1}'";
            sqlHelper.ExecuteNonQuery(String.Format(updateSQL, blockId, templateId));

        }
        #endregion
    }
}
