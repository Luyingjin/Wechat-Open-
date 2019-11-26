<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoginError.aspx.cs" Inherits="Portal.LoginError" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>金慧软件-BuildingEasy综合管理信息化系统</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
    <script language="JScript" src="/CommonWebResource/CoreLib/Combine/CustomPageInc.js"></script>
    <script language="JScript" src="/CommonWebResource/Theme/default/CssInc.js"></script>
    <script type="text/javascript">
        function Relogin() {
            window.location.href = "/portal/Login.aspx";
        }
        function PageInit() {
            if (window.parent != window)
                window.parent.location.href = "/portal/LoginError.aspx";
        }
    </script>
</head>
<body onload="PageInit()">
    <table border="0" align="center" cellpadding="0" height="100%" style="border-collapse: collapse"
        width="600">
        <tr height="64">
            <td width="20%" class="ToolBarBg">
                <img border="0" src="images/error_bg.jpg">
            </td>
            <td width="80%" class="ToolBarBg">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="2" bgcolor="#ffffff" valign="top">
                <table height="100" align="center">
                    <tr>
                        <td>
                            <font color="red" style="font-size: 10pt"><b>页面执行不成功</b></font>
                        </td>
                    </tr>
                </table>
                <table border="0" align="center" cellpadding="0" style="border-collapse: collapse"
                    width="90%" id="table2">
                    <tr>
                        <td align="center">
                            <br>
                            <font>可能由于<strong>服务没有启动完毕</strong>或者是<strong>用户登录已过期</strong> !<br>
                                <br>
                                <font color="red"><a href="javascript:;" onclick="Relogin()" style="font-size: 11pt;">点击重试登陆</a></font><br>
                                <br>
                                如果错误依然发生请联系管理员！<form id="TestForm">
                                </form>
                            </font>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>
