<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RdlView.aspx.cs" Inherits="MvcConfig.RdlView" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="Scripts/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        window.onload = function () {
            document.getElementById("ReportViewer1_ctl09").style.height = (document.documentElement.clientHeight - document.getElementById("divSearch").offsetHeight - 50) + "px";
        }

        function viewSearch() {
            var table = document.getElementById("table");
            if (table.style.display == "") {
                table.style.display = "none";
                //document.getElementById("btnAdvanceSearch").parentNode.classname = "";
            }
            else {
                //document.getElementById("btnAdvanceSearch").parentNode.classname = "advanced";
                table.style.display = "";
            }
        }

    </script>
    <style type="text/css">
        body
        {
            margin: 0px;
            padding: 0px;
        }
        .baseTable
        {
            width: 100%;
            border: 0px;
            padding: 0px;
            margin: 0px;
            font-size: 9pt;
            line-height: 17px;
        }
        .mini-toolbar
        {
            border: solid 1px #909aa6;
            padding: 3px;
            _padding-bottom: 4px;
            background: #E7EAEE url(/CommonWebResource/Theme/Default/MiniUI/images/toolbar/toolbar.png) repeat-x 0 0;
        }
        .mini-button-text
        {
            line-height: 16px;
            padding: 2px 8px 3px 8px;
            font-size: 9pt;
            line-height: 17px;
            vertical-align: top;
            display: inline-block;
            padding: 3px 8px 2px 25px;
        }
        .mini-button-text a
        {
            color: #000000;
            text-decoration: none;
        }
        
        .icon-find
        {
            background: url(/CommonWebResource/Theme/Default/MiniUI/icons/find.png) no-repeat;
            background-position: 6px 50%;
            background-repeat: no-repeat;
        }
    </style>
</head>
<body scroll="no">
    <form id="form1" runat="server" style="overflow: hidden">
    <div id="divSearch" class="mini-toolbar" runat="server">
        <table class="baseTable">
            <tr>
                <td width="80px" align="right">
                    <asp:Label ID="lblQXZ" Text="请选择：" runat="server"></asp:Label>
                </td>
                <td width="*">
                    <asp:RadioButtonList ID="radioList" runat="server" Visible="false" RepeatDirection="Horizontal" OnSelectedIndexChanged="radioList_Click" AutoPostBack="true" RepeatLayout="Flow">
                    </asp:RadioButtonList>
                </td>
                <td width="60px" align="right">
                    <a href="javascript:void(0)" onclick="viewSearch();" style="cursor: hand" id="btnAdvanceSearch" runat="server">高级筛选</a>
                </td>
            </tr>
        </table>
        <table id="table" runat="server" class="baseTable" style="background: #f6f9ff; display: none">
            <tr id="tr1" runat="server" visible="false">
                <td width="10%" style="padding: 3px;" align="right">
                    <asp:Label ID="lbl1" runat="server"></asp:Label>
                </td>
                <td width="20%">
                    <asp:TextBox ID="txt1" runat="server" Width="100%" Visible="false"></asp:TextBox>
                    <asp:DropDownList ID="dl1" runat="server" Width="100%" Visible="false">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="r1" runat="server" ControlToValidate="txt1" Visible="false" Display="None"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="e1" runat="server" ControlToValidate="txt1" Visible="false" Display="None"></asp:RegularExpressionValidator>
                </td>
                <td width="10%" align="right">
                    <asp:Label ID="lbl2" runat="server"></asp:Label>
                </td>
                <td width="20%">
                    <asp:TextBox ID="txt2" runat="server" Width="100%" Visible="false"></asp:TextBox>
                    <asp:DropDownList ID="dl2" runat="server" Width="100%" Visible="false">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="r2" runat="server" ControlToValidate="txt2" Visible="false" Display="None"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="e2" runat="server" ControlToValidate="txt2" Visible="false" Display="None"></asp:RegularExpressionValidator>
                </td>
                <td width="10%" align="right">
                    <asp:Label ID="lbl3" runat="server"></asp:Label>
                </td>
                <td width="20%">
                    <asp:TextBox ID="txt3" runat="server" Width="100%" Visible="false"></asp:TextBox>
                    <asp:DropDownList ID="dl3" runat="server" Width="100%" Visible="false">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="r3" runat="server" ControlToValidate="txt3" Visible="false" Display="None"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="e3" runat="server" ControlToValidate="txt3" Visible="false" Display="None"></asp:RegularExpressionValidator>
                </td>
                <td align="center" rowspan="2">
                    <span id="btnEnter" class="mini-button-text  mini-button-icon icon-find" onclick="javascript:btnOK.click();" runat="server"><a href="#">查询</a></span>
                    <asp:Button ID="btnOK" runat="server" OnClick="btnOK_Click" Style="display: none" />
                </td>
            </tr>
            <tr id="tr2" runat="server" visible="false">
                <td width="10%" style="padding: 3px;" align="right">
                    <asp:Label ID="lbl4" runat="server"></asp:Label>
                </td>
                <td width="20%">
                    <asp:TextBox ID="txt4" runat="server" Width="100%" Visible="false"></asp:TextBox>
                    <asp:DropDownList ID="dl4" runat="server" Width="100%" Visible="false">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="r4" runat="server" ControlToValidate="txt4" Visible="false" Display="None"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="e4" runat="server" ControlToValidate="txt4" Visible="false" Display="None"></asp:RegularExpressionValidator>
                </td>
                <td width="10%" align="right">
                    <asp:Label ID="lbl5" runat="server"></asp:Label>
                </td>
                <td width="20%">
                    <asp:TextBox ID="txt5" runat="server" Width="100%" Visible="false"></asp:TextBox>
                    <asp:DropDownList ID="dl5" runat="server" Width="100%" Visible="false">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="r5" runat="server" ControlToValidate="txt5" Visible="false" Display="None" ErrorMessage="*"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="e5" runat="server" ControlToValidate="txt5" Visible="false" Display="None"></asp:RegularExpressionValidator>
                </td>
                <td width="10%" align="right">
                    <asp:Label ID="lbl6" runat="server"></asp:Label>
                </td>
                <td width="20%">
                    <asp:TextBox ID="txt6" runat="server" Width="100%" Visible="false"></asp:TextBox>
                    <asp:DropDownList ID="dl6" runat="server" Width="100%" Visible="false">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="r6" runat="server" ControlToValidate="txt6" Visible="false" Display="None" ErrorMessage="*"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="e6" runat="server" ControlToValidate="txt6" Visible="false" Display="None"></asp:RegularExpressionValidator>
                </td>
            </tr>
        </table>
        <asp:ValidationSummary ID="summary" runat="server" />
    </div>
    <div style="overflow: hidden">
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%" Height="" Style="overflow: hidden">
        </rsweb:ReportViewer>
    </div>
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    </form>
</body>
</html>
