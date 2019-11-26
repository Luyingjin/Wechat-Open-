using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Data;
using Config;
using System.IO;
using Microsoft.Reporting.WebForms;

namespace MvcConfig
{
    public partial class RdlView : System.Web.UI.Page
    {
        private RdlcHelper _rdlcHelper = null;
        private RdlcHelper rdlcHelper
        {
            get
            {
                if (_rdlcHelper == null)
                {
                    _rdlcHelper = new RdlcHelper(HttpContext.Current.Request["ReportCode"], table);
                }
                return _rdlcHelper;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                loadData();

                setParam();
            }

        }

        private void loadData()
        {
            string rdlFilePath = HttpContext.Current.Server.MapPath("/") + "AutoReport/" + Request["ReportCode"] + ".rdl";
            if (!File.Exists(rdlFilePath))
                return;
            ReportViewer1.Reset();

            ReportViewer1.LocalReport.ReportPath = rdlFilePath;
            ReportViewer1.LocalReport.DataSources.Clear();

            ReportViewer1.LocalReport.EnableHyperlinks = true;
            ReportViewer1.HyperlinkTarget = "_blank";

            foreach (string key in rdlcHelper.dicDataSet.Keys)
            {
                ReportDataSource reportDataSource = new ReportDataSource();
                reportDataSource.Value = rdlcHelper.dicDataSet[key];
                reportDataSource.Name = key;
                ReportViewer1.LocalReport.DataSources.Add(reportDataSource);
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int start = 0;
                if (rdlcHelper.paramList.Count > 0 && rdlcHelper.paramList[0].ValidValues.Count > 0)
                {
                    foreach (var item in rdlcHelper.paramList.FirstOrDefault().ValidValues)
                    {
                        item.text += "　";
                    }
                    radioList.Visible = true;
                    radioList.DataSource = rdlcHelper.paramList.FirstOrDefault().ValidValues;
                    radioList.DataValueField = "value";
                    radioList.DataTextField = "text";
                    radioList.DataBind();
                    radioList.SelectedIndex = 0;
                    start = 1;
                }

                for (int i = start; i < rdlcHelper.paramList.Count; i++)
                {

                    Label lbl = table.FindControl("lbl" + (i - start + 1).ToString()) as Label;
                    TextBox txt = table.FindControl("txt" + (i - start + 1).ToString()) as TextBox;
                    lbl.Text = rdlcHelper.paramList[i].Name;
                    txt.Visible = true;
                    txt.Text = rdlcHelper.paramList[i].DefaultValue;

                    //必填验证
                    if (rdlcHelper.paramList[i].AllowBlank == "false" && rdlcHelper.paramList[i].Nullable == "false")
                    {
                        RequiredFieldValidator r = table.FindControl("r" + (i - start + 1).ToString()) as RequiredFieldValidator;
                        r.Visible = true;
                        r.ErrorMessage = rdlcHelper.paramList[i].Name + "不能为空";
                    }


                    if (rdlcHelper.paramList[i].ValidValues.Count > 0) //下拉框
                    {
                        DropDownList dl = table.FindControl("dl" + (i - start + 1).ToString()) as DropDownList;
                        txt.Visible = false;

                        dl.Visible = true;
                        dl.DataSource = rdlcHelper.paramList[i].ValidValues;
                        dl.DataValueField = "value";
                        dl.DataTextField = "text";
                        dl.SelectedValue = rdlcHelper.paramList[i].DefaultValue;
                        dl.DataBind();                       

                    }
                    else if (rdlcHelper.paramList[i].DataType == "DateTime") //日期框
                    {
                        txt.Attributes.Add("onClick", "WdatePicker();");                      

                        RegularExpressionValidator re = table.FindControl("e" + (i - start + 1).ToString()) as RegularExpressionValidator;
                        re.Visible = true;
                        re.ValidationExpression = "[0-9]{4}-[0-9]{1,2}-[0-9]{1,2}";
                        re.ErrorMessage = rdlcHelper.paramList[i].Name + "不是正确日期格式，格式如：2013-10-16";

                    }
                    else if (rdlcHelper.paramList[i].DataType == "Integer")
                    {
                        RegularExpressionValidator re = table.FindControl("e" + (i - start + 1).ToString()) as RegularExpressionValidator;
                        re.Visible = true;
                        re.ValidationExpression = "[1-9]{1}[0-9]{0,9}";
                        re.ErrorMessage = rdlcHelper.paramList[i].Name + "不是正确的整型数据";

                    }
                    else if (rdlcHelper.paramList[i].DataType == "Float")
                    {
                        RegularExpressionValidator re = table.FindControl("e" + (i - start + 1).ToString()) as RegularExpressionValidator;
                        re.Visible = true;
                        re.ValidationExpression = @"(\d*\.)?\d+";
                        re.ErrorMessage = rdlcHelper.paramList[i].Name + "不是正确的浮点型数据";

                    }

                    if (rdlcHelper.paramList[i].DataType == "Boolean")
                    {
                        RegularExpressionValidator re = table.FindControl("e" + (i - start + 1).ToString()) as RegularExpressionValidator;
                        re.Visible = true;
                        re.ValidationExpression = @"True|False|true|false|TRUE|FALSE";
                        re.ErrorMessage = rdlcHelper.paramList[i].Name + "不是正确的布尔型数据，请输入True或False";
                    }
                }

                if (rdlcHelper.paramList.Count - start > 0)
                    tr1.Visible = true;

                if (rdlcHelper.paramList.Count - start > 3)
                    tr2.Visible = true;

                if (rdlcHelper.paramList.Count - start == 0)
                    btnAdvanceSearch.Visible = false;

                if (rdlcHelper.paramList.Count == 0)
                {
                    divSearch.Style.Add("display", "none");
                }
                if (start == 0)
                {
                    lblQXZ.Visible = false;
                    radioList.Visible = false;
                }
            }

        }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            loadData();
            setParam();
        }

        protected void radioList_Click(object sender, EventArgs e)
        {
            loadData();
            setParam();
        }


        private void setParam()
        {
            List<ReportParameter> listParamter = new List<ReportParameter>();
            int start = 0;
            if (rdlcHelper.paramList.Count > 0 && rdlcHelper.paramList[0].ValidValues.Count > 0)
            {
                listParamter.Add(new ReportParameter(rdlcHelper.paramList[0].Name, radioList.SelectedValue));
                start = 1;
            }

            for (int i = start; i < rdlcHelper.paramList.Count; i++)
            {
                if (rdlcHelper.paramList[i].Prompt.StartsWith("@"))
                    continue;

                TextBox txt = table.FindControl("txt" + (i - start + 1).ToString()) as TextBox;
                DropDownList dl = table.FindControl("dl" + (i - start + 1).ToString()) as DropDownList;
                string text = "";
                if (dl.Visible == true)
                    text = dl.Text;
                else if (txt.Visible == true)
                    text = txt.Text;

                if (string.IsNullOrEmpty(text) && rdlcHelper.paramList[i].Nullable == "true")
                {
                    listParamter.Add(new ReportParameter(rdlcHelper.paramList[i].Name));
                }
                else
                {
                    listParamter.Add(new ReportParameter(rdlcHelper.paramList[i].Name, text));
                }
            }

            if (rdlcHelper.hasRootPath == true)
                listParamter.Add(new ReportParameter("RootPath", Request.Url.Scheme + "://" + Request.Url.Host + ":" + Request.Url.Port));

            this.ReportViewer1.LocalReport.SetParameters(listParamter.ToArray());

            //刷新显示 
            this.ReportViewer1.LocalReport.Refresh();

        }

    }
}


