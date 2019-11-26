using System;
using System.Linq;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Data.Metadata.Edm;
using Formula;
using Config;

namespace Portal.Door
{
    public class BaseBlockData : BasePage
    {
        private string UserID = FormulaHelper.UserID;
        private string content = "";
        private void Page_Load(object sender, System.EventArgs e)
        {
            // 在此处放置用户代码以初始化页面
            string reqAction = this.Request["Param"];
            switch (reqAction)
            {
                //获取各个块内容
                case "GetContent":
                    string blockId = this.Request["BlockId"];
                    BaseBlock bl = new BaseBlock(blockId);
                    content = GetBlockContent(bl);
                    break;
                #region 主页功能部分
                case "SaveOrder"://保存页面布局
                    string orders = this.Request["Orders"];
                    BaseBlock.SaveGetBlocks(orders, this.Request["TemplateId"]);
                    break;
                 case "GetAllBlock":
                    content = BaseBlock.GetAllBlockNames(this.Request["BlockType"], this.Request["TemplateId"]);
                    break;
                case "GetOneNew":
                    content = BaseBlock.GetOneBlockHtmls(this.Request["BlockId"]);
                    BaseBlock.UpdateAfterAddNewOneBlock(this.Request["BlockId"], this.Request["TemplateId"]);
                    break;
                case "DeleteBlock":
                    BaseBlock.DeleteBlockFromBaseTemplate(this.Request["BlockId"], this.Request["TemplateId"]);
                    break;
                case "ChangeColumns":
                    string columns = this.Request["Columns"];
                    string layout1 = this.Request["layout1"];
                    string layout2 = this.Request["layout2"];
                    string layout3 = this.Request["layout3"];
                    string layout4 = this.Request["layout4"];
                    string templateString = this.Request["TemplateString"];
                    BaseBlock.SaveChangeColumns(columns, templateString, this.Request["TemplateId"], layout1, layout2, layout3, layout4);
                    break;
                case "ChangeWidth":
                    string columns1 = this.Request["Columns"];
                    string layout11 = this.Request["layout1"];
                    string layout21 = this.Request["layout2"];
                    string layout31 = this.Request["layout3"];
                    string layout41 = this.Request["layout4"];
                    BaseBlock.ChangeColumnsWidth(columns1, this.Request["TemplateId"], layout11, layout21, layout31, layout41);
                    break;
                //case "Reset":
                //    BaseBlock.ResetBlocks(UserID,this.Request["TemplateId"]);
                //    break;
                #endregion

            }
            Response.Write(content);
            Response.End();

        }

        private string GetBlockContent(BaseBlock bl)
        {
            try
            {
                //特殊Block直接输出html
                switch (bl.BlockKey)
                {
                    case "MyHead"://头像
                        return GetPhotoContent();
                    case "Weather"://天气
                        return GetWeatherContent();
                    case "Frame"://图片新闻
                        return GetFrameContent(bl);
                    case "AuditTask":
                        return GetAuditTaskContent(bl);
                    case "AuditedTask":
                        return GetAuditedTaskContent(bl);
                    case "DesignTask":
                        return GetDesignTaskContent(bl);
                    default:
                        {
                            //如果是[Delegate]类型则自定义
                            if (bl.RepeatDataDataSql != null && bl.RepeatDataDataSql.StartsWith("["))
                                bl.GetServiceData += new GetServiceHandler(bl_GetServiceData);
                            
                            return bl.GetContentHtml() + bl.GetFootHtml();
                        }
                }
            }
            catch (Exception dpe)
            {
                LogWriter.Error(dpe);

                return dpe.Message;
            }

        }
        //这里添加每块需要的数据源DataList,根据datalist自动生成html
        private DataTable bl_GetServiceData(string blockKey, int? count)
        {
            DataTable dt = null;
            //判断PublicInform下是不是有BlockKey对应的栏目
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dtCatlog = sqlHelper.ExecuteDataTable("select * from S_I_PublicInformCatalog where CatalogKey = '" + blockKey + "'");
            string[] arrOrgFullID = FormulaHelper.GetUserInfo().UserFullOrgIDs.Replace(",", ".").Split('.').Distinct().ToArray();
            if (dtCatlog != null && dtCatlog.Rows.Count > 0)
            {
                int iCount = DBNull.Value.Equals(dtCatlog.Rows[0]["InHomePageNum"]) ? (count == null ? 5 : Convert.ToInt32(count)) : Convert.ToInt32(dtCatlog.Rows[0]["InHomePageNum"]);
                if (Config.Constant.IsOracleDb)
                {
                    string whereReceiveDeptId = string.Empty;
                    for (int i = 0; i < arrOrgFullID.Length; i++)
                    {
                        whereReceiveDeptId += "INSTR(ReceiveDeptId,'" + arrOrgFullID[i] + "',1,1) > 0";
                        if (i < arrOrgFullID.Length - 1)
                            whereReceiveDeptId += " or ";
                    }
                    string whereSql = " and ((nvl(ReceiveUserId,'empty') = 'empty' and nvl(ReceiveDeptId,'empty') = 'empty') or (nvl(ReceiveUserId,'empty') <> 'empty' and INSTR(ReceiveUserId,'" + UserID + "',1,1) > 0) or (nvl(ReceiveDeptId,'empty') <> 'empty' and (" + whereReceiveDeptId + "))) ";
                    dt = sqlHelper.ExecuteDataTable("select * from S_I_PublicInformation where CatalogId = '" + dtCatlog.Rows[0]["ID"].ToString() + "' and (ExpiresTime is null or to_char(ExpiresTime,'yyyy-MM-dd') >= '" + DateTime.Now.ToString("yyyy-MM-dd") + "') " + whereSql + " ORDER BY IsTop DESC, CreateTime DESC", 0, iCount, CommandType.Text);
                }
                else
                {
                    string whereReceiveDeptId = string.Empty;
                    for (int i = 0; i < arrOrgFullID.Length; i++)
                    {
                        whereReceiveDeptId += "charindex('" + arrOrgFullID[i] + "',ReceiveDeptId) > 0";
                        if (i < arrOrgFullID.Length - 1)
                            whereReceiveDeptId += " or ";
                    }
                    string whereSql = " and ((isnull(ReceiveUserId,'') = '' and isnull(ReceiveDeptId,'') = '') or (isnull(ReceiveUserId,'') <> '' and charindex('" + UserID + "',ReceiveUserId) > 0) or (isnull(ReceiveDeptId,'') <> '' and (" + whereReceiveDeptId + "))) ";
                    dt = sqlHelper.ExecuteDataTable("select * from S_I_PublicInformation where CatalogId = '" + dtCatlog.Rows[0]["ID"].ToString() + "' and isnull(DeptDoorId,'') = '' and (ExpiresTime is null or ExpiresTime >= convert(datetime,convert(varchar(10),getdate(),120))) " + whereSql + " ORDER BY Important DESC,Urgency DESC, CreateTime DESC", 0, iCount, CommandType.Text);
                }
                if (dt != null)
                {
                    dt.Columns.Add("CreateDate");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (!DBNull.Value.Equals(dt.Rows[i]["CreateTime"]))
                        {
                            dt.Rows[i]["CreateDate"] = Convert.ToDateTime(dt.Rows[i]["CreateTime"]).ToShortDateString();
                        }
                    }
                }
            }
            else
            {
                switch (blockKey.ToLower())
                {
                    case "friendlink":
                        {
                            int iCount = count == null ? 5 : Convert.ToInt32(count);
                            if (Config.Constant.IsOracleDb)
                            {
                                string whereReceiveDeptId = string.Empty;
                                for (int i = 0; i < arrOrgFullID.Length; i++)
                                {
                                    whereReceiveDeptId += "INSTR(DeptId,'" + arrOrgFullID[i] + "',1,1) > 0";
                                    if (i < arrOrgFullID.Length - 1)
                                        whereReceiveDeptId += " or ";
                                }
                                string whereSql = " and ((nvl(UserId,'empty') = 'empty' and nvl(DeptId,'empty') = 'empty') or (nvl(UserId,'empty') <> 'empty' and INSTR(DeptId,'" + UserID + "',1,1) > 0) or (nvl(DeptId,'empty') <> 'empty' and (" + whereReceiveDeptId + "))) ";
                                dt = sqlHelper.ExecuteDataTable("select * from S_I_FriendLink where 1=1 " + whereSql + " ORDER BY SortIndex", 0, iCount, CommandType.Text);
                            }
                            else
                            {
                                string whereReceiveDeptId = string.Empty;
                                for (int i = 0; i < arrOrgFullID.Length; i++)
                                {
                                    whereReceiveDeptId += "charindex('" + arrOrgFullID[i] + "',DeptId) > 0";
                                    if (i < arrOrgFullID.Length - 1)
                                        whereReceiveDeptId += " or ";
                                }
                                string whereSql = " where ((isnull(UserId,'') = '' and isnull(DeptId,'') = '') or (isnull(UserId,'') <> '' and charindex('" + UserID + "',UserId) > 0) or (isnull(DeptId,'') <> '' and (" + whereReceiveDeptId + "))) ";
                                dt = sqlHelper.ExecuteDataTable("select * from S_I_FriendLink " + whereSql + " ORDER BY SortIndex", 0, iCount, CommandType.Text);
                            }
                            break;
                        }

                }
            }
            return dt;
        }

        //获得天气预报
        private string GetWeatherContent()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<iframe allowtransparency=\"true\" frameborder=\"0\" width=\"290\" height=\"96\" scrolling=\"no\" src=\"http://tianqi.2345.com/plugin/widget/index.htm?s=2&z=3&t=0&v=0&d=2&k=&f=1&q=1&e=1&a=1&c=54511&w=290&h=96\"></iframe>");

            return sb.ToString();
        }

        private string GetFrameContent(BaseBlock bl)
        {
            if (bl != null)
                return bl.RepeatItemTemplate + bl.GetFootHtml();
            else
                return string.Empty;
        }
        
        //获得头像html，todo
        private string GetPhotoContent()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"<table width='100%' border='0' cellspacing='0' cellpadding='0' style='height:100%;cursor:hand'>
				<tr>
					<td width='100%' align='center' valign='top' style=' background-repeat:repeat-y;'>
					<table width='164' border='1' cellspacing='0' cellpadding='0'>
					<tr>");

            sb.Append("<td height='174' valign='bottom' align=center>");
            sb.Append(@"<table width='100%' border='0' cellpadding='0' cellspacing='0'  style='-moz-opacity:0.7; filter:alpha(opacity=70);' onMouseOver='this.style.MozOpacity=1;
							this.filters.alpha.opacity=100' onMouseOut='this.style.MozOpacity=0.7;
							this.filters.alpha.opacity=70'>");
            sb.Append(@"<tr>
							<td height='174'width='164'><image width='164' height='174'  id='UserPhoto' src='/MvcConfig/Image/GetUserPic?userId=" + UserID + @"'></td>
						</tr>");
            sb.Append(@"
						</table></td>
					</tr>
					</table>
					</td></tr>
					</table>");

            return sb.ToString();
        }

        private string GetAuditedTaskContent(BaseBlock bl)
        {
            string sql = @"select FormUrl,FormWidth,FormHeight,FormInstanceID,S_WF_InsTaskExec.ID,S_WF_InsTask.TaskName,S_WF_InsTaskExec.ExecTime from S_WF_InsTaskExec join S_WF_InsTask on ExecTime is not null and ExecUserID='{0}' and S_WF_InsTask.Type in ('Normal','Inital') and S_WF_InsTask.ID=InsTaskID join S_WF_InsFlow on S_WF_InsFlow.ID=S_WF_InsTask.InsFlowID join S_WF_InsDefFlow on S_WF_InsDefFlow.ID=S_WF_InsFlow.InsDefFlowID order by S_WF_InsTaskExec.ExecTime desc";

            sql = string.Format(sql, FormulaHelper.UserID);

            string template = bl.RepeatItemTemplate;
            string html = "";

            SQLHelper helper = SQLHelper.CreateSqlHelper("WorkFlow");
            int iCount = 8;
            if (bl.RepeatItemCount != null)
                iCount = bl.RepeatItemCount.Value;
            DataTable source = helper.ExecuteDataTable(sql, 0, iCount, CommandType.Text);

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

            }

            html += bl.GetFootHtml();

            return html;
        }

        private string GetAuditTaskContent(BaseBlock bl)
        {
            string preYear = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd");

            string sql = @"select S_WF_InsTaskExec.ID,S_WF_InsTask.TaskName,FormUrl,FormInstanceID,S_WF_InsTaskExec.CreateTime,FormWidth,FormHeight 
from S_WF_InsTaskExec join S_WF_InsTask on ExecTime is null and ExecUserID='{0}' and S_WF_InsTask.Type in('Normal','Inital') and (WaitingRoutings='' or WaitingRoutings is null) and (WaitingSteps='' or WaitingSteps is null) and (S_WF_InsTaskExec.CreateTime >= {1} ) and S_WF_InsTask.ID=InsTaskID 
join S_WF_InsFlow on S_WF_InsFlow.Status='Processing' and S_WF_InsFlow.ID=S_WF_InsTask.InsFlowID join S_WF_InsDefFlow on S_WF_InsFlow.InsDefFlowID=S_WF_InsDefFlow.ID  order by S_WF_InsTaskExec.CreateTime desc";

            sql = string.Format(sql, FormulaHelper.UserID,
                Config.Constant.IsOracleDb ? string.Format("to_date('{0}','yyyy-MM-dd')", preYear) : string.Format("'{0}'", preYear)
                );

            string template = bl.RepeatItemTemplate;
            string html = "";

            SQLHelper helper = SQLHelper.CreateSqlHelper("WorkFlow");
            int iCount = 8;
            if (bl.RepeatItemCount != null)
                iCount = bl.RepeatItemCount.Value;
            DataTable source = helper.ExecuteDataTable(sql, 0, iCount, CommandType.Text);

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

            }

            html += bl.GetFootHtml();

            return html;
        }

        private string GetDesignTaskContent(BaseBlock bl)
        {
            string sql = @"select ID,Name as TaskName,WBSFULLID,CreateDate from S_W_TaskWork Where (DesignerUserID  like '{0}%' or DesignerUserID like '%{0}%' or DesignerUserID like '%{0}') And (State is null Or State='' or State !='Finish')";
           
            sql = string.Format(sql, FormulaHelper.UserID);

            string template = bl.RepeatItemTemplate;
            string html = "";

            SQLHelper helper = SQLHelper.CreateSqlHelper("Project");
            DataTable source = helper.ExecuteDataTable(sql, 0, 8, CommandType.Text);

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

            }

            html += bl.GetFootHtml();

            return html;
        }

        #region Web 窗体设计器生成的代码
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: 该调用是 ASP.NET Web 窗体设计器所必需的。
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// 设计器支持所需的方法 - 不要使用代码编辑器修改
        /// 此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);
        }
        #endregion

    }
}
